<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import axios from "axios";

// Interface cho danh mục diễn đàn
interface ForumCategory {
  id: number;
  name: string;
  description: string;
  iconClass: string;
  postCount: number;
}

// Interface cho bài viết từ API
interface PostResponse {
  id: number;
  caption: string;
  content: string;
  createdDate: string;
  categoryId: number;
  user?: {
    fullName?: string;
    userName?: string;
  };
}

// Interface cho bài viết đã xử lý
interface ForumPost {
  id: number;
  caption: string;
  content: string;
  userName: string;
  createdAt: string;
  categoryId: number;
}

// API URL
const API_BASE_URL = "https://localhost:7232/api";

// State
const categories = ref<ForumCategory[]>([]);
const posts = ref<ForumPost[]>([]);
const searchQuery = ref<string>("");
const selectedCategory = ref<number | null>(null);
const currentPage = ref(1);
const postsPerPage = 10;

// Gọi API lấy danh mục diễn đàn
const fetchCategories = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/forumscategory/getall`);
    categories.value = response.data.result;
  } catch (error) {
    console.error("Lỗi khi lấy danh mục:", error);
  }
};

// Gọi API lấy bài viết theo danh mục hoặc tất cả bài viết
const fetchPosts = async (categoryId: number | null = null) => {
  try {
    const url = categoryId
      ? `${API_BASE_URL}/posts/by-category/${categoryId}`
      : `${API_BASE_URL}/posts/latest`;
    console.log("Gọi API:", url);
    const response = await axios.get(url);
    console.log("Dữ liệu trả về:", response.data);
    posts.value = response.data.result.map((post: PostResponse) => ({
      id: post.id,
      caption: post.caption,
      content: post.content,
      userName: post.user?.fullName || post.user?.userName || "Ẩn danh",
      createdAt: new Date(post.createdDate).toLocaleString(),
      categoryId: post.categoryId,
    }));
  } catch (error) {
    console.error("Lỗi khi lấy bài viết:", error);
    posts.value = []; // Đặt posts thành rỗng nếu có lỗi
  }
};

// Xử lý khi nhấn vào danh mục
const selectCategory = (categoryId: number) => {
  selectedCategory.value = categoryId;
  currentPage.value = 1;
  fetchPosts(categoryId);
};

// Reset về tất cả bài viết
const resetCategory = () => {
  selectedCategory.value = null;
  currentPage.value = 1;
  fetchPosts();
};

// Lọc bài viết theo tìm kiếm
const filteredPosts = computed(() => {
  let result = posts.value;
  if (searchQuery.value) {
    result = result.filter(
      (post) =>
        post.caption.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        post.content.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        post.userName.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }
  return result;
});

// Tính toán bài viết hiển thị theo trang
const paginatedPosts = computed(() => {
  const start = (currentPage.value - 1) * postsPerPage;
  const end = start + postsPerPage;
  return filteredPosts.value.slice(start, end);
});

// Tổng số trang
const totalPages = computed(() => {
  return Math.ceil(filteredPosts.value.length / postsPerPage);
});

// Chuyển trang
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

// Gọi API khi component mount
onMounted(() => {
  fetchCategories();
  fetchPosts();
});
</script>

<template>
  <div class="forum-container">
    <!-- Sidebar -->
    <aside class="sidebar">
      <h2 class="sidebar-title">GTZ</h2>
      <ul class="nav-links">
        <li><router-link to="/">🏠 Trang chủ</router-link></li>
        <li><router-link to="/forums">💬 Diễn đàn</router-link></li>
        <li><router-link to="/marketplace">🛒 Chợ giao dịch</router-link></li>
        <li><router-link to="/support">📞 Hỗ trợ</router-link></li>
      </ul>
    </aside>

    <!-- Nội dung chính -->
    <div class="forum-main">
      <!-- Thanh tìm kiếm -->
      <div class="forum-search">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Tìm kiếm..."
          class="search-input"
        />
      </div>

      <!-- Tiêu đề -->
      <header class="forum-header">
        <h1 class="forum-title">Diễn Đàn GameTradeZone</h1>
        <button v-if="selectedCategory" @click="resetCategory" class="back-btn">
          Quay lại tất cả bài viết
        </button>
      </header>

      <!-- Danh mục diễn đàn -->
      <section class="forum-categories">
        <div
          v-for="category in categories"
          :key="category.id"
          class="category-card"
          @click="selectCategory(category.id)"
        >
          <div class="category-header">
            <i
              :class="`fa-solid ${category.iconClass || 'fa-question-circle'}`"
              class="category-icon"
            ></i>
            <h3 class="category-title">{{ category.name }}</h3>
          </div>
          <div class="category-info">
            <p class="category-description">{{ category.description }}</p>
            <p class="post-meta">Bài viết: {{ category.postCount }}</p>
          </div>
        </div>
      </section>

      <!-- Danh sách bài viết -->
      <section class="forum-posts">
        <h2 class="section-title">
          {{
            selectedCategory ? "Bài viết theo danh mục" : "Bài viết mới nhất"
          }}
        </h2>
        <div v-if="paginatedPosts.length === 0" class="no-posts">
          Không có bài viết nào trong danh mục này.
        </div>
        <div v-else class="posts-list">
          <router-link
            v-for="post in paginatedPosts"
            :key="post.id"
            :to="`/post/${post.id}`"
            class="post-item"
          >
            <p class="post-content">{{ post.caption }}</p>
            <p class="post-meta">
              Đăng bởi <span class="post-author">{{ post.userName }}</span> •
              {{ post.createdAt }}
            </p>
          </router-link>
        </div>

        <!-- Phân trang -->
        <div v-if="paginatedPosts.length > 0" class="pagination">
          <button
            :disabled="currentPage === 1"
            @click="goToPage(currentPage - 1)"
          >
            Trước
          </button>
          <span>Trang {{ currentPage }} / {{ totalPages }}</span>
          <button
            :disabled="currentPage === totalPages"
            @click="goToPage(currentPage + 1)"
          >
            Sau
          </button>
        </div>
      </section>
    </div>

    <!-- Member Stats -->
    <aside class="member-stats">
      <h2 class="stats-title">THỐNG KÊ</h2>
      <ul class="stats-list">
        <li>Thành viên online: <span class="online-members">150</span></li>
        <li>Tổng bài viết: <span class="stats-total">12,345</span></li>
        <li>Tổng thành viên: <span class="stats-total">5,678</span></li>
      </ul>
    </aside>
  </div>
</template>

<style scoped>
/* Tổng thể */
.forum-container {
  display: flex;
  min-height: 100vh;
  background: linear-gradient(135deg, #0a1f2b 0%, #1a3c4a 100%);
  color: #e0e0e0;
  font-family: "Inter", "Arial", sans-serif;
}

/* Sidebar */
.sidebar {
  width: 240px;
  background: #0a1f2b;
  padding: 30px 20px;
  border-right: 1px solid rgba(0, 255, 204, 0.2);
  box-shadow: 2px 0 10px rgba(0, 255, 204, 0.1);
}

.sidebar-title {
  font-size: 2rem;
  font-weight: 700;
  margin-bottom: 30px;
  color: #00ffcc;
  text-align: center;
  text-shadow: 0 0 10px rgba(0, 255, 204, 0.5);
}

.nav-links li {
  padding: 12px 0;
  transition: transform 0.2s ease;
}

.nav-links li:hover {
  transform: translateX(5px);
}

.nav-links a {
  color: #00ffcc;
  text-decoration: none;
  font-size: 1.15rem;
}

.nav-links a:hover {
  color: #00cc99;
}

/* Nội dung chính */
.forum-main {
  flex: 1;
  padding: 40px;
  background: #12232e;
  border-radius: 10px;
  margin: 20px;
  box-shadow: 0 4px 20px rgba(0, 255, 204, 0.1);
}

/* Thanh tìm kiếm */
.search-input {
  padding: 12px 20px;
  border-radius: 25px;
  border: 1px solid rgba(0, 255, 204, 0.3);
  background: #1a3c4a;
  color: #e0e0e0;
  transition: all 0.3s ease;
}

.search-input:focus {
  border-color: #00ffcc;
  box-shadow: 0 0 15px rgba(0, 255, 204, 0.5);
}

/* Tiêu đề */
.forum-header {
  background: linear-gradient(90deg, #00ffcc, #00cc99);
  padding: 15px 20px;
  border-radius: 10px;
  margin-bottom: 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.forum-title {
  font-size: 2rem;
  color: #fff;
  margin: 0;
  text-shadow: 0 0 10px rgba(0, 255, 204, 0.5);
}

.back-btn {
  background: #00cc99;
  border: none;
  padding: 8px 15px;
  border-radius: 5px;
  color: #fff;
  cursor: pointer;
  transition: all 0.3s ease;
}

.back-btn:hover {
  background: #00ffcc;
  box-shadow: 0 0 10px rgba(0, 255, 204, 0.5);
}

/* Danh mục diễn đàn */
.forum-categories {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.category-card {
  background: #1a3c4a;
  padding: 20px;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.category-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 255, 204, 0.3);
}

.category-icon {
  font-size: 2.2rem;
  color: #00ffcc;
}

.category-title {
  font-size: 1.4rem;
  color: #e0e0e0;
}

.category-description {
  font-size: 0.95rem;
  color: #a0a0a0;
}

/* Bài viết */
.section-title {
  font-size: 1.5rem;
  color: #00ffcc;
  margin: 30px 0 15px;
  text-shadow: 0 0 10px rgba(0, 255, 204, 0.3);
}

.no-posts {
  font-size: 1.2rem;
  color: #a0a0a0;
  text-align: center;
  padding: 20px;
}

.posts-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.post-item {
  padding: 15px 20px;
  background: #1a3c4a;
  border-radius: 10px;
  transition: all 0.3s ease;
  text-decoration: none;
  color: inherit;
}

.post-item:hover {
  background: #00ffcc;
  color: #12232e;
  box-shadow: 0 0 15px rgba(0, 255, 204, 0.5);
}

.post-content {
  font-size: 1.1rem;
  font-weight: 500;
}

.post-author {
  color: #00ffcc;
}

/* Phân trang */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 15px;
  margin-top: 20px;
}

.pagination button {
  background: #00cc99;
  border: none;
  padding: 8px 15px;
  border-radius: 5px;
  color: #fff;
  cursor: pointer;
  transition: all 0.3s ease;
}

.pagination button:disabled {
  background: #1a3c4a;
  cursor: not-allowed;
}

.pagination button:hover:not(:disabled) {
  background: #00ffcc;
  box-shadow: 0 0 10px rgba(0, 255, 204, 0.5);
}

.pagination span {
  font-size: 1rem;
  color: #e0e0e0;
}

/* Member Stats */
.member-stats {
  width: 280px;
  padding: 30px 20px;
  background: #0a1f2b;
  border-left: 1px solid rgba(0, 255, 204, 0.2);
}

.stats-title {
  font-size: 1.5rem;
  color: #00ffcc;
  text-shadow: 0 0 10px rgba(0, 255, 204, 0.3);
}

.online-members {
  color: #00ffcc;
}

.stats-total {
  color: #a0a0a0;
}

/* Responsive */
@media (max-width: 768px) {
  .forum-container {
    flex-direction: column;
  }

  .sidebar,
  .member-stats {
    width: 100%;
  }

  .forum-main {
    margin: 0;
    border-radius: 0;
  }

  .forum-categories {
    grid-template-columns: 1fr;
  }
}
</style>

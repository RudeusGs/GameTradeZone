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
}

// API URL
const API_BASE_URL = "https://localhost:7232/api";

// State lưu dữ liệu từ API
const categories = ref<ForumCategory[]>([]);
const posts = ref<ForumPost[]>([]);
const searchQuery = ref<string>("");

// 🟢 Gọi API lấy danh mục diễn đàn
const fetchCategories = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/forumscategory/getall`);
    categories.value = response.data.result;
  } catch (error) {
    console.error("Lỗi khi lấy danh mục:", error);
  }
};

// 🟢 Gọi API lấy bài viết mới nhất
const fetchPosts = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/posts/latest`);
    console.log("Dữ liệu bài viết API:", response.data);

    posts.value = response.data.result.map((post: PostResponse) => ({
      id: post.id,
      caption: post.caption,
      content: post.content,
      userName: post.user?.fullName || post.user?.userName || "Ẩn danh", // Lấy fullName hoặc userName nếu có
      createdAt: new Date(post.createdDate).toLocaleString(), // Định dạng ngày tháng
    }));
  } catch (error) {
    console.error("Lỗi khi lấy bài viết:", error);
  }
};

// 🔄 Gọi API khi component mount
onMounted(() => {
  fetchCategories();
  fetchPosts();
});

// 🔎 Lọc bài viết theo tìm kiếm
const filteredPosts = computed(() => {
  return posts.value.filter(
    (post) =>
      post.caption.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      post.content.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      post.userName.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
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
      <header class="forum-header">
        <h1 class="forum-title">Diễn Đàn GameTradeZone</h1>
        <p class="forum-subtitle">Thảo luận, hỗ trợ và trao đổi tài khoản</p>
      </header>

      <!-- Danh mục diễn đàn -->
      <section class="forum-categories">
        <h2 class="section-title">Danh mục</h2>
        <div class="categories-grid">
          <div
            v-for="category in categories"
            :key="category.id"
            class="category-card"
          >
            <span class="category-icon">{{ category.iconClass || "❓" }}</span>
            <div class="category-info">
              <h3 class="category-title">{{ category.name }}</h3>
              <p class="category-description">{{ category.description }}</p>
              <p class="category-meta">Bài viết: {{ category.postCount }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Ô tìm kiếm -->
      <div class="forum-search">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Tìm bài viết..."
          class="search-input"
        />
      </div>

      <!-- 🟢 Danh sách bài viết -->
      <section class="forum-posts">
        <h2 class="section-title">Bài viết mới nhất</h2>
        <div class="posts-list">
          <router-link
            v-for="post in filteredPosts"
            :key="post.id"
            :to="`/post/${post.id}`"
            class="post-item"
          >
            <p class="post-content">{{ post.caption }}</p>
            <p class="post-meta">
              Đăng bởi <span class="post-author">{{ post.userName }}</span> -
              {{ post.createdAt }}
            </p>
          </router-link>
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
/* Tổng thể */
.forum-container {
  display: flex;
  min-height: 100vh;
  background: #1a0933;
  color: #e0e0e0;
  font-family: "Arial", sans-serif;
}

/* Sidebar */
.sidebar {
  width: 250px;
  background: #0d1b2a;
  padding: 20px;
}

.sidebar-title {
  font-size: 1.8rem;
  font-weight: bold;
  margin-bottom: 20px;
  color: #00ffff;
}

.nav-links {
  list-style: none;
  padding: 0;
}

.nav-links li {
  padding: 10px 0;
}

.nav-links a {
  color: #00ffff;
  text-decoration: none;
  font-size: 1.1rem;
}

.nav-links a:hover {
  color: #ff00ff;
}

/* Nội dung chính */
.forum-main {
  flex: 1;
  padding: 30px;
}

.forum-title {
  font-size: 2.5rem;
  color: #00ffff;
  text-shadow: 0 0 15px #00ffff;
}

.forum-subtitle {
  font-size: 1.2rem;
  color: #e0e0e0;
}

/* Danh mục diễn đàn */
.categories-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
}

.category-card {
  background: #12162d;
  padding: 20px;
  border-radius: 10px;
  width: 300px;
  transition: all 0.3s ease;
}

.category-card:hover {
  background: #00ffff;
  color: #1a0933;
}

.category-icon {
  font-size: 2rem;
}

/* Bài viết */
.posts-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.post-item {
  padding: 15px;
  background: #12162d;
  border-radius: 10px;
  transition: all 0.3s;
  cursor: pointer;
  text-decoration: none;
  color: inherit;
}

.post-item:hover {
  background: #00ffff;
  color: #1a0933;
}

.post-author {
  font-weight: bold;
  color: #00ffff;
}

/* Ô tìm kiếm */
.forum-search {
  margin: 20px 0;
  display: flex;
  align-items: center;
}

.search-input {
  width: 100%;
  padding: 10px;
  border-radius: 8px;
  border: none;
  background: #222;
  color: #f0f0f0;
}
</style>

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
      </header>

      <!-- Danh mục diễn đàn -->
      <section class="forum-categories">
        <div
          v-for="category in categories"
          :key="category.id"
          class="category-card"
        >
          <div class="category-header">
            <i
              :class="`fa-solid ${category.iconClass || 'fa-question-circle'}`"
              class="category-icon"
            ></i>
            <h3 class="category-title">{{ category.name }}</h3>
            <!-- <span class="category-meta">New</span> -->
          </div>
          <div class="category-info">
            <p class="category-description">{{ category.description }}</p>
            <p class="post-meta">Bài viết: {{ category.postCount }}</p>
          </div>
        </div>
      </section>

      <!-- Danh sách bài viết -->
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
              Đăng bởi <span class="post-author">{{ post.userName }}</span> •
              {{ post.createdAt }}
            </p>
          </router-link>
        </div>
      </section>
    </div>

    <!-- Member Stats (Phần bên phải) -->
    <aside class="member-stats">
      <h2 class="stats-title">THỐNG KÊ</h2>
      <ul class="stats-list"></ul>
    </aside>
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
  width: 200px;
  background: #0d1b2a;
  padding: 20px;
  border-right: 1px solid #333;
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
  padding: 20px;
  background: #12162d;
}

/* Thanh tìm kiếm */
.forum-search {
  margin-bottom: 20px;
}

.search-input {
  width: 100%;
  padding: 10px;
  border-radius: 5px;
  border: none;
  background: #222;
  color: #f0f0f0;
  font-size: 1rem;
}

/* Tiêu đề */
.forum-header {
  background: #ff4500; /* Màu cam giống MangaDex */
  padding: 10px 15px;
  border-radius: 5px;
  margin-bottom: 20px;
}

.forum-title {
  font-size: 1.8rem;
  color: #fff;
  margin: 0;
}

/* Danh mục diễn đàn */
.forum-categories {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.category-card {
  background: #1a1a1a;
  padding: 15px;
  border-radius: 5px;
  display: flex;
  flex-direction: column;
  gap: 5px;
  transition: all 0.3s ease;
}

.category-header {
  display: flex;
  align-items: center;
  gap: 10px;
}

.category-icon {
  font-size: 2rem;
  color: #ff4500;
}

.category-title {
  font-size: 1.2rem;
  color: #e0e0e0;
  margin: 0;
  flex: 1;
}

.category-meta {
  background: #ff0000;
  color: #fff;
  padding: 2px 8px;
  border-radius: 3px;
  font-size: 0.8rem;
}

.category-description {
  font-size: 0.9rem;
  color: #b0b0b0;
  margin: 0;
}

.post-meta {
  font-size: 0.8rem;
  color: #888;
}

/* Bài viết */
.section-title {
  font-size: 1.2rem;
  color: #00ffff;
  margin: 20px 0 10px;
}

.posts-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.post-item {
  padding: 10px;
  background: #1a1a1a;
  border-radius: 5px;
  transition: all 0.3s;
  cursor: pointer;
  text-decoration: none;
  color: inherit;
}

.post-item:hover {
  background: #00ffff;
  color: #1a0933;
}

.post-content {
  font-size: 1rem;
  margin: 0;
}

.post-meta {
  font-size: 0.8rem;
  color: #888;
}

.post-author {
  font-weight: bold;
  color: #00ffff;
}

/* Member Stats */
.member-stats {
  width: 250px;
  padding: 20px;
  background: #0d1b2a;
  border-left: 1px solid #333;
}

.stats-title {
  font-size: 1.2rem;
  color: #ff4500;
  margin: 20px 0 10px;
}

.stats-list {
  list-style: none;
  padding: 0;
  color: #e0e0e0;
  font-size: 0.9rem;
}

.stats-list li {
  padding: 5px 0;
}

.online-members {
  font-size: 0.9rem;
  color: #00ffff;
}

.stats-total {
  font-size: 0.9rem;
  color: #888;
  margin-top: 10px;
}
</style>

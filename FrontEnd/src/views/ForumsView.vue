<script setup lang="ts">
import { ref, computed } from "vue";

// Interface cho danh mục diễn đàn
interface ForumCategory {
  id: number;
  title: string;
  description: string;
  threads: number;
  messages: number;
  icon: string;
}

// Interface cho bài viết mới nhất
interface ForumPost {
  id: number;
  author: string;
  content: string;
  topic: string;
  time: string;
}

const categories = ref<ForumCategory[]>([
  {
    id: 1,
    title: "Thông Báo",
    description: "Cập nhật mới nhất từ GameTradeZone",
    threads: 170,
    messages: 17900,
    icon: "📢",
  },
  {
    id: 2,
    title: "Hỗ Trợ",
    description: "Hỏi đáp và trợ giúp từ cộng đồng",
    threads: 665,
    messages: 3900,
    icon: "❓",
  },
  {
    id: 3,
    title: "Báo Lỗi",
    description: "Báo cáo lỗi và góp ý cho website",
    threads: 304,
    messages: 1300,
    icon: "🐞",
  },
  {
    id: 4,
    title: "Góp Ý",
    description: "Đề xuất cải tiến và tính năng mới",
    threads: 750,
    messages: 4400,
    icon: "💡",
  },
]);

const posts = ref<ForumPost[]>([
  {
    id: 1,
    author: "UserA",
    content: "GameTradeZone vừa cập nhật giao diện mới!",
    topic: "Thông Báo",
    time: "20 phút trước",
  },
  {
    id: 2,
    author: "UserB",
    content: "Làm sao để mua tài khoản an toàn?",
    topic: "Hỗ Trợ",
    time: "40 phút trước",
  },
  {
    id: 3,
    author: "UserC",
    content: "Có ai bị lỗi không thể đăng nhập không?",
    topic: "Báo Lỗi",
    time: "1 giờ trước",
  },
]);

const searchQuery = ref<string>("");

// Lọc bài viết theo tìm kiếm
const filteredPosts = computed(() => {
  let result = posts.value;
  if (searchQuery.value) {
    result = result.filter(
      (post) =>
        post.content.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        post.author.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }
  return result;
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

    <!-- Main Content -->
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
            <span class="category-icon">{{ category.icon }}</span>
            <div class="category-info">
              <h3 class="category-title">{{ category.title }}</h3>
              <p class="category-description">{{ category.description }}</p>
              <p class="category-meta">
                Chủ đề: {{ category.threads }} | Bài viết:
                {{ category.messages }}
              </p>
            </div>
          </div>
        </div>
      </section>

      <!-- Search -->
      <div class="forum-search">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Tìm bài viết..."
          class="search-input"
        />
        <i class="fas fa-search search-icon"></i>
      </div>

      <!-- Bài viết mới nhất -->
      <section class="forum-posts">
        <h2 class="section-title">Bài viết mới nhất</h2>
        <div class="posts-list">
          <div v-for="post in filteredPosts" :key="post.id" class="post-item">
            <p class="post-content">{{ post.content }}</p>
            <p class="post-meta">
              Đăng bởi <span class="post-author">{{ post.author }}</span> -
              {{ post.time }}
            </p>
          </div>
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
  background: rgba(0, 255, 255, 0.1);
  padding: 20px;
  border-radius: 8px;
  width: 300px;
  transition: all 0.3s ease;
}

.category-card:hover {
  background: #00ffff;
  color: #1a0933;
}

.category-icon {
  font-size: 2rem;
  margin-right: 10px;
}

.category-title {
  font-size: 1.3rem;
  font-weight: bold;
}

/* Bài viết */
.posts-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.post-item {
  padding: 15px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 8px;
}

.post-item:hover {
  background: #00ffff;
  color: #1a0933;
}

.post-author {
  font-weight: bold;
  color: #00ffff;
}

/* Search */
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

.search-icon {
  margin-left: 10px;
  color: #00ffff;
}
</style>

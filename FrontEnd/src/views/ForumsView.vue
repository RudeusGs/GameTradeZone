<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import axios from "axios"; // Keep axios for user stats or replace if needed
import forumApi from "../api/forums"; // Import forum API
import forumCategoryApi from "../api/forumcategory"; // Import category API

// Router setup
const router = useRouter();

// Interface for forum category
interface ForumCategory {
  id: number;
  name: string;
  description: string;
  iconClass: string;
  postCount: number;
}

// Interface for API post response
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
  commentsCount: number;
  likesCount: number;
  views: number;
}

// Interface for processed post
interface ForumPost {
  id: number;
  caption: string;
  content: string;
  userName: string;
  createdAt: string;
  categoryId: number;
  commentsCount: number;
  likesCount: number;
  views: number;
}

// Interface for popular topics (based on API response)
interface PopularTopic {
  id: number;
  name: string;
  postCount: number;
}

// API URL (can be removed if baseApi handles it)
const API_BASE_URL = "https://localhost:7232/api";

// State
const categories = ref<ForumCategory[]>([]);
const posts = ref<ForumPost[]>([]);
const searchQuery = ref<string>("");
const selectedCategory = ref<number | null>(null);
const currentPage = ref(1);
const postsPerPage = 10;
const popularTopics = ref<PopularTopic[]>([]);
const totalPostCount = ref<number | null>(null);

// Fetch forum categories using forumCategoryApi
const fetchCategories = async () => {
  try {
    const response = await forumCategoryApi.getAllCategories();
    // Assuming the API returns data in response.data.result
    categories.value = response.data.result;
  } catch (error) {
    console.error("Lỗi khi lấy danh mục:", error);
    // Fallback data remains the same
    categories.value = [
      {
        id: 1,
        name: "Thông báo",
        description: "Cập nhật mới nhất từ GameTradeZone",
        iconClass: "fa-bullhorn",
        postCount: 1,
      },
      {
        id: 2,
        name: "Game",
        description: "Thảo luận về game",
        iconClass: "fa-gamepad",
        postCount: 0,
      },
      {
        id: 3,
        name: "Hỗ trợ kỹ thuật",
        description: "Giải đáp thắc mắc và hỗ trợ kỹ thuật",
        iconClass: "fa-wrench",
        postCount: 2,
      },
      {
        id: 4,
        name: "Thảo luận chung",
        description: "Nơi thảo luận tất cả các chủ đề khác",
        iconClass: "fa-comments",
        postCount: 5,
      },
    ];
  }
};

const fetchTotalPostCount = async () => {
  try {
    const response = await forumApi.GetPostCount();
    console.log("Total Post Count API Response:", response.data); // Log the response
    if (
      response.data &&
      response.data.result &&
      typeof response.data.result.data === "number"
    ) {
      totalPostCount.value = response.data.result.data;
    } else {
      console.warn(
        "Unexpected total post count API response structure:",
        response.data
      );
      totalPostCount.value = 0; // Set to 0 or handle error appropriately
    }
  } catch (error) {
    console.error("Lỗi khi lấy tổng số bài viết:", error);
    totalPostCount.value = 0; // Set to 0 or handle error appropriately
  }
};

const userStats = ref({
  totalUsers: 0,
  newUsersToday: 0,
  newUsersByDay: [],
});

const fetchUserStats = async () => {
  try {
    // Keep using axios or create a dedicated user stats API module
    const API_USER_STATS_URL = `${API_BASE_URL}/AccountGame/Get-All-User-Data-Stat`;
    const response = await axios.get(API_USER_STATS_URL);

    // Kiểm tra dữ liệu trả về
    console.log("Dữ liệu thống kê:", response.data);

    // Gán dữ liệu đúng vào userStats
    const stats = response.data.result?.data || {};
    userStats.value = {
      totalUsers: stats.totalUsers || 0,
      newUsersToday: stats.newUsersToday || 0,
      newUsersByDay: stats.newUsersByDay || [],
    };
  } catch (error) {
    console.error("Lỗi khi lấy thống kê người dùng:", error);
  }
};

// Fetch posts using forumApi
const fetchPosts = async (categoryId: number | null = null) => {
  try {
    let response;
    if (categoryId) {
      console.log("Gọi API: getPostsByCategory", categoryId);
      response = await forumApi.getPostsByCategory(String(categoryId)); // Ensure categoryId is string if required
    } else {
      console.log("Gọi API: getLatestPosts");
      response = await forumApi.getLatestPosts();
    }
    console.log("Dữ liệu trả về:", response.data);
    // Assuming the API returns data in response.data.result
    posts.value = response.data.result.map((post: PostResponse) => ({
      id: post.id,
      caption: post.caption,
      content: post.content,
      userName: post.user?.fullName || post.user?.userName || "Ẩn danh",
      createdAt: new Date(post.createdDate).toLocaleString(),
      categoryId: post.categoryId,
      commentsCount: post.commentsCount,
      likesCount: post.likesCount,
      views: post.views || 0,
    }));
  } catch (error) {
    console.error("Lỗi khi lấy bài viết:", error);
    posts.value = []; // Clear posts on error or handle appropriately
  }
};

const fetchPopularTopics = async () => {
  try {
    const response = await forumApi.GetAllCategoryPostCount();
    console.log("Popular Topics API Response:", response.data); // Log the response
    if (response.data && response.data.result && response.data.result.data) {
      // Sort by postCount ascending
      const sortedTopics = response.data.result.data.sort(
        (a: PopularTopic, b: PopularTopic) => b.postCount - a.postCount
      );
      popularTopics.value = sortedTopics;
    } else {
      console.warn(
        "Unexpected popular topics API response structure:",
        response.data
      );
      popularTopics.value = [];
    }
  } catch (error) {
    console.error("Lỗi khi lấy chủ đề phổ biến:", error);
    popularTopics.value = []; // Set empty on error
    // Optionally add fallback static data here if needed
  }
};

// Handle category selection
const selectCategory = (categoryId: number) => {
  selectedCategory.value = categoryId;
  currentPage.value = 1;
  fetchPosts(categoryId);
};

// Reset category filter
const resetCategory = () => {
  selectedCategory.value = null;
  currentPage.value = 1;
  fetchPosts();
};

// Filter posts by search query
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

// Paginate posts
const paginatedPosts = computed(() => {
  const start = (currentPage.value - 1) * postsPerPage;
  const end = start + postsPerPage;
  return filteredPosts.value.slice(start, end);
});

// Calculate total pages
const totalPages = computed(() => {
  return Math.ceil(filteredPosts.value.length / postsPerPage);
});

// Go to specific page
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

// Navigate to post page
const goToPost = (postId: number) => {
  router.push(`/post/${postId}`);
};

// Initialize data on component mount
onMounted(() => {
  fetchCategories();
  fetchPosts();
  fetchUserStats();
  fetchPopularTopics();
  fetchTotalPostCount();
});
</script>

<template>
  <div class="forum-container">
    <!-- Sidebar -->
    <aside class="sidebar">
      <!-- <h2 class="sidebar-logo">GTZ</h2> -->
      <ul class="nav-links">
        <li>
          <router-link to="/" class="nav-link">
            <i class="fas fa-home"></i>
            <span>Trang chủ</span>
          </router-link>
        </li>
        <li>
          <router-link to="/forums" class="nav-link active">
            <i class="fas fa-comments"></i>
            <span>Diễn đàn</span>
          </router-link>
        </li>
        <li>
          <router-link to="/support" class="nav-link">
            <i class="fas fa-headset"></i>
            <span>Hỗ trợ</span>
          </router-link>
        </li>
      </ul>

      <div class="sidebar-footer">
        <router-link to="/add-post">
          <button class="new-post-btn">
            <i class="fas fa-plus-circle"></i> Tạo bài viết mới
          </button>
        </router-link>
        <router-link to="/all-posts">
          <button class="my-posts-btn">
            <i class="fas fa-user-posts"></i> Bài viết của tôi
          </button>
        </router-link>
      </div>
    </aside>

    <!-- Main content -->
    <div class="forum-main">
      <div class="forum-header">
        <div class="forum-title-container">
          <h1 class="forum-title">Diễn Đàn GameTradeZone</h1>
          <p class="forum-subtitle">Nơi giao lưu và chia sẻ về game</p>
        </div>

        <!-- Search bar -->
        <div class="search-container">
          <i class="fas fa-search search-icon"></i>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Tìm kiếm chủ đề, bài viết..."
            class="search-input"
          />
        </div>
      </div>

      <!-- Forum filter tabs -->
      <div class="forum-tabs">
        <button
          class="tab-btn"
          :class="{ active: selectedCategory === null }"
          @click="resetCategory"
        >
          <i class="fas fa-th-large"></i> Tất cả
        </button>
        <button
          v-for="category in categories.slice(0, 4)"
          :key="category.id"
          class="tab-btn"
          :class="{ active: selectedCategory === category.id }"
          @click="selectCategory(category.id)"
        >
          <i :class="`fas ${category.iconClass}`"></i> {{ category.name }}
        </button>
      </div>

      <!-- Forum categories -->
      <div class="section-header">
        <h2 class="section-title">Chủ đề</h2>
        <div class="view-options">
          <span class="view-option active"
            ><i class="fas fa-th-large"></i
          ></span>
          <span class="view-option"><i class="fas fa-list"></i></span>
        </div>
      </div>

      <div class="category-grid">
        <div
          v-for="category in categories"
          :key="category.id"
          class="category-card"
          @click="selectCategory(category.id)"
        >
          <div class="category-header">
            <div class="category-icon-container">
              <i :class="`fas ${category.iconClass}`" class="category-icon"></i>
            </div>
            <div class="category-stats">
              <span class="post-badge">{{ category.postCount }}</span>
            </div>
          </div>
          <div class="category-content">
            <h3 class="category-name">{{ category.name }}</h3>
            <p class="category-description">{{ category.description }}</p>
          </div>
          <div class="category-footer">
            <span class="view-more"
              >Xem thêm <i class="fas fa-chevron-right"></i
            ></span>
          </div>
        </div>
      </div>

      <!-- Latest posts section -->
      <div class="section-header">
        <h2 class="section-title">
          {{
            selectedCategory !== null
              ? "Bài viết trong chủ đề"
              : "Bài viết mới nhất"
          }}
        </h2>
        <div v-if="selectedCategory !== null" class="badge-container">
          <span class="category-badge" @click="resetCategory">
            {{ categories.find((c) => c.id === selectedCategory)?.name }}
            <i class="fas fa-times"></i>
          </span>
        </div>
      </div>

      <div v-if="posts.length === 0" class="empty-state">
        <i class="fas fa-comment-slash"></i>
        <p>Chưa có bài viết nào.</p>
        <button class="create-post-btn">Tạo bài viết đầu tiên</button>
      </div>

      <div v-else class="post-list">
        <div
          v-for="post in paginatedPosts"
          :key="post.id"
          class="post-card"
          @click="goToPost(post.id)"
        >
          <div class="post-card-header">
            <div class="post-author-avatar">
              {{ post.userName.charAt(0).toUpperCase() }}
            </div>
            <div class="post-meta">
              <span class="post-author">{{ post.userName }}</span>
              <span class="post-date">
                <i class="far fa-clock"></i> {{ post.createdAt }}
              </span>
            </div>
          </div>
          <h3 class="post-title">{{ post.caption }}</h3>
          <p class="post-preview">
            {{ post.content.substring(0, 100)
            }}{{ post.content.length > 100 ? "..." : "" }}
          </p>
          <div class="post-footer">
            <div class="post-category-badge">
              <i
                :class="`fas ${
                  categories.find((c) => c.id === post.categoryId)?.iconClass ||
                  'fa-folder'
                }`"
              ></i>
              {{
                categories.find((c) => c.id === post.categoryId)?.name ||
                "Chủ đề"
              }}
            </div>
            <div class="post-stats">
              <span
                ><i class="far fa-comment"></i> {{ post.commentsCount }}</span
              >
              <span><i class="far fa-heart"></i> {{ post.likesCount }}</span>
              <span><i class="far fa-eye"></i> {{ post.views }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="pagination">
        <button
          @click="goToPage(currentPage - 1)"
          :disabled="currentPage === 1"
          class="page-btn prev"
        >
          <i class="fas fa-chevron-left"></i>
        </button>
        <span class="page-info">{{ currentPage }} / {{ totalPages }}</span>
        <button
          @click="goToPage(currentPage + 1)"
          :disabled="currentPage === totalPages"
          class="page-btn next"
        >
          <i class="fas fa-chevron-right"></i>
        </button>
      </div>
    </div>

    <!-- Stats sidebar -->
    <aside class="stats-sidebar">
      <div class="stats-card">
        <h3 class="stats-title">THỐNG KÊ</h3>
        <ul class="stats-list">
          <li>
            <i class="fas fa-comments"></i>
            <span class="stat-label">Tổng bài viết:</span>
            <span class="stat-value">{{
              totalPostCount !== null ? totalPostCount : "..."
            }}</span>
          </li>
          <li>
            <i class="fas fa-user-friends"></i>
            <span class="stat-label">Tổng thành viên:</span>
            <span class="stat-value">{{ userStats.totalUsers }}</span>
          </li>
          <li>
            <i class="fas fa-user-plus"></i>
            <span class="stat-label">Thành viên mới hôm nay:</span>
            <span class="stat-value">{{ userStats.newUsersToday }}</span>
          </li>
        </ul>
      </div>

      <div class="popular-topics-card">
        <h3 class="sidebar-subtitle">CHỦ ĐỀ PHỔ BIẾN</h3>
        <ul class="topic-list">
          <!-- Add a loading/empty state if desired -->
          <li v-if="popularTopics.length === 0">
            <span class="topic-name">Đang tải...</span>
          </li>
          <li v-for="topic in popularTopics" :key="topic.id">
            <a href="#" @click.prevent="selectCategory(topic.id)">
              <span class="topic-name">{{ topic.name }}</span>
              <span class="topic-posts">{{ topic.postCount }}</span>
            </a>
          </li>
        </ul>
      </div>

      <div class="community-card">
        <h3 class="sidebar-subtitle">CỘNG ĐỒNG</h3>
        <div class="social-links">
          <a href="#" class="social-link"><i class="fab fa-discord"></i></a>
          <a href="#" class="social-link"><i class="fab fa-facebook"></i></a>
          <a href="#" class="social-link"><i class="fab fa-twitter"></i></a>
          <a href="#" class="social-link"><i class="fab fa-youtube"></i></a>
        </div>
      </div>
    </aside>
  </div>
</template>

<style scoped>
/* Ensure html/body allow scrolling and have full height */
/* You might need to place this in a global CSS file (e.g., main.css or index.css)
   if not already present, but including it here for completeness of the concept. */
html,
body {
  height: 100%;
  margin: 0;
  padding: 0;
  /* Ensure no hidden overflow is blocking sticky */
  overflow: visible !important; /* Use !important cautiously, only if necessary */
}

/* Main container for the forum view */
.forum-container {
  display: flex;
  /* Use min-height to allow content to grow, but ensure it takes at least viewport height */
  min-height: 100vh;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
  color: #f0f0f0;
  font-family: "Inter", "Arial", sans-serif;
  /* Crucially, the container itself should NOT scroll vertically on desktop */
  /* overflow: hidden; */ /* Avoid setting overflow here unless specifically needed */
}

/* Left Sidebar */
.sidebar {
  width: 260px;
  background: rgba(13, 27, 42, 0.9);
  padding: 30px 0;
  border-right: 1px solid rgba(0, 179, 224, 0.2);
  display: flex;
  flex-direction: column;
  position: sticky; /* Make it sticky */
  top: 0; /* Stick to the top */
  height: 100vh; /* Take full viewport height */
  overflow-y: auto; /* Allow internal scrolling if content overflows */
  flex-shrink: 0; /* Prevent shrinking */
}

/* Add the bottom gradient effect back if desired */
.sidebar::after {
  content: "";
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 3px;
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  box-shadow: 0 0 10px rgba(0, 204, 255, 0.5);
}

/* Main Content Area - This MUST be the scrollable part */
.forum-main {
  flex: 1; /* Take remaining horizontal space */
  padding: 30px;
  background: transparent;
  overflow-y: auto; /* THIS makes the main content scrollable */
  /* min-width: 0; */ /* Add if needed to prevent flexbox overflow issues */
  /* No height or max-height needed here, let content dictate height */
}

/* Right Stats Sidebar */
.stats-sidebar {
  width: 280px;
  background: rgba(13, 27, 42, 0.9);
  padding: 30px 20px;
  border-left: 1px solid rgba(0, 179, 224, 0.2);
  display: flex;
  flex-direction: column;
  gap: 25px;
  position: sticky; /* Make it sticky */
  top: 0; /* Stick to the top */
  height: 100vh; /* Take full viewport height */
  overflow-y: auto; /* Allow internal scrolling if content overflows */
  flex-shrink: 0; /* Prevent shrinking */
}

/* Add the bottom gradient effect back if desired */
.stats-sidebar::after {
  content: "";
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 3px;
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  box-shadow: 0 0 10px rgba(0, 204, 255, 0.5);
}

/* Ensure content within stats sidebar doesn't shrink unexpectedly */
.stats-sidebar > * {
  flex-shrink: 0;
}

.sidebar-logo {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 40px;
  color: #00b3e0; /* Cyan for logo */
  text-align: center;
  text-shadow: 0 0 15px rgba(0, 204, 255, 0.5); /* Glow effect */
  letter-spacing: 2px;
}

.nav-links {
  list-style-type: none;
  padding: 0;
  margin-bottom: auto;
}

.nav-links li {
  margin-bottom: 5px;
}

.nav-link {
  display: flex;
  align-items: center;
  color: #e0e0e0; /* Secondary text */
  text-decoration: none;
  font-size: 1rem;
  padding: 12px 25px;
  border-left: 3px solid transparent;
  transition: all 0.3s ease;
}

.nav-link i {
  margin-right: 12px;
  width: 20px;
  text-align: center;
  font-size: 1.1rem;
  color: #00b3e0; /* Cyan icons */
}

.nav-link:hover {
  background: rgba(0, 179, 224, 0.3); /* Cyan hover */
  color: #f8f8f8; /* Brighter text on hover */
}

.nav-link.active {
  color: #f8f8f8;
  background: linear-gradient(45deg, #00b3e0, #ff00ff); /* Gradient like home */
  border-left: 3px solid #00b3e0;
}

.sidebar-footer {
  padding: 0 20px;
  margin-top: 20px;
}

.new-post-btn {
  width: 100%;
  background: linear-gradient(45deg, #00b3e0, #ff00ff); /* Gradient button */
  color: #f8f8f8;
  border: none;
  border-radius: 8px;
  padding: 12px 15px;
  font-weight: 600;
  font-size: 0.9rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  box-shadow: 0 0 10px rgba(0, 204, 255, 0.5); /* Cyan glow */
}

.new-post-btn i {
  margin-right: 8px;
  font-size: 1rem;
}

.new-post-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0, 204, 255, 0.7);
}

.my-posts-btn {
  width: 100%;
  background: linear-gradient(
    45deg,
    #ff00ff,
    #00b3e0
  ); /* Gradient ngược lại để phân biệt */
  color: #f8f8f8;
  border: none;
  border-radius: 8px;
  padding: 12px 15px;
  font-weight: 600;
  font-size: 0.9rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  box-shadow: 0 0 10px rgba(255, 0, 255, 0.5); /* Magenta glow */
  margin-top: 10px; /* Thêm khoảng cách phía trên */
}

.my-posts-btn i {
  margin-right: 8px;
  font-size: 1rem;
}

.my-posts-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(255, 0, 255, 0.7);
}

/* Forum header */
.forum-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
  gap: 20px;
}

.forum-title-container {
  flex: 1;
}

.forum-title {
  color: #00b3e0; /* Cyan title */
  font-size: 1.8rem;
  margin: 0 0 5px 0;
  font-weight: 700;
  letter-spacing: 0.5px;
  text-shadow: 0 0 10px rgba(0, 204, 255, 0.5); /* Glow effect */
}

.forum-subtitle {
  color: #b0b0b0; /* Tertiary text */
  margin: 0;
  font-size: 0.9rem;
}

/* Search bar */
.search-container {
  position: relative;
  min-width: 300px;
}

.search-input {
  width: 100%;
  background: rgba(28, 37, 38, 0.9); /* Card-like background */
  border: 1px solid #00b3e0; /* Cyan border */
  border-radius: 30px;
  padding: 12px 20px;
  padding-left: 45px;
  color: #f0f0f0;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #ff00ff; /* Magenta on focus */
  box-shadow: 0 0 10px rgba(0, 204, 255, 0.5); /* Glow effect */
}

.search-icon {
  position: absolute;
  left: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: #00b3e0; /* Cyan icon */
  font-size: 1rem;
}

/* Forum tabs */
.forum-tabs {
  display: flex;
  gap: 10px;
  margin-bottom: 30px;
  overflow-x: auto;
  padding-bottom: 5px;
}

.tab-btn {
  background: rgba(28, 37, 38, 0.9);
  border: 1px solid #00b3e0;
  color: #e0e0e0;
  padding: 10px 20px;
  border-radius: 20px;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
  display: flex;
  align-items: center;
  gap: 8px;
}

.tab-btn i {
  font-size: 0.85rem;
  color: #00b3e0;
}

.tab-btn:hover {
  background: rgba(0, 179, 224, 0.3);
  color: #f8f8f8;
  box-shadow: 0 0 10px rgba(0, 204, 255, 0.5);
}

.tab-btn.active {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  font-weight: 600;
  box-shadow: 0 0 10px rgba(0, 204, 255, 0.5);
}

/* Section header */
.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.section-title {
  color: #00b3e0;
  font-size: 1.3rem;
  margin: 0;
  font-weight: 600;
  text-shadow: 0 0 5px rgba(0, 204, 255, 0.3);
}

.view-options {
  display: flex;
  gap: 10px;
}

.view-option {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 35px;
  height: 35px;
  background: rgba(28, 37, 38, 0.9);
  border-radius: 8px;
  color: #b0b0b0;
  cursor: pointer;
  transition: all 0.2s ease;
}

.view-option:hover,
.view-option.active {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.5);
}

.badge-container {
  display: flex;
  gap: 10px;
}

.category-badge {
  background: rgba(0, 179, 224, 0.3);
  color: #00b3e0;
  padding: 5px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.category-badge i {
  font-size: 0.7rem;
}

.category-badge:hover {
  background: rgba(0, 179, 224, 0.5);
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.5);
}

/* Category grid */
.category-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 20px;
  margin-bottom: 40px;
}

.category-card {
  background: rgba(28, 37, 38, 0.9);
  border-radius: 12px;
  overflow: hidden;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  cursor: pointer;
  display: flex;
  flex-direction: column;
  border: 1px solid #00b3e0;
}

.category-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2), 0 0 15px rgba(0, 204, 255, 0.5);
  border: 1px solid #ff00ff;
}

.category-header {
  padding: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.category-icon-container {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 50px;
  height: 50px;
  background: rgba(0, 179, 224, 0.2);
  border-radius: 10px;
}

.category-icon {
  color: #00b3e0;
  font-size: 1.5rem;
  text-shadow: 0 0 5px rgba(0, 204, 255, 0.3);
}

.category-stats {
  display: flex;
  align-items: center;
}

.post-badge {
  background: rgba(0, 179, 224, 0.3);
  color: #00b3e0;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
}

.category-content {
  padding: 0 20px 20px;
  flex: 1;
}

.category-name {
  color: #00b3e0;
  font-size: 1.2rem;
  margin: 0 0 10px 0;
  font-weight: 600;
  text-shadow: 0 0 5px rgba(0, 204, 255, 0.3);
}

.category-description {
  color: #b0b0b0;
  font-size: 0.9rem;
  margin: 0;
  line-height: 1.4;
}

.category-footer {
  background: rgba(0, 0, 0, 0.2);
  padding: 15px 20px;
  text-align: right;
}

.view-more {
  color: #00b3e0;
  font-size: 0.85rem;
  display: inline-flex;
  align-items: center;
  gap: 5px;
  transition: all 0.2s ease;
}

.view-more i {
  font-size: 0.7rem;
  transition: transform 0.2s ease;
}

.category-card:hover .view-more i {
  transform: translateX(3px);
}

/* Empty state */
.empty-state {
  background: rgba(28, 37, 38, 0.9);
  border-radius: 12px;
  padding: 40px 20px;
  text-align: center;
  margin-bottom: 40px;
  border: 1px solid #00b3e0;
}

.empty-state i {
  font-size: 3rem;
  color: #00b3e0;
  margin-bottom: 15px;
}

.empty-state p {
  color: #b0b0b0;
  margin-bottom: 20px;
}

.create-post-btn {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  border: none;
  border-radius: 8px;
  padding: 10px 20px;
  font-weight: 600;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 0 10px rgba(0, 204, 255, 0.5);
}

.create-post-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0, 204, 255, 0.7);
}

/* Post list */
.post-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.post-card {
  background: rgba(28, 37, 38, 0.9);
  border-radius: 12px;
  padding: 20px;
  cursor: pointer;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  border: 1px solid #00b3e0;
  display: flex;
  flex-direction: column;
}

.post-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2), 0 0 15px rgba(0, 204, 255, 0.5);
  border: 1px solid #ff00ff;
}

.post-card-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 15px;
}

.post-author-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: #00b3e0;
  color: #f8f8f8;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.1rem;
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.5);
}

.post-meta {
  display: flex;
  flex-direction: column;
}

.post-author {
  color: #e0e0e0;
  font-weight: 600;
  font-size: 0.9rem;
}

.post-date {
  color: #b0b0b0;
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  gap: 5px;
}

.post-date i {
  color: #00b3e0;
}

.post-title {
  color: #00b3e0;
  font-size: 1.1rem;
  margin: 0 0 10px 0;
  line-height: 1.3;
  text-shadow: 0 0 5px rgba(0, 204, 255, 0.3);
}

.post-preview {
  color: #b0b0b0;
  font-size: 0.9rem;
  margin: 0 0 15px 0;
  line-height: 1.5;
  flex: 1;
}

.post-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 15px;
}

.post-category-badge {
  background: rgba(0, 179, 224, 0.2);
  color: #00b3e0;
  padding: 5px 10px;
  border-radius: 20px;
  font-size: 0.75rem;
  display: flex;
  align-items: center;
  gap: 5px;
}

.post-category-badge i {
  font-size: 0.7rem;
}

.post-stats {
  display: flex;
  gap: 10px;
}

.post-stats span {
  color: #b0b0b0;
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  gap: 5px;
}

.post-stats i {
  font-size: 0.85rem;
  color: #00b3e0;
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 15px;
  margin-bottom: 20px;
}

.page-btn {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(28, 37, 38, 0.9);
  border: 1px solid #00b3e0;
  color: #e0e0e0;
  cursor: pointer;
  transition: all 0.2s ease;
}

.page-btn:hover:not(:disabled) {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.5);
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-info {
  color: #b0b0b0;
  font-size: 0.9rem;
}

/* User card */
.user-card {
  display: flex;
  align-items: center;
  gap: 15px;
  padding-bottom: 20px;
  border-bottom: 1px solid rgba(0, 179, 224, 0.3);
}

.user-avatar {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background: #00b3e0;
  color: #f8f8f8;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.3rem;
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.5);
}

.user-info {
  flex: 1;
}

.user-name {
  color: #e0e0e0;
  font-size: 1.1rem;
  margin: 0 0 5px 0;
}

.user-status {
  color: #00b3e0;
  font-size: 0.8rem;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 5px;
}

.user-status::before {
  content: "";
  display: block;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #00b3e0;
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.5);
}

/* Stats card */
.stats-card,
.popular-topics-card,
.community-card {
  background: rgba(28, 37, 38, 0.9);
  border-radius: 12px;
  padding: 20px;
  border: 1px solid #00b3e0;
}

.stats-title {
  color: #00b3e0;
  font-size: 1.1rem;
  margin: 0 0 15px 0;
  font-weight: 600;
  text-shadow: 0 0 5px rgba(0, 204, 255, 0.3);
}

.stats-list {
  list-style-type: none;
  padding: 0;
  margin: 0;
}

.stats-list li {
  display: flex;
  align-items: center;
  margin-bottom: 12px;
}

.stats-list li:last-child {
  margin-bottom: 0;
}

.stats-list li i {
  color: #00b3e0;
  width: 20px;
  margin-right: 10px;
  font-size: 0.9rem;
}

.stat-label {
  color: #b0b0b0;
  flex: 1;
  font-size: 0.85rem;
}

.stat-value {
  color: #e0e0e0;
  font-weight: 600;
  font-size: 0.85rem;
}

.stat-value.highlight {
  color: #00b3e0;
  text-shadow: 0 0 5px rgba(0, 204, 255, 0.3);
}

.sidebar-subtitle {
  color: #00b3e0;
  font-size: 1.1rem;
  margin: 0 0 15px 0;
  font-weight: 600;
  text-shadow: 0 0 5px rgba(0, 204, 255, 0.3);
}

.topic-list {
  list-style-type: none;
  padding: 0;
  margin: 0;
}

.topic-list li {
  margin-bottom: 10px;
}

.topic-list li:last-child {
  margin-bottom: 0;
}

.topic-list a {
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: #b0b0b0;
  text-decoration: none;
  padding: 8px 0;
  transition: color 0.2s ease;
  border-bottom: 1px solid rgba(0, 179, 224, 0.3);
}

.topic-list a:hover {
  color: #00b3e0;
}

.topic-name {
  font-size: 0.85rem;
}

.topic-posts {
  background: rgba(0, 179, 224, 0.2);
  color: #00b3e0;
  padding: 2px 8px;
  border-radius: 20px;
  font-size: 0.75rem;
}

/* Social links */
.social-links {
  display: flex;
  justify-content: space-between;
  margin-top: 10px;
}

.social-link {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 179, 224, 0.2);
  color: #00b3e0;
  border-radius: 50%;
  transition: all 0.2s ease;
  font-size: 1.1rem;
}

.social-link:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  transform: translateY(-3px);
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.5);
}

/* Responsive design */
@media (max-width: 1300px) {
  .post-list {
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  }
}

@media (max-width: 1100px) {
  .stats-sidebar {
    display: none;
  }
}

@media (max-width: 900px) {
  .category-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .forum-container {
    flex-direction: column;
    /* Allow the container itself to scroll on mobile */
    overflow-y: auto;
    min-height: unset; /* Remove min-height if not needed */
    height: auto; /* Allow height to be determined by content */
  }

  .sidebar {
    width: 100%;
    height: auto; /* Auto height on mobile */
    padding: 15px;
    position: static; /* IMPORTANT: Disable sticky */
    border-right: none;
    border-bottom: 1px solid rgba(0, 179, 224, 0.3);
    overflow-y: visible; /* Disable internal scroll */
    flex-shrink: 1; /* Allow shrinking */
  }

  .sidebar::after {
    display: none; /* Hide gradient border on mobile */
  }

  .forum-main {
    /* Disable scrolling for main content on mobile, let container scroll */
    overflow-y: visible;
    flex: none; /* Reset flex property */
  }

  .stats-sidebar {
    /* Already hidden by a previous rule, but ensure it's static if shown */
    position: static;
    width: 100%;
    height: auto;
    overflow-y: visible;
    border-left: none;
    border-top: 1px solid rgba(0, 179, 224, 0.2);
    flex-shrink: 1;
  }
  .stats-sidebar::after {
    display: none; /* Hide gradient border on mobile */
  }

  .sidebar-logo {
    margin-bottom: 20px;
  }

  .nav-links {
    display: flex;
    flex-wrap: wrap;
    gap: 5px;
  }

  .nav-link {
    flex-direction: column;
    padding: 10px;
    border-left: none;
    border-radius: 8px;
    font-size: 0.8rem;
    flex: 1;
    min-width: 60px;
  }

  .nav-link i {
    margin-right: 0;
    margin-bottom: 5px;
  }

  .sidebar-footer {
    display: none;
  }

  .forum-main {
    padding: 15px;
  }

  .forum-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 15px;
  }

  .search-container {
    width: 100%;
    min-width: auto;
  }

  .post-list {
    grid-template-columns: 1fr;
  }
}
</style>

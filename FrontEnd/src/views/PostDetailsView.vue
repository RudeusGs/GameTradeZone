<script setup lang="ts">
import { ref, onMounted } from "vue";
import axios from "axios";
import { useRoute } from "vue-router";

// 🌐 API URL
const API_BASE_URL = "https://localhost:7232/api";

// 📌 Lấy postId từ route
const route = useRoute();
const postId = ref<number>(Number(route.params.id));

// 📝 Kiểu dữ liệu của Post và Comment
interface User {
  id: number;
  userName?: string;
  fullName?: string;
  avatar?: string;
}

interface Post {
  id: number;
  caption: string;
  content: string;
  createdDate: string;
  likesCount: number;
  imageUrl?: string;
  user?: User;
}

interface Comment {
  id: number;
  postId: number;
  user?: User;
  content: string;
  createdDate: string;
}

// 📌 State lưu trữ bài viết & bình luận
const post = ref<Post | null>(null);
const comments = ref<Comment[]>([]);
const newComment = ref<string>("");

// 🟢 Gọi API lấy chi tiết bài viết
const fetchPost = async () => {
  try {
    console.log("🔍 Gọi API:", `${API_BASE_URL}/posts/get/${postId.value}`);
    const response = await axios.get<{ result: Post }>(
      `${API_BASE_URL}/posts/get/${postId.value}`
    );
    post.value = response.data.result;
  } catch (error) {
    console.error("❌ Lỗi khi lấy bài viết:", error);
  }
};

// 🟢 Gọi API lấy danh sách bình luận
const fetchComments = async () => {
  try {
    console.log(
      "🔍 Gọi API:",
      `${API_BASE_URL}/PostInfo/comments/${postId.value}`
    );
    const response = await axios.get<{ result: Comment[] }>(
      `${API_BASE_URL}/PostInfo/comments/${postId.value}`
    );
    comments.value = response.data.result;
  } catch (error) {
    console.error("❌ Lỗi khi lấy bình luận:", error);
  }
};

// ❤️ Like bài viết
const likePost = async () => {
  try {
    await axios.post(`${API_BASE_URL}/PostInfo/like/${postId.value}`);
    post.value!.likesCount += 1; // Tăng số lượng like ngay trên giao diện
  } catch (error) {
    console.error("❌ Lỗi khi like bài viết:", error);
  }
};

// ✍️ Gửi bình luận mới
const submitComment = async () => {
  if (!newComment.value.trim()) return;
  try {
    await axios.post(`${API_BASE_URL}/PostInfo/comment`, {
      postId: postId.value,
      content: newComment.value,
    });
    newComment.value = ""; // Xóa nội dung sau khi gửi
    fetchComments(); // Cập nhật lại danh sách bình luận
  } catch (error) {
    console.error("❌ Lỗi khi gửi bình luận:", error);
  }
};

// 🔄 Gọi API khi component mount
onMounted(() => {
  fetchPost();
  fetchComments();
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
      <!-- Breadcrumbs -->
      <nav class="breadcrumbs">
        <router-link to="/forums">FORUMS</router-link> »
        <router-link to="/forums/category">CATEGORY</router-link> »
        <span>{{ post?.caption || "Bài viết" }}</span>
      </nav>

      <!-- Tiêu đề bài viết -->
      <header class="forum-header">
        <h1 class="forum-title">{{ post?.caption || "Đang tải..." }}</h1>
      </header>

      <!-- Nội dung bài viết -->
      <div v-if="post" class="post-content">
        <div class="post-user">
          <img
            v-if="post.user?.avatar"
            :src="post.user.avatar"
            class="avatar"
          />
          <div class="user-info">
            <span class="username">{{
              post.user?.fullName || post.user?.userName || "Ẩn danh"
            }}</span>
            <span class="post-meta">
              🕒 {{ new Date(post.createdDate).toLocaleString() }}
            </span>
          </div>
        </div>

        <!-- Nội dung bài viết -->
        <div class="post-body">
          <p class="post-text">{{ post.content }}</p>
          <div class="post-image-container" v-if="post.imageUrl">
            <img
              :src="post.imageUrl"
              alt="Hình ảnh bài viết"
              class="post-image"
            />
          </div>
        </div>

        <!-- ❤️ Nút like -->
        <button class="like-button" @click="likePost">
          ❤️ Thích ({{ post.likesCount }})
        </button>

        <!-- 💬 Danh sách bình luận -->
        <div v-if="comments.length > 0" class="comments-section">
          <div v-for="comment in comments" :key="comment.id" class="comment">
            <div class="comment-user">
              <img
                v-if="comment.user?.avatar"
                :src="comment.user.avatar"
                class="avatar"
              />
              <div class="user-info">
                <span class="username">{{
                  comment.user?.fullName || comment.user?.userName || "Ẩn danh"
                }}</span>
                <span class="comment-meta">
                  🕒 {{ new Date(comment.createdDate).toLocaleString() }}
                </span>
              </div>
            </div>
            <p class="comment-text">{{ comment.content }}</p>
          </div>
        </div>
        <div v-else class="no-comments">Chưa có bình luận nào.</div>

        <!-- ✍️ Nhập bình luận -->
        <div class="comment-box">
          <textarea
            v-model="newComment"
            placeholder="Nhập bình luận..."
            class="comment-input"
          ></textarea>
          <button class="submit-button" @click="submitComment">Gửi</button>
        </div>
      </div>

      <!-- 🔄 Loading -->
      <p v-else class="loading-text">Đang tải...</p>

      <!-- Similar Threads -->
      <section class="similar-threads">
        <h2 class="section-title">Similar Threads</h2>
        <div class="thread-list">
          <!-- Dữ liệu tĩnh, có thể thay bằng API nếu cần -->
          <div class="thread-item">
            <span class="thread-label">[Planned feature] Soon™</span>
            <span class="thread-meta">Dec 11, 2024 • kouyo</span>
          </div>
          <div class="thread-item">
            <span class="thread-label"
              >Change "Popular New Titles" to "Popular Weekly Titles" or
              something along those lines [Planned]</span
            >
            <span class="thread-meta">Feb 19, 2024 • kabachi</span>
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
  background: #101020; /* Giữ màu nền của Image 2 */
  color: #e0e0e0;
  font-family: "Arial", sans-serif;
}

/* Sidebar */
.sidebar {
  width: 200px;
  background: #1a1a1a; /* Tối hơn nền chính một chút */
  padding: 20px;
  border-right: 1px solid #333;
}

.sidebar-title {
  font-size: 1.8rem;
  font-weight: bold;
  margin-bottom: 20px;
  color: #00ffff; /* Màu chữ của Image 2 */
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
  color: #ff5555; /* Màu hover giống Image 2 */
}

/* Nội dung chính */
.forum-main {
  flex: 1;
  padding: 20px;
  background: #101020;
}

/* Breadcrumbs */
.breadcrumbs {
  font-size: 0.9rem;
  margin-bottom: 10px;
}

.breadcrumbs a,
.breadcrumbs span {
  color: #ff4500; /* Màu cam giống MangaDex */
  text-decoration: none;
}

.breadcrumbs a:hover {
  color: #ff5555; /* Màu hover giống Image 2 */
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

/* Người đăng bài */
.post-user {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 15px;
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid #00ffff; /* Giữ màu viền của Image 2 */
}

.user-info {
  display: flex;
  flex-direction: column;
}

.username {
  font-weight: bold;
  color: #00ffff; /* Màu tên người dùng giống Image 2 */
}

.post-meta,
.comment-meta {
  font-size: 0.8rem;
  color: #888;
}

/* Nội dung bài viết */
.post-body {
  background: #222; /* Tối hơn nền chính một chút */
  padding: 15px;
  border-radius: 5px;
  margin-bottom: 15px;
}

.post-text {
  font-size: 1rem;
  line-height: 1.5;
  margin: 0;
}

.post-image-container {
  margin-top: 15px;
}

.post-image {
  max-width: 100%;
  height: auto;
  border-radius: 5px;
  border: 2px solid #00ffff; /* Viền màu giống Image 2 */
}

/* Nút like */
.like-button {
  background: #ff5555; /* Màu nút của Image 2 */
  color: white;
  border: none;
  padding: 10px 20px;
  font-size: 1rem;
  font-weight: bold;
  border-radius: 5px;
  cursor: pointer;
  margin-bottom: 20px;
}

.like-button:hover {
  background: #ff3333; /* Màu hover của Image 2 */
}

/* Bình luận */
.comments-section {
  margin-top: 20px;
}

.comment {
  padding: 10px;
  margin-top: 10px;
  background: #222;
  border-radius: 5px;
}

.comment-user {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 5px;
}

.comment-text {
  font-size: 0.9rem;
  margin: 0;
}

/* Nhập bình luận */
.comment-box {
  margin-top: 20px;
}

.comment-input {
  width: 100%;
  height: 80px;
  padding: 10px;
  border-radius: 5px;
  border: none;
  background: #1a1a1a;
  color: white;
  resize: vertical;
  margin-bottom: 10px;
}

.submit-button {
  background: #00ffff; /* Màu nút của Image 2 */
  color: black;
  border: none;
  padding: 10px 20px;
  cursor: pointer;
  border-radius: 5px;
}

.submit-button:hover {
  background: #009999; /* Màu hover của Image 2 */
}

/* Similar Threads */
.similar-threads {
  margin-top: 30px;
}

.section-title {
  font-size: 1.2rem;
  color: #ff4500; /* Màu cam giống MangaDex */
  margin-bottom: 10px;
}

.thread-list {
  background: #1a1a1a;
  padding: 10px;
  border-radius: 5px;
}

.thread-item {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
  border-bottom: 1px solid #333;
}

.thread-label {
  color: #e0e0e0;
}

.thread-meta {
  font-size: 0.8rem;
  color: #888;
}

/* Loading */
.loading-text {
  font-size: 1.2rem;
  color: #00ffff;
}

/* No comments */
.no-comments {
  color: #888;
  margin-top: 20px;
}
</style>

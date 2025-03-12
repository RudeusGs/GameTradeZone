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
  <div class="post-detail-container">
    <div v-if="post" class="post-content">
      <!-- 🖼️ Hiển thị avatar của người đăng bài -->
      <div class="post-user">
        <img v-if="post.user?.avatar" :src="post.user.avatar" class="avatar" />
        <span class="post-meta">
          🖊 Đăng bởi
          <strong>{{
            post.user?.fullName || post.user?.userName || "Ẩn danh"
          }}</strong>
          - 🕒 {{ new Date(post.createdDate).toLocaleString() }}
        </span>
      </div>

      <!-- 📄 Nội dung bài viết -->
      <h1 class="post-title">{{ post.caption }}</h1>
      <p class="post-text">{{ post.content }}</p>
      <div class="post-image-container" v-if="post.imageUrl">
        <img :src="post.imageUrl" alt="Hình ảnh bài viết" class="post-image" />
      </div>

      <!-- ❤️ Nút like -->
      <button class="like-button" @click="likePost">
        ❤️ Thích ({{ post.likesCount }})
      </button>

      <!-- 💬 Danh sách bình luận -->
      <h2 class="comment-header">💬 Bình luận</h2>
      <div v-if="comments.length === 0" class="no-comments">
        Chưa có bình luận nào.
      </div>

      <div v-for="comment in comments" :key="comment.id" class="comment">
        <div class="comment-user">
          <img
            v-if="comment.user?.avatar"
            :src="comment.user.avatar"
            class="avatar"
          />
          <span
            ><strong>{{
              comment.user?.fullName || comment.user?.userName || "Ẩn danh"
            }}</strong></span
          >
        </div>
        <p>{{ comment.content }}</p>
        <small>🕒 {{ new Date(comment.createdDate).toLocaleString() }}</small>
      </div>

      <!-- ✍️ Nhập bình luận -->
      <div class="comment-box">
        <textarea
          v-model="newComment"
          placeholder="Nhập bình luận..."
        ></textarea>
        <button class="submit-button" @click="submitComment">Gửi</button>
      </div>
    </div>

    <!-- 🔄 Loading -->
    <p v-else class="loading-text">Đang tải...</p>
  </div>
</template>

<style scoped>
/* 🌟 Container chính */
.post-detail-container {
  max-width: 900px;
  margin: 30px auto;
  padding: 20px;
  background: #101020;
  color: white;
  border-radius: 10px;
  text-align: center;
}

/* 🖼️ Avatar */
.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
  margin-right: 10px;
  border: 2px solid #00ffff;
}

/* 📝 Người đăng bài */
.post-user {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  margin-bottom: 10px;
}

/* 💬 Người bình luận */
.comment-user {
  display: flex;
  align-items: center;
  gap: 10px;
}

/* 💬 Bình luận */
.comment {
  padding: 10px;
  margin-top: 10px;
  background: #222;
  border-radius: 5px;
  text-align: left;
}

/* ✍️ Nhập bình luận */
.comment-box {
  margin-top: 20px;
}

textarea {
  width: 100%;
  height: 80px;
  padding: 10px;
  border-radius: 5px;
  border: none;
  background: #1a1a1a;
  color: white;
}

/* 🎯 Nút */
button {
  background: #ff5555;
  color: white;
  border: none;
  padding: 10px;
  margin-top: 10px;
  cursor: pointer;
  border-radius: 5px;
}

button:hover {
  background: #ff3333;
}

/* ❤️ Nút like */
.like-button {
  background: #ff4444;
  padding: 10px 20px;
  font-size: 1rem;
  font-weight: bold;
  border-radius: 5px;
}

.submit-button {
  background: #00ffff;
  color: black;
  border: none;
  padding: 10px;
  cursor: pointer;
  border-radius: 5px;
}

.submit-button:hover {
  background: #009999;
}
</style>

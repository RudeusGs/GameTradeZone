<script setup lang="ts">
import { ref, onMounted } from "vue";
import axios from "axios";
import { userStore } from "@/stores/auth";

// API URL
const API_BASE_URL = "https://localhost:7232/api";

// State
interface Post {
  id: string;
  caption: string;
  content: string;
  createdAt: string;
  categoryId: string;
  commentsCount: number;
  likesCount: number;
  views: number;
  imageUrls: string[]; // Thay imageUrl thành imageUrls để hỗ trợ nhiều hình ảnh
}
interface Category {
  id: string;
  name: string;
  description: string;
  postCount: number;
  iconClass: string | null;
}

const posts = ref<Post[]>([]);
const loading = ref(false);
const categories = ref<Category[]>([]);
const error = ref<string | null>(null);
const editingPost = ref<Post | null>(null); // State để lưu bài viết đang chỉnh sửa
const updatedCaption = ref(""); // Caption mới
const updatedContent = ref(""); // Content mới
const updatedCategoryId = ref(""); // CategoryId mới
const updatedImages = ref<File[]>([]); // Danh sách file hình ảnh mới
const existingImageUrls = ref<string[]>([]); // Danh sách URL hình ảnh hiện có

// Lấy thông tin người dùng từ store
const store = userStore();
const userId = store.user?.id;

const fetchCategories = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/forumscategory/getall`);
    if (response.data && response.data.result) {
      categories.value = response.data.result;
    } else {
      console.warn("Không thể tải danh sách danh mục.");
    }
  } catch (err) {
    console.error("Lỗi khi fetch categories:", err);
  }
};

const getCategoryName = (categoryId: string): string => {
  const category = categories.value.find((c) => c.id === categoryId);
  return category ? category.name : "Không xác định";
};

// Fetch posts của người dùng hiện tại
const fetchUserPosts = async () => {
  try {
    loading.value = true;

    if (!userId) {
      throw new Error("Vui lòng đăng nhập để xem bài viết của bạn!");
    }

    const response = await axios.get(
      `${API_BASE_URL}/posts/by-user/${userId}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    if (response.data && response.data.result) {
      posts.value = response.data.result.map((post: any) => ({
        id: post.id,
        caption: post.caption,
        content: post.content,
        createdAt: new Date(post.createdDate).toLocaleString(),
        categoryId: post.categoryId,
        commentsCount: post.commentsCount,
        likesCount: post.likesCount,
        views: post.views || 0,
        imageUrls: post.imageUrls || [post.imageUrl] || [], // Hỗ trợ cả trường imageUrl (nếu API trả về) và imageUrls
      }));
    } else {
      throw new Error("Không tìm thấy bài viết nào!");
    }
  } catch (err: any) {
    console.error("Lỗi khi fetch bài viết:", err);
    error.value =
      err.response?.data?.message || err.message || "Lỗi khi tải bài viết!";
  } finally {
    loading.value = false;
  }
};

// Xóa bài viết
const deletePost = async (postId: string) => {
  if (!confirm("Bạn có chắc chắn muốn xóa bài viết này?")) return;

  try {
    await axios.delete(`${API_BASE_URL}/posts/delete/${postId}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    });
    posts.value = posts.value.filter((post) => post.id !== postId);
    alert("Xóa bài viết thành công!");
  } catch (err: any) {
    console.error("Lỗi khi xóa bài viết:", err);
    alert(err.response?.data?.message || "Lỗi khi xóa bài viết!");
  }
};

// Bắt đầu chỉnh sửa bài viết
const startEditing = (post: Post) => {
  editingPost.value = post;
  updatedCaption.value = post.caption;
  updatedContent.value = post.content;
  updatedCategoryId.value = post.categoryId;
  existingImageUrls.value = [...post.imageUrls]; // Sao chép danh sách URL hình ảnh hiện có
  updatedImages.value = []; // Reset danh sách file hình ảnh mới
};

// Xử lý khi chọn file hình ảnh
const handleImageChange = (event: Event) => {
  const target = event.target as HTMLInputElement;
  if (target.files) {
    updatedImages.value = Array.from(target.files);
  }
};

// Xóa một hình ảnh hiện có
const removeExistingImage = (index: number) => {
  existingImageUrls.value.splice(index, 1);
};

// Hủy chỉnh sửa
const cancelEditing = () => {
  editingPost.value = null;
  updatedCaption.value = "";
  updatedContent.value = "";
  updatedCategoryId.value = "";
  updatedImages.value = [];
  existingImageUrls.value = [];
};

// Cập nhật bài viết
const updatePost = async () => {
  if (!editingPost.value) return;

  try {
    const formData = new FormData();
    formData.append("caption", updatedCaption.value);
    formData.append("content", updatedContent.value);
    formData.append("categoryId", updatedCategoryId.value);

    // Thêm danh sách URL hình ảnh hiện có (nếu còn)
    existingImageUrls.value.forEach((url, index) => {
      formData.append(`images[${index}]`, url);
    });

    // Thêm các file hình ảnh mới
    updatedImages.value.forEach((file, index) => {
      formData.append("images", file);
    });

    const response = await axios.post(
      `${API_BASE_URL}/posts/update/${editingPost.value.id}`,
      formData,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
          "Content-Type": "multipart/form-data",
        },
      }
    );

    if (response.status === 200) {
      const updatedPost = posts.value.find(
        (post) => post.id === editingPost.value!.id
      );
      if (updatedPost) {
        updatedPost.caption = updatedCaption.value;
        updatedPost.content = updatedContent.value;
        updatedPost.categoryId = updatedCategoryId.value;
        // Giả định API trả về danh sách imageUrls mới
        updatedPost.imageUrls = response.data.result?.imageUrls || [
          ...existingImageUrls.value,
          ...updatedImages.value.map((file) => URL.createObjectURL(file)), // Tạm thời hiển thị URL cho hình ảnh mới
        ];
      }
      alert("Cập nhật bài viết thành công!");
      cancelEditing();
    }
  } catch (err: any) {
    console.error("Lỗi khi cập nhật bài viết:", err);
    alert(err.response?.data?.message || "Lỗi khi cập nhật bài viết!");
  }
};

// Load dữ liệu khi component được mount
onMounted(() => {
  fetchUserPosts();
  fetchCategories();
});
</script>

<template>
  <div class="my-posts-container">
    <h1 class="page-title">Bài viết của tôi</h1>

    <!-- Loading state -->
    <div v-if="loading" class="loading">Đang tải...</div>

    <!-- Error state -->
    <div v-else-if="error" class="error">{{ error }}</div>

    <!-- Empty state -->
    <div v-else-if="posts.length === 0" class="empty-state">
      <p>Bạn chưa có bài viết nào!</p>
    </div>

    <!-- Post list -->
    <div v-else class="post-list">
      <div v-for="post in posts" :key="post.id" class="post-card">
        <!-- Hiển thị bài viết -->
        <div v-if="editingPost?.id !== post.id">
          <h3>{{ post.caption }}</h3>
          <p>{{ post.content.substring(0, 100) }}...</p>
          <div
            v-if="post.imageUrls && post.imageUrls.length"
            class="post-images"
          >
            <img
              v-for="(imageUrl, index) in post.imageUrls"
              :key="index"
              :src="imageUrl"
              alt="Post image"
              class="post-image"
            />
          </div>
          <p>
            <small>{{ post.createdAt }}</small>
          </p>
          <div class="post-stats">
            <span>{{ post.commentsCount }} bình luận</span>
            <span>{{ post.likesCount }} lượt thích</span>
            <span>{{ post.views }} lượt xem</span>
          </div>
          <div class="post-actions">
            <button class="edit-btn" @click="startEditing(post)">Sửa</button>
            <button class="delete-btn" @click="deletePost(post.id)">Xóa</button>
          </div>
        </div>

        <!-- Form chỉnh sửa bài viết -->
        <div v-else class="edit-form">
          <label>Tiêu đề</label>
          <input v-model="updatedCaption" placeholder="Tiêu đề mới" />

          <label>Nội dung</label>
          <textarea
            v-model="updatedContent"
            placeholder="Nội dung mới"
          ></textarea>

          <label>Danh mục</label>
          <select v-model="updatedCategoryId" class="category-select">
            <option value="" disabled>-- Chọn danh mục --</option>
            <option
              v-for="category in categories"
              :key="category.id"
              :value="category.id"
            >
              {{ category.name }}
            </option>
          </select>

          <label>Hình ảnh hiện có</label>
          <div v-if="existingImageUrls.length" class="existing-images">
            <div
              v-for="(imageUrl, index) in existingImageUrls"
              :key="index"
              class="image-preview"
            >
              <img :src="imageUrl" alt="Existing image" />
              <button
                class="remove-image-btn"
                @click="removeExistingImage(index)"
              >
                Xóa
              </button>
            </div>
          </div>
          <p v-else>Không có hình ảnh hiện có.</p>

          <label>Thêm hình ảnh mới</label>
          <input
            type="file"
            multiple
            @change="handleImageChange"
            accept="image/*"
          />

          <div class="edit-actions">
            <button class="save-btn" @click="updatePost">Lưu</button>
            <button class="cancel-btn" @click="cancelEditing">Hủy</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Theme màu sắc từ GameTradeZone */
:root {
  --primary-neon: #00f2fe;
  --secondary-purple: #1a0b3b;
  --background-dark: #0d0d0d;
  --text-light: #ffffff;
  --accent-gradient: linear-gradient(45deg, #00f2fe, #ff00ff);
}

.my-posts-container {
  padding: 2rem;
  background: var(--background-dark);
  min-height: 100vh;
  color: var(--text-light);
  font-family: "Poppins", sans-serif;
}

.page-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--primary-neon);
  text-align: center;
  margin-bottom: 2rem;
  text-shadow: 0 0 10px rgba(0, 242, 254, 0.5);
}

.loading,
.error,
.empty-state {
  text-align: center;
  font-size: 1.2rem;
  color: var(--primary-neon);
  margin: 2rem 0;
}

.post-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.post-card {
  background: var(--secondary-purple);
  border: 1px solid var(--primary-neon);
  border-radius: 10px;
  padding: 1.5rem;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  box-shadow: 0 0 15px rgba(0, 242, 254, 0.2);
}

.post-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 0 20px rgba(0, 242, 254, 0.5);
}

.post-card h3 {
  font-size: 1.5rem;
  color: var(--primary-neon);
  margin-bottom: 0.5rem;
}

.post-card p {
  font-size: 1rem;
  color: var(--text-light);
  margin-bottom: 0.5rem;
}

.post-card small {
  color: #b0b0b0;
}

.post-images {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  flex-wrap: wrap;
}

.post-image {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 5px;
  border: 1px solid var(--primary-neon);
}

.post-stats {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
  font-size: 0.9rem;
  color: #b0b0b0;
}

.post-actions {
  margin-top: 1rem;
  display: flex;
  gap: 0.5rem;
}

.edit-btn,
.delete-btn,
.save-btn,
.cancel-btn,
.remove-image-btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-weight: 600;
  transition: background 0.3s ease, box-shadow 0.3s ease;
}

.edit-btn {
  background: var(--primary-neon);
  color: var(--background-dark);
}

.edit-btn:hover {
  box-shadow: 0 0 10px rgba(0, 242, 254, 0.5);
}

.delete-btn {
  background: #ff4d4d;
  color: var(--text-light);
}

.delete-btn:hover {
  box-shadow: 0 0 10px rgba(255, 77, 77, 0.5);
}

.edit-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.edit-form label {
  font-size: 1rem;
  color: var(--primary-neon);
}

.edit-form input,
.edit-form textarea {
  padding: 0.5rem;
  border: 1px solid var(--primary-neon);
  border-radius: 5px;
  background: #1a1a1a;
  color: var(--text-light);
  font-family: "Poppins", sans-serif;
}

.edit-form textarea {
  min-height: 100px;
  resize: vertical;
}

.category-select {
  width: 100%;
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='%2300f2fe' viewBox='0 0 16 16'%3E%3Cpath d='M7.247 11.14 2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 0.7rem center;
  background-size: 1em;
  padding-right: 2.5rem;
}

.existing-images {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.image-preview {
  position: relative;
}

.image-preview img {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 5px;
  border: 1px solid var(--primary-neon);
}

.remove-image-btn {
  position: absolute;
  top: 5px;
  right: 5px;
  background: #ff4d4d;
  color: var(--text-light);
  padding: 0.2rem 0.5rem;
  font-size: 0.8rem;
}

.remove-image-btn:hover {
  box-shadow: 0 0 10px rgba(255, 77, 77, 0.5);
}

.edit-actions {
  display: flex;
  gap: 0.5rem;
}

.save-btn {
  background: var(--primary-neon);
  color: var(--background-dark);
}

.save-btn:hover {
  box-shadow: 0 0 10px rgba(0, 242, 254, 0.5);
}

.cancel-btn {
  background: #b0b0b0;
  color: var(--background-dark);
}

.cancel-btn:hover {
  box-shadow: 0 0 10px rgba(176, 176, 176, 0.5);
}
</style>

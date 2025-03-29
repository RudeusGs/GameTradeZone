<template>
  <div class="add-post-view">
    <h2>Create New Post</h2>
    <form @submit.prevent="submitPost" enctype="multipart/form-data">
      <div class="form-group">
        <label for="caption">Caption</label>
        <input
          v-model="form.caption"
          type="text"
          id="caption"
          placeholder="Enter caption"
          required
        />
      </div>

      <div class="form-group">
        <label for="category">Category</label>
        <select v-model="form.categoryId" id="category" required>
          <option value="">Select a category</option>
          <option
            v-for="category in categories"
            :key="category.id"
            :value="category.id"
          >
            {{ category.name }}
          </option>
        </select>
      </div>

      <div class="form-group">
        <label for="image">Images</label>
        <input
          type="file"
          id="image"
          name="image"
          multiple
          accept="image/*"
          @change="handleFileChange"
        />
        <div v-if="imagePreviews.length" class="image-preview">
          <img
            v-for="(preview, index) in imagePreviews"
            :key="index"
            :src="preview"
            alt="Preview"
          />
        </div>
      </div>

      <div class="form-group">
        <label for="content">Content</label>
        <textarea
          v-model="form.content"
          id="content"
          placeholder="Enter post content"
          rows="5"
          required
        ></textarea>
      </div>

      <button type="submit" :disabled="isSubmitting">Create Post</button>
    </form>

    <div v-if="isSubmitting" class="loading-message">Creating post...</div>
    <div v-else-if="success" class="success-message">
      Post created successfully! Redirecting...
    </div>
    <div v-else-if="error" class="error-message">{{ error }}</div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";

// Interface for forum category
interface ForumCategory {
  id: number;
  name: string;
  description: string;
  iconClass: string;
  postCount: number;
}

// API URL
const API_BASE_URL = "https://localhost:7232/api";

// Router setup
const router = useRouter();

// State
const form = ref({
  caption: "",
  categoryId: "",
  content: "",
  images: [] as File[],
});
const categories = ref<ForumCategory[]>([]);
const imagePreviews = ref<string[]>([]);
const isSubmitting = ref(false);
const error = ref("");
const success = ref(false);

// Handle file change for image uploads
const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const files = Array.from(target.files || []);
  form.value.images = files;

  console.log("Selected files:", files);

  imagePreviews.value = [];
  files.forEach((file) => {
    const reader = new FileReader();
    reader.onload = (e) => {
      if (e.target?.result) {
        imagePreviews.value.push(e.target.result as string);
      }
    };
    reader.readAsDataURL(file);
  });
};

// Fetch categories
const fetchCategories = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/forumsCategory/getall`);
    console.log("Categories response:", response.data);
    categories.value = response.data.result;
  } catch (err) {
    console.error("Lỗi khi lấy danh mục:", err);
    error.value = "Failed to load categories";
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

// Submit post with redirect to /forums on success
const submitPost = async () => {
  isSubmitting.value = true;
  error.value = "";
  success.value = false;

  const formData = new FormData();
  formData.append("caption", form.value.caption);
  formData.append("categoryId", form.value.categoryId);
  formData.append("content", form.value.content);

  // Append images with the correct key ('image' instead of 'images')
  form.value.images.forEach((image, index) => {
    formData.append("image", image); // Match the key expected by the backend
  });

  // Log FormData contents for debugging
  for (const [key, value] of formData.entries()) {
    console.log(`FormData entry: ${key} =`, value);
  }

  try {
    const response = await axios.post(
      `${API_BASE_URL}/posts/create`,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    console.log("Post created successfully:", response.data);

    success.value = true;
    form.value = {
      caption: "",
      categoryId: "",
      content: "",
      images: [],
    };
    imagePreviews.value = [];

    setTimeout(() => {
      router.push("/forums");
    }, 1000);
  } catch (err) {
    console.error("Error creating post:", {
      message: (err as any).message,
      response: (err as any).response
        ? {
            status: (err as any).response.status,
            data: (err as any).response.data,
          }
        : "No response",
    });
    const errorResponse = err as any; // Cast 'err' to 'any' to access its properties
    error.value =
      errorResponse.response?.data?.message ||
      "An error occurred while creating the post";
  } finally {
    isSubmitting.value = false;
  }
};

// Fetch categories on mount
fetchCategories();
</script>

<style scoped>
.add-post-view {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
}

input,
textarea,
select {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

button {
  background-color: #007bff;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.image-preview {
  margin-top: 10px;
  display: flex;
  gap: 10px;
}

.image-preview img {
  max-width: 100px;
  max-height: 100px;
  object-fit: cover;
}

.loading-message {
  color: blue;
  margin-top: 10px;
}

.error-message {
  color: red;
  margin-top: 10px;
}

.success-message {
  color: green;
  margin-top: 10px;
}
</style>

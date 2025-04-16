<template>
  <div class="cosmic-container">
    <div class="cosmic-card">
      <div class="cosmic-header">
        <div class="cosmic-title">
          <i class="cosmic-icon fa-solid fa-meteor"></i>
          <h2>CREATE NEW POST</h2>
        </div>
        <div class="cosmic-line"></div>
      </div>

      <form @submit.prevent="submitPost" enctype="multipart/form-data" class="cosmic-form">
        <div class="form-group">
          <label for="caption">
            <span class="label-text">CAPTION</span>
          </label>
          <div class="input-wrapper">
            <input
              v-model="form.caption"
              type="text"
              id="caption"
              placeholder="Enter your epic caption"
              required
            />
          </div>
        </div>

        <div class="form-group">
          <label for="category">
            <span class="label-text">CATEGORY</span>
          </label>
          <div class="select-wrapper">
            <select v-model="form.categoryId" id="category" required>
              <option value="">Select your arena</option>
              <option
                v-for="category in categories"
                :key="category.id"
                :value="category.id"
              >
                {{ category.name }}
              </option>
            </select>
            <i class="select-icon fa-solid fa-chevron-down"></i>
          </div>
        </div>

        <div class="form-group">
          <label for="image">
            <span class="label-text">IMAGES</span>
          </label>
          <div class="file-upload-wrapper">
            <label for="image" class="file-upload-label">
              <i class="fa-solid fa-cloud-arrow-up"></i>
              <span>{{ form.images.length ? `${form.images.length} files selected` : 'Choose images' }}</span>
            </label>
            <input
              type="file"
              id="image"
              name="image"
              multiple
              accept="image/*"
              @change="handleFileChange"
              class="file-input"
            />
          </div>
          
          <div v-if="imagePreviews.length" class="image-preview">
            <div v-for="(preview, index) in imagePreviews" :key="index" class="preview-item">
              <img :src="preview" alt="Preview" />
              <button type="button" class="remove-image" @click="removeImage(index)">
                <i class="fa-solid fa-times"></i>
              </button>
            </div>
          </div>
        </div>

        <div class="form-group">
          <label for="content">
            <span class="label-text">CONTENT</span>
          </label>
          <div class="textarea-wrapper">
            <textarea
              v-model="form.content"
              id="content"
              placeholder="Share your gaming experience..."
              rows="5"
              required
            ></textarea>
          </div>
        </div>

        <button type="submit" :disabled="isSubmitting" class="cosmic-button">
          <span class="button-text">{{ isSubmitting ? 'CREATING...' : 'CREATE POST' }}</span>
          <i class="button-icon fa-solid fa-rocket"></i>
        </button>
      </form>

      <div v-if="isSubmitting" class="status-message loading">
        <i class="fa-solid fa-spinner fa-spin"></i> Creating your cosmic post...
      </div>
      <div v-else-if="success" class="status-message success">
        <i class="fa-solid fa-check-circle"></i> Post created successfully! Redirecting...
      </div>
      <div v-else-if="error" class="status-message error">
        <i class="fa-solid fa-exclamation-triangle"></i> {{ error }}
      </div>
    </div>
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

// Remove image from preview and form
const removeImage = (index: number) => {
  imagePreviews.value.splice(index, 1);
  const newImages = [...form.value.images];
  newImages.splice(index, 1);
  form.value.images = newImages;
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
  form.value.images.forEach((image) => {
    formData.append("image", image); // Match the key expected by the backend
  });

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
    }, 1500);
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
    const errorResponse = err as any;
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
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;600;700&family=Rajdhani:wght@300;400;500;600;700&display=swap');

.cosmic-container {
  max-width: 900px;
  margin: 2rem auto;
  padding: 0 1rem;
  font-family: 'Rajdhani', sans-serif;
  color: #e0f2ff;
}

.cosmic-card {
  background: linear-gradient(135deg, rgba(13, 17, 33, 0.95) 0%, rgba(26, 32, 66, 0.95) 100%);
  border-radius: 12px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3), 
              0 0 0 1px rgba(87, 119, 242, 0.1),
              0 0 20px rgba(87, 119, 242, 0.2);
  padding: 2rem;
  position: relative;
  overflow: hidden;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(87, 119, 242, 0.2);
}

.cosmic-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, #5777f2, #8a5cf5, #ff4ecd);
  z-index: 1;
}

.cosmic-header {
  margin-bottom: 2rem;
}

.cosmic-title {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}

.cosmic-icon {
  font-size: 1.5rem;
  color: #ff4ecd;
  text-shadow: 0 0 10px rgba(255, 78, 205, 0.7);
}

.cosmic-title h2 {
  font-family: 'Orbitron', sans-serif;
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
  background: linear-gradient(90deg, #5777f2, #8a5cf5, #ff4ecd);
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
  letter-spacing: 1px;
}

.cosmic-line {
  height: 1px;
  background: linear-gradient(90deg, 
    rgba(87, 119, 242, 0.1), 
    rgba(87, 119, 242, 0.8), 
    rgba(87, 119, 242, 0.1));
  position: relative;
}

.cosmic-line::after {
  content: '';
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 100px;
  height: 3px;
  background: linear-gradient(90deg, #5777f2, #8a5cf5);
  filter: blur(1px);
}

.cosmic-form {
  display: grid;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.label-text {
  font-family: 'Orbitron', sans-serif;
  font-size: 0.85rem;
  font-weight: 600;
  letter-spacing: 1px;
  color: #8a9cdb;
  display: inline-block;
  margin-left: 0.5rem;
}

.input-wrapper, .select-wrapper, .textarea-wrapper {
  position: relative;
  border-radius: 8px;
  background: rgba(16, 20, 38, 0.6);
  border: 1px solid rgba(87, 119, 242, 0.3);
  transition: all 0.3s ease;
  overflow: hidden;
}

.input-wrapper::before, .select-wrapper::before, .textarea-wrapper::before {
  content: '';
  position: absolute;
  top: -2px;
  left: -2px;
  right: -2px;
  bottom: -2px;
  background: linear-gradient(45deg, #5777f2, transparent, #ff4ecd);
  z-index: -1;
  border-radius: 10px;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.input-wrapper:focus-within::before, 
.select-wrapper:focus-within::before, 
.textarea-wrapper:focus-within::before {
  opacity: 1;
}

input, select, textarea {
  width: 100%;
  background: transparent;
  border: none;
  color: #e0f2ff;
  padding: 0.8rem 1rem;
  font-family: 'Rajdhani', sans-serif;
  font-size: 1rem;
  outline: none;
}

input::placeholder, textarea::placeholder {
  color: rgba(224, 242, 255, 0.4);
}

.select-wrapper {
  position: relative;
}

.select-icon {
  position: absolute;
  right: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: #5777f2;
  pointer-events: none;
}

select {
  appearance: none;
  cursor: pointer;
}

.file-upload-wrapper {
  position: relative;
}

.file-upload-label {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.8rem 1rem;
  background: rgba(87, 119, 242, 0.1);
  border: 1px dashed rgba(87, 119, 242, 0.5);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #8a9cdb;
}

.file-upload-label:hover {
  background: rgba(87, 119, 242, 0.2);
  border-color: rgba(87, 119, 242, 0.7);
}

.file-input {
  position: absolute;
  width: 0.1px;
  height: 0.1px;
  opacity: 0;
  overflow: hidden;
  z-index: -1;
}

.image-preview {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 1rem;
  margin-top: 1rem;
}

.preview-item {
  position: relative;
  border-radius: 8px;
  overflow: hidden;
  aspect-ratio: 1;
  background: rgba(16, 20, 38, 0.6);
  border: 1px solid rgba(87, 119, 242, 0.3);
}

.preview-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.remove-image {
  position: absolute;
  top: 0.25rem;
  right: 0.25rem;
  background: rgba(0, 0, 0, 0.6);
  color: #ff4ecd;
  border: none;
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  opacity: 0;
  transition: opacity 0.2s ease;
}

.preview-item:hover .remove-image {
  opacity: 1;
}

.cosmic-button {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  background: linear-gradient(45deg, #5777f2, #8a5cf5);
  border: none;
  border-radius: 8px;
  padding: 0.9rem 2rem;
  color: white;
  font-family: 'Orbitron', sans-serif;
  font-weight: 600;
  font-size: 1rem;
  letter-spacing: 1px;
  cursor: pointer;
  transition: all 0.3s ease;
  overflow: hidden;
  margin-top: 1rem;
}

.cosmic-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(45deg, #5777f2, #8a5cf5, #ff4ecd);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.cosmic-button:hover::before {
  opacity: 1;
}

.cosmic-button:disabled {
  background: #2a3050;
  cursor: not-allowed;
}

.cosmic-button:disabled::before {
  display: none;
}

.button-text, .button-icon {
  position: relative;
  z-index: 1;
}

.button-icon {
  font-size: 1.1rem;
}

.status-message {
  margin-top: 1.5rem;
  padding: 1rem;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-weight: 500;
}

.loading {
  background: rgba(87, 119, 242, 0.1);
  color: #5777f2;
  border-left: 3px solid #5777f2;
}

.success {
  background: rgba(72, 187, 120, 0.1);
  color: #48bb78;
  border-left: 3px solid #48bb78;
}

.error {
  background: rgba(245, 101, 101, 0.1);
  color: #f56565;
  border-left: 3px solid #f56565;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .cosmic-card {
    padding: 1.5rem;
  }
  
  .cosmic-title h2 {
    font-size: 1.5rem;
  }
  
  .image-preview {
    grid-template-columns: repeat(auto-fill, minmax(80px, 1fr));
  }
}

/* Animation for the cosmic button */
@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(87, 119, 242, 0.7);
  }
  70% {
    box-shadow: 0 0 0 10px rgba(87, 119, 242, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(87, 119, 242, 0);
  }
}

.cosmic-button:not(:disabled) {
  animation: pulse 2s infinite;
}
</style>
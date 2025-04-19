<template>
  <div class="add-post-view">
    <div class="post-card">
      <div class="header-actions">
        <button @click="goBack" class="back-button">
          <i class="fas fa-arrow-left"></i> Back to Forums
        </button>
        <h2 class="title">Create New Post</h2>
      </div>

      <form @submit.prevent="submitPost" enctype="multipart/form-data">
        <!-- Caption Input -->
        <div class="form-group">
          <label for="caption"> <i class="fas fa-heading"></i> Caption </label>
          <input
            v-model="form.caption"
            type="text"
            id="caption"
            placeholder="Enter an engaging caption"
            required
            class="modern-input"
          />
        </div>

        <!-- Category Selector -->
        <div class="form-group">
          <label for="category"> <i class="fas fa-tags"></i> Category </label>
          <div class="select-wrapper">
            <select
              v-model="form.categoryId"
              id="category"
              required
              class="modern-select"
            >
              <option value="" disabled>Select a category</option>
              <option
                v-for="category in categories"
                :key="category.id"
                :value="category.id"
              >
                {{ category.name }}
              </option>
            </select>
          </div>
        </div>

        <!-- Image Upload -->
        <div class="form-group">
          <label> <i class="fas fa-images"></i> Images </label>
          <div
            class="image-drop-zone"
            @dragover.prevent
            @drop.prevent="handleDrop"
            :class="{ 'drag-over': isDragging }"
            @dragenter.prevent="isDragging = true"
            @dragleave.prevent="isDragging = false"
          >
            <div class="upload-icon">
              <i class="fas fa-cloud-upload-alt"></i>
            </div>
            <p>Drag and drop images here or</p>
            <label class="upload-button">
              Choose Files
              <input
                type="file"
                multiple
                accept="image/*"
                @change="handleFileChange"
                class="hidden"
              />
            </label>
          </div>

          <!-- Image Preview Grid -->
          <div v-if="imagePreviews.length" class="image-preview-grid">
            <div
              v-for="(preview, index) in imagePreviews"
              :key="index"
              class="preview-item"
            >
              <img :src="preview" alt="Preview" />
              <button
                type="button"
                class="remove-image"
                @click="removeImage(index)"
              >
                ×
              </button>
            </div>
          </div>
        </div>

        <!-- Content Editor -->
        <div class="form-group">
          <label for="content">
            <i class="fas fa-paragraph"></i> Content
          </label>
          <textarea
            v-model="form.content"
            id="content"
            placeholder="Share your thoughts..."
            rows="5"
            required
            class="modern-textarea"
          ></textarea>
        </div>

        <!-- Submit Button -->
        <button
          type="submit"
          class="submit-button"
          :class="{ loading: isSubmitting }"
          :disabled="isSubmitting"
        >
          <span v-if="!isSubmitting">
            <i class="fas fa-paper-plane"></i> Create Post
          </span>
          <span v-else class="loading-spinner"></span>
        </button>
      </form>

      <!-- Notifications -->
      <div class="notification-container">
        <transition name="fade">
          <div v-if="success" class="notification success">
            Post created successfully! Redirecting...
          </div>
        </transition>

        <transition name="fade">
          <div v-if="error" class="notification error">
            {{ error }}
          </div>
        </transition>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";

// Interface definitions remain the same
interface ForumCategory {
  id: number;
  name: string;
  description: string;
  iconClass: string;
  postCount: number;
}

// API and router setup remain the same
const API_BASE_URL = "https://localhost:7232/api";
const router = useRouter();

// Enhanced state management
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
const isDragging = ref(false);

// Function to navigate back
const goBack = () => {
  router.push("/forums"); // Navigate specifically to the forums page
  // Or use router.back(); to go to the previous page in history
};

// Handle drag and drop
const handleDrop = (event: DragEvent) => {
  isDragging.value = false;
  const files = Array.from(event.dataTransfer?.files || []).filter((file) =>
    file.type.startsWith("image/")
  );

  if (files.length) {
    handleFiles(files);
  }
};

// Handle file selection
const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const files = Array.from(target.files || []);
  handleFiles(files);
};

// Common file handling logic
const handleFiles = (files: File[]) => {
  form.value.images = [...form.value.images, ...files];

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

// Remove image
const removeImage = (index: number) => {
  form.value.images.splice(index, 1);
  imagePreviews.value.splice(index, 1);
};

// Fetch categories implementation remains the same
const fetchCategories = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/forumsCategory/getall`);
    categories.value = response.data.result;
  } catch (err) {
    error.value = "Failed to load categories";
    // Fallback categories remain the same
  }
};

// Submit post implementation remains the same
const submitPost = async () => {
  isSubmitting.value = true;
  error.value = "";
  success.value = false;

  const formData = new FormData();
  formData.append("caption", form.value.caption);
  formData.append("categoryId", form.value.categoryId);
  formData.append("content", form.value.content);

  // Append images with the correct key ('images')
  form.value.images.forEach((imageFile, index) => {
    formData.append("images", imageFile);
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
  margin: 2rem auto;
  padding: 0 1rem;
}

.post-card {
  background: #141824; /* Nền tối */
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 0 20px rgba(0, 242, 254, 0.15);
  border: 1px solid rgba(0, 242, 254, 0.1);
  color: #ffffff;
}

.header-actions {
  display: flex;
  align-items: center;
  justify-content: space-between; /* Adjust as needed */
  margin-bottom: 2rem;
  position: relative; /* To position title if needed */
}

.back-button {
  background: none;
  border: 1px solid rgba(0, 242, 254, 0.5);
  color: #00f2fe;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 0.9rem;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.back-button:hover {
  background: rgba(0, 242, 254, 0.1);
  border-color: #00f2fe;
  box-shadow: 0 0 10px rgba(0, 242, 254, 0.3);
}

.title {
  font-size: 1.8rem;
  color: #00f2fe; /* Màu xanh neon */
  /* margin-bottom: 2rem; */ /* Removed bottom margin */
  flex-grow: 1; /* Allow title to take space */
  text-align: center;
  font-family: "Orbitron", sans-serif; /* Font kiểu gaming */
  text-transform: uppercase;
  letter-spacing: 2px;
  text-shadow: 0 0 10px rgba(0, 242, 254, 0.5);
}

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #00f2fe; /* Màu xanh neon */
  text-transform: uppercase;
  font-size: 0.9rem;
  letter-spacing: 1px;
}

.modern-input,
.modern-select,
.modern-textarea {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 2px solid rgba(0, 242, 254, 0.3);
  border-radius: 8px;
  background: rgba(10, 14, 23, 0.8);
  color: #ffffff;
  font-family: "Exo 2", sans-serif;
  transition: all 0.3s ease;
  font-size: 1rem;
}

.modern-input:focus,
.modern-select:focus,
.modern-textarea:focus {
  border-color: #00f2fe;
  box-shadow: 0 0 10px rgba(0, 242, 254, 0.3);
  outline: none;
}

.select-wrapper {
  position: relative;
}

.select-wrapper::after {
  content: "▼";
  position: absolute;
  right: 15px;
  top: 12px;
  color: #00f2fe;
  pointer-events: none;
  font-size: 12px;
}

.image-drop-zone {
  border: 2px dashed rgba(0, 242, 254, 0.3);
  border-radius: 8px;
  padding: 2rem;
  text-align: center;
  transition: all 0.3s ease;
  cursor: pointer;
  background: rgba(10, 14, 23, 0.8);
  color: #a0a7b7;
}

.image-drop-zone.drag-over {
  border-color: #00f2fe;
  background: rgba(0, 242, 254, 0.1);
}

.upload-icon {
  font-size: 2rem;
  color: #00f2fe;
  margin-bottom: 1rem;
}

.upload-button {
  display: inline-block;
  padding: 0.5rem 1.2rem;
  background: linear-gradient(45deg, #00f2fe, #4eff8a);
  color: #000;
  font-weight: bold;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 1rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: 0.9rem;
}

.upload-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 0 15px rgba(0, 242, 254, 0.4);
}

.hidden {
  display: none;
}

.image-preview-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 1rem;
  margin-top: 1rem;
}

.preview-item {
  position: relative;
  aspect-ratio: 1;
  border-radius: 8px;
  overflow: hidden;
  border: 2px solid #00f2fe;
  box-shadow: 0 0 10px rgba(0, 242, 254, 0.2);
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
  background: rgba(0, 0, 0, 0.7);
  color: #ffffff;
  border: none;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.3s ease;
}

.remove-image:hover {
  background: rgba(255, 58, 124, 0.9);
}

.submit-button {
  width: 100%;
  padding: 1rem;
  background: linear-gradient(45deg, #00f2fe, #4eff8a);
  color: #000;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.3s ease;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.submit-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0, 242, 254, 0.4);
}

.submit-button:disabled {
  background: #2a2a2a;
  color: #555;
  cursor: not-allowed;
  box-shadow: none;
}

.loading-spinner {
  display: inline-block;
  width: 20px;
  height: 20px;
  border: 2px solid #ffffff;
  border-radius: 50%;
  border-top-color: transparent;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.notification-container {
  margin-top: 1rem;
}

.notification {
  padding: 1rem;
  border-radius: 8px;
  text-align: center;
  margin-top: 0.5rem;
}

.notification.success {
  background: rgba(78, 255, 138, 0.1);
  color: #4eff8a;
  border: 1px solid rgba(78, 255, 138, 0.3);
}

.notification.error {
  background: rgba(255, 58, 124, 0.1);
  color: #ff3a7c;
  border: 1px solid rgba(255, 58, 124, 0.3);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* Responsive Design */
@media (max-width: 768px) {
  .add-post-view {
    max-width: 100%;
  }
  .header-actions {
    flex-direction: column-reverse; /* Stack title above button */
    align-items: center;
    gap: 1rem;
  }

  .title {
    text-align: center;
  }
}
</style>
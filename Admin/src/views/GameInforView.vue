<template>
  <div class="gameinfor-container" :style="{ paddingTop: `${navbarHeight}px` }" :class="{ collapsed: isCollapsed }">
    <div class="search-filter">
      <input
        type="text"
        v-model="searchQuery"
        placeholder="Tìm kiếm theo tên game..."
        class="search-input"
      />
      <button @click="fetchGamesWithPagination" class="search-btn">Tìm kiếm</button>
    </div>

    <div v-if="errorMessage" :class="['message', { 'error': isError, 'success': !isError }]">
      {{ errorMessage }}
    </div>

    <div v-if="loading" class="loading">Đang tải...</div>

    <div class="table-container" v-else>
      <button class="add-game-button" @click="openAddGameModal">
        <i class="fas fa-plus"></i> Thêm game mới
      </button>

      <table>
        <thead>
          <tr>
            <th>#</th>
            <th>Tên game</th>
            <th>Thể loại</th>
            <th>Hình ảnh</th>
            <th>Thuộc tính</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(game, index) in paginatedGames" :key="game.id">
            <td>{{ (currentPage - 1) * pageSize + index + 1 }}</td>
            <td>{{ game.gameName || "Chưa có tên" }}</td>
            <td>{{ game.genre || "Chưa có thể loại" }}</td>
            <td>
              <button @click="openImageModal(game)" class="action-btn view-image-btn">
                Xem hình ảnh
              </button>
            </td>
            <td>
              <button @click="loadGameFields(game.id)" class="action-btn view-field-btn">
                Xem thuộc tính
              </button>
            </td>
            <td>
              <button @click="openEditGameModal(game)" class="action-btn">Sửa</button>
              <button @click="removeGame(game.id)" class="action-btn danger">Xóa</button>
            </td>
          </tr>
        </tbody>
      </table>

      <div class="pagination">
        <button @click="changePage(currentPage - 1)" :disabled="currentPage === 1">Trước</button>
        <span>Trang {{ currentPage }} / {{ totalPages }}</span>
        <button @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages">Sau</button>
      </div>
    </div>

    <div class="modal" v-if="showModal" @click.self="closeModal">
      <div class="modal-content">
        <button class="modal-close" @click="closeModal">×</button>
        <h2>{{ isEditing ? "Sửa game" : "Thêm game mới" }}</h2>
        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label for="gameName">Tên game:</label>
            <input type="text" id="gameName" v-model="form.gameName" required placeholder="Nhập tên game" />
          </div>
          <div class="form-group">
            <label for="genre">Thể loại:</label>
            <input type="text" id="genre" v-model="form.genre" placeholder="Nhập thể loại (tùy chọn)" />
          </div>
          <div class="form-group">
            <label for="fileUpload">Tải lên hình ảnh:</label>
            <div class="image-upload-wrapper">
              <label for="fileUpload" class="custom-file-upload">
                Chọn hình ảnh
              </label>
              <input type="file" id="fileUpload" ref="fileInput" @change="handleFileUpload" accept="image/*" multiple />
            </div>
            <div v-if="isEditing && form.id && games.find(g => g.id === form.id)?.image" class="current-image">
              <p>Hình ảnh hiện tại:</p>
              <img :src="getFullImageUrl(games.find(g => g.id === form.id)?.image)" alt="Hình ảnh hiện tại" />
            </div>
            <div v-if="previewImages.length > 0" class="selected-images">
              <div v-for="(img, idx) in previewImages" :key="idx" class="selected-image">
                <img :src="img" alt="Preview Image" @click="openImagePreview(previewImages, idx)" />
                <button type="button" @click="removeImage(idx)" class="remove-image-button">×</button>
              </div>
            </div>
          </div>
          <div class="modal-actions">
            <button type="submit" class="action-btn">{{ isEditing ? "Cập nhật" : "Thêm" }}</button>
            <button v-if="isEditing" type="button" @click="cancelEdit" class="action-btn cancel">Hủy</button>
          </div>
        </form>
      </div>
    </div>

    <div class="modal" v-if="imageModalVisible" @click.self="closeImageModal">
      <div class="modal-content">
        <button class="modal-close" @click="closeImageModal">×</button>
        <h2>Hình ảnh của game: {{ selectedGame?.gameName }}</h2>
        <div v-if="selectedGameImages.length > 0" class="image-gallery">
          <div v-for="(img, idx) in selectedGameImages" :key="idx" class="gallery-image">
            <img :src="img" alt="Game Image" @click="openImagePreview(selectedGameImages, idx)" />
          </div>
        </div>
        <p v-else>Chưa có hình ảnh cho game này.</p>
        <button @click="closeImageModal" class="close-btn">Đóng</button>
      </div>
    </div>

    <div class="modal" v-if="showFieldModal" @click.self="closeFieldModal">
      <div class="modal-content">
        <button class="modal-close" @click="closeFieldModal">×</button>
        <h2>Quản lý thuộc tính của game: {{ selectedGame?.gameName }}</h2>
        <div v-if="loadingFields" class="loading">Đang tải thuộc tính...</div>
        <div v-else-if="gameFields.length > 0" class="fields-table">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Tên thuộc tính</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="field in gameFields" :key="field.id">
                <td>{{ field.id }}</td>
                <td>{{ field.fieldName }}</td>
                <td>
                  <button @click="openEditFieldModal(field)" class="action-btn">Sửa</button>
                  <button @click="removeField(field.id)" class="action-btn danger">Xóa</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <p v-else>Chưa có thuộc tính nào cho game này.</p>
        <div class="modal-actions">
          <button @click="openAddFieldModal" class="action-btn">Thêm thuộc tính</button>
        </div>
      </div>
    </div>

    <div class="modal" v-if="showFieldEditModal" @click.self="closeFieldEditModal">
      <div class="modal-content">
        <button class="modal-close" @click="closeFieldEditModal">×</button>
        <h2>{{ isEditingField ? "Sửa thuộc tính" : "Thêm thuộc tính mới" }}</h2>
        <form @submit.prevent="handleFieldSubmit">
          <div class="form-group">
            <label for="fieldName">Tên thuộc tính:</label>
            <input type="text" id="fieldName" v-model="fieldForm.fieldName" required placeholder="Nhập tên thuộc tính" />
          </div>
          <div class="modal-actions">
            <button type="submit" class="action-btn">{{ isEditingField ? "Cập nhật" : "Thêm" }}</button>
            <button type="button" @click="closeFieldEditModal" class="action-btn cancel">Hủy</button>
          </div>
        </form>
      </div>
    </div>

    <div class="modal" v-if="imagePreviewVisible" @click.self="closeImagePreview">
      <div class="modal-content image-preview-modal">
        <button class="modal-close" @click="closeImagePreview">×</button>
        <button type="button" @click="prevImage" class="nav-button prev-button">❮</button>
        <img :src="selectedImages[selectedImageIndex]" alt="Image Preview" class="preview-image" />
        <button type="button" @click="nextImage" class="nav-button next-button">❯</button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, computed, onMounted, reactive } from "vue";
import gameInforApi from "@/api/gameinfor.api";
import gameFieldApi, { type AddGameFieldModel, type UpdateGameFieldModel } from "@/api/gamefield.api";
import type { GameInfor } from "@/models/gameinfor.model";
import type { GameField } from "@/models/gameinfor.model";

const getFullImageUrl = (imageString: string | null | undefined): string => {
  if (!imageString || imageString.trim() === "") return "https://via.placeholder.com/150";
  const baseUrl = "https://localhost:7232/";
  const images = imageString.split(";").filter(img => img.trim() !== "");
  return images.length > 0 ? `${baseUrl}${images[0]}` : "https://via.placeholder.com/150";
};

export default defineComponent({
  name: "AdminGameInfor",
  props: {
    navbarHeight: {
      type: Number,
      default: 64,
    },
    isCollapsed: {
      type: Boolean,
      default: false,
    },
  },
  setup(props) {
    const games = ref<GameInfor[]>([]);
    const gameFields = ref<GameField[]>([]);
    const showModal = ref(false);
    const showFieldModal = ref(false);
    const showFieldEditModal = ref(false);
    const isEditing = ref(false);
    const isEditingField = ref(false);
    const fileInput = ref<HTMLInputElement | null>(null);
    const previewImages = ref<string[]>([]);
    const imageModalVisible = ref<boolean>(false);
    const imagePreviewVisible = ref<boolean>(false);
    const selectedImages = ref<string[]>([]);
    const selectedImageIndex = ref<number>(0);
    const selectedGame = ref<GameInfor | null>(null);
    const selectedGameImages = ref<string[]>([]);
    const selectedField = ref<GameField | null>(null);
    const searchQuery = ref("");
    const currentPage = ref(1);
    const pageSize = ref(10);
    const loading = ref(false);
    const loadingFields = ref(false);
    const errorMessage = ref<string | null>(null);
    const isError = ref(true);

    const form = reactive({
      id: 0,
      gameName: "",
      genre: "",
      files: [] as File[],
    });

    const fieldForm = reactive({
      id: 0,
      gameInforId: null as number | null,
      fieldName: "",
    });

    const filteredGames = computed(() => {
      if (!searchQuery.value) return games.value;
      return games.value.filter((game) =>
        (game.gameName || "").toLowerCase().includes(searchQuery.value.toLowerCase())
      );
    });

    const totalPages = computed(() => Math.ceil(filteredGames.value.length / pageSize.value));

    const paginatedGames = computed(() => {
      const start = (currentPage.value - 1) * pageSize.value;
      const end = start + pageSize.value;
      return filteredGames.value.slice(start, end);
    });

    const loadGames = async () => {
      loading.value = true;
      try {
        const response = await gameInforApi.getAll();
        if (response.data.result.isSuccess && Array.isArray(response.data.result.data)) {
          games.value = response.data.result.data;
        } else {
          errorMessage.value = response.data.result.message || "Lỗi khi tải danh sách game";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi tải danh sách game: " + error.message;
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      } finally {
        loading.value = false;
      }
    };

    const loadGameFields = async (gameId: number) => {
      loadingFields.value = true;
      try {
        const response = await gameFieldApi.getField(gameId);
        if (response.data && response.data.result && response.data.result.isSuccess) {
          if (Array.isArray(response.data.result.data)) {
            gameFields.value = response.data.result.data;
            selectedGame.value = games.value.find(g => g.id === gameId) || null;
            showFieldModal.value = true;
            if (gameFields.value.length === 0) {
              errorMessage.value = "Game này không có thuộc tính nào.";
              isError.value = true;
              setTimeout(() => (errorMessage.value = null), 3000);
            }
          } else {
            errorMessage.value = "Dữ liệu trả về không phải là mảng";
            isError.value = true;
            setTimeout(() => (errorMessage.value = null), 3000);
          }
        } else {
          errorMessage.value = response.data?.result?.message || "Lỗi khi tải thuộc tính";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi tải thuộc tính: " + error.message;
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      } finally {
        loadingFields.value = false;
      }
    };

    const loadSingleGameField = async (id: number) => {
      try {
        const response = await gameFieldApi.getField(id);
        if (response.data.result.isSuccess && response.data.result.data) {
          const field = response.data.result.data;
          gameFields.value = [field];
          selectedGame.value = games.value.find(g => g.id === field.gameInforId) || null;
          showFieldModal.value = true;
        } else {
          errorMessage.value = response.data.result.message || "Không tìm thấy thuộc tính";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Lỗi khi tải thuộc tính: " + error.message;
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const handleFileUpload = (event: Event) => {
      const target = event.target as HTMLInputElement;
      if (target.files && target.files.length > 0) {
        form.files = Array.from(target.files);
        previewImages.value = [];
        Array.from(target.files).forEach((file: File) => {
          const reader = new FileReader();
          reader.onload = (e) => {
            if (e.target?.result) {
              previewImages.value.push(e.target.result as string);
            }
          };
          reader.readAsDataURL(file);
        });
      } else {
        form.files = [];
        previewImages.value = [];
      }
    };

    const removeImage = (index: number) => {
      form.files.splice(index, 1);
      previewImages.value.splice(index, 1);
    };

    const handleSubmit = async () => {
      try {
        if (!form.gameName) {
          errorMessage.value = "Vui lòng điền tên game!";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
          return;
        }

        if (form.files.length === 0 && !isEditing.value) {
          errorMessage.value = "Vui lòng chọn ít nhất một hình ảnh!";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
          return;
        }

        const formData = new FormData();
        formData.append("GameName", form.gameName);
        if (form.genre) formData.append("Genre", form.genre);
        if (isEditing.value) formData.append("Id", form.id.toString());

        if (form.files.length > 0) {
          form.files.forEach((file: File) => formData.append("Files", file));
        }

        let response;
        if (isEditing.value) {
          response = await gameInforApi.updateWithFormData(formData);
        } else {
          response = await gameInforApi.addWithFormData(formData);
        }

        if (response.data.result.isSuccess) {
          errorMessage.value = isEditing.value ? "Cập nhật game thành công" : "Thêm game thành công";
          isError.value = false;
          setTimeout(() => (errorMessage.value = null), 3000);
          await loadGames();
          closeModal();
        } else {
          errorMessage.value = response.data.result.message || "Lỗi khi xử lý game";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi xử lý game: " + error.message;
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const resetForm = () => {
      form.id = 0;
      form.gameName = "";
      form.genre = "";
      form.files = [];
      previewImages.value = [];
      isEditing.value = false;
      if (fileInput.value) fileInput.value.value = "";
    };

    const openAddGameModal = () => {
      resetForm();
      showModal.value = true;
    };

    const openEditGameModal = (game: GameInfor) => {
      isEditing.value = true;
      form.id = game.id || 0;
      form.gameName = game.gameName || "";
      form.genre = game.genre || "";
      form.files = [];
      previewImages.value = [];
      showModal.value = true;
    };

    const closeModal = () => {
      showModal.value = false;
      resetForm();
    };

    const cancelEdit = () => {
      closeModal();
    };

    const removeGame = async (id: number) => {
      if (!confirm("Bạn có chắc chắn muốn xóa game này?")) return;
      try {
        const response = await gameInforApi.delete(id);
        if (response.data.result.isSuccess) {
          errorMessage.value = "Xóa game thành công";
          isError.value = false;
          setTimeout(() => (errorMessage.value = null), 3000);
          await loadGames();
        } else {
          errorMessage.value = response.data.result.message || "Lỗi khi xóa game";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi xóa game: " + error.message;
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const openImageModal = (game: GameInfor) => {
      selectedGame.value = game;
      selectedGameImages.value = game.image ? game.image.split(";").map(img => getFullImageUrl(img)) : [];
      imageModalVisible.value = true;
    };

    const closeImageModal = () => {
      imageModalVisible.value = false;
      selectedGame.value = null;
      selectedGameImages.value = [];
    };

    const openImagePreview = (imagesList: string[], index: number) => {
      selectedImages.value = imagesList;
      selectedImageIndex.value = index;
      imagePreviewVisible.value = true;
    };

    const closeImagePreview = () => {
      imagePreviewVisible.value = false;
      selectedImages.value = [];
      selectedImageIndex.value = 0;
    };

    const nextImage = () => {
      if (selectedImageIndex.value < selectedImages.value.length - 1) {
        selectedImageIndex.value += 1;
      }
    };

    const prevImage = () => {
      if (selectedImageIndex.value > 0) {
        selectedImageIndex.value -= 1;
      }
    };

    const openAddFieldModal = () => {
      isEditingField.value = false;
      fieldForm.id = 0;
      fieldForm.gameInforId = selectedGame.value?.id || null;
      fieldForm.fieldName = "";
      showFieldEditModal.value = true;
    };

    const openEditFieldModal = (field: GameField) => {
      isEditingField.value = true;
      fieldForm.id = field.id;
      fieldForm.gameInforId = field.gameInforId;
      fieldForm.fieldName = field.fieldName ?? "";
      showFieldEditModal.value = true;
    };

    const closeFieldModal = () => {
      showFieldModal.value = false;
      gameFields.value = [];
    };

    const closeFieldEditModal = () => {
      showFieldEditModal.value = false;
      fieldForm.id = 0;
      fieldForm.gameInforId = null;
      fieldForm.fieldName = "";
    };

    const handleFieldSubmit = async () => {
      try {
        if (!fieldForm.fieldName) {
          errorMessage.value = "Vui lòng điền tên thuộc tính!";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
          return;
        }

        let response;
        if (isEditingField.value) {
          const updateModel: UpdateGameFieldModel = {
            id: fieldForm.id,
            gameInforId: fieldForm.gameInforId!,
            fieldName: fieldForm.fieldName,
          };
          response = await gameFieldApi.update(updateModel);
        } else {
          const addModel: AddGameFieldModel = {
            gameInforId: fieldForm.gameInforId!,
            fieldName: fieldForm.fieldName,
          };
          response = await gameFieldApi.add(addModel);
        }

        if (response.data.result.isSuccess) {
          errorMessage.value = isEditingField.value ? "Cập nhật thuộc tính thành công" : "Thêm thuộc tính thành công";
          isError.value = false;
          setTimeout(() => (errorMessage.value = null), 3000);
          closeFieldEditModal();
          await loadGameFields(fieldForm.gameInforId!);
        } else {
          errorMessage.value = response.data.result.message || "Lỗi khi xử lý thuộc tính";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi xử lý thuộc tính: " + error.message;
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const removeField = async (id: number) => {
      if (!confirm("Bạn có chắc chắn muốn xóa thuộc tính này?")) return;
      try {
        const response = await gameFieldApi.delete(id);
        if (response.data.result.isSuccess) {
          errorMessage.value = "Xóa thuộc tính thành công";
          isError.value = false;
          setTimeout(() => (errorMessage.value = null), 3000);
          if (selectedGame.value) await loadGameFields(selectedGame.value.id);
        } else {
          errorMessage.value = response.data.result.message || "Lỗi khi xóa thuộc tính";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi xóa thuộc tính: " + error.message;
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const changePage = (page: number) => {
      if (page >= 1 && page <= totalPages.value) {
        currentPage.value = page;
      }
    };

    const fetchGamesWithPagination = () => {
      currentPage.value = 1;
    };

    onMounted(() => {
      loadGames();
    });

    return {
      games,
      gameFields,
      showModal,
      showFieldModal,
      showFieldEditModal,
      form,
      fieldForm,
      isEditing,
      isEditingField,
      handleSubmit,
      handleFieldSubmit,
      openAddGameModal,
      openEditGameModal,
      closeModal,
      cancelEdit,
      removeGame,
      removeField,
      getFullImageUrl,
      fileInput,
      handleFileUpload,
      previewImages,
      removeImage,
      imageModalVisible,
      imagePreviewVisible,
      selectedImages,
      selectedImageIndex,
      selectedGame,
      selectedGameImages,
      selectedField,
      openImageModal,
      closeImageModal,
      openImagePreview,
      closeImagePreview,
      nextImage,
      prevImage,
      openAddFieldModal,
      openEditFieldModal,
      closeFieldModal,
      closeFieldEditModal,
      loadGameFields,
      loadSingleGameField,
      searchQuery,
      currentPage,
      pageSize,
      paginatedGames,
      totalPages,
      changePage,
      fetchGamesWithPagination,
      loading,
      loadingFields,
      errorMessage,
      isError,
    };
  },
});
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.gameinfor-container {
  padding: 30px;
  max-width: 1400px;
  margin: 0 auto;
  background: #f9fafb;
  min-height: 100vh;
  transition: margin-left 0.3s ease;
}

.gameinfor-container.collapsed {
  margin-left: 60px;
}

.search-filter {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  justify-content: flex-end;
  align-items: center;
}

.search-input {
  padding: 10px 10px 10px 35px;
  width: 300px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
  background: url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="%236b7280" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>') no-repeat 10px center;
  background-size: 20px;
  transition: border-color 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.search-btn {
  padding: 10px 20px;
  background: #3b82f6;
  color: #fff;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background 0.3s ease;
}

.search-btn:hover {
  background: #2563eb;
}

.message {
  text-align: center;
  margin-bottom: 20px;
  font-size: 14px;
  padding: 10px;
  border-radius: 6px;
}

.message.error {
  color: #dc2626;
  background: #fee2e2;
}

.message.success {
  color: #059669;
  background: #d1fae5;
}

.loading {
  text-align: center;
  font-size: 16px;
  color: #6b7280;
  padding: 20px;
}

.add-game-button {
  display: inline-flex;
  align-items: center;
  padding: 10px 20px;
  background: #10b981;
  color: #fff;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background 0.3s ease;
  margin-bottom: 20px;
}

.add-game-button i {
  margin-right: 8px;
}

.add-game-button:hover {
  background: #059669;
}

.table-container table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  background: #fff;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  border-radius: 8px;
  overflow: hidden;
}

.table-container th,
.table-container td {
  padding: 16px 20px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
  font-size: 15px;
  color: #374151;
}

.table-container th {
  background: #3b82f6;
  color: #fff;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.table-container tr:hover {
  background: #f9fafb;
}

.action-btn {
  padding: 8px 16px;
  margin-right: 8px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  background: #3b82f6;
  color: #fff;
  font-size: 14px;
  font-weight: 500;
  transition: background 0.3s ease;
}

.action-btn:hover {
  background: #2563eb;
}

.action-btn.danger {
  background: #ef4444;
}

.action-btn.danger:hover {
  background: #dc2626;
}

.action-btn.view-image-btn {
  background: #8b5cf6;
}

.action-btn.view-image-btn:hover {
  background: #7c3aed;
}

.action-btn.view-field-btn {
  background: #f59e0b;
}

.action-btn.view-field-btn:hover {
  background: #d97706;
}

.action-btn.cancel {
  background: #6b7280;
}

.action-btn.cancel:hover {
  background: #4b5563;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 10px;
  margin-top: 20px;
}

.pagination button {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  background: #e5e7eb;
  color: #374151;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.3s ease;
}

.pagination button:hover:not(:disabled) {
  background: #d1d5db;
}

.pagination button:disabled {
  background: #f3f4f6;
  cursor: not-allowed;
}

.pagination span {
  font-size: 14px;
  color: #374151;
  font-weight: 500;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.modal-content {
  background: #fff;
  padding: 30px;
  border-radius: 12px;
  max-width: 600px;
  width: 90%;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  position: relative;
  animation: slideUp 0.3s ease;
}

@keyframes slideUp {
  from {
    transform: translateY(20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-content h2 {
  margin-bottom: 20px;
  color: #1f2937;
  font-size: 22px;
  font-weight: 600;
  border-bottom: 1px solid #e5e7eb;
  padding-bottom: 10px;
}

.modal-close {
  position: absolute;
  top: 10px;
  right: 10px;
  background: none;
  border: none;
  font-size: 24px;
  color: #6b7280;
  cursor: pointer;
  transition: color 0.3s ease;
}

.modal-close:hover {
  color: #374151;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  font-weight: 600;
  margin-bottom: 8px;
  color: #1f2937;
}

.form-group input[type="text"] {
  width: 100%;
  padding: 10px;
  font-size: 15px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  transition: border-color 0.3s ease;
}

.form-group input[type="text"]:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.image-upload-wrapper {
  margin-top: 8px;
}

.custom-file-upload {
  display: inline-flex;
  align-items: center;
  background: #3b82f6;
  color: #fff;
  padding: 10px 16px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 15px;
  transition: background 0.3s ease;
}

.custom-file-upload:hover {
  background: #2563eb;
}

input[type="file"] {
  display: none;
}

.current-image {
  margin-top: 15px;
}

.current-image p {
  font-size: 14px;
  color: #1f2937;
  margin-bottom: 8px;
}

.current-image img {
  width: 120px;
  height: 120px;
  object-fit: cover;
  border-radius: 8px;
}

.selected-images {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  margin-top: 15px;
}

.selected-image {
  position: relative;
  width: 100px;
  height: 100px;
}

.selected-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 8px;
  cursor: pointer;
}

.remove-image-button {
  position: absolute;
  top: -8px;
  right: -8px;
  background: #ef4444;
  border: none;
  color: #fff;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  cursor: pointer;
  font-weight: bold;
  transition: background 0.3s ease;
}

.remove-image-button:hover {
  background: #dc2626;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

.image-gallery {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  justify-content: center;
  margin-bottom: 20px;
}

.gallery-image img {
  width: 150px;
  height: 150px;
  object-fit: cover;
  border-radius: 12px;
  cursor: pointer;
  transition: transform 0.3s ease;
}

.gallery-image img:hover {
  transform: scale(1.05);
}

.fields-table table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

.fields-table th,
.fields-table td {
  padding: 12px 16px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
  font-size: 14px;
  color: #374151;
}

.fields-table th {
  background: #3b82f6;
  color: #fff;
  font-weight: 600;
}

.fields-table tr:hover {
  background: #f9fafb;
}

.image-preview-modal {
  max-width: 90%;
  max-height: 90%;
  padding: 20px;
}

.preview-image {
  width: 100%;
  height: auto;
  border-radius: 12px;
}

.nav-button {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(0, 0, 0, 0.6);
  border: none;
  color: #fff;
  padding: 12px;
  cursor: pointer;
  border-radius: 50%;
  font-size: 20px;
  transition: background 0.3s ease;
}

.nav-button:hover {
  background: rgba(0, 0, 0, 0.9);
}

.prev-button {
  left: 20px;
}

.next-button {
  right: 20px;
}

.close-btn {
  padding: 10px 20px;
  margin-top: 20px;
  background: #6b7280;
  color: #fff;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.3s ease;
  width: 100%;
}

.close-btn:hover {
  background: #4b5563;
}

@media (max-width: 768px) {
  .gameinfor-container {
    padding: 20px;
  }

  .search-filter {
    flex-direction: column;
    align-items: stretch;
  }

  .search-input {
    width: 100%;
  }

  .table-container table {
    display: block;
    overflow-x: auto;
    white-space: nowrap;
  }

  .pagination {
    flex-direction: column;
    gap: 10px;
  }

  .modal-content {
    max-width: 90%;
    padding: 20px;
  }

  .modal-content h2 {
    font-size: 18px;
  }

  .action-btn {
    padding: 8px 12px;
    font-size: 13px;
  }

  .gallery-image img {
    width: 100px;
    height: 100px;
  }
}
</style>
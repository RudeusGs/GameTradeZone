<script setup lang="ts">
import { ref } from 'vue';

// Danh sách game mẫu với hình ảnh
const gameOptions = ref([
  { id: 1, name: 'Valorant', image: 'https://via.placeholder.com/150?text=Valorant' },
  { id: 2, name: 'Genshin Impact', image: 'https://via.placeholder.com/150?text=Genshin+Impact' },
  { id: 3, name: 'League of Legends', image: 'https://via.placeholder.com/150?text=LoL' },
]);

// Dữ liệu form
const formData = ref({
  selectedGame: '',
  accountName: '',
  password: '',
  price: null,
  priceMin: null,
  files: [] as File[],
  previewImages: [] as string[],
});

// Step hiện tại (1: chọn game, 2: nhập thông tin, 3: upload ảnh, 4: xem lại)
const currentStep = ref(1);

// Modal thông báo
const showWarningModal = ref(false);
const showSuccessModal = ref(false);

// Trạng thái xác nhận radio
const isConfirmed = ref(false);

// Tham chiếu đến input file
const fileInputRef = ref<HTMLInputElement | null>(null);

// Chọn game và chuyển bước
const selectGame = (game: string) => {
  formData.value.selectedGame = game;
  currentStep.value = 2;
};

// Quay lại bước trước
const previousStep = () => {
  if (currentStep.value > 1) currentStep.value--;
};

// Tiếp tục bước sau
const nextStep = () => {
  if (currentStep.value < 4) currentStep.value++;
};

// Xử lý khi chọn file
const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const files = target.files;
  if (files) {
    const newFiles = Array.from(files);
    formData.value.files = [...formData.value.files, ...newFiles];
    newFiles.forEach(file => {
      const reader = new FileReader();
      reader.onload = (e) => {
        if (e.target?.result) formData.value.previewImages.push(e.target.result as string);
      };
      reader.readAsDataURL(file);
    });
    target.value = '';
  }
};

// Kích hoạt click vào input file
const triggerFileInput = () => {
  if (fileInputRef.value) fileInputRef.value.click();
};

// Xóa ảnh preview
const removeImage = (index: number) => {
  formData.value.files.splice(index, 1);
  formData.value.previewImages.splice(index, 1);
};

// Submit form
const submitForm = () => {
  showWarningModal.value = true;
};

// Xác nhận trong modal cảnh báo
const confirmSubmission = () => {
  if (isConfirmed.value) {
    console.log('Form Data:', formData.value);
    showWarningModal.value = false;
    showSuccessModal.value = true;
    isConfirmed.value = false;
    currentStep.value = 1;
    formData.value = { selectedGame: '', accountName: '', password: '', price: null, priceMin: null, files: [], previewImages: [] };
  }
};
</script>

<template>
  <div class="add-account-container">
    <!-- Bubble Background -->
    <div class="bubble-background">
      <div v-for="i in 50" :key="i" class="bubble"></div>
    </div>

    <!-- Form thêm tài khoản -->
    <div class="form-card">
      <div class="form-header">
        <h2 class="form-title">Thêm tài khoản game</h2>
        <div class="step-indicator">
          <span :class="{ active: currentStep === 1 }">1</span>
          <span :class="{ active: currentStep === 2 }">2</span>
          <span :class="{ active: currentStep === 3 }">3</span>
          <span :class="{ active: currentStep === 4 }">4</span>
        </div>
      </div>
      <p class="form-subtitle">Bước {{ currentStep }}: {{ currentStep === 1 ? 'Chọn game' : currentStep === 2 ? 'Nhập thông tin' : currentStep === 3 ? 'Tải ảnh' : 'Xem lại' }}</p>

      <!-- Bước 1: Chọn game -->
      <div v-if="currentStep === 1" class="step-content game-step">
        <div class="game-list">
          <div v-for="game in gameOptions" :key="game.id" class="game-item" @click="selectGame(game.name)">
            <img :src="game.image" alt="Game Image" class="game-image" />
            <p>{{ game.name }}</p>
          </div>
        </div>
      </div>

      <!-- Bước 2: Nhập thông tin -->
      <div v-if="currentStep === 2" class="step-content">
        <div class="form-group">
          <label>Tên tài khoản</label>
          <input v-model="formData.accountName" type="text" class="form-input" placeholder="Tên tài khoản" required />
        </div>
        <div class="form-group">
          <label>Mật khẩu</label>
          <input v-model="formData.password" type="password" class="form-input" placeholder="Mật khẩu" required />
        </div>
        <div class="form-row">
          <div class="form-group">
            <label>Giá bán (VND)</label>
            <input v-model.number="formData.price" type="number" class="form-input" placeholder="Giá bán" min="0" step="1000" required />
          </div>
          <div class="form-group">
            <label>Giá tối thiểu (VND)</label>
            <input v-model.number="formData.priceMin" type="number" class="form-input" placeholder="Giá tối thiểu" min="0" step="1000" required />
          </div>
        </div>
      </div>

      <!-- Bước 3: Upload ảnh -->
      <div v-if="currentStep === 3" class="step-content">
        <div class="form-group">
          <label>Hình ảnh tài khoản</label>
          <div class="upload-area" @click="triggerFileInput">
            <i class="fas fa-cloud-upload-alt upload-icon"></i>
            <p>Tải ảnh lên (nhấp hoặc kéo thả)</p>
            <input ref="fileInputRef" type="file" multiple accept="image/*" @change="handleFileChange" class="file-input" />
          </div>
        </div>
        <div class="preview-container" v-if="formData.previewImages.length > 0">
          <div class="preview-images">
            <div v-for="(image, index) in formData.previewImages" :key="index" class="preview-item">
              <img :src="image" alt="Preview" class="preview-image" />
              <button class="remove-btn" @click="removeImage(index)">×</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Bước 4: Xem lại thông tin -->
      <div v-if="currentStep === 4" class="step-content review-step">
        <div class="review-card">
          <div class="review-group">
            <span class="review-label">Game:</span>
            <span class="review-value">{{ formData.selectedGame }}</span>
          </div>
          <div class="review-group">
            <span class="review-label">Tên tài khoản:</span>
            <span class="review-value">{{ formData.accountName }}</span>
          </div>
          <div class="review-group">
            <span class="review-label">Mật khẩu:</span>
            <span class="review-value">{{ formData.password }}</span>
          </div>
          <div class="review-group">
            <span class="review-label">Giá bán:</span>
            <span class="review-value">{{ formData.price }} VND</span>
          </div>
          <div class="review-group">
            <span class="review-label">Giá tối thiểu:</span>
            <span class="review-value">{{ formData.priceMin }} VND</span>
          </div>
          <div class="review-group">
            <span class="review-label">Hình ảnh:</span>
            <div class="review-images" v-if="formData.previewImages.length > 0">
              <img v-for="(image, index) in formData.previewImages" :key="index" :src="image" alt="Preview" class="review-image" />
            </div>
            <span v-else class="review-value">Chưa có ảnh</span>
          </div>
        </div>
      </div>

      <!-- Điều hướng bước (icon lùi/tiến) -->
      <div class="step-navigation">
        <button v-if="currentStep > 1" @click="previousStep" class="nav-btn prev-btn">
          <i class="fas fa-arrow-left"></i>
        </button>
        <button v-if="currentStep < 4" @click="nextStep" class="nav-btn next-btn">
          <i class="fas fa-arrow-right"></i>
        </button>
        <button v-if="currentStep === 4" @click="submitForm" class="nav-btn submit-btn">
          <i class="fas fa-check"></i>
        </button>
      </div>
    </div>

    <!-- Modal cảnh báo -->
    <transition name="fade">
      <div v-if="showWarningModal" class="modal-overlay">
        <div class="modal-content warning-modal">
          <h3>Cảnh báo quan trọng</h3>
          <p>Chúng tôi yêu cầu bạn cung cấp thông tin chính xác. Mọi hành vi cố ý đăng tải sai lệch sẽ dẫn đến khóa tài khoản vĩnh viễn.</p>
          <div class="confirmation-group">
            <input type="radio" id="confirm" v-model="isConfirmed" :value="true" />
            <label for="confirm">Tôi cam kết thông tin chính xác</label>
          </div>
          <div class="modal-actions">
            <button @click="confirmSubmission" class="modal-confirm" :disabled="!isConfirmed">Xác nhận</button>
            <button @click="showWarningModal = false" class="modal-cancel">Hủy bỏ</button>
          </div>
        </div>
      </div>
    </transition>

    <!-- Modal thành công -->
    <transition name="fade">
      <div v-if="showSuccessModal" class="modal-overlay">
        <div class="modal-content success-modal">
          <i class="fas fa-check-circle success-icon"></i>
          <h3>Thành công!</h3>
          <p>Tài khoản của bạn đã được thêm vào danh sách bán.</p>
          <button @click="showSuccessModal = false" class="modal-close">Đóng</button>
        </div>
      </div>
    </transition>
  </div>
</template>

<style scoped>
/* Tổng thể */
.add-account-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
  font-family: 'Arial', sans-serif;
  padding: 40px;
  position: relative;
  overflow: hidden;
}

/* Bubble Background */
.bubble-background {
  position: absolute;
  inset: 0;
  z-index: 0;
}

.bubble {
  position: absolute;
  border-radius: 50%;
  opacity: 0.7;
  animation: bubbleRise 6s infinite ease-in-out;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.3);
}

.bubble:nth-child(odd) {
  background: rgba(255, 0, 255, 0.4);
  width: 15px;
  height: 15px;
}

.bubble:nth-child(even) {
  background: rgba(0, 255, 255, 0.4);
  width: 25px;
  height: 25px;
}

.bubble:nth-child(1) { left: 5%; bottom: 5%; animation-duration: 7s; }
.bubble:nth-child(2) { left: 15%; bottom: 10%; animation-duration: 5s; }
.bubble:nth-child(3) { left: 25%; bottom: 15%; animation-duration: 6.5s; }
.bubble:nth-child(4) { left: 35%; bottom: 20%; animation-duration: 5.5s; }
.bubble:nth-child(5) { left: 45%; bottom: 25%; animation-duration: 6s; }
.bubble:nth-child(6) { left: 55%; bottom: 30%; animation-duration: 7.5s; }
.bubble:nth-child(7) { left: 65%; bottom: 35%; animation-duration: 5.8s; }
.bubble:nth-child(8) { left: 75%; bottom: 40%; animation-duration: 6.2s; }
.bubble:nth-child(9) { left: 85%; bottom: 45%; animation-duration: 5.7s; }
.bubble:nth-child(10) { left: 95%; bottom: 50%; animation-duration: 6.8s; }
.bubble:nth-child(11) { left: 10%; bottom: 55%; animation-duration: 5.9s; }
.bubble:nth-child(12) { left: 20%; bottom: 60%; animation-duration: 6.3s; }
.bubble:nth-child(13) { left: 30%; bottom: 65%; animation-duration: 5.6s; }
.bubble:nth-child(14) { left: 40%; bottom: 70%; animation-duration: 6.1s; }
.bubble:nth-child(15) { left: 50%; bottom: 75%; animation-duration: 6.4s; }
.bubble:nth-child(16) { left: 60%; bottom: 80%; animation-duration: 5.5s; }
.bubble:nth-child(17) { left: 70%; bottom: 85%; animation-duration: 6.7s; }
.bubble:nth-child(18) { left: 80%; bottom: 90%; animation-duration: 6s; }
.bubble:nth-child(19) { left: 90%; bottom: 95%; animation-duration: 5.9s; }
.bubble:nth-child(20) { left: 15%; bottom: 88%; animation-duration: 6.2s; }
.bubble:nth-child(21) { left: 25%; bottom: 83%; animation-duration: 6.5s; }
.bubble:nth-child(22) { left: 35%; bottom: 78%; animation-duration: 5.8s; }
.bubble:nth-child(23) { left: 45%; bottom: 73%; animation-duration: 6.1s; }
.bubble:nth-child(24) { left: 55%; bottom: 68%; animation-duration: 5.7s; }
.bubble:nth-child(25) { left: 65%; bottom: 63%; animation-duration: 6.3s; }
.bubble:nth-child(26) { left: 75%; bottom: 58%; animation-duration: 6s; }
.bubble:nth-child(27) { left: 85%; bottom: 53%; animation-duration: 5.9s; }
.bubble:nth-child(28) { left: 95%; bottom: 48%; animation-duration: 6.4s; }
.bubble:nth-child(29) { left: 5%; bottom: 43%; animation-duration: 6.1s; }
.bubble:nth-child(30) { left: 15%; bottom: 38%; animation-duration: 5.8s; }
.bubble:nth-child(31) { left: 25%; bottom: 33%; animation-duration: 7s; }
.bubble:nth-child(32) { left: 35%; bottom: 28%; animation-duration: 6.5s; }
.bubble:nth-child(33) { left: 45%; bottom: 23%; animation-duration: 5.9s; }
.bubble:nth-child(34) { left: 55%; bottom: 18%; animation-duration: 6.2s; }
.bubble:nth-child(35) { left: 65%; bottom: 13%; animation-duration: 6.8s; }
.bubble:nth-child(36) { left: 75%; bottom: 8%; animation-duration: 5.6s; }
.bubble:nth-child(37) { left: 85%; bottom: 3%; animation-duration: 6.3s; }
.bubble:nth-child(38) { left: 95%; bottom: 7%; animation-duration: 6s; }
.bubble:nth-child(39) { left: 10%; bottom: 12%; animation-duration: 5.7s; }
.bubble:nth-child(40) { left: 20%; bottom: 17%; animation-duration: 6.4s; }
.bubble:nth-child(41) { left: 30%; bottom: 22%; animation-duration: 6.1s; }
.bubble:nth-child(42) { left: 40%; bottom: 27%; animation-duration: 5.9s; }
.bubble:nth-child(43) { left: 50%; bottom: 32%; animation-duration: 6.5s; }
.bubble:nth-child(44) { left: 60%; bottom: 37%; animation-duration: 6.2s; }
.bubble:nth-child(45) { left: 70%; bottom: 42%; animation-duration: 5.8s; }
.bubble:nth-child(46) { left: 80%; bottom: 47%; animation-duration: 6.7s; }
.bubble:nth-child(47) { left: 90%; bottom: 52%; animation-duration: 6s; }
.bubble:nth-child(48) { left: 5%; bottom: 57%; animation-duration: 5.9s; }
.bubble:nth-child(49) { left: 15%; bottom: 62%; animation-duration: 6.3s; }
.bubble:nth-child(50) { left: 25%; bottom: 67%; animation-duration: 6.1s; }

@keyframes bubbleRise {
  0% { transform: translateY(100vh) scale(0.5); opacity: 0.7; }
  30% { transform: translateY(70vh) scale(0.8); opacity: 0.9; }
  60% { transform: translateY(30vh) scale(1.1); opacity: 0.8; }
  100% { transform: translateY(-50px) scale(0.9); opacity: 0; }
}

/* Form Card */
.form-card {
  background: linear-gradient(135deg, #0d1b2a 0%, #1a0933 100%);
  border-radius: 20px;
  padding: 40px;
  width: 100%;
  max-width: 900px;
  box-shadow: 0 15px 50px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.3);
  position: relative;
  z-index: 1;
}

/* Form Header */
.form-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
}

.form-title {
  font-size: 2.5rem;
  font-weight: 800;
  color: #00ffff;
  margin: 0;
  text-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.step-indicator {
  display: flex;
  gap: 12px;
}

.step-indicator span {
  width: 35px;
  height: 35px;
  background: linear-gradient(135deg, #0d1b2a, #1a0933);
  color: #e0e0e0;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.1rem;
  transition: all 0.3s ease;
}

.step-indicator span.active {
  background: linear-gradient(135deg, #00ffff, #ff00ff);
  color: #fff;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.form-subtitle {
  font-size: 1.1rem;
  color: #e0e0e0;
  text-align: center;
  margin-bottom: 35px;
  font-style: italic;
}

/* Step Content */
.step-content {
  display: flex;
  flex-direction: column;
  gap: 25px;
  animation: slideIn 0.5s ease-in-out;
}

@keyframes slideIn {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Bước 1: Danh sách game */
.game-step {
  padding: 20px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 12px;
}

.game-list {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 25px;
}

.game-item {
  background: #0d1b2a;
  border-radius: 16px;
  padding: 20px;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.game-item:hover {
  background: linear-gradient(135deg, #00ffff, #ff00ff);
  transform: scale(1.05);
  box-shadow: 0 10px 25px rgba(0, 255, 255, 0.4);
}

.game-image {
  width: 100%;
  height: 120px;
  object-fit: cover;
  border-radius: 10px;
  margin-bottom: 15px;
  transition: all 0.3s ease;
}

.game-item:hover .game-image {
  filter: brightness(110%);
}

.game-item p {
  margin: 0;
  color: #e0e0e0;
  font-size: 1.1rem;
  font-weight: 600;
}

/* Form Group */
.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 1rem;
  color: #e0e0e0;
  font-weight: 500;
}

.form-input {
  padding: 14px 18px;
  background: #0d1b2a;
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 10px;
  color: #e0e0e0;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.form-input:focus {
  border-color: #00ffff;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.4);
  outline: none;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 25px;
}

/* Upload Area */
.upload-area {
  padding: 35px;
  border: 2px dashed rgba(0, 255, 255, 0.3);
  border-radius: 14px;
  text-align: center;
  cursor: pointer;
  background: rgba(0, 255, 255, 0.05);
  transition: all 0.3s ease;
}

.upload-area:hover {
  border-color: #00ffff;
  background: rgba(0, 255, 255, 0.1);
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.3);
}

.upload-icon {
  font-size: 2.5rem;
  color: #00ffff;
  margin-bottom: 15px;
}

.upload-area p {
  margin: 0;
  color: #e0e0e0;
  font-size: 1rem;
}

.file-input {
  display: none;
}

/* Preview Images */
.preview-container {
  padding: 15px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 12px;
}

.preview-images {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
}

.preview-item {
  position: relative;
  width: 120px;
  height: 120px;
}

.preview-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 10px;
  border: 1px solid rgba(0, 255, 255, 0.3);
  transition: all 0.3s ease;
}

.preview-image:hover {
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.4);
}

.remove-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  background: #ff00ff;
  color: #fff;
  border: none;
  border-radius: 50%;
  width: 24px;
  height: 24px;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.remove-btn:hover {
  background: #ff66ff;
  transform: scale(1.1);
}

/* Xem lại thông tin */
.review-step {
  display: flex;
  flex-direction: column;
  gap: 20px;
  padding: 20px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 12px;
}

.review-card {
  background: #0d1b2a;
  padding: 25px;
  border-radius: 16px;
  box-shadow: 0 5px 15px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.2);
  transition: all 0.3s ease;
}

.review-card:hover {
  box-shadow: 0 8px 20px rgba(0, 255, 255, 0.3);
}

.review-group {
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 12px 0;
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
}

.review-group:last-child {
  border-bottom: none;
}

.review-label {
  font-size: 1.1rem;
  color: #e0e0e0;
  font-weight: 500;
  min-width: 130px;
}

.review-value {
  font-size: 1.1rem;
  color: #00ffff;
}

.review-images {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  max-height: 200px;
  overflow-y: auto;
  padding: 10px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 10px;
}

.review-image {
  width: 100px;
  height: 100px;
  object-fit: cover;
  border-radius: 8px;
  border: 1px solid rgba(0, 255, 255, 0.3);
  transition: all 0.3s ease;
}

.review-image:hover {
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.3);
}

/* Điều hướng bước (icon lùi/tiến) */
.step-navigation {
  display: flex;
  justify-content: space-between;
  margin-top: 40px;
}

.nav-btn {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #0d1b2a, #1a0933);
  color: #e0e0e0;
  border: none;
  border-radius: 50%;
  font-size: 1.2rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.nav-btn:hover {
  background: rgba(0, 255, 255, 0.2);
  box-shadow: 0 5px 15px rgba(0, 255, 255, 0.2);
}

.prev-btn {
  background: linear-gradient(135deg, #0d1b2a, #1a0933);
}

.prev-btn:hover {
  background: rgba(0, 255, 255, 0.2);
}

.next-btn,
.submit-btn {
  background: linear-gradient(135deg, #00ffff, #ff00ff);
  color: #fff;
}

.next-btn:hover,
.submit-btn:hover {
  background: linear-gradient(135deg, #ff00ff, #00ffff);
  box-shadow: 0 5px 15px rgba(0, 255, 255, 0.4);
}

/* Modal overlay */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 999;
}

/* Modal warning */
.warning-modal {
  background: linear-gradient(135deg, #0d1b2a, #1a0933);
  color: #e0e0e0;
  padding: 35px;
  border-radius: 16px;
  width: 450px;
  max-width: 90%;
  text-align: center;
  box-shadow: 0 15px 40px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.3);
  animation: popIn 0.4s ease-in-out;
}

.warning-modal h3 {
  color: #ff00ff;
  font-size: 1.8rem;
  margin-bottom: 20px;
  text-shadow: 0 0 10px rgba(255, 0, 255, 0.5);
}

.warning-modal p {
  margin-bottom: 25px;
  font-size: 1.1rem;
  line-height: 1.6;
}

.confirmation-group {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 25px;
  justify-content: center;
}

.confirmation-group input[type="radio"] {
  accent-color: #00ffff;
  width: 20px;
  height: 20px;
}

.confirmation-group label {
  font-size: 1rem;
  color: #e0e0e0;
}

.modal-actions {
  display: flex;
  gap: 20px;
  justify-content: center;
}

.modal-confirm,
.modal-cancel {
  padding: 12px 25px;
  border-radius: 10px;
  font-weight: 600;
  cursor: pointer;
  border: none;
  transition: all 0.3s ease;
}

.modal-confirm {
  background: linear-gradient(135deg, #00ffff, #ff00ff);
  color: #fff;
}

.modal-confirm:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.modal-confirm:hover:not(:disabled) {
  background: linear-gradient(135deg, #ff00ff, #00ffff);
  box-shadow: 0 8px 20px rgba(0, 255, 255, 0.4);
}

.modal-cancel {
  background: linear-gradient(135deg, #0d1b2a, #1a0933);
  color: #e0e0e0;
}

.modal-cancel:hover {
  background: rgba(0, 255, 255, 0.2);
}

/* Modal success */
.success-modal {
  background: linear-gradient(135deg, #0d1b2a, #1a0933);
  color: #e0e0e0;
  padding: 35px;
  border-radius: 16px;
  width: 400px;
  text-align: center;
  box-shadow: 0 15px 40px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.3);
  display: flex;
  flex-direction: column;
  gap: 20px;
  animation: popIn 0.4s ease-in-out;
}

.success-icon {
  font-size: 3rem;
  color: #00ffff;
  text-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.success-modal h3 {
  color: #00ffff;
  font-size: 1.8rem;
}

.modal-close {
  padding: 12px 25px;
  background: linear-gradient(135deg, #00ffff, #ff00ff);
  color: #fff;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.modal-close:hover {
  background: linear-gradient(135deg, #ff00ff, #00ffff);
  box-shadow: 0 8px 20px rgba(0, 255, 255, 0.4);
}

/* Animation */
@keyframes popIn {
  from { transform: scale(0.9); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}

/* Transition fade */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.5s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(20px);
}

/* Responsive */
@media (max-width: 768px) {
  .form-card { padding: 30px; max-width: 90%; }
  .form-title { font-size: 2rem; }
  .form-row { grid-template-columns: 1fr; }
  .game-list { grid-template-columns: 1fr; }
  .warning-modal, .success-modal { width: 90%; }
}
</style>
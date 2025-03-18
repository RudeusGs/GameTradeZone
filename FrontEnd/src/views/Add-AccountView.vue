<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import gameApi from '@/api/gameinfor.api';
import gameFieldApi from '@/api/gamefield.api';
import gameAccountApi from '@/api/gameaccount.api';
import type { GameField } from '@/models/gameinfor.model';
import type { AddAccountGameModel } from '@/api/gameaccount.api';
import type { GameAccountField } from '@/models/gameaccountfield.model';

// Danh sách game lấy từ API
const gameOptions = ref<any[]>([]);

// Thanh tìm kiếm
const searchQuery = ref<string>('');

// Dữ liệu form
const formData = ref({
  selectedGameId: null as number | null,
  selectedGame: '',
  accountName: '',
  password: '',
  price: null as number | null,
  priceMin: null as number | null,
  files: [] as File[],
  previewImages: [] as string[],
  gameFields: [] as { id: number; name: string; value: string }[],
});

// Thông báo lỗi
const errors = ref({
  accountName: '',
  password: '',
  price: '',
  priceMin: '',
  gameFields: '',
});

// Step hiện tại
const currentStep = ref(1);

// Modal thông báo
const showWarningModal = ref(false);
const showSuccessModal = ref(false);
const showGuideModal = ref(false);

// Trạng thái xác nhận radio
const isConfirmed = ref(false);

// Tham chiếu đến input file
const fileInputRef = ref<HTMLInputElement | null>(null);

// Hàm lấy URL hình ảnh đầy đủ
const getFullImageUrl = (imageString: string | null | undefined): string => {
  if (!imageString || imageString.trim() === '') return 'https://via.placeholder.com/150';
  const baseUrl = 'https://localhost:7232/';
  const images = imageString.split(';').filter(img => img.trim() !== '');
  return images.length > 0 ? `${baseUrl}${images[0]}` : 'https://via.placeholder.com/150';
};

// Lọc danh sách game dựa trên tìm kiếm
const filteredGames = computed(() => {
  if (!searchQuery.value) return gameOptions.value;
  return gameOptions.value.filter((game) =>
    game.name.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

// Gọi API để lấy danh sách game khi component được mounted
onMounted(async () => {
  try {
    const response = await gameApi.getAll();
    if (response.data?.result?.data) {
      gameOptions.value = response.data.result.data.map((game: any, index: number) => ({
        id: game.id || index + 1,
        name: game.gameName,
        image: getFullImageUrl(game.image),
      }));
    } else {
      gameOptions.value = [
        { id: 1, name: 'Valorant', image: getFullImageUrl('ValorantImageString') },
        { id: 2, name: 'Genshin Impact', image: getFullImageUrl('GenshinImageString') },
        { id: 3, name: 'League of Legends', image: getFullImageUrl('LoLImageString') },
      ];
    }
  } catch (error) {
    console.error('Failed to fetch game list:', error);
    gameOptions.value = [
      { id: 1, name: 'Valorant', image: getFullImageUrl('ValorantImageString') },
      { id: 2, name: 'Genshin Impact', image: getFullImageUrl('GenshinImageString') },
      { id: 3, name: 'League of Legends', image: getFullImageUrl('LoLImageString') },
    ];
  }
});

// Chọn game và chuyển bước, tải thuộc tính ngay lập tức
const selectGame = async (game: any) => {
  formData.value.selectedGameId = game.id;
  formData.value.selectedGame = game.name;
  await fetchGameFields();
  currentStep.value = 2;
};

// Lấy tất cả các thuộc tính của game
const fetchGameFields = async () => {
  if (formData.value.selectedGameId) {
    try {
      const response = await gameFieldApi.getField(formData.value.selectedGameId);

      if (response.data?.result?.data && Array.isArray(response.data.result.data)) {
        formData.value.gameFields = response.data.result.data
          .filter((field: GameField) => field.isDelete !== true)
          .map((field: GameField) => ({
            id: field.id,
            name: field.fieldName || 'Thuộc tính không xác định',
            value: '',
          }));
      } else {
        formData.value.gameFields = [];
        console.warn('No valid game fields data received');
      }
    } catch (error) {
      console.error('Failed to fetch game fields:', error);
      formData.value.gameFields = [];
    }
  }
};

// Quay lại bước trước
const previousStep = () => {
  if (currentStep.value > 1) currentStep.value--;
  errors.value = { accountName: '', password: '', price: '', priceMin: '', gameFields: '' };
};

// Validate form trước khi submit
const validateStep2 = () => {
  let isValid = true;
  errors.value = { accountName: '', password: '', price: '', priceMin: '', gameFields: '' };

  if (!formData.value.accountName) {
    errors.value.accountName = 'Tên tài khoản không được để trống';
    isValid = false;
  }
  if (!formData.value.password) {
    errors.value.password = 'Mật khẩu không được để trống';
    isValid = false;
  }
  if (formData.value.price === null || formData.value.price <= 0) {
    errors.value.price = 'Giá bán phải lớn hơn 0';
    isValid = false;
  }
  if (formData.value.priceMin === null || formData.value.priceMin <= 0) {
    errors.value.priceMin = 'Giá tối thiểu phải lớn hơn 0';
    isValid = false;
  }
  if (formData.value.priceMin && formData.value.price && formData.value.priceMin > formData.value.price) {
    errors.value.priceMin = 'Giá tối thiểu không được lớn hơn giá bán';
    isValid = false;
  }

  formData.value.gameFields.forEach((field) => {
    if (!field.value) {
      errors.value.gameFields = 'Các thuộc tính của game không được để trống';
      isValid = false;
    }
  });

  return isValid;
};

// Tiếp tục bước sau
const nextStep = () => {
  if (currentStep.value === 1) {
    currentStep.value++;
  } else if (currentStep.value === 2) {
    if (validateStep2()) {
      currentStep.value++;
    }
  } else if (currentStep.value < 4) {
    currentStep.value++;
  }
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

// Hiển thị modal cảnh báo trước khi submit
const showWarning = () => {
  if (validateStep2()) {
    showWarningModal.value = true;
  } else {
    alert('Vui lòng điền đầy đủ thông tin');
  }
};

// Submit form sau khi xác nhận
const confirmSubmission = async () => {
  if (isConfirmed.value) {
    showWarningModal.value = false;
    try {
      const formDataToSend = new FormData();
      formDataToSend.append('GameInforId', formData.value.selectedGameId!.toString());
      formDataToSend.append('AccountName', formData.value.accountName);
      formDataToSend.append('Password', formData.value.password);
      formDataToSend.append('Price', formData.value.price!.toString());
      formDataToSend.append('PriceMin', formData.value.priceMin!.toString());
      formData.value.files.forEach((file) => {
        formDataToSend.append('Files', file);
      });

      const response = await gameAccountApi.add(formDataToSend);

      if (response.data?.result?.isSuccess) {
        const accountId = response.data.result.data?.id;
        if (accountId && formData.value.gameFields.length > 0) {
          for (const field of formData.value.gameFields) {
            const gameAccountField: GameAccountField = {
              GameAccountId: accountId,
              GameFieldId: field.id,
              FieldValue: field.value,
              id: 0,
            };
            await gameAccountApi.addFieldForGame(gameAccountField);
          }
        }
        showSuccessModal.value = true;
        formData.value = {
          selectedGameId: null,
          selectedGame: '',
          accountName: '',
          password: '',
          price: null,
          priceMin: null,
          files: [],
          previewImages: [],
          gameFields: [],
        };
        currentStep.value = 1;
        searchQuery.value = '';
      } else {
        alert('Thêm tài khoản thất bại');
      }
    } catch (error) {
      console.error('Error adding account:', error);
      alert('Đã xảy ra lỗi khi thêm tài khoản');
    }
  } else {
    alert('Bạn cần cam kết thông tin chính xác trước khi gửi!');
  }
};

// Mở modal hướng dẫn
const toggleGuideModal = () => {
  showGuideModal.value = !showGuideModal.value;
};
</script>

<template>
  <div class="add-account-container">
    <!-- Bubble Background -->
    <div class="bubble-background">
      <div v-for="i in 50" :key="i" class="bubble"></div>
    </div>

    <!-- Layout chính với 3 cột -->
    <div class="main-layout">
      <!-- Sidebar trái: Thống kê giao dịch gần đây -->
      <div class="sidebar sidebar-left">
        <h3 class="sidebar-title">Giao dịch gần đây</h3>
        <div class="transaction-list">
          <div class="transaction-item">Tài khoản Valorant - 500,000 VND</div>
          <div class="transaction-item">Tài khoản Genshin Impact - 1,200,000 VND</div>
          <div class="transaction-item">Tài khoản League of Legends - 800,000 VND</div>
        </div>
      </div>

      <!-- Nội dung chính -->
      <div class="main-content">
        <!-- Form thêm tài khoản -->
        <div class="form-card">
          <div class="form-header">
            <div class="step-indicator">
              <span :class="{ active: currentStep === 1 }">1</span>
              <span :class="{ active: currentStep === 2 }">2</span>
              <span :class="{ active: currentStep === 3 }">3</span>
              <span :class="{ active: currentStep === 4 }">4</span>
            </div>
          </div>
          <!-- Bước 1: Chọn game -->
          <div v-if="currentStep === 1" class="step-content game-step">
            <div class="search-bar">
              <input
                v-model="searchQuery"
                type="text"
                class="search-input"
                placeholder="Tìm kiếm game..."
              />
            </div>
            <div class="game-list">
              <div v-for="game in filteredGames" :key="game.id" class="game-item" @click="selectGame(game)">
                <img :src="game.image" alt="Game Image" class="game-image" />
                <p>{{ game.name }}</p>
              </div>
            </div>
          </div>

          <!-- Bước 2: Nhập thông tin -->
          <div v-if="currentStep === 2" class="step-content">
            <div class="form-group">
              <label>Tên tài khoản <span class="required">*</span></label>
              <input
                v-model="formData.accountName"
                type="text"
                class="form-input"
                placeholder="Tên tài khoản"
                required
              />
              <span class="error-message">{{ errors.accountName }}</span>
            </div>
            <div class="form-group">
              <label>Mật khẩu <span class="required">*</span></label>
              <input
                v-model="formData.password"
                type="password"
                class="form-input"
                placeholder="Mật khẩu"
                required
              />
              <span class="error-message">{{ errors.password }}</span>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label>Giá bán (VND) <span class="required">*</span></label>
                <input
                  v-model.number="formData.price"
                  type="number"
                  class="form-input"
                  placeholder="Giá bán"
                  min="0"
                  step="1000"
                  required
                />
                <span class="error-message">{{ errors.price }}</span>
              </div>
              <div class="form-group">
                <label>Giá tối thiểu (VND) <span class="required">*</span></label>
                <input
                  v-model.number="formData.priceMin"
                  type="number"
                  class="form-input"
                  placeholder="Giá tối thiểu"
                  min="0"
                  step="1000"
                  required
                />
                <span class="error-message">{{ errors.priceMin }}</span>
              </div>
            </div>
            <!-- Input động cho các thuộc tính của game -->
            <div v-if="formData.gameFields.length > 0" class="dynamic-fields">
              <span class="spanTitle">Thông tin thêm</span>
              <div v-for="(field, index) in formData.gameFields" :key="field.id" class="form-group">
                <label>{{ field.name }} <span class="required">*</span></label>
                <input
                  v-model="formData.gameFields[index].value"
                  type="text"
                  class="form-input"
                  :placeholder="'Nhập ' + field.name"
                  required
                />
                <span class="error-message" v-if="!field.value && errors.gameFields">{{ errors.gameFields }}</span>
              </div>
            </div>
            <div v-else class="no-fields-message">
              <p>Không có thuộc tính nào cho game này. Vui lòng liên hệ hỗ trợ nếu cần.</p>
            </div>
          </div>

          <!-- Bước 3: Upload ảnh -->
          <div v-if="currentStep === 3" class="step-content">
            <div class="form-group">
              <label>Hình ảnh tài khoản</label>
              <div class="upload-area" @click="triggerFileInput">
                <i class="fas fa-cloud-upload-alt upload-icon"></i>
                <p>Tải ảnh lên (nhấp hoặc kéo thả)</p>
                <input
                  ref="fileInputRef"
                  type="file"
                  multiple
                  accept="image/*"
                  @change="handleFileChange"
                  class="file-input"
                />
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
                  <img
                    v-for="(image, index) in formData.previewImages"
                    :key="index"
                    :src="image"
                    alt="Preview"
                    class="review-image"
                  />
                </div>
                <span v-else class="review-value">Chưa có ảnh</span>
              </div>
              <!-- Hiển thị các thuộc tính của game trong phần xem lại -->
              <div v-for="(field, index) in formData.gameFields" :key="index" class="review-group">
                <span class="review-label">{{ field.name }}:</span>
                <span class="review-value">{{ field.value }}</span>
              </div>
            </div>
          </div>

          <!-- Điều hướng bước -->
          <div class="step-navigation">
            <button v-if="currentStep > 1" @click="previousStep" class="nav-btn prev-btn">
              <i class="fas fa-arrow-left"></i>
            </button>
            <button v-if="currentStep < 4" @click="nextStep" class="nav-btn next-btn">
              <i class="fas fa-arrow-right"></i>
            </button>
            <button v-if="currentStep === 4" @click="showWarning" class="nav-btn submit-btn">
              <i class="fas fa-check"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Sidebar phải: Đánh giá từ người dùng -->
      <div class="sidebar sidebar-right">
        <h3 class="sidebar-title">Đánh giá từ người dùng</h3>
        <div class="review-list">
          <div class="review-item">
            <p>"Giao dịch nhanh chóng và an toàn!" - Nguyễn Văn A</p>
          </div>
          <div class="review-item">
            <p>"Hỗ trợ khách hàng tuyệt vời!" - Trần Thị B</p>
          </div>
          <div class="review-item">
            <p>"Tôi rất hài lòng với dịch vụ!" - Lê Văn C</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Nút dấu hỏi -->
    <button class="help-btn" @click="toggleGuideModal">
      <i class="fas fa-question"></i>
    </button>

    <!-- Modal cảnh báo -->
    <transition name="fade">
      <div v-if="showWarningModal" class="modal-overlay">
        <div class="modal-content warning-modal">
          <h3>CẢNH BÁO NGHIÊM KHẮC</h3>
          <p class="warning-text">
            TẤT CẢ THÔNG TIN TÀI KHOẢN PHẢI CHÍNH XÁC 100%! NẾU PHÁT HIỆN CỐ Ý ĐĂNG TẢI THÔNG TIN SAI SỰ THẬT, TÀI KHOẢN CỦA BẠN SẼ BỊ KHÓA VĨNH VIỄN KHÔNG CÓ LÝ DO!
          </p>
          <div class="confirmation-group">
            <input type="checkbox" id="confirm" v-model="isConfirmed" />
            <label for="confirm">Tôi cam kết thông tin chính xác</label>
          </div>
          <div class="modal-actions">
            <button @click="confirmSubmission" class="modal-confirm" :disabled="!isConfirmed">XÁC NHẬN</button>
            <button @click="showWarningModal = false" class="modal-cancel">HỦY BỎ</button>
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

    <!-- Modal hướng dẫn -->
    <transition name="fade">
      <div v-if="showGuideModal" class="modal-overlay">
        <div class="modal-content guide-modal">
          <h3>Hướng Dẫn Đăng Tài Khoản</h3>
          <p>Chào mừng bạn đến với thế giới giao dịch tài khoản game! Để mọi thứ diễn ra suôn sẻ và an toàn, hãy làm theo các hướng dẫn dưới đây nhé!</p>
          <h4>1. Bảo vệ thông tin cá nhân</h4>
          <p>Trước khi đăng bán, hãy kiểm tra kỹ Gmail liên kết với tài khoản game. Đừng để sót thông tin nhạy cảm như số điện thoại hay dữ liệu cá nhân – bảo vệ sự riêng tư của bạn là điều quan trọng nhất!</p>
          <h4>2. Sử dụng Gmail rác</h4>
          <p>Người mua có thể hỏi tên Gmail của tài khoản. Hãy dùng Gmail rác – loại không chứa thông tin quan trọng – để giữ an toàn. Tốt nhất là liên kết tài khoản với Gmail dùng một lần trước khi bán.</p>
          <h4>3. Cung cấp thông tin cho người mua</h4>
          <p>Giao dịch xong xuôi? Hãy gửi đầy đủ và chính xác thông tin như tên tài khoản, mật khẩu cho người mua. Uy tín của bạn sẽ được củng cố, và chẳng ai thích tranh cãi sau khi deal xong đâu, đúng không?</p>
          <h4>4. Hướng dẫn đăng tài khoản</h4>
          <p>Sẵn sàng bán tài khoản chưa? Đây là cách thực hiện đơn giản:</p>
          <ul>
            <li><strong>Bước 1:</strong> Chọn game bạn muốn bán từ danh sách.</li>
            <li><strong>Bước 2:</strong> Điền thông tin: tên tài khoản, mật khẩu, giá bán, giá tối thiểu và các thuộc tính của tài khoản.</li>
            <li><strong>Bước 3:</strong> Thêm hình ảnh tài khoản (nếu có) để thu hút người mua.</li>
            <li><strong>Bước 4:</strong> Kiểm tra lại và nhấn "Gửi" – thế là xong!</li>
          </ul>
          <p>Có thắc mắc gì không? Đừng ngại liên hệ nhé. Chúc bạn đăng bán thành công và kiếm được kha khá từ tài khoản của mình!</p>
          <button @click="toggleGuideModal" class="modal-close">Đóng</button>
        </div>
      </div>
    </transition>
  </div>
</template>

<style scoped>
/* Tổng thể */
.add-account-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
  font-family: 'Arial', sans-serif;
  padding: 20px;
  position: relative;
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
  width: 10px;
  height: 10px;
}

.bubble:nth-child(even) {
  background: rgba(0, 255, 255, 0.4);
  width: 15px;
  height: 15px;
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

/* Layout chính */
.main-layout {
  display: grid;
  grid-template-columns: 250px 1fr 250px;
  gap: 20px;
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
}

/* Sidebar chung */
.sidebar {
  background: linear-gradient(135deg, #0d1b2a 0%, #1a0933 100%);
  border-radius: 15px;
  padding: 15px;
  box-shadow: 0 10px 30px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.3);
  height: fit-content;
}

.sidebar-title {
  font-size: 1.2rem;
  color: #00ffff;
  margin-bottom: 15px;
  text-shadow: 0 0 8px rgba(0, 255, 255, 0.5);
}

/* Sidebar trái: Thống kê giao dịch gần đây */
.transaction-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.transaction-item {
  background: rgba(0, 255, 255, 0.1);
  padding: 10px;
  border-radius: 10px;
  color: #e0e0e0;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.transaction-item:hover {
  background: rgba(0, 255, 255, 0.2);
  box-shadow: 0 5px 10px rgba(0, 255, 255, 0.2);
}

/* Sidebar phải: Đánh giá từ người dùng */
.review-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.review-item {
  background: rgba(255, 0, 255, 0.1);
  padding: 10px;
  border-radius: 10px;
  color: #e0e0e0;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.review-item:hover {
  background: rgba(255, 0, 255, 0.2);
  box-shadow: 0 5px 10px rgba(255, 0, 255, 0.2);
}

/* Nội dung chính */
.main-content {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* Form Card */
.form-card {
  background: linear-gradient(135deg, #0d1b2a 0%, #1a0933 100%);
  border-radius: 15px;
  padding: 20px;
  width: 100%;
  box-shadow: 0 10px 30px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.3);
  position: relative;
  z-index: 1;
}

/* Form Header */
.form-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.step-indicator {
  display: flex;
  gap: 8px;
}

.step-indicator span {
  width: 30px;
  height: 30px;
  background: linear-gradient(135deg, #0d1b2a, #1a0933);
  color: #e0e0e0;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.step-indicator span.active {
  background: linear-gradient(135deg, #00ffff, #ff00ff);
  color: #fff;
  box-shadow: 0 0 8px rgba(0, 255, 255, 0.5);
}

/* Step Content */
.step-content {
  display: flex;
  flex-direction: column;
  gap: 15px;
  animation: slideIn 0.5s ease-in-out;
}

@keyframes slideIn {
  from { opacity: 0; transform: translateY(15px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Thanh tìm kiếm */
.search-bar {
  margin-bottom: 10px;
}

.search-input {
  width: 100%;
  padding: 10px 14px;
  background: #0d1b2a;
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 8px;
  color: #e0e0e0;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.search-input:focus {
  border-color: #00ffff;
  box-shadow: 0 0 8px rgba(0, 255, 255, 0.4);
  outline: none;
}

/* Bước 1: Danh sách game */
.game-step {
  padding: 15px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 10px;
}

.game-list {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  justify-content: center;
}

.game-item {
  background: #0d1b2a;
  border-radius: 12px;
  padding: 15px;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  width: 150px;
  flex: 0 0 150px;
}

.game-item:hover {
  background: linear-gradient(135deg, #00ffff, #ff00ff);
  transform: scale(1.05);
  box-shadow: 0 8px 20px rgba(0, 255, 255, 0.4);
}

.game-image {
  width: 100%;
  height: 90px;
  object-fit: cover;
  border-radius: 8px;
  margin-bottom: 10px;
  transition: all 0.3s ease;
}

.game-item:hover .game-image {
  filter: brightness(110%);
}

.game-item p {
  margin: 0;
  color: #e0e0e0;
  font-size: 0.9rem;
  font-weight: 600;
}

/* Form Group */
.form-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.form-group label {
  font-size: 0.9rem;
  color: #e0e0e0;
  font-weight: 500;
}

.required {
  color: #ff00ff;
}

.form-input {
  padding: 10px 14px;
  background: #0d1b2a;
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 8px;
  color: #e0e0e0;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.form-input:focus {
  border-color: #00ffff;
  box-shadow: 0 0 8px rgba(0, 255, 255, 0.4);
  outline: none;
}

.error-message {
  color: #ff00ff;
  font-size: 0.8rem;
  min-height: 16px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 15px;
}

.dynamic-fields {
  margin-top: 15px;
}

.no-fields-message {
  color: #e0e0e0;
  font-size: 0.9rem;
  text-align: center;
  padding: 10px;
  background: rgba(255, 0, 255, 0.1);
  border-radius: 8px;
}

.no-fields-message p {
  margin: 0;
}

/* Upload Area */
.upload-area {
  padding: 20px;
  border: 2px dashed rgba(0, 255, 255, 0.3);
  border-radius: 10px;
  text-align: center;
  cursor: pointer;
  background: rgba(0, 255, 255, 0.05);
  transition: all 0.3s ease;
}

.upload-area:hover {
  border-color: #00ffff;
  background: rgba(0, 255, 255, 0.1);
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.3);
}

.upload-icon {
  font-size: 2rem;
  color: #00ffff;
  margin-bottom: 10px;
}

.upload-area p {
  margin: 0;
  color: #e0e0e0;
  font-size: 0.9rem;
}

.file-input {
  display: none;
}

/* Preview Images */
.preview-container {
  padding: 10px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 10px;
}

.preview-images {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.preview-item {
  position: relative;
  width: 90px;
  height: 90px;
}

.preview-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 8px;
  border: 1px solid rgba(0, 255, 255, 0.3);
  transition: all 0.3s ease;
}

.preview-image:hover {
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.4);
}

.remove-btn {
  position: absolute;
  top: 5px;
  right: 5px;
  background: #ff00ff;
  color: #fff;
  border: none;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  font-size: 0.8rem;
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
  gap: 15px;
  padding: 15px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 10px;
}

.review-card {
  background: #0d1b2a;
  padding: 15px;
  border-radius: 12px;
  box-shadow: 0 5px 10px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.2);
  transition: all 0.3s ease;
}

.review-card:hover {
  box-shadow: 0 5px 15px rgba(0, 255, 255, 0.3);
}

.spanTitle {
  display: flex;
  justify-content: center;
}

.review-group {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 0;
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
}

.review-group:last-child {
  border-bottom: none;
}

.review-label {
  font-size: 0.9rem;
  color: #e0e0e0;
  font-weight: 500;
  min-width: 100px;
}

.review-value {
  font-size: 0.9rem;
  color: #00ffff;
}

.review-images {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  padding: 8px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 8px;
}

.review-image {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 6px;
  border: 1px solid rgba(0, 255, 255, 0.3);
  transition: all 0.3s ease;
}

.review-image:hover {
  box-shadow: 0 0 8px rgba(0, 255, 255, 0.3);
}

/* Điều hướng bước */
.step-navigation {
  display: flex;
  justify-content: space-between;
  margin-top: 20px;
}

.nav-btn {
  width: 35px;
  height: 35px;
  background: linear-gradient(135deg, #0d1b2a, #1a0933);
  color: #e0e0e0;
  border: none;
  border-radius: 50%;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.nav-btn:hover {
  background: rgba(0, 255, 255, 0.2);
  box-shadow: 0 5px 10px rgba(0, 255, 255, 0.2);
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
  box-shadow: 0 5px 10px rgba(0, 255, 255, 0.4);
}

/* Nút dấu hỏi */
.help-btn {
  position: fixed;
  bottom: 20px;
  right: 20px;
  width: 40px;
  height: 40px;
  background: linear-gradient(45deg, #00ffff, #ff00ff);
  border: none;
  border-radius: 50%;
  color: #fff;
  font-size: 1.2rem;
  cursor: pointer;
  z-index: 1000;
  animation: pulse 2s infinite;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
  transition: all 0.3s ease;
}

.help-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 0 15px rgba(255, 0, 255, 0.7);
}

@keyframes pulse {
  0% { box-shadow: 0 0 0 0 rgba(0, 255, 255, 0.7); }
  70% { box-shadow: 0 0 0 10px rgba(0, 255, 255, 0); }
  100% { box-shadow: 0 0 0 0 rgba(0, 255, 255, 0); }
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
  padding: 20px;
  border-radius: 12px;
  width: 350px;
  max-width: 90%;
  text-align: center;
  box-shadow: 0 10px 30px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(255, 0, 0, 0.5);
  animation: popIn 0.4s ease-in-out;
}

.warning-modal h3 {
  color: #ff0000;
  font-size: 1.8rem;
  margin-bottom: 15px;
  text-shadow: 0 0 8px rgba(255, 0, 0, 0.5);
  font-weight: bold;
}

.warning-text {
  margin-bottom: 15px;
  font-size: 1rem;
  line-height: 1.5;
  color: #ffcccc;
  font-weight: 600;
}

.confirmation-group {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 15px;
  justify-content: center;
}

.confirmation-group input[type='checkbox'] {
  accent-color: #ff0000;
  width: 18px;
  height: 18px;
}

.confirmation-group label {
  font-size: 0.95rem;
  color: #e0e0e0;
  font-weight: 500;
}

.modal-actions {
  display: flex;
  gap: 15px;
  justify-content: center;
}

.modal-confirm,
.modal-cancel {
  padding: 10px 20px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  border: none;
  transition: all 0.3s ease;
  font-size: 0.9rem;
}

.modal-confirm {
  background: linear-gradient(135deg, #ff0000, #ff6666);
  color: #fff;
}

.modal-confirm:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.modal-confirm:hover:not(:disabled) {
  background: linear-gradient(135deg, #ff6666, #ff0000);
  box-shadow: 0 5px 15px rgba(255, 0, 0, 0.4);
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
  padding: 20px;
  border-radius: 12px;
  width: 300px;
  text-align: center;
  box-shadow: 0 10px 30px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.3);
  display: flex;
  flex-direction: column;
  gap: 15px;
  animation: popIn 0.4s ease-in-out;
}

.success-icon {
  font-size: 2.5rem;
  color: #00ffff;
  text-shadow: 0 0 8px rgba(0, 255, 255, 0.5);
}

.success-modal h3 {
  color: #00ffff;
  font-size: 1.5rem;
}

/* Modal guide */
.guide-modal {
  background: linear-gradient(135deg, #0d1b2a, #1a0933);
  color: #e0e0e0;
  padding: 20px;
  border-radius: 12px;
  width: 500px;
  max-width: 90%;
  max-height: 80vh;
  overflow-y: auto;
  text-align: left;
  box-shadow: 0 10px 30px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.3);
  animation: popIn 0.4s ease-in-out;
}

.guide-modal h3 {
  color: #00ffff;
  font-size: 1.5rem;
  margin-bottom: 15px;
  text-shadow: 0 0 8px rgba(0, 255, 255, 0.5);
}

.guide-modal h4 {
  color: #ff00ff;
  font-size: 1.2rem;
  margin-top: 15px;
  margin-bottom: 8px;
}

.guide-modal p {
  font-size: 0.9rem;
  line-height: 1.5;
  margin-bottom: 10px;
}

.guide-modal ul {
  list-style-type: disc;
  margin-left: 15px;
  margin-bottom: 15px;
}

.guide-modal li {
  font-size: 0.9rem;
  line-height: 1.5;
  color: #e0e0e0;
}

.guide-modal li strong {
  color: #00ff00;
}

.modal-close {
  padding: 10px 20px;
  background: linear-gradient(135deg, #00ffff, #ff00ff);
  color: #fff;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 15px;
  font-size: 0.9rem;
}

.modal-close:hover {
  background: linear-gradient(135deg, #ff00ff, #00ffff);
  box-shadow: 0 5px 15px rgba(0, 255, 255, 0.4);
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
  transform: translateY(15px);
}

/* Responsive */
@media (max-width: 1024px) {
  .main-layout {
    grid-template-columns: 1fr;
  }
  .sidebar {
    display: none;
  }
}

@media (max-width: 768px) {
  .form-card {
    padding: 15px;
  }
  .form-row {
    grid-template-columns: 1fr;
  }
  .game-list {
    justify-content: center;
  }
  .game-item {
    width: 140px;
    flex: 0 0 140px;
  }
  .warning-modal,
  .success-modal,
  .guide-modal {
    width: 90%;
    padding: 15px;
  }
  .guide-modal {
    max-height: 70vh;
  }
}
</style>
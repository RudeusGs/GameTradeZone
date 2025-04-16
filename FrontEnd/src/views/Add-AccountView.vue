<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import gameApi from '@/api/gameinfor.api';
import gameFieldApi from '@/api/gamefield.api';
import gameAccountApi from '@/api/gameaccount.api';
import type { GameField } from '@/models/gameinfor.model';
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
}
</script>

<template>
  <div class="add-account-container">
    <!-- Glass Panel Background -->
    <div class="glass-panels">
      <div class="glass-panel panel-1"></div>
      <div class="glass-panel panel-2"></div>
      <div class="glass-panel panel-3"></div>
      <div class="glass-panel panel-4"></div>
    </div>

    <!-- Animated Particles -->
    <div class="particles">
      <div v-for="i in 30" :key="i" class="particle"></div>
    </div>

    <!-- Header -->
    <header class="app-header">
      <button class="help-button" @click="toggleGuideModal">
        <i class="fas fa-question-circle"></i>
        <span>Hướng dẫn</span>
      </button>
    </header>

    <!-- Main Container -->
    <div class="main-container">
      <!-- Progress Steps -->
      <div class="progress-bar">
        <div class="progress-step" :class="{ active: currentStep >= 1, completed: currentStep > 1 }">
          <div class="step-number">1</div>
          <div class="step-label">Chọn Game</div>
        </div>
        <div class="progress-line" :class="{ active: currentStep > 1 }"></div>
        <div class="progress-step" :class="{ active: currentStep >= 2, completed: currentStep > 2 }">
          <div class="step-number">2</div>
          <div class="step-label">Thông Tin</div>
        </div>
        <div class="progress-line" :class="{ active: currentStep > 2 }"></div>
        <div class="progress-step" :class="{ active: currentStep >= 3, completed: currentStep > 3 }">
          <div class="step-number">3</div>
          <div class="step-label">Hình Ảnh</div>
        </div>
        <div class="progress-line" :class="{ active: currentStep > 3 }"></div>
        <div class="progress-step" :class="{ active: currentStep >= 4 }">
          <div class="step-number">4</div>
          <div class="step-label">Xác Nhận</div>
        </div>
      </div>

      <!-- Content Card -->
      <div class="content-card">
        <!-- Step 1: Game Selection -->
        <div v-if="currentStep === 1" class="step-content game-selection">
          <h2 class="step-title">Chọn Game Cần Bán</h2>
          <div class="search-container">
            <div class="search-box">
              <i class="fas fa-search"></i>
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Tìm tên game..."
                class="search-input"
              />
            </div>
          </div>

          <div class="game-grid">
            <div
              v-for="game in filteredGames"
              :key="game.id"
              class="game-card"
              @click="selectGame(game)"
            >
              <div class="game-img-container">
                <img :src="game.image" :alt="game.name" class="game-img" />
              </div>
              <div class="game-name">{{ game.name }}</div>
            </div>
          </div>
        </div>

        <!-- Step 2: Account Details -->
        <div v-if="currentStep === 2" class="step-content account-details">
          <h2 class="step-title">Thông Tin Tài Khoản</h2>
          <p class="game-selected">
            <span class="label">Game đã chọn:</span>
            <span class="value">{{ formData.selectedGame }}</span>
          </p>

          <div class="form-group">
            <label for="accountName">
              Tên tài khoản <span class="required">*</span>
            </label>
            <div class="input-container">
              <i class="fas fa-user"></i>
              <input
                id="accountName"
                v-model="formData.accountName"
                type="text"
                placeholder="Nhập tên tài khoản"
              />
            </div>
            <p class="error-message" v-if="errors.accountName">{{ errors.accountName }}</p>
          </div>

          <div class="form-group">
            <label for="password">
              Mật khẩu <span class="required">*</span>
            </label>
            <div class="input-container">
              <i class="fas fa-lock"></i>
              <input
                id="password"
                v-model="formData.password"
                type="password"
                placeholder="Nhập mật khẩu"
              />
            </div>
            <p class="error-message" v-if="errors.password">{{ errors.password }}</p>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="price">
                Giá bán (VNĐ) <span class="required">*</span>
              </label>
              <div class="input-container">
                <i class="fas fa-tag"></i>
                <input
                  id="price"
                  v-model.number="formData.price"
                  type="number"
                  placeholder="Nhập giá bán"
                  min="0"
                  step="1000"
                />
              </div>
              <p class="error-message" v-if="errors.price">{{ errors.price }}</p>
            </div>

            <div class="form-group">
              <label for="priceMin">
                Giá tối thiểu (VNĐ) <span class="required">*</span>
              </label>
              <div class="input-container">
                <i class="fas fa-money-bill-wave"></i>
                <input
                  id="priceMin"
                  v-model.number="formData.priceMin"
                  type="number"
                  placeholder="Nhập giá tối thiểu"
                  min="0"
                  step="1000"
                />
              </div>
              <p class="error-message" v-if="errors.priceMin">{{ errors.priceMin }}</p>
            </div>
          </div>

          <div v-if="formData.gameFields.length > 0" class="game-fields">
            <h3 class="fields-title">Thông Tin Bổ Sung</h3>

            <div v-for="(field, index) in formData.gameFields" :key="field.id" class="form-group">
              <label :for="'field-' + field.id">
                {{ field.name }} <span class="required">*</span>
              </label>
              <div class="input-container">
                <i class="fas fa-info-circle"></i>
                <input
                  :id="'field-' + field.id"
                  v-model="formData.gameFields[index].value"
                  type="text"
                  :placeholder="'Nhập ' + field.name"
                />
              </div>
              <p class="error-message" v-if="!field.value && errors.gameFields">
                {{ errors.gameFields }}
              </p>
            </div>
          </div>

          <div v-else class="no-fields">
            <i class="fas fa-exclamation-circle"></i>
            <p>Không có thuộc tính bổ sung cho game này</p>
          </div>
        </div>

        <!-- Step 3: Image Upload -->
        <div v-if="currentStep === 3" class="step-content image-upload">
          <h2 class="step-title">Hình Ảnh Tài Khoản</h2>

          <div class="upload-zone" @click="triggerFileInput">
            <div class="upload-icon">
              <i class="fas fa-cloud-upload-alt"></i>
            </div>
            <p class="upload-text">Kéo thả hoặc nhấp để tải ảnh lên</p>
            <p class="upload-hint">Hình ảnh giúp tăng khả năng bán tài khoản</p>
            <input
              ref="fileInputRef"
              type="file"
              multiple
              accept="image/*"
              @change="handleFileChange"
              class="file-input"
            />
          </div>

          <div v-if="formData.previewImages.length > 0" class="image-preview-container">
            <div class="image-count">
              <i class="fas fa-images"></i>
              <span>{{ formData.previewImages.length }} hình ảnh</span>
            </div>

            <div class="image-grid">
              <div v-for="(image, index) in formData.previewImages" :key="index" class="image-item">
                <img :src="image" alt="Preview" class="preview-img" />
                <button class="remove-image" @click="removeImage(index)">
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Step 4: Review -->
        <div v-if="currentStep === 4" class="step-content review-step">
          <h2 class="step-title">Xác Nhận Thông Tin</h2>

          <div class="review-data">
            <div class="review-section">
              <h3 class="review-section-title">
                <i class="fas fa-gamepad"></i> Thông tin game
              </h3>
              <div class="review-field">
                <span class="field-name">Game:</span>
                <span class="field-value">{{ formData.selectedGame }}</span>
              </div>
            </div>

            <div class="review-section">
              <h3 class="review-section-title">
                <i class="fas fa-user-lock"></i> Thông tin tài khoản
              </h3>
              <div class="review-field">
                <span class="field-name">Tên tài khoản:</span>
                <span class="field-value">{{ formData.accountName }}</span>
              </div>
              <div class="review-field">
                <span class="field-name">Mật khẩu:</span>
                <span class="field-value">{{ formData.password }}</span>
              </div>
            </div>

            <div class="review-section">
              <h3 class="review-section-title">
                <i class="fas fa-tag"></i> Thông tin giá
              </h3>
              <div class="review-field">
                <span class="field-name">Giá bán:</span>
                <span class="field-value price-value">{{ formData.price?.toLocaleString() }} VNĐ</span>
              </div>
              <div class="review-field">
                <span class="field-name">Giá tối thiểu:</span>
                <span class="field-value price-value">{{ formData.priceMin?.toLocaleString() }} VNĐ</span>
              </div>
            </div>

            <div v-if="formData.gameFields.length > 0" class="review-section">
              <h3 class="review-section-title">
                <i class="fas fa-info-circle"></i> Thông tin bổ sung
              </h3>
              <div v-for="field in formData.gameFields" :key="field.id" class="review-field">
                <span class="field-name">{{ field.name }}:</span>
                <span class="field-value">{{ field.value }}</span>
              </div>
            </div>

            <div v-if="formData.previewImages.length > 0" class="review-section">
              <h3 class="review-section-title">
                <i class="fas fa-images"></i> Hình ảnh ({{ formData.previewImages.length }})
              </h3>
              <div class="review-images">
                <img
                  v-for="(image, index) in formData.previewImages"
                  :key="index"
                  :src="image"
                  alt="Preview"
                  class="review-image"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Navigation Controls -->
        <div class="step-controls">
          <button
            v-if="currentStep > 1"
            @click="previousStep"
            class="btn btn-prev"
          >
            <i class="fas fa-arrow-left"></i>
            <span>Quay lại</span>
          </button>

          <button
            v-if="currentStep < 4"
            @click="nextStep"
            class="btn btn-next"
          >
            <span>Tiếp tục</span>
            <i class="fas fa-arrow-right"></i>
          </button>

          <button
            v-if="currentStep === 4"
            @click="showWarning"
            class="btn btn-submit"
          >
            <span>Đăng bán</span>
            <i class="fas fa-check"></i>
          </button>
        </div>
      </div>

      <!-- Information cards -->
      <div class="info-cards">
        <div class="info-card">
          <div class="info-icon">
            <i class="fas fa-shield-alt"></i>
          </div>
          <div class="info-content">
            <h3>Bảo mật tuyệt đối</h3>
            <p>Thông tin tài khoản của bạn được mã hóa và bảo vệ an toàn</p>
          </div>
        </div>
        
        <div class="info-card">
          <div class="info-icon">
            <i class="fas fa-bolt"></i>
          </div>
          <div class="info-content">
            <h3>Giao dịch nhanh chóng</h3>
            <p>Tài khoản của bạn sẽ được đăng ngay lập tức sau khi xác nhận</p>
          </div>
        </div>
        
        <div class="info-card">
          <div class="info-icon">
            <i class="fas fa-percentage"></i>
          </div>
          <div class="info-content">
            <h3>Phí giao dịch thấp</h3>
            <p>Chỉ 5% giá trị giao dịch sẽ được trích làm phí dịch vụ</p>
          </div>
        </div>
        
        <div class="info-card">
          <div class="info-icon">
            <i class="fas fa-headset"></i>
          </div>
          <div class="info-content">
            <h3>Hỗ trợ 24/7</h3>
            <p>Đội ngũ hỗ trợ luôn sẵn sàng giải đáp mọi thắc mắc của bạn</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Warning Modal -->
    <transition name="modal-fade">
      <div v-if="showWarningModal" class="modal-overlay">
        <div class="modal-container">
          <div class="modal-card">
            <div class="modal-header">
              <h3>Xác nhận thông tin</h3>
              <button @click="showWarningModal = false" class="close-btn">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <div class="warning-box">
                <div class="warning-icon">
                  <i class="fas fa-exclamation-triangle"></i>
                </div>
                <p>
                  Tôi cam kết rằng tất cả thông tin tài khoản đã cung cấp là <strong>chính xác 100%</strong>.
                  Tôi hiểu rằng việc cố ý cung cấp thông tin sai sự thật có thể dẫn đến việc
                  <strong>khóa vĩnh viễn</strong> tài khoản của tôi.
                </p>
              </div>
              <div class="checkbox-container">
                <label class="checkbox-label">
                  <input type="checkbox" v-model="isConfirmed" />
                  <span class="checkbox-text">Tôi xác nhận thông tin là chính xác và đồng ý với các điều khoản</span>
                </label>
              </div>
            </div>
            <div class="modal-footer">
              <button @click="showWarningModal = false" class="btn-cancel">Hủy bỏ</button>
              <button 
                @click="confirmSubmission" 
                :disabled="!isConfirmed" 
                :class="['btn-confirm', {'btn-disabled': !isConfirmed}]"
              >
                Xác nhận
              </button>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- Success Modal -->
    <transition name="modal-fade">
      <div v-if="showSuccessModal" class="modal-overlay">
        <div class="modal-container">
          <div class="modal-card success-card">
            <div class="success-icon">
              <i class="fas fa-check-circle"></i>
            </div>
            <div class="modal-body text-center">
              <h3 class="success-title">Thành công!</h3>
              <p class="success-message">
                Tài khoản của bạn đã được đăng thành công và đang được hiển thị cho người mua tiềm năng.
              </p>
              <div class="success-info">
                <div class="info-item">
                  <i class="fas fa-check"></i>
                  <span>Thông tin đã được xác nhận</span>
                </div>
                <div class="info-item">
                  <i class="fas fa-eye"></i>
                  <span>Tài khoản đã hiển thị công khai</span>
                </div>
                <div class="info-item">
                  <i class="fas fa-bell"></i>
                  <span>Bạn sẽ nhận thông báo khi có người đặt mua</span>
                </div>
              </div>
            </div>
            <div class="modal-footer center">
              <button @click="showSuccessModal = false" class="btn-success">Đóng</button>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- Guide Modal -->
    <transition name="modal-fade">
      <div v-if="showGuideModal" class="modal-overlay guide-modal-overlay">
        <div class="modal-container guide-modal-container">
          <div class="modal-card">
            <div class="modal-header">
              <h3><i class="fas fa-book"></i> Hướng dẫn đăng tài khoản</h3>
              <button @click="toggleGuideModal" class="close-btn">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body guide-content">
              <div class="guide-section">
                <h4 class="guide-title">
                  <i class="fas fa-shield-alt"></i>
                  Bảo vệ thông tin cá nhân
                </h4>
                <p>
                  Trước khi đăng bán, hãy kiểm tra kỹ Gmail liên kết với tài khoản game.
                  Đừng để sót thông tin nhạy cảm như số điện thoại hay dữ liệu cá nhân –
                  bảo vệ sự riêng tư của bạn là điều quan trọng nhất!
                </p>
              </div>

              <div class="guide-section">
                <h4 class="guide-title">
                  <i class="fas fa-envelope"></i>
                  Sử dụng Gmail rác
                </h4>
                <p>
                  Người mua có thể hỏi tên Gmail của tài khoản. Hãy dùng Gmail rác –
                  loại không chứa thông tin quan trọng – để giữ an toàn. Tốt nhất là liên kết
                  tài khoản với Gmail dùng một lần trước khi bán.
                </p>
              </div>

              <div class="guide-section">
                <h4 class="guide-title">
                  <i class="fas fa-exchange-alt"></i>
                  Cung cấp thông tin cho người mua
                </h4>
                <p>
                  Giao dịch xong xuôi? Hãy gửi đầy đủ và chính xác thông tin như
                  tên tài khoản, mật khẩu cho người mua. Uy tín của bạn sẽ được củng cố,
                  và chẳng ai thích tranh cãi sau khi deal xong đâu, đúng không?
                </p>
              </div>

              <div class="guide-section">
                <h4 class="guide-title">
                  <i class="fas fa-list-ol"></i>
                  Các bước đăng bán tài khoản
                </h4>
                <div class="steps">
                  <div class="step">
                    <div class="step-indicator">1</div>
                    <div class="step-description">
                      <strong>Chọn game</strong>: Chọn loại game bạn muốn bán tài khoản từ danh sách
                    </div>
                  </div>

                  <div class="step">
                    <div class="step-indicator">2</div>
                    <div class="step-description">
                      <strong>Nhập thông tin</strong>: Điền tên đăng nhập, mật khẩu, giá bán và các thuộc tính cụ thể
                    </div>
                  </div>

                  <div class="step">
                    <div class="step-indicator">3</div>
                    <div class="step-description">
                      <strong>Tải ảnh lên</strong>: Thêm ảnh chụp màn hình tài khoản mà bạn muốn đăng, nhưng thông tin phải khớp với hình ảnh
                    </div>
                  </div>

                  <div class="step">
                    <div class="step-indicator">4</div>
                    <div class="step-description">
                      <strong>Xác nhận</strong>: Kiểm tra lại thông tin và xác nhận để đăng bán
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button @click="toggleGuideModal" class="btn-primary">Đã hiểu</button>
            </div>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<style scoped>
/* Main Layout & Base Styles */
.add-account-container {
  min-height: 100vh;
  background-color: #0a0e17;
  font-family: 'Poppins', sans-serif;
  color: #e1e7ef;
  position: relative;
  overflow: hidden;
  padding: 24px;
}

/* Glass Panels Background */
.glass-panels {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 0;
  overflow: hidden;
}

.glass-panel {
  position: absolute;
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(5px);
  border-radius: 30px;
  box-shadow: 0 0 30px rgba(0, 161, 255, 0.1);
  transform: rotate(15deg);
  border: 1px solid rgba(255, 255, 255, 0.05);
  transition: all 0.5s ease;
}

.panel-1 {
  width: 50%;
  height: 60%;
  top: -10%;
  left: -5%;
  background: linear-gradient(45deg, rgba(33, 33, 99, 0.05), rgba(51, 153, 255, 0.05));
  animation: floatPanel 20s infinite alternate ease-in-out;
}

.panel-2 {
  width: 60%;
  height: 50%;
  top: 15%;
  right: -15%;
  background: linear-gradient(45deg, rgba(76, 0, 255, 0.05), rgba(0, 204, 255, 0.05));
  animation: floatPanel 15s infinite alternate-reverse ease-in-out;
}

.panel-3 {
  width: 40%;
  height: 40%;
  bottom: -5%;
  left: 10%;
  background: linear-gradient(45deg, rgba(255, 0, 153, 0.05), rgba(102, 0, 255, 0.05));
  animation: floatPanel 18s infinite alternate ease-in-out;
}

.panel-4 {
  width: 70%;
  height: 30%;
  bottom: 10%;
  right: 5%;
  background: linear-gradient(45deg, rgba(102, 255, 204, 0.05), rgba(0, 102, 255, 0.05));
  animation: floatPanel 25s infinite alternate-reverse ease-in-out;
}

@keyframes floatPanel {
  0% {
    transform: rotate(15deg) translate(0, 0);
  }
  50% {
    transform: rotate(13deg) translate(-15px, 15px);
  }
  100% {
    transform: rotate(16deg) translate(15px, -15px);
  }
}

/* Particles */
.particles {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 1;
  overflow: hidden;
}

.particle {
  position: absolute;
  width: 3px;
  height: 3px;
  background: rgba(255, 255, 255, 0.5);
  border-radius: 50%;
  pointer-events: none;
}

.particle:nth-child(odd) {
  background: linear-gradient(45deg, #3498db, #9b59b6);
  box-shadow: 0 0 10px 2px rgba(52, 152, 219, 0.5);
  animation: floatUp 15s infinite ease-in-out;
}

.particle:nth-child(even) {
  background: linear-gradient(45deg, #ff758c, #ff7eb3);
  box-shadow: 0 0 8px 2px rgba(255, 117, 140, 0.5);
  animation: floatUp 20s infinite ease-in-out;
}

.particle:nth-child(3n) {
  animation-delay: 2s;
}

.particle:nth-child(3n+1) {
  animation-delay: 5s;
}

.particle:nth-child(3n+2) {
  animation-delay: 9s;
}

.particle:nth-child(1) { left: 5%; top: 90%; }
.particle:nth-child(2) { left: 15%; top: 70%; }
.particle:nth-child(3) { left: 25%; top: 80%; }
.particle:nth-child(4) { left: 35%; top: 75%; }
.particle:nth-child(5) { left: 45%; top: 85%; }
.particle:nth-child(6) { left: 55%; top: 90%; }
.particle:nth-child(7) { left: 65%; top: 85%; }
.particle:nth-child(8) { left: 75%; top: 80%; }
.particle:nth-child(9) { left: 85%; top: 75%; }
.particle:nth-child(10) { left: 95%; top: 70%; }
.particle:nth-child(11) { left: 10%; top: 65%; }
.particle:nth-child(12) { left: 20%; top: 60%; }
.particle:nth-child(13) { left: 30%; top: 55%; }
.particle:nth-child(14) { left: 40%; top: 50%; }
.particle:nth-child(15) { left: 50%; top: 45%; }
.particle:nth-child(16) { left: 60%; top: 40%; }
.particle:nth-child(17) { left: 70%; top: 35%; }
.particle:nth-child(18) { left: 80%; top: 30%; }
.particle:nth-child(19) { left: 90%; top: 25%; }
.particle:nth-child(20) { left: 5%; top: 20%; }
.particle:nth-child(21) { left: 15%; top: 15%; }
.particle:nth-child(22) { left: 25%; top: 10%; }
.particle:nth-child(23) { left: 35%; top: 5%; }
.particle:nth-child(24) { left: 45%; top: 10%; }
.particle:nth-child(25) { left: 55%; top: 15%; }
.particle:nth-child(26) { left: 65%; top: 20%; }
.particle:nth-child(27) { left: 75%; top: 25%; }
.particle:nth-child(28) { left: 85%; top: 30%; }
.particle:nth-child(29) { left: 95%; top: 35%; }
.particle:nth-child(30) { left: 50%; top: 95%; }

@keyframes floatUp {
  0% {
    transform: translateY(0) scale(1);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100vh) scale(0.5);
    opacity: 0;
  }
}

/* Header */
.app-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  position: relative;
  z-index: 10;
}

.logo {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #e1e7ef;
  font-weight: 600;
  font-size: 1.5rem;
}

.logo i {
  color: #3498db;
  font-size: 1.8rem;
}

.help-button {
  display: flex;
  align-items: center;
  gap: 8px;
  background: rgba(52, 152, 219, 0.1);
  border: none;
  outline: none;
  padding: 8px 15px;
  border-radius: 50px;
  color: #e1e7ef;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.help-button i {
  color: #3498db;
}

.help-button:hover {
  background: rgba(52, 152, 219, 0.2);
  transform: translateY(-2px);
}

/* Main Container */
.main-container {
  max-width: 1200px;
  margin: 10px auto;
  position: relative;
  z-index: 10;
}

.progress-bar {
  z-index: 10;
  display: flex; /* Sử dụng flexbox để xếp các phần tử theo chiều ngang */
  flex-direction: row; /* Đảm bảo hướng là ngang (mặc định, nhưng thêm để rõ ràng) */
  align-items: center; /* Căn giữa các phần tử theo chiều dọc */
  justify-content: space-between; /* Phân bố đều khoảng cách giữa các bước */
  max-width: 800px; /* Giới hạn chiều rộng tối đa */
  margin: 0 auto 40px auto; /* Căn giữa và thêm khoảng cách dưới */
  padding: 0 20px; /* Khoảng cách lề trái/phải */
}

/* Progress Step */
.progress-step {
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  z-index: 10000; /* Tăng z-index lên cao hơn modal */
  min-width: 80px;
}

/* Step Number */
.step-number {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: #1a2234; /* Màu nền mặc định */
  border: 2px solid #2c3e50; /* Viền mặc định */
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  color: #e1e7ef; /* Màu chữ */
  transition: all 0.3s ease; /* Hiệu ứng chuyển đổi mượt mà */
  margin-bottom: 8px; /* Khoảng cách giữa số và nhãn */
}

/* Step Label */
.step-label {
  font-size: 0.875rem; /* Kích thước chữ nhỏ */
  color: #8896ae; /* Màu chữ mặc định */
  font-weight: 500;
  transition: all 0.3s ease; /* Hiệu ứng chuyển đổi */
  text-align: center;
}

/* Trạng thái Active cho Progress Step */
.progress-step.active .step-number {
  background: linear-gradient(45deg, #3498db, #9b59b6); /* Màu gradient khi active */
  border-color: #3498db; /* Viền đổi màu */
  box-shadow: 0 0 15px rgba(52, 152, 219, 0.5); /* Hiệu ứng bóng */
  transform: scale(1.1); /* Phóng to nhẹ */
}

.progress-step.active .step-label {
  color: #e1e7ef; /* Màu chữ sáng hơn */
  font-weight: 600; /* Đậm hơn */
}

/* Trạng thái Completed cho Progress Step */
.progress-step.completed .step-number {
  background: #2ecc71; /* Màu xanh khi hoàn thành */
  border-color: #2ecc71; /* Viền xanh */
}

/* Progress Line */
.progress-line {
  flex: 1; /* Chiếm toàn bộ không gian còn lại giữa các bước */
  height: 3px; /* Độ dày đường nối */
  background: #2c3e50; /* Màu mặc định */
  position: relative;
  z-index: 1; /* Nằm dưới các bước */
  transition: all 0.3s ease; /* Hiệu ứng chuyển đổi */
}

/* Trạng thái Active cho Progress Line */
.progress-line.active {
  background: linear-gradient(to right, #3498db, #9b59b6); /* Gradient khi active */
  box-shadow: 0 0 10px rgba(52, 152, 219, 0.5); /* Hiệu ứng bóng */
}

/* Content Card */
.content-card {
  background: rgba(26, 34, 52, 0.6);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(52, 152, 219, 0.2);
  padding: 30px;
  margin-bottom: 40px;
  position: relative;
  max-width: 800px;
  margin-left: auto;
  margin-right: auto;
}

.step-content {
  animation: fadeIn 0.5s ease-in-out;
  min-height: 300px;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.step-title {
  font-size: 1.75rem;
  font-weight: 600;
  color: #e1e7ef;
  margin-bottom: 25px;
  text-align: center;
  background: linear-gradient(45deg, #3498db, #9b59b6);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

/* Step Navigation Controls */
.step-controls {
  display: flex;
  justify-content: space-between;
  margin-top: 30px;
}

.btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 24px;
  border-radius: 50px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  border: none;
  outline: none;
  font-size: 1rem;
}

.btn-prev {
  background: rgba(255, 255, 255, 0.1);
  color: #e1e7ef;
}

.btn-prev:hover {
  background: rgba(255, 255, 255, 0.2);
  transform: translateX(-5px);
}

.btn-next, .btn-submit {
  background: linear-gradient(45deg, #3498db, #9b59b6);
  color: white;
  box-shadow: 0 5px 15px rgba(52, 152, 219, 0.4);
}

.btn-next:hover, .btn-submit:hover {
  transform: translateX(5px);
  box-shadow: 0 5px 20px rgba(52, 152, 219, 0.6);
}

.btn-submit {
  background: linear-gradient(45deg, #2ecc71, #3498db);
}

.btn-submit:hover {
  background: linear-gradient(45deg, #27ae60, #2980b9);
}

/* Game Selection (Step 1) */
.search-container {
  margin-bottom: 20px;
}

.search-box {
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 50px;
  padding: 10px 20px;
  transition: all 0.3s ease;
  border: 1px solid rgba(52, 152, 219, 0.2);
}

.search-box:focus-within {
  background: rgba(255, 255, 255, 0.1);
  border-color: #3498db;
  box-shadow: 0 0 15px rgba(52, 152, 219, 0.3);
}

.search-box i {
  color: #8896ae;
  margin-right: 10px;
  font-size: 1.1rem;
}

.search-input {
  flex: 1;
  background: transparent;
  border: none;
  outline: none;
  color: #e1e7ef;
  font-size: 1rem;
}

.search-input::placeholder {
  color: #8896ae;
}

.game-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 20px;
}

.game-card {
  background: rgba(26, 34, 52, 0.8);
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
  border: 1px solid rgba(52, 152, 219, 0.2);
}

.game-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
  border-color: #3498db;
}

.game-img-container {
  width: 100%;
  height: 100px;
  overflow: hidden;
}

.game-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: all 0.3s ease;
}

.game-card:hover .game-img {
  transform: scale(1.1);
}

.game-name {
  padding: 12px;
  font-weight: 500;
  text-align: center;
  color: #e1e7ef;
}

/* Account Details (Step 2) */
.game-selected {
  display: flex;
  align-items: center;
  background: rgba(52, 152, 219, 0.1);
  padding: 12px 15px;
  border-radius: 10px;
  margin-bottom: 20px;
}

.game-selected .label {
  font-weight: 600;
  color: #8896ae;
  margin-right: 8px;
}

.game-selected .value {
  color: #3498db;
  font-weight: 600;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  font-size: 0.95rem;
  color: #e1e7ef;
  margin-bottom: 8px;
  font-weight: 500;
}

.form-group .required {
  color: #e74c3c;
  margin-left: 4px;
}

.input-container {
  display: flex;
  align-items: center;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
  padding: 0 15px;
  transition: all 0.3s ease;
  border: 1px solid rgba(52, 152, 219, 0.2);
}

.input-container:focus-within {
  background: rgba(255, 255, 255, 0.1);
  border-color: #3498db;
  box-shadow: 0 0 15px rgba(52, 152, 219, 0.3);
}

.input-container i {
  color: #8896ae;
  font-size: 1.1rem;
  margin-right: 10px;
}

.input-container input {
  flex: 1;
  background: transparent;
  border: none;
  outline: none;
  color: #e1e7ef;
  padding: 15px 0;
  font-size: 1rem;
}

.input-container input::placeholder {
  color: #8896ae;
}

.error-message {
  color: #e74c3c;
  font-size: 0.85rem;
  margin-top: 5px;
  min-height: 17px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.game-fields {
  background: rgba(52, 152, 219, 0.05);
  border-radius: 12px;
  padding: 20px;
  margin-top: 25px;
  border: 1px solid rgba(52, 152, 219, 0.2);
}

.fields-title {
  font-size: 1.2rem;
  font-weight: 600;
  color: #e1e7ef;
  margin-bottom: 15px;
  text-align: center;
}

.no-fields {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 20px;
  background: rgba(52, 152, 219, 0.05);
  border-radius: 12px;
  margin-top: 25px;
}

.no-fields i {
  font-size: 2rem;
  color: #8896ae;
  margin-bottom: 10px;
}

.no-fields p {
  color: #8896ae;
  text-align: center;
}

/* Image Upload (Step 3) */
.upload-zone {
  border: 2px dashed rgba(52, 152, 219, 0.4);
  border-radius: 15px;
  padding: 30px;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  background: rgba(52, 152, 219, 0.05);
}

.upload-zone:hover {
  background: rgba(52, 152, 219, 0.1);
  border-color: #3498db;
}

.upload-icon {
  margin-bottom: 15px;
}

.upload-icon i {
  font-size: 3rem;
  color: #3498db;
}

.upload-text {
  font-size: 1.1rem;
  font-weight: 600;
  color: #e1e7ef;
  margin-bottom: 8px;
}

.upload-hint {
  color: #8896ae;
  font-size: 0.9rem;
}

.file-input {
  display: none;
}

.image-preview-container {
  margin-top: 30px;
  background: rgba(52, 152, 219, 0.05);
  border-radius: 15px;
  padding: 20px;
  border: 1px solid rgba(52, 152, 219, 0.2);
}

.image-count {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 15px;
  color: #e1e7ef;
  font-size: 0.95rem;
  font-weight: 500;
}

.image-count i {
  color: #3498db;
}

.image-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 15px;
}

.image-item {
  position: relative;
  border-radius: 10px;
  overflow: hidden;
  height: 100px;
}

.preview-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.remove-image {
  position: absolute;
  top: 5px;
  right: 5px;
  background: rgba(231, 76, 60, 0.8);
  border: none;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  opacity: 0;
  transition: all 0.3s ease;
  color: white;
}

.image-item:hover .remove-image {
  opacity: 1;
}

/* Review Step (Step 4) */
.review-data {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.review-section {
  background: rgba(26, 34, 52, 0.8);
  border-radius: 12px;
  padding: 15px;
  border: 1px solid rgba(52, 152, 219, 0.2);
}

.review-section-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 1.1rem;
  font-weight: 600;
  color: #3498db;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid rgba(52, 152, 219, 0.2);
}

.review-section-title i {
  font-size: 1.2rem;
}

.review-field {
  display: flex;
  align-items: flex-start;
  margin-bottom: 10px;
  padding-bottom: 10px;
  border-bottom: 1px dashed rgba(255, 255, 255, 0.05);
}

.review-field:last-child {
  margin-bottom: 0;
  padding-bottom: 0;
  border-bottom: none;
}

.field-name {
  min-width: 150px;
  color: #8896ae;
  font-weight: 500;
}

.field-value {
  color: #e1e7ef;
  font-weight: 500;
  word-break: break-word;
}

.price-value {
  color: #2ecc71;
  font-weight: 600;
}

.review-images {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(80px, 1fr));
  gap: 10px;
  margin-top: 5px;
}

.review-image {
  width: 100%;
  height: 80px;
  object-fit: cover;
  border-radius: 8px;
  border: 1px solid rgba(52, 152, 219, 0.2);
  transition: all 0.3s ease;
}

.review-image:hover {
  transform: scale(1.05);
  border-color: #3498db;
}

/* Info Cards */
.info-cards {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.info-card {
  background: rgba(26, 34, 52, 0.6);
  backdrop-filter: blur(10px);
  border-radius: 15px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 15px;
  transition: all 0.3s ease;
  border: 1px solid rgba(52, 152, 219, 0.2);
}

.info-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
  border-color: #3498db;
}

.info-icon {
  background: linear-gradient(45deg, #3498db, #9b59b6);
  width: 50px;
  height: 50px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.5rem;
}

.info-content {
  flex: 1;
}

.info-content h3 {
  font-size: 1.1rem;
  font-weight: 600;
  color: #e1e7ef;
  margin-bottom: 5px;
}

.info-content p {
  color: #8896ae;
  font-size: 0.9rem;
  line-height: 1.4;
}

/* Enhanced Modal Styles */
.modal {
  background: #1a2234;
  border-radius: 20px;
  width: 100%;
  max-width: 500px;
  position: relative;
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.3);
  overflow: hidden;
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(52, 152, 219, 0.3);
}

.warning-modal, .success-modal {
  max-height: 90vh;
  overflow-y: auto;
}

.modal-icon {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 30px auto 10px;
  font-size: 2.5rem;
  color: white;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
}

.modal-icon.warning {
  background: linear-gradient(45deg, #e74c3c, #e67e22);
}

.modal-icon.success {
  background: linear-gradient(45deg, #2ecc71, #1abc9c);
}

.modal-content {
  padding: 20px 30px 30px;
}

.modal-title {
  font-size: 1.8rem;
  font-weight: 700;
  color: #e1e7ef;
  margin-bottom: 20px;
  text-align: center;
}

.success-title {
  color: #2ecc71;
}

.warning-message {
  background: rgba(231, 76, 60, 0.1);
  border-radius: 12px;
  padding: 15px;
  border-left: 4px solid #e74c3c;
  margin-bottom: 20px;
}

.warning-message p {
  color: #e1e7ef;
  line-height: 1.6;
  font-size: 0.95rem;
  margin: 0;
}

.success-message {
  background: rgba(46, 204, 113, 0.1);
  border-radius: 12px;
  padding: 15px;
  margin-bottom: 20px;
}

.success-message p {
  color: #e1e7ef;
  line-height: 1.6;
  font-size: 0.95rem;
  margin: 0;
  text-align: center;
}

.confirmation-checkbox {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 25px;
  background: rgba(255, 255, 255, 0.05);
  padding: 15px;
  border-radius: 10px;
}

.confirmation-checkbox input[type="checkbox"] {
  width: 20px;
  height: 20px;
  accent-color: #3498db;
}

.confirmation-checkbox label {
  color: #e1e7ef;
  font-size: 0.95rem;
  line-height: 1.4;
}

.modal-actions {
  display: flex;
  gap: 15px;
  justify-content: space-between;
}

.modal-actions.center {
  justify-content: center;
}

.btn-cancel {
  background: rgba(255, 255, 255, 0.1);
  color: #e1e7ef;
}

.btn-cancel:hover {
  background: rgba(255, 255, 255, 0.2);
}

.btn-confirm {
  background: linear-gradient(45deg, #e74c3c, #e67e22);
  color: white;
  box-shadow: 0 5px 15px rgba(231, 76, 60, 0.4);
}

.btn-confirm:hover {
  box-shadow: 0 5px 20px rgba(231, 76, 60, 0.6);
}

.btn-confirm:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-success {
  background: linear-gradient(45deg, #2ecc71, #1abc9c);
  color: white;
  box-shadow: 0 5px 15px rgba(46, 204, 113, 0.4);
  padding: 12px 30px;
}

.btn-success:hover {
  box-shadow: 0 5px 20px rgba(46, 204, 113, 0.6);
}

.success-details {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 25px;
  background: rgba(255, 255, 255, 0.05);
  padding: 15px;
  border-radius: 10px;
}

.success-detail {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #e1e7ef;
  font-size: 0.95rem;
}

.success-detail i {
  color: #2ecc71;
  font-size: 1.1rem;
}

/* Modal Animation */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s ease, transform 0.3s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
  transform: scale(0.95);
}
/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(10, 14, 23, 0.8);
  backdrop-filter: blur(5px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999; /* Tăng z-index lên cao hơn */
  padding: 20px;
}

/* Specific styles for guide modal overlay */
.guide-modal-overlay {
  align-items: flex-start; /* Align to top instead of center */
  padding-top: 150px; /* Add padding to push modal down */
}

.modal-container {
  width: 100%;
  max-width: 500px;
  margin: 0 auto;
}

.modal-card {
  background-color: #1a2234;
  border-radius: 16px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
  overflow: hidden;
  border: 1px solid rgba(52, 152, 219, 0.2);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  background-color: #212a3e;
  border-bottom: 1px solid rgba(52, 152, 219, 0.2);
}

.modal-header h3 {
  margin: 0;
  color: #e1e7ef;
  font-size: 1.25rem;
  font-weight: 600;
}

.close-btn {
  background: none;
  border: none;
  color: #8896ae;
  font-size: 1.25rem;
  cursor: pointer;
  transition: color 0.2s;
}

.close-btn:hover {
  color: #e1e7ef;
}

.modal-body {
  padding: 24px;
}

.modal-footer {
  padding: 16px 24px 24px;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.modal-footer.center {
  justify-content: center;
}

.warning-box {
  display: flex;
  gap: 16px;
  background-color: rgba(231, 76, 60, 0.1);
  padding: 16px;
  border-radius: 12px;
  margin-bottom: 20px;
}

.warning-icon {
  color: #e74c3c;
  font-size: 24px;
  flex-shrink: 0;
}

.warning-box p {
  margin: 0;
  color: #e1e7ef;
  font-size: 0.95rem;
  line-height: 1.5;
}

.checkbox-container {
  margin-bottom: 8px;
}

.checkbox-label {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  cursor: pointer;
}

.checkbox-label input[type="checkbox"] {
  width: 18px;
  height: 18px;
  accent-color: #3498db;
  cursor: pointer;
}

.checkbox-text {
  color: #e1e7ef;
  font-size: 0.95rem;
  line-height: 1.4;
}

.btn-cancel {
  padding: 10px 20px;
  background-color: transparent;
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: #e1e7ef;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-cancel:hover {
  background-color: rgba(255, 255, 255, 0.1);
}

.btn-confirm {
  padding: 10px 20px;
  background-color: #e74c3c;
  border: none;
  color: white;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-confirm:hover {
  background-color: #c0392b;
}

.btn-disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-disabled:hover {
  background-color: #e74c3c;
}

/* Success Modal */
.success-card {
  text-align: center;
  padding-top: 24px;
}

.success-icon {
  width: 80px;
  height: 80px;
  background-color: #2ecc71;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 20px;
  font-size: 40px;
  color: white;
  box-shadow: 0 10px 20px rgba(46, 204, 113, 0.3);
}

.success-title {
  color: #2ecc71;
  font-size: 1.75rem;
  margin: 0 0 16px;
}

.success-message {
  color: #e1e7ef;
  margin-bottom: 20px;
  font-size: 1rem;
  line-height: 1.5;
}

.success-info {
  background-color: rgba(46, 204, 113, 0.1);
  border-radius: 12px;
  padding: 16px;
  margin-bottom: 20px;
  text-align: left;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 10px;
  color: #e1e7ef;
}

.info-item:last-child {
  margin-bottom: 0;
}

.info-item i {
  color: #2ecc71;
}

.btn-success {
  padding: 10px 30px;
  background-color: #2ecc71;
  border: none;
  color: white;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-success:hover {
  background-color: #27ae60;
}

.text-center {
  text-align: center;
}

/* Modal Animation */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: all 0.3s;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
  transform: scale(0.95);
}

/* Guide Modal Styles */
.guide-modal-container {
  max-width: 650px;
}

.guide-content {
  max-height: 55vh; /* Giảm chiều cao tối đa để tránh bị tràn */
  overflow-y: auto;
  padding-right: 16px;
  padding-top: 10px; /* Thêm padding-top */
}

.guide-section {
  margin-bottom: 24px;
}

.guide-section:last-child {
  margin-bottom: 0;
}

.guide-title {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 1.1rem;
  color: #3498db;
  margin-bottom: 12px;
  font-weight: 600;
}

.guide-title i {
  font-size: 1.1rem;
}

.guide-section p {
  color: #e1e7ef;
  line-height: 1.6;
  font-size: 0.95rem;
  margin: 0 0 16px 0;
}

.steps {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.step {
  display: flex;
  gap: 16px;
  align-items: flex-start;
}

.step-indicator {
  width: 28px;
  height: 28px;
  background: linear-gradient(45deg, #3498db, #9b59b6);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  flex-shrink: 0;
}

.step-description {
  flex: 1;
  color: #e1e7ef;
  line-height: 1.5;
  font-size: 0.95rem;
  padding-top: 3px;
}

.step-description strong {
  color: #3498db;
}

.btn-primary {
  padding: 10px 30px;
  background: linear-gradient(45deg, #3498db, #9b59b6);
  border: none;
  color: white;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary:hover {
  background: linear-gradient(45deg, #2980b9, #8e44ad);
}
</style>


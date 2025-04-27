<template>
  <div class="profile-container">
    <div class="cosmo-particles"></div>
    <div class="profile-card">
      <div class="profile-grid">
        <!-- Left column: Avatar + Basic Info -->
        <div class="profile-left">
          <div class="avatar-container">
            <div class="avatar-ring">
              <img :src="user.avatar || defaultAvatar" alt="Avatar" class="avatar" @mouseenter="isAvatarHovered = true" @mouseleave="isAvatarHovered = false" />
              <div class="status-indicator" :class="{ 'online': user.status }">
                <span class="status-pulse"></span>
              </div>
            </div>
            <transition name="scale">
              <div v-if="isAvatarHovered" class="avatar-overlay" @click="changeAvatar">
                <i class="fas fa-camera"></i>
                <span class="overlay-text">Đổi avatar</span>
              </div>
            </transition>
          </div>
          <h2 class="profile-name">{{ user.fullName || 'Chưa có tên' }}</h2>
          <div class="profile-big-icon" :class="{ 'verified': user.isAuthen }">
            <i :class="user.isAuthen ? 'fas fa-check-circle' : 'fas fa-exclamation-circle'" :title="user.isAuthen ? 'Tài khoản đã xác thực' : 'Tài khoản chưa xác thực'"></i>
          </div>
          <div class="profile-badges">
            <div class="badge" :class="levelClass">Level {{ user.level }}</div>
            <div class="badge membership">Premium</div>
          </div>
          <div class="balance-container">
            <div class="balance-card">
              <div class="balance-icon"><i class="fas fa-wallet"></i></div>
              <div class="balance-info">
                <span class="balance-label">Số dư</span>
                <span class="balance-value">{{ formatCurrency(user.balance) }}</span>
              </div>
            </div>
            <div class="balance-card">
              <div class="balance-icon"><i class="fas fa-coins"></i></div>
              <div class="balance-info">
                <span class="balance-label">Xu</span>
                <span class="balance-value">{{ user.coin || 0 }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Right column: Details + Actions -->
        <div class="profile-right">
          <div class="section-title">
            <i class="fas fa-user-astronaut"></i>
            <span>Thông tin tài khoản</span>
          </div>

          <div class="profile-details">
            <div class="detail-item">
              <i class="fas fa-envelope detail-icon"></i>
              <div class="detail-content">
                <span class="detail-label">Email</span>
                <span class="detail-value">{{ user.email || 'Chưa cập nhật' }}</span>
              </div>
            </div>

            <!-- Thêm phần xác thực tài khoản -->
            <div class="detail-item verification-item">
              <i class="fas fa-shield-alt detail-icon"></i>
              <div class="detail-content">
                <span class="detail-label">Trạng thái xác thực</span>
                <div class="verification-content">
                  <span class="verification-status" :class="{ 'verified': user.isAuthen }">
                    {{ user.isAuthen ? 'Đã xác thực' : 'Chưa xác thực' }}
                  </span>
                  <div v-if="!user.isAuthen" class="verification-actions">
                    <button v-if="!showOtpInput" class="action-btn verify-btn" @click="sendOtp">
                      <i class="fas fa-paper-plane"></i>
                      <span>Gửi OTP</span>
                    </button>
                    <div v-if="showOtpInput" class="otp-input-container">
                      <input
                        v-model="otp"
                        type="text"
                        placeholder="Nhập OTP"
                        maxlength="6"
                        class="otp-input"
                      />
                      <button class="action-btn confirm-btn" @click="verifyOtp">
                        <i class="fas fa-check"></i>
                        <span>Xác thực</span>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="detail-item exp-item">
              <i class="fas fa-star detail-icon"></i>
              <div class="detail-content exp-content">
                <span class="detail-label">Kinh nghiệm</span>
                <div class="exp-container">
                  <div class="exp-bar-container">
                    <div class="exp-bar" :style="{ width: expPercentage + '%' }"></div>
                    <span class="exp-text">{{ user.experience || 0 }} / {{ expToNextLevel }} XP</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="detail-item bank-item">
              <i class="fas fa-university detail-icon"></i>
              <div class="detail-content">
                <span class="detail-label">Thông tin ngân hàng</span>
                <div class="bank-info">
                  <span class="bank-name">{{ user.bankName || 'Chưa cập nhật' }}</span>
                  <span class="bank-number">{{ formatBankNumber(user.bankNumber) }}</span>
                </div>
              </div>
              <i class="fas fa-shield-alt security-icon" title="Thông tin được mã hóa"></i>
            </div>

            <div class="detail-item">
              <i class="fas fa-calendar-alt detail-icon"></i>
              <div class="detail-content">
                <span class="detail-label">Ngày tham gia</span>
                <span class="detail-value">{{ formatDate(user.createdDate) }}</span>
              </div>
            </div>
          </div>

          <div class="section-title">
            <i class="fas fa-cog"></i>
            <span>Quản lý tài khoản</span>
          </div>

          <div class="profile-actions">
            <button class="action-btn edit-btn" @click="editProfile">
              <i class="fas fa-pen"></i>
              <span>Chỉnh sửa</span>
            </button>
            <button class="action-btn security-btn" @click="securitySettings">
              <i class="fas fa-shield-alt"></i>
              <span>Bảo mật</span>
            </button>
            <button class="action-btn logout-btn" @click="logout">
              <i class="fas fa-sign-out-alt"></i>
              <span>Đăng xuất</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal for Notifications and Alerts -->
    <transition name="modal">
      <div v-if="isModalOpen" class="modal-backdrop" @click="closeModal">
        <div class="modal-content" @click.stop>
          <h3>{{ modalTitle }}</h3>
          <p>{{ modalMessage }}</p>
          <button @click="modalAction">{{ modalActionText }}</button>
          <button @click="closeModal">Đóng</button>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import profileApi from '@/api/profile.api';
import authApi from '@/api/authenticate.api';
import { userStore } from '@/stores/auth.ts';
import type { UserInfoModel } from '@/models/user-model';

const router = useRouter();
const store = userStore();

const user = ref<UserInfoModel>({
  userName: '',
  id: 0,
  email: '',
  fullName: '',
  balance: 0,
  coin: 0,
  level: 1,
  status: false,
  experience: null,
  bankName: '',
  bankNumber: '',
  avatar: null,
  createdDate: null,
  isAuthen: false,
});

const defaultAvatar = 'https://via.placeholder.com/150?text=No+Avatar';
const isAvatarHovered = ref(false);
const showOtpInput = ref(false);
const otp = ref('');

// Modal reactive variables
const isModalOpen = ref(false);
const modalTitle = ref('');
const modalMessage = ref('');
const modalActionText = ref('');
const modalAction = ref(() => {});

// Experience thresholds matching backend PurchasedAccountService
const expThresholds = [
  100000,  // level 0 -> 1
  300000,  // level 1 -> 2
  500000,  // level 2 -> 3
  700000,  // level 3 -> 4
  1300000, // level 4 -> 5
  1600000, // level 5 -> 6
  2000000, // level 6 -> 7
  2500000, // level 7 -> 8
  3000000, // level 8 -> 9
  4000000  // level 9 -> 10
];

// Compute experience required for the next level
const expToNextLevel = computed(() => {
  const currentLevel = user.value.level || 0;
  if (currentLevel >= expThresholds.length) {
    return expThresholds[expThresholds.length - 1]; // Max level, use last threshold
  }
  return expThresholds[currentLevel];
});

// Compute experience percentage for the progress bar
const expPercentage = computed(() => {
  const exp = user.value.experience || 0;
  const currentLevel = user.value.level || 0;
  if (currentLevel >= expThresholds.length) {
    return 100; // Max level, bar is full
  }
  const threshold = expThresholds[currentLevel];
  return Math.min((exp / threshold) * 100, 100); // Cap at 100%
});

// Tính toán class màu sắc dựa trên level
const levelClass = computed(() => {
  const level = user.value.level || 0;
  if (level >= 0 && level <= 2) return 'level-gray';
  if (level >= 3 && level <= 5) return 'level-green';
  if (level >= 6 && level <= 8) return 'level-gold';
  if (level >= 9 && level <= 10) return 'level-purple';
  return 'level-red'; // Cấp 11 trở lên
});

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
};

const formatDate = (date?: string | null) => {
  if (!date) return 'Chưa có thông tin';
  return new Date(date).toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
};

const formatBankNumber = (number?: string | null) => {
  if (!number) return 'Chưa cập nhật';
  return number.length > 4 ? '••••••' + number.slice(-4) : number;
};

const fetchUserProfile = async () => {
  try {
    const userId = store.user?.id || JSON.parse(localStorage.getItem('user') || '{}').id;
    if (!userId) {
      console.error('No user ID found');
      router.push('/login');
      return;
    }
    const response = await profileApi.getByIdProfile(userId);
    console.log(response.data.result);
    if (response.data.result.isSuccess) {
      user.value = response.data.result.data;
      console.log('user.value.IsAuthen:', user.value.isAuthen);
    } else {
      console.error('Failed to fetch profile:', response.data.result.message);
    }
  } catch (error) {
    console.error('Error fetching profile:', error);
    router.push('/login');
  }
};

// Gửi OTP để xác thực email
const sendOtp = async () => {
  try {
    const response = await authApi.sendOtpForEmailVerification();
    showModal('Thông báo', response.Message || 'OTP đã được gửi. Vui lòng kiểm tra email của bạn.', 'Đóng', closeModal);
    showOtpInput.value = true; // Hiển thị input để nhập OTP
  } catch (error: any) {
    showModal('Lỗi', error.message || 'Không thể gửi OTP. Vui lòng thử lại.', 'Thử lại', sendOtp);
  }
};

// Xác thực OTP
const verifyOtp = async () => {
  if (!user.value.userName) {
    showModal('Lỗi', 'Không tìm thấy tên người dùng.', 'Đóng', closeModal);
    return;
  }
  if (!otp.value || otp.value.length !== 6) {
    showModal('Lỗi', 'Vui lòng nhập OTP hợp lệ (6 chữ số).', 'Đóng', closeModal);
    return;
  }
  try {
    const response = await authApi.verifyOtpForEmailVerification(user.value.userName, otp.value);
    showModal('Thành công', response.Message || 'Xác thực email thành công.', 'Đóng', () => {
      user.value.isAuthen = true;
      showOtpInput.value = false;
      otp.value = '';
      closeModal();
    });
  } catch (error: any) {
    showModal('Lỗi', error.message || 'Xác thực OTP thất bại. Vui lòng thử lại.', 'Thử lại', verifyOtp);
  }
};
// Modal control functions
const showModal = (title: string, message: string, actionText: string, action: () => void) => {
  modalTitle.value = title;
  modalMessage.value = message;
  modalActionText.value = actionText;
  modalAction.value = action;
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};

const changeAvatar = () => {
  console.log('Người dùng muốn đổi avatar!');
};

const editProfile = () => {
  router.push('/profile/edit');
};

const securitySettings = () => {
  console.log('Mở trang cài đặt bảo mật');
};

const logout = async () => {
  await store.logout();
  router.push('/login');
};

onMounted(() => {
  fetchUserProfile();
  initParticles();
});

// Initialize particles
const initParticles = () => {
  requestAnimationFrame(() => {
    const container = document.querySelector('.cosmo-particles');
    if (!container) return;

    for (let i = 0; i < 50; i++) {
      const particle = document.createElement('div');
      particle.classList.add('particle');

      // Random positioning
      const size = Math.random() * 4 + 1;
      particle.style.width = `${size}px`;
      particle.style.height = `${size}px`;
      particle.style.left = `${Math.random() * 100}%`;
      particle.style.top = `${Math.random() * 100}%`;

      // Random animation duration
      const duration = Math.random() * 50 + 10;
      particle.style.animationDuration = `${duration}s`;

      // Random animation delay
      const delay = Math.random() * 10;
      particle.style.animationDelay = `-${delay}s`;

      // Random opacity
      particle.style.opacity = (Math.random() * 0.5 + 0.1).toString();

      container.appendChild(particle);
    }
  });
};
</script>

<style scoped>
/* Base styles and animations */
@import url('https://fonts.googleapis.com/css2?family=Rajdhani:wght@500;600;700&family=Orbitron:wght@400;500;700&display=swap');

@keyframes float {
  0%, 100% { transform: translateY(0) translateX(0); }
  25% { transform: translateY(-10px) translateX(5px); }
  50% { transform: translateY(5px) translateX(-5px); }
  75% { transform: translateY(-5px) translateX(-10px); }
}

@keyframes pulse {
  0%, 100% { transform: scale(1); opacity: 0.8; }
  50% { transform: scale(1.2); opacity: 1; }
}

@keyframes rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes statusPulse {
  0% { transform: scale(1); opacity: 1; }
  100% { transform: scale(2); opacity: 0; }
}

@keyframes particle-float {
  0% { transform: translateY(0) rotate(0deg); }
  100% { transform: translateY(-100vh) rotate(360deg); }
}

/* Main container */
.profile-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  position: relative;
  overflow: hidden;
  padding: 100px 20px 40px;
  color: #e0e0ff;
  font-family: 'Rajdhani', sans-serif;
}

/* Particle background */
.cosmo-particles {
  position: absolute;
  width: 100%;
  height: 100%;
  top: 0;
  left: 0;
  pointer-events: none;
  z-index: 0;
}

.particle {
  position: absolute;
  background: linear-gradient(180deg, #00ffff, #ff00ff);
  border-radius: 50%;
  animation: particle-float linear infinite;
  z-index: 0;
}

/* Profile Card */
.profile-card {
  background: rgba(10, 10, 32, 0.7);
  backdrop-filter: blur(20px);
  border-radius: 16px;
  padding: 40px;
  width: 100%;
  max-width: 1200px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5), 0 0 0 1px rgba(0, 255, 255, 0.1) inset, 0 0 30px rgba(0, 255, 255, 0.1) inset;
  position: relative;
  z-index: 1;
  overflow: hidden;
  transition: all 0.4s ease;
  border: 1px solid rgba(0, 255, 255, 0.2);
}

.profile-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(0, 255, 255, 0.5), transparent);
}

.profile-card::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(0, 255, 255, 0.5), transparent);
}

/* Grid layout */
.profile-grid {
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 40px;
}

/* Left column */
.profile-left {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding-right: 40px;
  border-right: 1px solid rgba(0, 255, 255, 0.15);
}

/* Avatar styling */
.avatar-container {
  position: relative;
  margin-bottom: 30px;
  perspective: 1000px;
}

.avatar-ring {
  position: relative;
  padding: 8px;
  border-radius: 50%;
  background: linear-gradient(135deg, rgba(0, 255, 255, 0.2), rgba(255, 0, 255, 0.2));
}

.avatar {
  width: 160px;
  height: 160px;
  border-radius: 50%;
  object-fit: cover;
  border: 3px solid transparent;
  background: linear-gradient(#0a0a20, #0a0a20) padding-box, linear-gradient(135deg, #00ffff, #ff00ff) border-box;
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  filter: saturate(1.1) brightness(1.05);
}

.avatar-container:hover .avatar {
  transform: scale(1.03);
  filter: saturate(1.2) brightness(1.1);
}

.status-indicator {
  position: absolute;
  bottom: 10px;
  right: 10px;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: #0a0a20;
  display: flex;
  justify-content: center;
  align-items: center;
  border: 3px solid rgba(10, 10, 32, 0.8);
  z-index: 2;
}

.status-indicator.online {
  background: #00ffaa;
}

.status-indicator:not(.online) {
  background: #ff5252;
}

.status-pulse {
  position: absolute;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  animation: statusPulse 2s infinite;
  background: inherit;
}

.avatar-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  background: rgba(10, 10, 32, 0.7);
  backdrop-filter: blur(2px);
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  gap: 8px;
  z-index: 3;
}

.avatar-overlay i {
  font-size: 1.8rem;
  color: #00ffff;
}

.overlay-text {
  font-size: 0.9rem;
  font-weight: 500;
  color: #00ffff;
  margin-top: 5px;
}

/* Icon xác thực lớn bên dưới tên */
.profile-big-icon {
  margin-bottom: 15px;
}

.profile-big-icon i {
  font-size: 2rem;
}

.profile-big-icon.verified i {
  color: #00ffaa;
}

.profile-big-icon:not(.verified) i {
  color: #ff5252;
}

/* Name and badges */
.profile-name {
  font-size: 1.8rem;
  font-weight: 700;
  background: linear-gradient(135deg, #00ffff, #ff00ff);
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
  margin: 0 0 15px;
  letter-spacing: -0.5px;
  font-family: 'Orbitron', sans-serif;
}

.profile-badges {
  display: flex;
  gap: 10px;
  margin-bottom: 25px;
}

.badge {
  padding: 5px 12px;
  border-radius: 8px;
  font-size: 0.8rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 8px rgba(0,0,0,0.2);
}

.membership {
  background: linear-gradient(135deg, #ff9d00, #ff612c);
  color: #fff;
}

/* Level badge colors */
.level-gray {
  background: linear-gradient(135deg, #9e9e9e, #616161);
  color: #ffffff;
}

.level-green {
  background: linear-gradient(135deg, #00ffaa, #00cc88);
  color: #0a0a20;
}

.level-gold {
  background: linear-gradient(135deg, #ffd54f, #ffb300);
  color: #0a0a20;
}

.level-purple {
  background: linear-gradient(135deg, #ff00ff, #cc00cc);
  color: #ffffff;
}

.level-red {
  background: linear-gradient(135deg, #ff5252, #d50000);
  color: #ffffff;
}

/* Balance cards */
.balance-container {
  display: flex;
  flex-direction: column;
  gap: 15px;
  width: 100%;
  margin-top: 10px;
}

.balance-card {
  background: rgba(0, 255, 255, 0.05);
  border-radius: 12px;
  padding: 15px;
  display: flex;
  align-items: center;
  gap: 15px;
  transition: all 0.3s ease;
  border: 1px solid rgba(0, 255, 255, 0.1);
}

.balance-card:hover {
  transform: translateY(-3px);
  background: rgba(0, 255, 255, 0.1);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
  border-color: rgba(0, 255, 255, 0.3);
}

.balance-icon {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 10px;
  font-size: 1.2rem;
  color: #00ffff;
}

.balance-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

.balance-label {
  font-size: 0.8rem;
  color: #b0b0cc;
  margin-bottom: 3px;
}

.balance-value {
  font-size: 1.1rem;
  font-weight: 600;
  color: #fff;
}

/* Right column */
.profile-right {
  display: flex;
  flex-direction: column;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 20px;
  font-size: 1.2rem;
  font-weight: 600;
  color: #fff;
}

.section-title i {
  color: #00ffff;
}

/* Detail items */
.profile-details {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-bottom: 30px;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 15px;
  background: rgba(0, 255, 255, 0.05);
  padding: 16px;
  border-radius: 12px;
  transition: all 0.3s ease;
  border: 1px solid rgba(0, 255, 255, 0.1);
}

.detail-item:hover {
  background: rgba(0, 255, 255, 0.1);
  transform: translateY(-3px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1);
  border-color: rgba(0, 255, 255, 0.3);
}

.detail-icon {
  font-size: 1.3rem;
  color: #00ffff;
  width: 24px;
  text-align: center;
}

.detail-content {
  display: flex;
  flex-direction: column;
  flex: 1;
}

.detail-label {
  font-size: 0.8rem;
  color: #b0b0cc;
  margin-bottom: 4px;
}

.detail-value {
  font-size: 1rem;
  color: #fff;
}

/* Verification item */
.verification-item {
  align-items: flex-start;
}

.verification-content {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.verification-status {
  font-size: 1rem;
  font-weight: 600;
}

.verification-status.verified {
  color: #00ffaa;
}

.verification-status:not(.verified) {
  color: #ff5252;
}

.verification-actions {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.otp-input-container {
  display: flex;
  gap: 10px;
  align-items: center;
}

.otp-input {
  padding: 8px 12px;
  border-radius: 8px;
  border: 1px solid rgba(0, 255, 255, 0.2);
  background: rgba(10, 10, 32, 0.6);
  color: #fff;
  font-family: 'Rajdhani', sans-serif;
  font-size: 0.9rem;
  width: 120px;
  outline: none;
  transition: all 0.3s ease;
}

.otp-input:focus {
  border-color: #00ffff;
  background: rgba(10, 10, 32, 0.8);
}

.verify-btn {
  background: rgba(0, 255, 255, 0.1);
  color: #00ffff;
  position: relative;
  overflow: hidden;
}

.verify-btn:hover {
  background: rgba(0, 255, 255, 0.2);
}

.confirm-btn {
  background: rgba(0, 255, 170, 0.1);
  color: #00ffaa;
  position: relative;
  overflow: hidden;
}

.confirm-btn:hover {
  background: rgba(0, 255, 170, 0.2);
}

/* Experience bar */
.exp-item {
  background: rgba(0, 255, 255, 0.05);
}

.exp-content {
  width: 100%;
}

.exp-container {
  width: 100%;
  margin-top: 5px;
}

.exp-bar-container {
  width: 100%;
  background: rgba(10, 10, 32, 0.6);
  border-radius: 8px;
  height: 12px;
  position: relative;
  overflow: hidden;
}

.exp-bar {
  height: 100%;
  background: linear-gradient(90deg, #00ffff, #ff00ff);
  border-radius: 8px;
  transition: width 1s cubic-bezier(0.19, 1, 0.22, 1);
}

.exp-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: #fff;
  font-size: 0.7rem;
  font-weight: 600;
  text-shadow: 0 0 3px rgba(0, 0, 0, 0.5);
  white-space: nowrap;
}

/* Bank info */
.bank-item {
  position: relative;
}

.bank-info {
  display: flex;
  flex-direction: column;
}

.bank-name {
  font-size: 1rem;
  color: #fff;
}

.bank-number {
  font-size: 0.9rem;
  color: #b0b0cc;
  font-family: monospace;
  letter-spacing: 1px;
}

.security-icon {
  position: absolute;
  right: 16px;
  color: #00ffff;
  font-size: 1.1rem;
  opacity: 0.7;
}

/* Action buttons */
.profile-actions {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 15px;
  margin-top: 10px;
}

.action-btn {
  padding: 12px;
  border: none;
  border-radius: 10px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.19, 1, 0.22, 1);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  background: rgba(0, 255, 255, 0.05);
  color: #fff;
  border: 1px solid rgba(0, 255, 255, 0.1);
  font-family: 'Rajdhani', sans-serif;
}

.action-btn i {
  font-size: 0.9rem;
}

.action-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1);
  border-color: rgba(0, 255, 255, 0.3);
}

.action-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(0, 255, 255, 0.2), transparent);
  transition: left 0.5s ease;
}

.action-btn:hover::before {
  left: 100%;
}

.edit-btn {
  background: rgba(0, 255, 255, 0.1);
  color: #00ffff;
  position: relative;
  overflow: hidden;
}

.edit-btn:hover {
  background: rgba(0, 255, 255, 0.2);
}

.security-btn {
  background: rgba(0, 255, 170, 0.1);
  color: #00ffaa;
  position: relative;
  overflow: hidden;
}

.security-btn:hover {
  background: rgba(0, 255, 170, 0.2);
}

.logout-btn {
  background: rgba(255, 82, 82, 0.1);
  color: #ff5252;
  position: relative;
  overflow: hidden;
}

.logout-btn:hover {
  background: rgba(255, 82, 82, 0.2);
}

/* Modal Styles */
.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1200;
}

.modal-content {
  background: linear-gradient(135deg, #1e0b45, #0c0c1d);
  padding: 30px;
  border-radius: 12px;
  text-align: center;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(157, 78, 221, 0.3);
  max-width: 400px;
  width: 90%;
  color: #e0e0e0;
}

.modal-content h3 {
  color: #c77dff;
  margin-bottom: 15px;
  font-size: 1.5rem;
}

.modal-content p {
  margin-bottom: 20px;
  font-size: 1rem;
}

.modal-content button {
  padding: 10px 20px;
  margin: 5px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 1rem;
}

.modal-content button:first-child {
  background: linear-gradient(135deg, #9d4edd, #c77dff);
  color: white;
}

.modal-content button:first-child:hover {
  background: linear-gradient(135deg, #c77dff, #9d4edd);
  transform: translateY(-2px);
}

.modal-content button:last-child {
  background: rgba(157, 78, 221, 0.1);
  color: #c77dff;
}

.modal-content button:last-child:hover {
  background: rgba(157, 78, 221, 0.2);
  transform: translateY(-2px);
}

/* Transitions */
.scale-enter-active,
.scale-leave-active {
  transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.scale-enter-from,
.scale-leave-to {
  opacity: 0;
  transform: scale(0.9);
}

.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

/* Responsive */
@media (max-width: 1024px) {
  .profile-card {
    padding: 30px;
  }

  .profile-grid {
    gap: 30px;
  }

  .profile-left {
    padding-right: 30px;
  }

  .avatar {
    width: 140px;
    height: 140px;
  }
}

@media (max-width: 768px) {
  .profile-card {
    padding: 25px;
    max-width: 90%;
  }

  .profile-grid {
    grid-template-columns: 1fr;
    gap: 30px;
  }

  .profile-left {
    padding-right: 0;
    padding-bottom: 30px;
    border-right: none;
    border-bottom: 1px solid rgba(0, 255, 255, 0.15);
  }

  .avatar {
    width: 120px;
    height: 120px;
  }

  .balance-container {
    max-width: 300px;
  }

  .profile-actions {
    grid-template-columns: 1fr;
  }

  .otp-input-container {
    flex-direction: column;
    align-items: flex-start;
  }

  .otp-input {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .profile-card {
    padding: 20px;
  }

  .avatar {
    width: 100px;
    height: 100px;
  }

  .profile-name {
    font-size: 1.5rem;
  }

  .detail-item {
    padding: 12px;
  }
}
</style>
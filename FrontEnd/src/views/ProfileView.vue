<template>
  <div class="profile-container">
    <div class="profile-card">
      <div class="profile-grid">
        <!-- Cột trái: Avatar + Info cơ bản -->
        <div class="profile-left">
          <div class="avatar-container">
            <img :src="user.avatar || defaultAvatar" alt="Avatar" class="avatar" @mouseenter="isAvatarHovered = true" @mouseleave="isAvatarHovered = false" />
            <div class="status-indicator" :class="{ 'online': user.status }"></div>
            <transition name="fade">
              <div v-if="isAvatarHovered" class="avatar-overlay" @click="changeAvatar">
                <span class="overlay-text">Đổi avatar</span>
              </div>
            </transition>
          </div>
          <h2 class="profile-name">{{ user.fullName || 'Chưa có tên' }}</h2>
          <div class="balance-coins">
            <span class="balance"><i class="fas fa-wallet"></i> {{ formatCurrency(user.balance) }}</span>
            <span class="coins"><i class="fas fa-coins"></i> {{ user.coin || 0 }}</span>
          </div>
        </div>

        <!-- Cột phải: Chi tiết + Hành động -->
        <div class="profile-right">
          <div class="profile-details">
            <div class="detail-item">
              <i class="fas fa-envelope detail-icon"></i>
              <span class="detail-label">Email</span>
              <span class="detail-value">{{ user.email || 'Chưa cập nhật' }}</span>
            </div>
            <div class="detail-item exp-item">
              <i class="fas fa-star detail-icon"></i>
              <span class="detail-label">Kinh nghiệm</span>
              <span class="level-badge" :class="levelClass">Level {{ user.level }}</span>
              <div class="exp-container">
                <div class="exp-bar-container">
                  <div class="exp-bar" :style="{ width: expPercentage + '%' }"></div>
                  <span class="exp-text">{{ user.experience || 0 }} / {{ expToNextLevel }} XP</span>
                </div>
              </div>
            </div>
            <div class="detail-item">
              <i class="fas fa-university detail-icon"></i>
              <span class="detail-label">Ngân hàng</span>
              <span class="detail-value">{{ user.bankName || 'Chưa cập nhật' }}</span>
            </div>
            <div class="detail-item">
              <i class="fas fa-credit-card detail-icon"></i>
              <span class="detail-label">Số tài khoản</span>
              <span class="detail-value">{{ user.bankNumber || 'Chưa cập nhật' }}</span>
            </div>
            <div class="detail-item">
              <i class="fas fa-calendar-alt detail-icon"></i>
              <span class="detail-label">Ngày tham gia</span>
              <span class="detail-value">{{ formatDate(user.createdDate) }}</span>
            </div>
          </div>
          <div class="profile-actions">
            <button class="action-btn edit-btn" @click="editProfile">Chỉnh sửa</button>
            <button class="action-btn logout-btn" @click="logout">Đăng xuất</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import profileApi from '@/api/profile.api';
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
});

const defaultAvatar = 'https://via.placeholder.com/150?text=No+Avatar';
const isAvatarHovered = ref(false);
const expToNextLevel = 1000;

// Tính toán class màu sắc dựa trên level
const levelClass = computed(() => {
  const level = user.value.level || 0;
  if (level >= 0 && level <= 2) return 'level-gray';
  if (level >= 3 && level <= 5) return 'level-green';
  if (level >= 6 && level <= 8) return 'level-gold';
  if (level >= 9 && level <= 10) return 'level-purple';
  return 'level-red'; // Cấp 11 trở lên
});

const expPercentage = computed(() => {
  const exp = user.value.experience || 0;
  const currentLevelExp = exp % expToNextLevel;
  return (currentLevelExp / expToNextLevel) * 100;
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

const fetchUserProfile = async () => {
  try {
    const userId = store.user?.id || JSON.parse(localStorage.getItem('user') || '{}').id;
    if (!userId) {
      console.error('No user ID found');
      router.push('/login');
      return;
    }
    const response = await profileApi.getByIdProfile(userId);
    if (response.data.result.isSuccess) {
      user.value = response.data.result.data;
    } else {
      console.error('Failed to fetch profile:', response.data.result.message);
    }
  } catch (error) {
    console.error('Error fetching profile:', error);
    router.push('/login');
  }
};

const changeAvatar = () => {
  console.log('Người dùng muốn đổi avatar!');
};

const editProfile = () => {
  router.push('/profile/edit');
};

const logout = async () => {
  await store.logout();
  router.push('/login');
};

onMounted(() => {
  fetchUserProfile();
});
</script>

<style scoped>
/* Tổng thể */
.profile-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column; /* Đặt container dọc để sát lên trên */
  background: linear-gradient(135deg, #1e1e2f 0%, #2a2a40 100%);
  position: relative;
  overflow: hidden;
  padding-top: 20px; /* Thêm padding trên để tránh dính sát quá */
}

.profile-container::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle, rgba(0, 221, 235, 0.15), transparent);
  animation: pulseGlow 10s infinite;
  z-index: 0;
}

@keyframes pulseGlow {
  0%,
  100% {
    transform: scale(1);
    opacity: 0.6;
  }
  50% {
    transform: scale(1.2);
    opacity: 0.9;
  }
}

/* Profile Card */
.profile-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(15px);
  border-radius: 20px;
  padding: 30px;
  width: 100%;
  max-width: 1200px; /* Tăng max-width để container rộng hơn */
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4);
  border: 1px solid rgba(0, 221, 235, 0.3);
  position: relative;
  z-index: 1;
  overflow: hidden;
  margin: 0 auto; /* Canh giữa theo chiều ngang */
}

/* Grid 2 cột */
.profile-grid {
  display: grid;
  grid-template-columns: 1fr 2fr; /* Tăng tỷ lệ cột phải để chứa nội dung dài */
  gap: 30px;
}

/* Cột trái */
.profile-left {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.avatar-container {
  position: relative;
  margin-bottom: 20px;
}

.avatar {
  width: 140px;
  height: 140px;
  border-radius: 50%;
  object-fit: cover;
  border: 4px solid #00ddeb;
  box-shadow: 0 0 20px rgba(0, 221, 235, 0.6);
  transition: filter 0.3s ease;
}

.avatar-container:hover .avatar {
  filter: brightness(70%);
}

.avatar-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 221, 235, 0.3);
  border-radius: 50%;
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.overlay-text {
  color: #ffffff;
  font-size: 1.2rem;
  font-weight: 600;
  text-shadow: 0 0 5px rgba(0, 221, 235, 0.8);
}

.status-indicator {
  position: absolute;
  bottom: 10px;
  right: 10px;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  border: 2px solid #1e1e2f;
}

.status-indicator.online {
  background: #00ff00;
  box-shadow: 0 0 10px rgba(0, 255, 0, 0.8);
}

.status-indicator:not(.online) {
  background: #ff0000;
  box-shadow: 0 0 10px rgba(255, 0, 0, 0.8);
}

.profile-name {
  font-size: 1.5rem; /* Giảm kích thước font của tên */
  font-weight: 700;
  color: #00ddeb;
  text-shadow: 0 0 10px rgba(0, 221, 235, 0.6);
  margin: 0;
  white-space: nowrap; /* Ngăn tên dài xuống dòng */
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 100%; /* Đảm bảo tên không vượt ra ngoài */
}

.balance-coins {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-top: 15px;
}

.balance,
.coins {
  font-size: 1rem;
  color: #ffffff;
}

.balance i,
.coins i {
  color: #00ddeb;
  margin-right: 8px;
}

/* Cột phải */
.profile-right {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.profile-details {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 15px;
  background: rgba(255, 255, 255, 0.03);
  padding: 10px 15px;
  border-radius: 10px;
  position: relative; /* Thêm để chứa .level-badge */
  transition: all 0.3s ease;
}

.detail-item:hover {
  background: rgba(0, 221, 235, 0.1);
  box-shadow: 0 0 10px rgba(0, 221, 235, 0.3);
}

.exp-item {
  flex-direction: column;
  align-items: flex-start;
}

.detail-icon {
  color: #00ddeb;
  font-size: 1.2rem;
}

.detail-label {
  color: #b0b0b0;
  font-size: 0.95rem;
  font-weight: 600;
  min-width: 100px;
}

.detail-value {
  color: #ffffff;
  font-size: 0.95rem;
}

/* Container kinh nghiệm */
.exp-container {
  width: 100%;
  margin-top: 5px;
}

.level-badge {
  position: absolute;
  top: 5px; /* Điều chỉnh khoảng cách từ trên */
  right: 15px; /* Điều chỉnh khoảng cách từ bên phải */
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.9rem;
  font-weight: 600;
  box-shadow: 0 0 10px rgba(0, 221, 235, 0.6);
  z-index: 1;
}

/* Màu sắc cho level-badge theo cấp độ */
.level-gray {
  background: #808080; /* Xám nhạt - Cấp 0-2 */
  color: #ffffff;
}

.level-green {
  background: #00ff00; /* Xanh lá - Cấp 3-5 */
  color: #1e1e2f;
}

.level-gold {
  background: #ffd700; /* Vàng - Cấp 6-8 */
  color: #1e1e2f;
}

.level-purple {
  background: #9400d3; /* Tím - Cấp 9-10 */
  color: #ffffff;
}

.level-red {
  background: #ff0000; /* Đỏ rực - Cấp 11 trở lên */
  color: #ffffff;
}

.exp-bar-container {
  width: 100%;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  height: 20px;
  position: relative;
  overflow: hidden;
}

.exp-bar {
  height: 100%;
  background: linear-gradient(90deg, #00ddeb, #33e6f2);
  border-radius: 8px;
  transition: width 0.5s ease;
  box-shadow: 0 0 10px rgba(0, 221, 235, 0.6);
}

.exp-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: #1e1e2f;
  font-size: 0.85rem;
  font-weight: 600;
  text-shadow: 0 0 2px rgba(255, 255, 255, 0.8);
}

/* Profile Actions */
.profile-actions {
  display: flex;
  gap: 15px;
  margin-top: 20px;
}

.action-btn {
  flex: 1;
  padding: 12px;
  border: none;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.4s ease;
}

.edit-btn {
  background: #00ddeb;
  color: #1e1e2f;
  box-shadow: 0 4px 12px rgba(0, 221, 235, 0.4);
}

.edit-btn:hover {
  background: #33e6f2;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 221, 235, 0.6);
}

.logout-btn {
  background: #ff007a;
  color: #ffffff;
  box-shadow: 0 4px 12px rgba(255, 0, 122, 0.4);
}

.logout-btn:hover {
  background: #ff3399;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 0, 122, 0.6);
}

/* Transition cho overlay */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.3s ease;
}
.detail-item.bank-info {
  background: rgba(0, 221, 235, 0.1);
  border-left: 3px solid #00ddeb;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: scale(0.9);
}

/* Responsive */
@media (max-width: 768px) {
  .profile-card {
    padding: 20px;
    max-width: 90%;
  }
  .profile-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  .avatar {
    width: 100px;
    height: 100px;
  }
  .profile-name {
    font-size: 1.3rem; /* Giảm thêm ở màn hình nhỏ */
  }
  .balance-coins {
    flex-direction: row;
    gap: 20px;
  }
  .exp-bar-container {
    height: 18px;
  }
  .level-badge {
    font-size: 0.8rem;
    padding: 3px 6px;
    top: 5px;
    right: 10px;
  }
}
</style>
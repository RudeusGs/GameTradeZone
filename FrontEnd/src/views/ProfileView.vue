<template>
  <div class="profile-container">
    <div class="profile-card">
      <div class="profile-grid">
        <!-- Cột trái: Avatar + Info cơ bản -->
        <div class="profile-left">
          <div class="avatar-container">
            <img :src="user.Avatar || defaultAvatar" alt="Avatar" class="avatar" />
            <div class="status-indicator" :class="{ 'online': user.Status }"></div>
            <transition name="fade">
              <div v-if="isAvatarHovered" class="avatar-overlay" @click="changeAvatar">
                <span class="overlay-text">Đổi avatar</span>
              </div>
            </transition>
          </div>
          <h2 class="profile-name">{{ user.FullName || 'Chưa có tên' }}</h2>
          <p class="profile-level">Level {{ user.Level }}</p>
          <div class="balance-coins">
            <span class="balance"><i class="fas fa-wallet"></i> {{ user.Balance?.toFixed(2) || 0 }} $</span>
            <span class="coins"><i class="fas fa-coins"></i> {{ user.Coin || 0 }}</span>
          </div>
        </div>

        <!-- Cột phải: Chi tiết + Hành động -->
        <div class="profile-right">
          <div class="profile-details">
            <div class="detail-item">
              <i class="fas fa-envelope detail-icon"></i>
              <span class="detail-label">Email</span>
              <span class="detail-value">{{ user.Email || 'Chưa cập nhật' }}</span>
            </div>
            <div class="detail-item exp-item">
              <i class="fas fa-star detail-icon"></i>
              <span class="detail-label">Kinh nghiệm</span>
              <div class="exp-bar-container">
                <div class="exp-bar" :style="{ width: expPercentage + '%' }"></div>
                <span class="exp-text">{{ user.Experience || 0 }} / {{ expToNextLevel }} XP</span>
              </div>
            </div>
            <div class="detail-item">
              <i class="fas fa-calendar-alt detail-icon"></i>
              <span class="detail-label">Ngày tham gia</span>
              <span class="detail-value">{{ formatDate(user.CreatedDate) }}</span>
            </div>
          </div>
          <div class="profile-actions">
            <button class="action-btn edit-btn">Chỉnh sửa</button>
            <button class="action-btn logout-btn">Đăng xuất</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';

interface User {
  FullName?: string | null;
  Balance?: number | null;
  Coin?: number | null;
  Experience?: number | null;
  Level: number;
  Status: boolean;
  Avatar?: string | null;
  CreatedDate?: Date | null;
  Email?: string | null;
}

const user = ref<User>({
  FullName: "Nguyễn Văn A",
  Balance: 150.75,
  Coin: 500,
  Experience: 2500,
  Level: 3,
  Status: true,
  Avatar: "data:image/jpeg;base64,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",
  CreatedDate: new Date("2023-05-15T10:30:00Z"),
  Email: "nguyenvana@example.com"
});

const defaultAvatar = "https://via.placeholder.com/150?text=No+Avatar";
const isAvatarHovered = ref(false);

// Giả lập mức kinh nghiệm tối đa để lên level (1000 XP/level)
const expToNextLevel = 1000;

// Tính phần trăm kinh nghiệm cho thanh tiến độ
const expPercentage = computed(() => {
  const exp = user.value.Experience || 0;
  const currentLevelExp = exp % expToNextLevel; // Lấy phần dư sau khi lên level
  return (currentLevelExp / expToNextLevel) * 100;
});

const formatDate = (date?: Date | null) => {
  if (!date) return "Chưa có thông tin";
  return new Date(date).toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  });
};

const changeAvatar = () => {
  console.log("Người dùng muốn đổi avatar!");
};
</script>

<style scoped>
/* Tổng thể */
.profile-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #1e1e2f 0%, #2a2a40 100%);
  position: relative;
  overflow: hidden;
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
  0%, 100% { transform: scale(1); opacity: 0.6; }
  50% { transform: scale(1.2); opacity: 0.9; }
}

/* Profile Card */
.profile-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(15px);
  border-radius: 20px;
  padding: 30px;
  width: 100%;
  max-width: 700px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4);
  border: 1px solid rgba(0, 221, 235, 0.3);
  position: relative;
  z-index: 1;
  overflow: hidden;
}

/* Grid 2 cột */
.profile-grid {
  display: grid;
  grid-template-columns: 1fr 1.5fr;
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
  font-size: 2rem;
  font-weight: 700;
  color: #00ddeb;
  text-shadow: 0 0 10px rgba(0, 221, 235, 0.6);
  margin: 0;
}

.profile-level {
  font-size: 1.1rem;
  color: #b0b0b0;
  margin: 5px 0 15px;
}

.balance-coins {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.balance, .coins {
  font-size: 1rem;
  color: #ffffff;
}

.balance i, .coins i {
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

/* Thanh kinh nghiệm */
.exp-bar-container {
  width: 100%;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  height: 20px;
  position: relative;
  overflow: hidden;
  margin-top: 5px;
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
    font-size: 1.6rem;
  }
  .profile-level {
    font-size: 1rem;
  }
  .balance-coins {
    flex-direction: row;
    gap: 20px;
  }
  .exp-bar-container {
    height: 18px;
  }
}
</style>
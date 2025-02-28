<template>
  <div class="transaction-container">
    <div class="transaction-card">
      <!-- Header -->
      <div class="transaction-header">
        <h2 class="transaction-title">Xác nhận giao dịch tài khoản</h2>
        <p class="transaction-subtitle">Kiểm tra và hoàn tất giao dịch</p>
      </div>

      <!-- Hình ảnh tài khoản -->
      <div class="account-image-container">
        <img :src="account.image" alt="Account Image" class="account-image" />
      </div>

      <!-- Thông tin tài khoản (thêm tên game và người đăng) -->
      <div class="account-details">
        <div class="detail-item">
          <i class="fas fa-user detail-icon"></i>
          <span class="detail-label">Tài khoản:</span>
          <span class="detail-value">{{ account.username }}</span>
        </div>
        <div class="detail-item">
          <i class="fas fa-lock detail-icon"></i>
          <span class="detail-label">Mật khẩu:</span>
          <span class="detail-value">{{ account.password }}</span>
        </div>
        <div class="detail-item">
          <i class="fas fa-gamepad detail-icon"></i>
          <span class="detail-label">Tên game:</span>
          <span class="detail-value">{{ account.game }}</span>
        </div>
        <div class="detail-item">
          <i class="fas fa-user-circle detail-icon"></i>
          <span class="detail-label">Người đăng:</span>
          <span class="detail-value">{{ account.seller }}</span>
        </div>
      </div>

      <!-- Hành động và trạng thái -->
      <div class="transaction-footer">
        <div class="action-group">
          <!-- Yêu cầu Gmail -->
          <button class="action-btn email-btn" @click="handleRequestEmail">
            Yêu cầu gửi Gmail
          </button>
          <transition name="fade">
            <div v-if="receivedEmail" class="received-info">
              <i class="fas fa-envelope received-icon"></i>
              <span class="received-label">Email:</span>
              <span class="received-value">{{ receivedEmail }}</span>
            </div>
          </transition>

          <!-- Yêu cầu OTP -->
          <button class="action-btn otp-btn" @click="handleRequestOTP">
            Yêu cầu gửi OTP
          </button>
          <transition name="fade">
            <div v-if="receivedOTP" class="received-info">
              <i class="fas fa-key received-icon"></i>
              <span class="received-label">OTP:</span>
              <span class="received-value">{{ receivedOTP }}</span>
            </div>
          </transition>
        </div>

        <!-- Nhóm đổi trạng thái (di chuyển xuống dưới) -->
        <div class="status-container">
          <select class="status-combobox" v-model="tempStatus">
            <option value="pending">Đang kiểm tra</option>
            <option value="rejected">Từ chối</option>
            <option value="completed">Kiểm tra xong</option>
          </select>
          <button class="confirm-status-btn" @click="openConfirmModal">
            Cập nhật
          </button>
        </div>
      </div>
    </div>

    <!-- Modal xác nhận thay đổi trạng thái -->
    <transition name="fade">
      <div v-if="showModal" class="modal-overlay">
        <div class="modal-content">
          <h3>Xác nhận thay đổi trạng thái</h3>
          <p>
            Bạn có chắc muốn chuyển giao dịch sang trạng thái:
            <strong>{{ getStatusText(tempStatus) }}</strong>?
          </p>
          <div class="modal-buttons">
            <button @click="confirmStatusChange" class="modal-confirm">
              Xác nhận
            </button>
            <button @click="showModal = false" class="modal-cancel">
              Hủy
            </button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

// Giả lập dữ liệu tài khoản (thêm tên game và người đăng)
const account = {
  username: "Gamer123",
  password: "Abc@12345",
  game: "Valorant", // Thêm tên game
  seller: "UserA",  // Thêm người đăng bán
  image: "https://via.placeholder.com/400x200?text=Game+Account"
};

// Trạng thái thật của giao dịch
const transactionStatus = ref('pending');

// Biến tạm để chọn trong combobox (tránh thay đổi ngay)
const tempStatus = ref(transactionStatus.value);

// Các biến mô phỏng đã nhận được Email/OTP
const receivedEmail = ref('');
const receivedOTP = ref('');

// Điều khiển hiển thị Modal
const showModal = ref(false);

// Mở modal xác nhận
function openConfirmModal() {
  if (tempStatus.value === transactionStatus.value) {
    return;
  }
  showModal.value = true;
}

// Đồng ý thay đổi trạng thái
function confirmStatusChange() {
  transactionStatus.value = tempStatus.value;
  showModal.value = false;
}

// Chuyển status -> text hiển thị
function getStatusText(status: string) {
  switch(status) {
    case 'pending': return 'Đang kiểm tra';
    case 'rejected': return 'Từ chối';
    case 'completed': return 'Kiểm tra xong';
    default: return 'Không xác định';
  }
}

// Giả lập khi bấm "Yêu cầu gửi Gmail"
function handleRequestEmail() {
  receivedEmail.value = "example@gmail.com";
}

// Giả lập khi bấm "Yêu cầu gửi OTP"
function handleRequestOTP() {
  receivedOTP.value = "X7K9P2";
}
</script>

<style scoped>
/* Tổng thể */
.transaction-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #1e1e2f 0%, #2a2a40 100%);
  position: relative;
  overflow: hidden;
}

.transaction-container::before {
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

/* Transaction Card */
.transaction-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(15px);
  border-radius: 20px;
  padding: 25px;
  width: 100%;
  max-width: 600px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4);
  border: 1px solid rgba(0, 221, 235, 0.3);
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* Header */
.transaction-header {
  text-align: center;
}

.transaction-title {
  font-size: 1.8rem;
  font-weight: 700;
  color: #00ddeb;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  text-shadow: 0 0 10px rgba(0, 221, 235, 0.6);
}

.transaction-subtitle {
  font-size: 1rem;
  color: #b0b0b0;
  margin-top: 5px;
}

/* Hình ảnh tài khoản */
.account-image-container {
  width: 100%;
}

.account-image {
  width: 100%;
  height: 250px;
  object-fit: cover;
  border-radius: 12px;
  border: 2px solid rgba(0, 221, 235, 0.3);
  transition: all 0.3s ease;
}

.account-image:hover {
  border-color: #00ddeb;
  box-shadow: 0 0 15px rgba(0, 221, 235, 0.5);
}

/* Thông tin tài khoản */
.account-details {
  display: grid;
  grid-template-columns: repeat(2, 1fr); /* Chia 2 cột */
  gap: 15px;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 10px;
  background: rgba(255, 255, 255, 0.03);
  padding: 10px 15px;
  border-radius: 10px;
  transition: all 0.3s ease;
}

.detail-item:hover {
  background: rgba(0, 221, 235, 0.1);
  box-shadow: 0 0 10px rgba(0, 221, 235, 0.3);
}

.detail-icon {
  color: #00ddeb;
  font-size: 1.2rem;
}

.detail-label {
  color: #b0b0b0;
  font-size: 0.95rem;
  font-weight: 600;
  min-width: 80px;
}

.detail-value {
  color: #ffffff;
  font-size: 0.95rem;
}

/* Hành động & Trạng thái */
.transaction-footer {
  display: flex;
  flex-direction: column; /* Đặt hành động và trạng thái theo cột */
  gap: 20px;
}

.action-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

/* Các nút Yêu cầu */
.action-btn {
  width: 100%;
  padding: 12px;
  border: none;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.4s ease;
}

.email-btn {
  background: #00ddeb;
  color: #1e1e2f;
  box-shadow: 0 4px 12px rgba(0, 221, 235, 0.4);
}
.email-btn:hover {
  background: #33e6f2;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 221, 235, 0.6);
}

.otp-btn {
  background: #ffcc00;
  color: #1e1e2f;
  box-shadow: 0 4px 12px rgba(255, 204, 0, 0.4);
}
.otp-btn:hover {
  background: #ffd633;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 204, 0, 0.6);
}

.received-info {
  display: flex;
  align-items: center;
  gap: 10px;
  background: rgba(255, 255, 255, 0.03);
  padding: 10px 15px;
  border-radius: 10px;
}

.received-icon {
  color: #00ddeb;
  font-size: 1.2rem;
}

.received-label {
  color: #b0b0b0;
  font-size: 0.95rem;
  font-weight: 600;
  min-width: 80px;
}

.received-value {
  color: #ffffff;
  font-size: 0.95rem;
}

/* Trạng thái & nút cập nhật (CSS cải tiến) */
.status-container {
  display: flex;
  justify-content: end;
  gap: 10px;
}

.status-combobox {
  width: 100%; /* Kéo dài combobox */
  max-width: 200px;
  padding: 10px;
  background: linear-gradient(135deg, #0d1b2a 0%, #1a0933 100%);
  border: 1px solid #00ffff;
  border-radius: 12px;
  color: #e0e0e0;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  outline: none;
  transition: all 0.3s ease;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.3);
}

.status-combobox:hover {
  border-color: #ff00ff;
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.5);
}

.status-combobox option {
  background: #1e1e2f;
  color: #e0e0e0;
}

.confirm-status-btn {
  padding: 10px 20px;
  border-radius: 12px;
  font-size: 1rem;
  background: #00ddeb;
  color: #1e1e2f;
  font-weight: 600;
  box-shadow: 0 4px 12px rgba(0, 221, 235, 0.4);
  transition: all 0.4s ease;
  cursor: pointer;
  border: none;
}
.confirm-status-btn:hover {
  background: #33e6f2;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 221, 235, 0.6);
}

/* Modal overlay */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0,0,0,0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 999;
}

/* Modal content */
.modal-content {
  background: #1e1e2f;
  color: #fff;
  padding: 30px;
  border-radius: 15px;
  width: 300px;
  max-width: 90%;
  text-align: center;
  box-shadow: 0 10px 30px rgba(0,0,0,0.3);
  border: 1px solid rgba(0, 221, 235, 0.2);
}

.modal-content h3 {
  margin-bottom: 15px;
  font-size: 1.3rem;
  color: #00ddeb;
}

.modal-buttons {
  display: flex;
  justify-content: space-around;
  margin-top: 20px;
}

.modal-confirm, .modal-cancel {
  padding: 10px 20px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  border: none;
}

.modal-confirm {
  background: #00ddeb;
  color: #1e1e2f;
}
.modal-confirm:hover {
  background: #33e6f2;
}

.modal-cancel {
  background: #999999;
  color: #fff;
}
.modal-cancel:hover {
  background: #aaaaaa;
}

/* Transition fade */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.5s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

/* Responsive */
@media (max-width: 768px) {
  .transaction-card {
    padding: 20px;
    max-width: 90%;
  }
  .transaction-title {
    font-size: 1.5rem;
  }
  .account-image {
    height: 200px;
  }
  .account-details {
    grid-template-columns: 1fr; /* Một cột trên mobile */
  }
  .transaction-footer {
    gap: 15px;
  }
  .status-combobox {
    max-width: 100%; /* Kéo dài combobox trên mobile */
  }
}
</style>

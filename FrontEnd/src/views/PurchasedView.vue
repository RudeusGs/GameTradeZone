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
  
        <!-- Thông tin tài khoản -->
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
        </div>
  
        <!-- Hành động và trạng thái -->
        <div class="transaction-footer">
          <div class="action-group">
            <!-- Yêu cầu Gmail -->
            <button class="action-btn email-btn">Yêu cầu gửi Gmail</button>
            <transition name="fade">
              <div v-if="receivedEmail" class="received-info">
                <i class="fas fa-envelope received-icon"></i>
                <span class="received-label">Email:</span>
                <span class="received-value">{{ receivedEmail }}</span>
              </div>
            </transition>
  
            <!-- Yêu cầu OTP -->
            <button class="action-btn otp-btn">Yêu cầu gửi OTP</button>
            <transition name="fade">
              <div v-if="receivedOTP" class="received-info">
                <i class="fas fa-key received-icon"></i>
                <span class="received-label">OTP:</span>
                <span class="received-value">{{ receivedOTP }}</span>
              </div>
            </transition>
          </div>
  
          <!-- Trạng thái -->
          <select class="status-combobox" v-model="transactionStatus">
            <option value="pending">Đang kiểm tra</option>
            <option value="completed">Kiểm tra xong</option>
            <option value="rejected">Từ chối</option>
          </select>
        </div>
      </div>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref } from 'vue';
  
  const account = {
    username: "Gamer123",
    password: "Abc@12345",
    image: "https://via.placeholder.com/400x200?text=Game+Account" // Hình placeholder đơn giản
  };
  
  const transactionStatus = ref('pending');
  const receivedEmail = ref('example@gmail.com'); // Giả lập email nhận được
  const receivedOTP = ref('X7K9P2'); // Giả lập OTP nhận được
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
    height: 250px; /* Tăng chiều cao để hình nổi bật hơn */
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
    display: flex;
    justify-content: space-between;
    gap: 15px;
  }
  
  .detail-item {
    flex: 1;
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
  
  /* Hành động và trạng thái */
  .transaction-footer {
    display: flex;
    gap: 20px;
  }
  
  .action-group {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
  
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
  
  .status-combobox {
    flex: 1;
    padding: 12px;
    background: rgba(255, 255, 255, 0.05);
    border: 1px solid rgba(0, 221, 235, 0.3);
    border-radius: 12px;
    color: #ffffff;
    font-size: 0.95rem;
    font-weight: 600;
    cursor: pointer;
    outline: none;
    transition: all 0.3s ease;
  }
  
  .status-combobox:hover {
    border-color: #00ddeb;
    box-shadow: 0 0 10px rgba(0, 221, 235, 0.3);
  }
  
  .status-combobox option {
    background: #1e1e2f;
    color: #ffffff;
  }
  
  /* Transition cho thông tin nhận được */
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
      flex-direction: column;
    }
    .transaction-footer {
      flex-direction: column;
    }
  }
  </style>
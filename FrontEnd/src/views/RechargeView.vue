<template>
  <div class="recharge-container" v-if="user">
    <!-- Particle Background -->
    <div class="particle-background">
      <div v-for="i in 15" :key="i" class="particle"></div>
    </div>
    <section class="recharge-content">
      <h1 class="recharge-title">Nạp Tiền GameTradeZone</h1>
      <p class="recharge-subtitle">
        Xin chào {{ fullname }}, nhập số tiền bạn muốn nạp và bấm xác nhận để
        tiếp tục.
      </p>

      <!-- Nhập số tiền -->
      <div v-if="!showPaymentDetails" class="amount-section">
        <label for="rechargeAmount" class="amount-label">Số tiền (VNĐ):</label>
        <input
          v-model="rechargeAmount"
          type="number"
          id="rechargeAmount"
          class="amount-input"
          placeholder="Nhập số tiền"
          min="10000"
          required
          :disabled="!isAuthenticated"
        />
        <div class="preset-amounts">
          <button
            v-for="amount in presetAmounts"
            :key="amount"
            @click="rechargeAmount = amount"
            class="preset-btn"
            :disabled="!isAuthenticated"
          >
            {{ amount.toLocaleString() }} VNĐ
          </button>
        </div>
        <button
          @click="confirmRecharge"
          class="confirm-btn"
          :disabled="!isValidAmount || !isAuthenticated"
        >
          <i class="fas fa-check"></i> Xác Nhận
        </button>
        <p v-if="!isAuthenticated" class="auth-warning">
          Vui lòng đăng nhập để nạp tiền!
        </p>
      </div>

      <!-- Hiển thị mã QR và thông tin ngân hàng -->
      <div v-else class="payment-details">
        <h2 class="method-title">Thông Tin Thanh Toán</h2>
        <div class="qr-section">
          <img :src="qrCodeUrl" alt="Mã QR Thanh Toán" class="qr-image" />
          <p class="qr-instruction">Quét mã QR để chuyển khoản</p>
        </div>
        <div class="bank-details">
          <p><strong>Số Tài Khoản: </strong> {{ bankAccount.number }}</p>
          <p><strong>Tên Chủ Tài Khoản: </strong> {{ bankAccount.holder }}</p>
          <p><strong>Số tiền chuyển khoản: </strong>{{ rechargeAmount }}</p>
          <p><strong>Nội Dung Chuyển Khoán: </strong> USERID {{ userId }}</p>
        </div>
        <p class="note-text">
          Vui lòng chuyển khoản theo thông tin trên. Giao dịch sẽ được xử lý
          trong 1-5 phút. Nếu gặp vấn đề, liên hệ:
          <a href="mailto:support@gametradezone.com" class="note-link"
            >support@gametradezone.com</a
          >
        </p>
        <button @click="goBack" class="back-btn">
          <i class="fas fa-arrow-left"></i> Quay Lại
        </button>
      </div>
    </section>

    <!-- Footer -->
    <footer class="recharge-footer">
      <p class="footer-text">
        © 2025 GameTradeZone. Tất cả quyền được bảo lưu.
      </p>
    </footer>
  </div>
  <div v-else class="login-prompt">
    <h2>Bạn cần đăng nhập để sử dụng tính năng này!</h2>
    <button @click="redirectToLogin" class="login-btn">Đăng nhập</button>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from "vue";
import { userStore } from "@/stores/auth";
import * as signalR from "@microsoft/signalr";
import type { UserInfoModel } from "@/models/user-model"; // Adjust the import path as needed

const store = userStore();
const user = computed(() => store.user as UserInfoModel | null);
const fullname = computed(() => store.fullname);
const userId = computed(() => user.value?.id || 0);
const rechargeAmount = ref<number>(0);
const showPaymentDetails = ref<boolean>(false);
const qrCodeUrl = ref<string>("");
const bankAccount = {
  number: "123456789",
  holder: "GameTradeZone Corp",
};
const presetAmounts = [10000, 20000, 50000, 100000, 200000];

// SignalR connection
let connection: signalR.HubConnection | null = null;

const isAuthenticated = computed(() => !!user.value);
const isValidAmount = computed(
  () => rechargeAmount.value >= 10000 && isAuthenticated.value
);

const generateQRCode = () => {
  const qrData = `https://qr.sepay.vn/img?acc=96247XAAD6&bank=BIDV&amount=${rechargeAmount.value}&des=USERID%20${userId.value}`;
  qrCodeUrl.value = qrData;
};

const confirmRecharge = () => {
  if (isValidAmount.value) {
    generateQRCode();
    showPaymentDetails.value = true;
  }
};

const goBack = () => {
  showPaymentDetails.value = false;
  rechargeAmount.value = 0;
};

const redirectToLogin = () => {
  // Replace with your login route
  window.location.href = "/login";
};

const initializeSignalR = async () => {
  if (!isAuthenticated.value) return;

  connection = new signalR.HubConnectionBuilder()
    .withUrl(
      "hhttps://d2a0-2402-800-63af-bfoe-7981-11d7-6552-5e2c.ngrok-free.app/transactionHub",
      {
        // Replace with your deployed SignalR hub URL
        accessTokenFactory: () => "", // Removed token assumption since UserInfoModel doesn't have it
      }
    )
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveTransactionStatus", (message: string) => {
    alert(message); // Show the success message
  });

  connection.on("PaymentCompleted", () => {
    showPaymentDetails.value = false; // Redirect back to the amount selection screen
    rechargeAmount.value = 0;
    alert("Thanh toán thành công!");
  });

  try {
    await connection.start();
    console.log("SignalR Connected for UserId: ", userId.value);
  } catch (err) {
    console.error("SignalR Connection Error:", err);
  }
};

onMounted(() => {
  store.init(); // Initialize user from localStorage
  if (isAuthenticated.value) {
    initializeSignalR();
  }
});

onUnmounted(() => {
  if (connection) {
    connection.stop();
    connection = null;
  }
});
</script>

<style scoped>
/* Existing styles remain unchanged */
.recharge-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #0f0c29, #302b63, #24243e);
  color: #f0f0f0;
  font-family: Arial, sans-serif;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  overflow: hidden;
  position: relative;
}

.particle-background {
  position: absolute;
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.particle {
  position: absolute;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 50%;
  animation: float 15s infinite;
}

@keyframes float {
  0% {
    transform: translateY(100vh);
  }
  100% {
    transform: translateY(-10vh);
  }
}

.recharge-content {
  padding: 40px 20px;
  text-align: center;
  z-index: 1;
}

.recharge-title {
  font-size: 2.5rem;
  margin-bottom: 10px;
  color: #00b3e0;
}

.recharge-subtitle {
  font-size: 1.2rem;
  margin-bottom: 20px;
  color: #ccc;
}

.amount-section,
.payment-details {
  background: rgba(255, 255, 255, 0.1);
  padding: 20px;
  border-radius: 10px;
  backdrop-filter: blur(5px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.amount-label {
  display: block;
  margin-bottom: 10px;
  font-size: 1.1rem;
}

.amount-input {
  width: 100%;
  padding: 10px;
  margin-bottom: 15px;
  border: 1px solid #00b3e0;
  border-radius: 5px;
  background: rgba(0, 0, 0, 0.3);
  color: #f0f0f0;
}

.preset-amounts {
  display: flex;
  gap: 10px;
  margin-top: 15px;
  flex-wrap: wrap;
  justify-content: center;
}

.preset-btn {
  padding: 8px 15px;
  background: rgba(0, 179, 224, 0.7);
  border: 1px solid #00b3e0;
  border-radius: 8px;
  color: #f0f0f0;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease-in-out;
}

.preset-btn:hover {
  background: rgba(0, 179, 224, 0.9);
  box-shadow: 0 0 8px rgba(0, 204, 255, 0.3);
}

.preset-btn:disabled,
.confirm-btn:disabled,
.back-btn:disabled {
  background: #666;
  border-color: #666;
  cursor: not-allowed;
}

.confirm-btn,
.back-btn {
  padding: 10px 20px;
  background: rgba(0, 179, 224, 0.7);
  border: 1px solid #00b3e0;
  border-radius: 8px;
  color: #f0f0f0;
  font-size: 1rem;
  cursor: pointer;
  margin-top: 15px;
  transition: all 0.2s ease-in-out;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
}

.confirm-btn:hover,
.back-btn:hover {
  background: rgba(0, 179, 224, 0.9);
  box-shadow: 0 0 8px rgba(0, 204, 255, 0.3);
}

.back-btn {
  background: rgba(255, 0, 255, 0.7);
  border: 1px solid #ff00ff;
}

.back-btn:hover {
  background: rgba(255, 0, 255, 0.9);
  box-shadow: 0 0 8px rgba(255, 0, 255, 0.3);
}

.payment-details {
  margin-top: 20px;
}

.qr-section {
  margin-bottom: 20px;
}

.qr-image {
  max-width: 200px;
  border: 1px solid #00b3e0;
  border-radius: 5px;
}

.qr-instruction {
  margin-top: 10px;
  font-size: 0.9rem;
  color: #ccc;
}

.bank-details {
  text-align: left;
  margin-bottom: 20px;
}

.bank-details p {
  margin: 5px 0;
}

.note-text {
  font-size: 0.9rem;
  color: #ccc;
  margin-bottom: 20px;
}

.note-link {
  color: #00b3e0;
  text-decoration: none;
}

.note-link:hover {
  text-decoration: underline;
}

.recharge-footer {
  padding: 20px;
  text-align: center;
  background: rgba(0, 0, 0, 0.5);
  color: #ccc;
  z-index: 1;
}

.footer-text {
  margin: 0;
  font-size: 0.9rem;
}

.login-prompt {
  min-height: 100vh;
  background: linear-gradient(135deg, #0f0c29, #302b63, #24243e);
  color: #f0f0f0;
  font-family: Arial, sans-serif;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  text-align: center;
}

.login-btn {
  padding: 10px 20px;
  background: rgba(0, 179, 224, 0.7);
  border: 1px solid #00b3e0;
  border-radius: 8px;
  color: #f0f0f0;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease-in-out;
}

.login-btn:hover {
  background: rgba(0, 179, 224, 0.9);
  box-shadow: 0 0 8px rgba(0, 204, 255, 0.3);
}

.auth-warning {
  color: #ff4444;
  font-size: 0.9rem;
  margin-top: 10px;
}

/* Particle animation sizes and positions */
.particle:nth-child(1) {
  width: 10px;
  height: 10px;
  left: 10%;
  animation-delay: 0s;
}
.particle:nth-child(2) {
  width: 15px;
  height: 15px;
  left: 20%;
  animation-delay: 2s;
}
.particle:nth-child(3) {
  width: 12px;
  height: 12px;
  left: 30%;
  animation-delay: 4s;
}
</style>

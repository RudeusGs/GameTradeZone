<template>
  <div class="recharge-container" v-if="user">
    <!-- Gradient Background -->
    <div class="gradient-background"></div>

    <section class="recharge-content">
      <div class="recharge-header">
        <h1 class="recharge-title">Nạp Tiền GameTradeZone</h1>
        <p class="recharge-subtitle">
          Xin chào <span class="username">{{ fullname }}</span>, hãy chọn số tiền bạn muốn nạp.
        </p>
      </div>

      <!-- Nhập số tiền -->
      <div v-if="!showPaymentDetails" class="amount-section">
        <div class="card-container">
          <div class="card-header">
            <div class="card-icon"><i class="fas fa-wallet"></i></div>
            <h2 class="section-title">Chọn Số Tiền</h2>
          </div>

          <div class="input-group">
            <label for="rechargeAmount" class="amount-label">Số tiền (VNĐ):</label>
            <div class="input-wrapper">
              <span class="currency-prefix">₫</span>
              <input
                v-model="rechargeAmount"
                id="rechargeAmount"
                class="amount-input"
                placeholder="Nhập số tiền"
                min="10000"
                required
                :disabled="!isAuthenticated"
              />
            </div>
          </div>

          <div class="preset-amounts">
            <button
              v-for="amount in presetAmounts"
              :key="amount"
              @click="rechargeAmount = amount"
              class="preset-btn"
              :class="{ 'active': rechargeAmount === amount }"
              :disabled="!isAuthenticated"
            >
              {{ amount.toLocaleString() }} ₫
            </button>
          </div>

          <div class="payment-info">
            <div class="info-item">
              <i class="fas fa-info-circle"></i>
              <span>Số tiền tối thiểu: 10.000₫</span>
            </div>
            <div class="info-item">
              <i class="fas fa-info-circle"></i>
              <span>Tài khoản được cập nhật sau 1-5 phút</span>
            </div>
          </div>

          <button
            @click="confirmRecharge"
            class="confirm-btn"
            :disabled="!isValidAmount || !isAuthenticated"
          >
            <i class="fas fa-check-circle"></i> Tiếp Tục Thanh Toán
          </button>

          <p v-if="!isAuthenticated" class="auth-warning">
            <i class="fas fa-exclamation-triangle"></i> Vui lòng đăng nhập để nạp tiền!
          </p>
        </div>
      </div>

      <!-- Hiển thị mã QR và thông tin ngân hàng -->
      <div v-else class="payment-details-container">
        <div class="card-container payment-details">
          <div class="card-header">
            <div class="card-icon"><i class="fas fa-credit-card"></i></div>
            <h2 class="section-title">Thông Tin Thanh Toán</h2>
          </div>

          <div class="payment-grid">
            <div class="qr-section">
              <div class="qr-image-container">
                <img :src="qrCodeUrl" alt="Mã QR Thanh Toán" class="qr-image" />
              </div>
              <p class="qr-instruction">Quét mã QR bằng ứng dụng ngân hàng để thanh toán</p>
            </div>

            <div class="bank-details">
              <div class="payment-amount">
                <span class="amount-label">Số tiền:</span>
                <span class="amount-value">{{ rechargeAmount.toLocaleString() }}₫</span>
              </div>

              <div class="detail-item">
                <span class="detail-label"><i class="fas fa-university"></i> Ngân Hàng:</span>
                <span class="detail-value">MB Bank</span>
              </div>
              <div class="detail-item">
                <span class="detail-label"><i class="fas fa-id-card"></i> Số Tài Khoản:</span>
                <span class="detail-value">{{ bankAccount.number }}</span>
                <button class="copy-btn" @click="copyToClipboard(bankAccount.number)" title="Sao chép">
                  <i class="fas fa-copy"></i>
                </button>
              </div>
              <div class="detail-item">
                <span class="detail-label"><i class="fas fa-user"></i> Chủ Tài Khoản:</span>
                <span class="detail-value">{{ bankAccount.holder }}</span>
              </div>
              <div class="detail-item">
                <span class="detail-label"><i class="fas fa-file-alt"></i> Nội Dung CK:</span>
                <span class="detail-value">USERID {{ userId }}</span>
                <button class="copy-btn" @click="copyToClipboard(`USERID ${userId}`)" title="Sao chép">
                  <i class="fas fa-copy"></i>
                </button>
              </div>

              <div class="payment-note">
                <i class="fas fa-exclamation-circle"></i>
                <p>Vui lòng nhập chính xác nội dung chuyển khoản</p>
              </div>
            </div>
          </div>

          <div class="payment-steps">
            <div class="step">
              <div class="step-number">1</div>
              <div class="step-content">
                <h3>Chuyển khoản</h3>
                <p>Thực hiện chuyển khoản theo thông tin bên trên</p>
              </div>
            </div>
            <div class="step">
              <div class="step-number">2</div>
              <div class="step-content">
                <h3>Xác nhận</h3>
                <p>Hệ thống sẽ tự động xác nhận giao dịch của bạn</p>
              </div>
            </div>
            <div class="step">
              <div class="step-number">3</div>
              <div class="step-content">
                <h3>Hoàn tất</h3>
                <p>Số dư tài khoản sẽ được cập nhật trong vòng 1-5 phút</p>
              </div>
            </div>
          </div>

          <div class="support-info">
            <p>
              Cần hỗ trợ? Liên hệ:
              <a href="mailto:support@gametradezone.com" class="support-link">
                <i class="fas fa-envelope"></i> support@gametradezone.com
              </a>
            </p>
          </div>

          <button @click="goBack" class="back-btn">
            <i class="fas fa-arrow-left"></i> Quay Lại
          </button>
        </div>
      </div>
    </section>
  </div>
  <div v-else class="login-prompt">
    <div class="login-card">
      <i class="fas fa-lock login-icon"></i>
      <h2>Bạn cần đăng nhập để sử dụng tính năng này!</h2>
      <button @click="redirectToLogin" class="login-btn">
        <i class="fas fa-sign-in-alt"></i> Đăng nhập
      </button>
    </div>
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
  holder: "VU THANH LAM",
};
const presetAmounts = [10000, 20000, 50000, 100000, 200000, 500000, 1000000, 2000000, 5000000];

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

const copyToClipboard = (text: string) => {
  navigator.clipboard.writeText(text)
    .then(() => {
      alert("Đã sao chép vào clipboard!");
    })
    .catch(err => {
      console.error('Không thể sao chép: ', err);
    });
};

const initializeSignalR = async () => {
  if (!isAuthenticated.value) return;

  connection = new signalR.HubConnectionBuilder()
    .withUrl(
      "https://d2a0-2402-800-63af-bfoe-7981-11d7-6552-5e2c.ngrok-free.app/transactionHub",
      {
        accessTokenFactory: () => "",
      }
    )
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveTransactionStatus", (message: string) => {
    alert(message); 
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
/* Modern payment design */
.recharge-container {
  min-height: 100vh;
  background: #0f1222;
  color: #f0f0f0;
  font-family: 'Segoe UI', 'Roboto', sans-serif;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  position: relative;
}

.gradient-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #0a101f, #171e3c);
  background-size: 400% 400%;
  animation: gradient 15s ease infinite;
  opacity: 0.8;
  z-index: 0;
}

@keyframes gradient {
  0% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
  100% {
    background-position: 0% 50%;
  }
}

.recharge-content {
  padding: 40px 20px;
  max-width: 1000px;
  margin: 0 auto;
  width: 100%;
  position: relative;
  z-index: 1;
}

.recharge-header {
  text-align: center;
  margin-bottom: 30px;
}

.recharge-title {
  font-size: 2.2rem;
  margin-bottom: 10px;
  color: #ffffff;
  text-shadow: 0 0 15px rgba(82, 109, 255, 0.5);
}

.recharge-subtitle {
  font-size: 1.1rem;
  color: #b8c4ff;
}

.username {
  color: #61dafb;
  font-weight: 600;
}

.card-container {
  background: rgba(20, 30, 60, 0.7);
  border-radius: 16px;
  border: 1px solid rgba(79, 102, 255, 0.1);
  padding: 40px; /* Tăng từ 30px lên 40px để giao diện thoáng hơn */
  backdrop-filter: blur(10px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.25);
  transition: all 0.3s ease;
}

.card-header {
  display: flex;
  align-items: center;
  margin-bottom: 25px;
  padding-bottom: 15px;
  border-bottom: 1px solid rgba(79, 102, 255, 0.2);
}
.qr-image-container{
  padding: 10px;
  border: 1px solid white;
}
.card-icon {
  background: linear-gradient(135deg, #4661ff, #6e8cff);
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 15px;
  font-size: 1.2rem;
}

.section-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #ffffff;
  margin: 0;
}

.input-group {
  margin-bottom: 20px;
}

.amount-label {
  display: block;
  margin-bottom: 10px;
  font-size: 1rem;
  color: #c4c9e0;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.currency-prefix {
  position: absolute;
  left: 15px;
  color: #6e8cff;
  font-weight: bold;
  font-size: 1.1rem;
}

.amount-input {
  width: 100%;
  padding: 15px 15px 15px 35px;
  border: 1px solid rgba(79, 102, 255, 0.3);
  border-radius: 8px;
  background: rgba(14, 23, 49, 0.6);
  color: #ffffff;
  font-size: 1.1rem;
  transition: all 0.3s ease;
}

.amount-input:focus {
  outline: none;
  border-color: #4f66ff;
  box-shadow: 0 0 0 3px rgba(79, 102, 255, 0.25);
}

.preset-amounts {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
  gap: 12px;
  margin-bottom: 25px;
}

.preset-btn {
  padding: 12px 10px;
  background: rgba(20, 34, 75, 0.8);
  border: 1px solid rgba(79, 102, 255, 0.2);
  border-radius: 8px;
  color: #c4c9e0;
  font-size: 0.95rem;
  cursor: pointer;
  transition: all 0.2s ease-in-out;
}

.preset-btn:hover {
  background: rgba(32, 52, 115, 0.8);
  border-color: #4f66ff;
  color: #ffffff;
}

.preset-btn.active {
  background: linear-gradient(135deg, #3651d4, #5d76f0);
  border-color: #4f66ff;
  color: #ffffff;
  font-weight: 600;
  box-shadow: 0 4px 12px rgba(79, 102, 255, 0.3);
}

.payment-info {
  margin-bottom: 25px;
  background: rgba(14, 23, 49, 0.6);
  padding: 15px;
  border-radius: 8px;
  border-left: 3px solid #4f66ff;
}

.info-item {
  display: flex;
  align-items: center;
  margin-bottom: 8px;
  color: #b8c4ff;
  font-size: 0.9rem;
}

.info-item:last-child {
  margin-bottom: 0;
}

.info-item i {
  color: #4f66ff;
  margin-right: 10px;
}

.confirm-btn {
  width: 100%;
  padding: 15px;
  background: linear-gradient(135deg, #4661ff, #6e8cff);
  border: none;
  border-radius: 8px;
  color: #ffffff;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.confirm-btn:hover {
  background: linear-gradient(135deg, #3b52d9, #5d76f0);
  box-shadow: 0 4px 12px rgba(79, 102, 255, 0.4);
  transform: translateY(-2px);
}

.confirm-btn:disabled {
  background: #3a3e52;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.auth-warning {
  color: #ff6b6b;
  font-size: 0.95rem;
  margin-top: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

/* Payment details styles */
.payment-details-container {
  animation: fadeIn 0.5s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.payment-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 30px;
  margin-bottom: 30px;
}

@media (max-width: 768px) {
  .payment-grid {
    grid-template-columns: 1fr;
  }
}

.qr-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.qr-image {
  max-width: 200px;
  border: 3px solid #ffffff;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
}

.qr-instruction {
  margin-top: 10px;
  font-size: 0.9rem;
  color: #b8c4ff;
  text-align: center;
}

.bank-details {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.payment-amount {
  background: rgba(14, 23, 49, 0.6);
  padding: 15px;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  margin-bottom: 5px;
}

.amount-label {
  font-size: 0.9rem;
  color: #b8c4ff;
}

.amount-value {
  font-size: 1.6rem;
  font-weight: 700;
  color: #61dafb;
}

.detail-item {
  display: flex;
  align-items: center;
  padding: 12px;
  background: rgba(14, 23, 49, 0.4);
  border-radius: 8px;
  position: relative;
}

.detail-label {
  width: 140px;
  color: #b8c4ff;
  font-size: 0.9rem;
  display: flex;
  align-items: center;
  gap: 8px;
}

.detail-value {
  font-weight: 600;
  color: #ffffff;
}

.copy-btn {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(79, 102, 255, 0.2);
  border: none;
  color: #b8c4ff;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
}

.copy-btn:hover {
  background: rgba(79, 102, 255, 0.4);
  color: #ffffff;
}

.payment-note {
  display: flex;
  gap: 10px;
  background: rgba(255, 107, 107, 0.1);
  padding: 12px;
  border-radius: 8px;
  margin-top: 10px;
  border-left: 3px solid #ff6b6b;
}

.payment-note i {
  color: #ff6b6b;
  font-size: 1.2rem;
}

.payment-note p {
  margin: 0;
  font-size: 0.9rem;
  color: #ffb0b0;
}

.payment-steps {
  display: flex;
  justify-content: space-between;
  margin: 30px 0;
}

@media (max-width: 768px) {
  .payment-steps {
    flex-direction: column;
    gap: 20px;
  }
}

.step {
  display: flex;
  align-items: flex-start;
  gap: 15px;
  flex: 1;
}

.step-number {
  background: linear-gradient(135deg, #4661ff, #6e8cff);
  color: white;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
}

.step-content h3 {
  margin: 0 0 5px 0;
  font-size: 1rem;
  color: #ffffff;
}

.step-content p {
  margin: 0;
  font-size: 0.85rem;
  color: #b8c4ff;
}

.support-info {
  text-align: center;
  margin: 20px 0;
  color: #b8c4ff;
  font-size: 0.9rem;
}

.support-link {
  color: #61dafb;
  text-decoration: none;
  transition: all 0.2s ease;
}

.support-link:hover {
  text-decoration: underline;
  color: #85e6ff;
}

.back-btn {
  padding: 12px 20px;
  background: rgba(14, 23, 49, 0.6);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  color: #ffffff;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease-in-out;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  width: 100%;
  margin-top: 20px;
}

.back-btn:hover {
  background: rgba(20, 34, 75, 0.8);
  border-color: rgba(255, 255, 255, 0.2);
}

.recharge-footer {
  padding: 20px;
  text-align: center;
  background: rgba(10, 16, 31, 0.8);
  color: #a0a9c8;
  z-index: 1;
  font-size: 0.9rem;
}

.login-prompt {
  min-height: 100vh;
  background: linear-gradient(135deg, #0a101f, #171e3c);
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  text-align: center;
  padding: 20px;
}

.login-card {
  background: rgba(20, 30, 60, 0.7);
  border-radius: 16px;
  border: 1px solid rgba(79, 102, 255, 0.1);
  padding: 40px 30px;
  backdrop-filter: blur(10px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.25);
  max-width: 400px;
  width: 100%;
}

.login-icon {
  font-size: 3rem;
  color: #4f66ff;
  margin-bottom: 20px;
}

.login-btn {
  padding: 15px 25px;
  background: linear-gradient(135deg, #4661ff, #6e8cff);
  border: none;
  border-radius: 8px;
  color: #ffffff;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  margin: 20px auto 0;
}

.login-btn:hover {
  background: linear-gradient(135deg, #3b52d9, #5d76f0);
  box-shadow: 0 4px 12px rgba(79, 102, 255, 0.4);
  transform: translateY(-2px);
}
</style>
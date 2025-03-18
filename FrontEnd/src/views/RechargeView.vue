<template>
  <div class="recharge-container">
    <!-- Particle Background -->
    <div class="particle-background">
      <div v-for="i in 15" :key="i" class="particle"></div>
    </div>
    <section class="recharge-content">
      <h1 class="recharge-title">Nạp Tiền GameTradeZone</h1>
      <p class="recharge-subtitle">Nhập số tiền bạn muốn nạp và bấm xác nhận để tiếp tục.</p>

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
        />
        <button
          @click="confirmRecharge"
          class="confirm-btn"
          :disabled="!isValidAmount"
        >
          <i class="fas fa-check"></i> Xác Nhận
        </button>
      </div>

      <!-- Hiển thị mã QR và thông tin ngân hàng -->
      <div v-else class="payment-details">
        <h2 class="method-title">Thông Tin Thanh Toán</h2>
        <div class="qr-section">
          <img :src="qrCodeUrl" alt="Mã QR Thanh Toán" class="qr-image" />
          <p class="qr-instruction">Quét mã QR để chuyển khoản</p>
        </div>
        <div class="bank-details">
          <p><strong>Số Tài Khoản:</strong> {{ bankAccount.number }}</p>
          <p><strong>Tên Chủ Tài Khoản:</strong> {{ bankAccount.holder }}</p>
          <p><strong>Nội Dung Chuyển Khoản:</strong> USERID {{ userId }}</p>
        </div>
        <p class="note-text">
          Vui lòng chuyển khoản theo thông tin trên. Giao dịch sẽ được xử lý trong 1-5 phút.
          Nếu gặp vấn đề, liên hệ: 
          <a href="mailto:support@gametradezone.com" class="note-link">support@gametradezone.com</a>
        </p>
      </div>
    </section>

    <!-- Footer -->
    <footer class="recharge-footer">
      <p class="footer-text">© 2025 GameTradeZone. Tất cả quyền được bảo lưu.</p>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { userStore } from '@/stores/auth';

const store = userStore();
const userId = store.user?.id || JSON.parse(localStorage.getItem('user') || '{}').id;
const rechargeAmount = ref<number>(0);
const showPaymentDetails = ref<boolean>(false);
const qrCodeUrl = ref<string>('');
const bankAccount = {
  number: '123456789', // Thay bằng số tài khoản thực tế
  holder: 'GameTradeZone Corp' // Thay bằng tên chủ tài khoản thực tế
};

const isValidAmount = computed(() => rechargeAmount.value >= 10000);

const generateQRCode = () => {
  const qrData = `https://qr.sepay.vn/img?acc=96247XAAD6&bank=BIDV&amount=${rechargeAmount.value}&des=USERID%20${userId}`;
  qrCodeUrl.value = qrData;
};

const confirmRecharge = () => {
  if (isValidAmount.value) {
    generateQRCode();
    showPaymentDetails.value = true;
  }
};
</script>

<style scoped>
/* Tổng thể */
.recharge-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
  font-family: 'Arial', sans-serif;
  color: #f0f0f0;
  position: relative;
  overflow: hidden;
}

/* Particle Background */
.particle-background {
  position: absolute;
  inset: 0;
  z-index: -1;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(0, 179, 224, 0.5);
  border-radius: 50%;
  animation: float 10s infinite ease-in-out;
}

.particle:nth-child(odd) {
  background: rgba(255, 0, 255, 0.5);
}

@keyframes float {
  0% { transform: translateY(0) scale(1); opacity: 0.8; }
  50% { transform: translateY(-100vh) scale(1.3); opacity: 0.3; }
  100% { transform: translateY(0) scale(1); opacity: 0.8; }
}

/* Main Content */
.recharge-content {
  max-width: 700px;
  margin: 0 auto;
  padding: 40px 20px;
  position: relative;
  z-index: 1;
}

/* Titles */
.recharge-title {
  font-size: 2.8rem;
  font-weight: 800;
  color: #f8f8f8;
  text-align: center;
  margin-bottom: 15px;
  text-shadow: 0 0 15px #00b3e0, 0 0 5px #ff00ff;
  transition: all 0.3s ease-in-out;
}

.recharge-subtitle {
  font-size: 1.2rem;
  color: #e0e0e0;
  text-align: center;
  margin-bottom: 25px;
  text-shadow: 0 0 4px rgba(255, 255, 255, 0.3);
}

/* Amount Section */
.amount-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
  background: rgba(28, 37, 38, 0.9);
  padding: 25px;
  border: 1px solid #00b3e0;
  border-radius: 8px;
  box-shadow: 0 5px 15px rgba(0, 204, 255, 0.2);
}

.amount-label {
  font-size: 1.2rem;
  color: #00b3e0;
  text-shadow: 0 0 3px #00b3e0;
}

.amount-input {
  width: 100%;
  max-width: 300px;
  padding: 10px;
  background: rgba(28, 37, 38, 0.9);
  border: 1px solid #00b3e0;
  border-radius: 8px;
  color: #f0f0f0;
  font-size: 1.1rem;
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.1);
  transition: all 0.2s ease-in-out;
}

.amount-input:focus {
  box-shadow: 0 0 10px rgba(0, 204, 255, 0.3);
  outline: none;
  border-color: #ff00ff;
}

/* Payment Details */
.payment-details {
  background: rgba(28, 37, 38, 0.95);
  border: 1px solid #00b3e0;
  border-radius: 8px;
  padding: 25px;
  box-shadow: 0 5px 15px rgba(0, 204, 255, 0.2);
  margin-bottom: 15px;
}

.method-title {
  font-size: 1.8rem;
  font-weight: 700;
  color: #00b3e0;
  margin-bottom: 15px;
  text-shadow: 0 0 10px #00b3e0, 0 0 4px #ff00ff;
}

.qr-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  margin-bottom: 20px;
}

.qr-image {
  width: 200px;
  height: 200px;
  border: 1px solid #00b3e0;
  border-radius: 8px;
  box-shadow: 0 0 8px rgba(0, 204, 255, 0.1);
}

.qr-instruction {
  font-size: 1rem;
  color: #e0e0e0;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.2);
}

.bank-details {
  background: rgba(28, 37, 38, 0.7);
  padding: 15px;
  border-radius: 8px;
  box-shadow: 0 3px 10px rgba(0, 204, 255, 0.1);
  margin-bottom: 15px;
}

.bank-details p {
  font-size: 1.1rem;
  color: #e0e0e0;
  margin: 5px 0;
}

.bank-details strong {
  color: #00b3e0;
}

/* Confirm Button */
.confirm-btn {
  width: 100%;
  max-width: 300px;
  padding: 10px;
  background: rgba(28, 37, 38, 0.9);
  color: #f0f0f0;
  border: 1px solid #00b3e0;
  border-radius: 8px;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease-in-out;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.2);
  box-shadow: 0 3px 10px rgba(0, 204, 255, 0.1);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.confirm-btn:disabled {
  background: rgba(28, 37, 38, 0.5);
  color: #808080;
  border-color: #808080;
  cursor: not-allowed;
  box-shadow: none;
}

.confirm-btn:hover:not(:disabled) {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  box-shadow: 0 5px 15px rgba(0, 204, 255, 0.3);
  transform: translateY(-2px);
}

/* Note Text */
.note-text {
  font-size: 0.9rem;
  color: #b0b0b0;
  margin-top: 10px;
  text-align: center;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.2);
}

.note-link {
  color: #00b3e0;
  text-decoration: underline;
  transition: all 0.2s ease-in-out;
}

.note-link:hover {
  color: #ff00ff;
  text-shadow: 0 0 6px #ff00ff;
}

/* Footer */
.recharge-footer {
  margin-top: 20px;
  text-align: center;
  padding: 15px;
  background: rgba(13, 27, 42, 0.9);
  border-radius: 8px;
  box-shadow: 0 3px 10px rgba(0, 204, 255, 0.1);
}

.footer-text {
  font-size: 1rem;
  color: #b0b0b0;
  margin: 5px 0;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.2);
}

/* Responsive */
@media (max-width: 768px) {
  .recharge-content {
    padding: 20px 15px;
  }

  .recharge-title {
    font-size: 2rem;
  }

  .recharge-subtitle {
    font-size: 1rem;
  }

  .amount-section,
  .payment-details {
    padding: 15px;
  }

  .qr-image {
    width: 150px;
    height: 150px;
  }

  .confirm-btn {
    padding: 8px;
    font-size: 1rem;
  }

  .note-text {
    font-size: 0.8rem;
  }

  .footer-text {
    font-size: 0.9rem;
  }
}
</style>
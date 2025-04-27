<template>
  <div class="registration-container">
    <div class="registration-card">
      <div class="card-header">
        <h2 class="reg-title">Đăng Ký GTZ</h2>
        <p class="reg-subtitle">Tham gia ngay để trao đổi tài khoản game!</p>
      </div>

      <form @submit.prevent="registerUser" class="reg-form">
        <div class="form-group">
          <div class="input-group">
            <i class="fas fa-user input-icon"></i>
            <input
              type="text"
              id="accountName"
              v-model="accountName"
              placeholder="Tên đăng nhập"
              required
            />
          </div>

          <div class="input-group">
            <i class="fas fa-envelope input-icon"></i>
            <input
              type="email"
              id="email"
              v-model="email"
              placeholder="Email"
              required
            />
          </div>

          <div class="input-group">
            <i class="fas fa-id-card input-icon"></i>
            <input
              type="text"
              id="fullName"
              v-model="fullName"
              placeholder="Họ và tên"
              required
            />
          </div>

          <div class="input-group">
            <i class="fas fa-lock input-icon"></i>
            <input
              type="password"
              id="password"
              v-model="password"
              placeholder="Mật khẩu"
              required
            />
            <p v-if="passwordWarning" class="warning-text">{{ passwordWarning }}</p>
          </div>

          <div class="input-group">
            <i class="fas fa-lock input-icon"></i>
            <input
              type="password"
              id="confirmPassword"
              v-model="confirmPassword"
              placeholder="Xác nhận mật khẩu"
              required
            />
          </div>

          <div class="input-group bank-select-group">
            <i class="fas fa-university input-icon"></i>
            <select v-model="bankName" class="bank-select" required>
              <option value="" disabled selected>Chọn ngân hàng</option>
              <option v-for="bank in banks" :key="bank.id" :value="bank.name">
                {{ bank.name }}
              </option>
            </select>
          </div>

          <div class="input-group">
            <i class="fas fa-credit-card input-icon"></i>
            <input
              type="text"
              id="bankNumber"
              v-model="bankNumber"
              placeholder="Số tài khoản ngân hàng"
              required
            />
          </div>
        </div>

        <button type="submit" class="submit-btn">Đăng Ký</button>
      </form>

      <p class="terms">
        Bằng cách đăng ký, bạn đồng ý với <a href="/policy">Điều khoản sử dụng</a>,
        <a href="#">Chính sách bảo mật</a> và <a href="#">Chính sách cookie</a> của GTZ.
      </p>

      <transition name="fade">
        <p v-if="errorMessage" class="error-notification">{{ errorMessage }}</p>
      </transition>

      <!-- Modal thông báo đăng ký thành công -->
      <transition name="fade">
        <div v-if="showSuccessModal" class="modal-overlay">
          <div class="modal-content">
            <i class="fas fa-check-circle modal-icon"></i>
            <p>{{ successMessage }}</p>
            <button @click="closeModalAndRedirect" class="modal-close-btn">Đóng</button>
          </div>
        </div>
      </transition>
    </div>
  </div>
</template>

<script>
import api from '@/api/base.api';
import DOMPurify from 'dompurify';

export default {
  name: "RegistrationForm",
  data() {
    return {
      accountName: "",
      email: "",
      fullName: "",
      password: "",
      confirmPassword: "",
      bankName: "",
      bankNumber: "",
      errorMessage: "",
      successMessage: "",
      passwordWarning: "",
      showSuccessModal: false,
      banks: [
        { id: 1, name: "Vietcombank" },
        { id: 2, name: "Techcombank" },
        { id: 3, name: "MB Bank" },
        { id: 4, name: "Agribank" },
        { id: 5, name: "TPBank" },
        { id: 6, name: "Sacombank" },
        { id: 7, name: "BIDV" },
        { id: 8, name: "VPBank" },
      ],
    };
  },
  methods: {
    async registerUser() {
      this.errorMessage = "";
      this.successMessage = "";
      this.passwordWarning = "";
      this.showSuccessModal = false;

      if (!this.isPasswordValid(this.password)) {
        this.passwordWarning = "Mật khẩu phải bao gồm ký tự in hoa, số và ký tự đặc biệt";
        return;
      }

      const sanitizedAccountName = DOMPurify.sanitize(this.accountName);
      const sanitizedEmail = DOMPurify.sanitize(this.email);
      const sanitizedFullName = DOMPurify.sanitize(this.fullName);
      const sanitizedPassword = DOMPurify.sanitize(this.password);
      const sanitizedConfirmPassword = DOMPurify.sanitize(this.confirmPassword);
      const sanitizedBankName = DOMPurify.sanitize(this.bankName);
      const sanitizedBankNumber = DOMPurify.sanitize(this.bankNumber);

      if (sanitizedPassword !== sanitizedConfirmPassword) {
        this.errorMessage = "Mật khẩu không khớp";
        return;
      }

      try {
        const response = await api.register(
          sanitizedAccountName,
          sanitizedPassword,
          sanitizedFullName,
          sanitizedEmail,
          sanitizedBankName,
          sanitizedBankNumber
        );

        this.successMessage = "Đăng ký thành công!";
        this.showSuccessModal = true;
        this.clearForm();
      } catch (error) {
        const errorMsg = error?.response?.data?.result?.message ||
                        error?.response?.data?.message ||
                        error.message ||
                        "Có lỗi xảy ra. Vui lòng thử lại.";
        this.errorMessage = errorMsg;
      }
    },

    isPasswordValid(password) {
      const regex = /^(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[A-Z])[A-Za-z0-9!@#$%^&*]{8,}$/;
      return regex.test(password);
    },

    clearForm() {
      this.accountName = "";
      this.email = "";
      this.fullName = "";
      this.password = "";
      this.confirmPassword = "";
      this.bankName = "";
      this.bankNumber = "";
    },

    closeModalAndRedirect() {
      this.showSuccessModal = false;
      this.$router.push("/login");
    },
  },
};
</script>

<style scoped>
.registration-container {
  min-height: 120vh;
  display: flex;
  justify-content: center;
  align-items: center;
  position: relative;
  overflow: hidden;
}

.registration-container::before {
  content: '';
  position: absolute;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(0, 255, 255, 0.15), transparent 70%);
  animation: glowShift 8s infinite ease-in-out;
  z-index: 0;
}

@keyframes glowShift {
  0%, 100% { transform: translate(0, 0) scale(1); opacity: 0.6; }
  50% { transform: translate(20px, -20px) scale(1.1); opacity: 0.9; }
}

.registration-card {
  background: rgba(20, 20, 40, 0.9);
  border: 1px solid rgba(0, 255, 255, 0.3);
  padding: 30px;
  width: 100%;
  max-width: 400px; /* Giảm chiều rộng để phù hợp với bố cục dọc */
  box-shadow: 0 10px 40px rgba(0, 255, 255, 0.1);
  position: relative;
  z-index: 1;
}

.card-header {
  text-align: center;
  margin-bottom: 20px;
}

.reg-title {
  font-size: 2rem;
  font-weight: 800;
  color: #00ffff;
  text-transform: uppercase;
  letter-spacing: 2px;
  text-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.reg-subtitle {
  font-size: 0.9rem;
  color: #a0a0a0;
  margin-top: 8px;
}

.reg-form {
  display: flex;
  flex-direction: column;
  gap: 25px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 15px; /* Giãn cách giữa các trường nhập liệu */
}

.input-group {
  position: relative;
}

.input-icon {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #00ffff;
  font-size: 1.1rem;
}

input,
.bank-select {
  width: 100%;
  padding: 12px 12px 12px 40px;
  background: rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(0, 255, 255, 0.4);
  color: #ffffff;
  font-size: 0.95rem;
  outline: none;
  transition: all 0.3s ease;
  border-radius: 0;
}

input:focus,
.bank-select:focus {
  border-color: #00ffff;
  box-shadow: 0 0 8px rgba(0, 255, 255, 0.5);
}

.bank-select-group {
  position: relative;
  display: flex;
  align-items: center;
}

.bank-select {
  flex-grow: 1;
  background: rgba(0, 0, 0, 0.5);
  color: #ffffff;
  cursor: pointer;
}

.bank-select option {
  background: #1c1c3a;
  color: #ffffff;
}

.warning-text {
  color: #ffcc00;
  font-size: 0.85rem;
  margin-top: 5px;
  text-align: left;
}

.submit-btn {
  width: 100%;
  padding: 12px;
  background: linear-gradient(135deg, #00ffff, #00b7b7);
  color: #ffffff;
  border: none;
  border-radius: 5px;
  font-size: 1.1rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(0, 255, 255, 0.3);
}

.submit-btn:hover {
  background: linear-gradient(135deg, #00b7b7, #00ffff);
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(0, 255, 255, 0.5);
}

.terms {
  font-size: 0.85rem;
  color: #a0a0a0;
  margin-top: 15px;
  text-align: center;
}

.terms a {
  color: #00ffff;
  text-decoration: none;
  transition: all 0.3s ease;
}

.terms a:hover {
  color: #66ffff;
  text-shadow: 0 0 5px rgba(0, 255, 255, 0.5);
}

.error-notification {
  background: rgba(255, 75, 75, 0.9);
  color: #ffffff;
  padding: 8px 15px;
  border-radius: 5px;
  font-size: 0.9rem;
  box-shadow: 0 4px 15px rgba(255, 75, 75, 0.3);
  margin-top: 15px;
  text-align: center;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 100;
}

.modal-content {
  background: rgba(20, 20, 40, 0.9);
  padding: 15px 25px;
  border-radius: 10px;
  text-align: center;
  border: 1px solid rgba(0, 255, 255, 0.3);
  color: #ffffff;
  width: 100%;
  max-width: 300px;
}

.modal-icon {
  font-size: 2.5rem;
  color: #00ff00;
  margin-bottom: 8px;
}

.modal-content p {
  font-size: 1rem;
  margin: 0 0 10px 0;
}

.modal-close-btn {
  padding: 6px 15px;
  background: linear-gradient(135deg, #00ffff, #00b7b7);
  color: #ffffff;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 0.9rem;
}

.modal-close-btn:hover {
  background: linear-gradient(135deg, #00b7b7, #00ffff);
  box-shadow: 0 4px 15px rgba(0, 255, 255, 0.5);
}

.fade-enter-active,
.fade-leave-active {
  transition: all 0.5s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

@media (max-width: 768px) {
  .registration-card {
    padding: 20px;
    max-width: 350px; /* Giảm thêm trên mobile */
  }
  .reg-title {
    font-size: 1.6rem;
  }
  .reg-subtitle {
    font-size: 0.8rem;
  }
  .submit-btn {
    padding: 10px;
  }
  .error-notification {
    font-size: 0.8rem;
  }
  .modal-content {
    max-width: 250px;
  }
}
</style>
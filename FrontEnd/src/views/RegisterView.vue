<template>
  <div class="registration-container">
    <div class="registration-card">
      <div class="card-header">
        <h2 class="reg-title">Đăng Ký GTZ</h2>
        <p class="reg-subtitle">Tham gia thị trường tài khoản game ngay!</p>
      </div>

      <form @submit.prevent="registerUser" class="reg-form">
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

        <button type="submit" class="submit-btn">Đăng Ký</button>
      </form>

      <p class="terms">
        Bằng cách đăng ký, bạn đồng ý với <a href="/policy">Điều khoản sử dụng</a>,
        <a href="#">Chính sách bảo mật</a> và <a href="#">Chính sách cookie</a> của GTZ.
      </p>

      <transition name="fade">
        <p v-if="errorMessage" class="error-notification">{{ errorMessage }}</p>
      </transition>
      <transition name="fade">
        <p v-if="successMessage" class="success-notification">{{ successMessage }}</p>
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
      errorMessage: "",
      successMessage: "",
      passwordWarning: "",
    };
  },
  methods: {
    async registerUser() {
      this.errorMessage = "";
      this.successMessage = "";
      this.passwordWarning = "";

      if (!this.isPasswordValid(this.password)) {
        this.passwordWarning = "Mật khẩu phải có chữ in hoa, số và ký tự đặc biệt.";
        return;
      }

      const sanitizedAccountName = DOMPurify.sanitize(this.accountName);
      const sanitizedEmail = DOMPurify.sanitize(this.email);
      const sanitizedFullName = DOMPurify.sanitize(this.fullName);
      const sanitizedPassword = DOMPurify.sanitize(this.password);
      const sanitizedConfirmPassword = DOMPurify.sanitize(this.confirmPassword);

      if (sanitizedPassword !== sanitizedConfirmPassword) {
        this.errorMessage = "Mật khẩu không khớp.";
        return;
      }

      try {
        const response = await api.register(
          sanitizedAccountName,
          sanitizedPassword,
          sanitizedFullName,
          sanitizedEmail
        );
        this.successMessage = "Đăng ký thành công!";
        this.clearForm();
      } catch (error) {
        if (error.response) {
          this.errorMessage = error.response.data.message || "Đăng ký thất bại. Vui lòng thử lại.";
        } else {
          this.errorMessage = "Có lỗi xảy ra. Vui lòng thử lại.";
        }
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
    }
  }
};
</script>

<style scoped>
/* Tổng thể */
.registration-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #1e1e2f 0%, #2a2a40 100%);
  position: relative;
  overflow: hidden;
}

.registration-container::before {
  content: '';
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle, rgba(0, 221, 235, 0.1), transparent);
  animation: pulseGlow 10s infinite;
  z-index: 0;
}

@keyframes pulseGlow {
  0%, 100% { transform: scale(1); opacity: 0.5; }
  50% { transform: scale(1.2); opacity: 0.8; }
}

/* Registration Card */
.registration-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(15px);
  border-radius: 16px;
  padding: 25px;
  width: 100%;
  max-width: 360px;
  box-shadow: 0 15px 30px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(0, 221, 235, 0.2);
  position: relative;
  z-index: 1;
  overflow: hidden;
}

/* Card Header */
.card-header {
  text-align: center;
  margin-bottom: 15px;
}

.reg-title {
  font-size: 1.6rem;
  font-weight: 700;
  color: #00ddeb;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  text-shadow: 0 0 8px rgba(0, 221, 235, 0.5);
}

.reg-subtitle {
  font-size: 0.85rem;
  color: #b0b0b0;
  margin-top: 6px;
}

/* Form */
.reg-form {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.input-group {
  position: relative;
}

.input-icon {
  position: absolute;
  left: 15px; /* Tăng từ 12px để có khoảng cách */
  top: 50%;
  transform: translateY(-50%);
  color: #00ddeb;
  font-size: 1rem;
}

input {
  width: 100%;
  padding: 10px 15px 10px 40px; /* Tăng padding-left từ 30px lên 40px để chữ cách icon */
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(0, 221, 235, 0.3);
  color: #ffffff;
  font-size: 0.9rem;
  outline: none;
  transition: all 0.3s ease;
}

input:focus {
  border-color: #00ddeb;
  box-shadow: 0 0 12px rgba(0, 221, 235, 0.5);
}

.warning-text {
  color: #ffcc00;
  font-size: 0.8rem;
  margin-top: 5px;
  text-align: left;
}

/* Submit Button */
.submit-btn {
  width: 100%;
  padding: 10px;
  background: #00ddeb;
  color: #1e1e2f;
  border: none;
  border-radius: 10px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.4s ease;
  box-shadow: 0 4px 12px rgba(0, 221, 235, 0.4);
}

.submit-btn:hover {
  background: #33e6f2;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 221, 235, 0.6);
}

/* Terms */
.terms {
  font-size: 0.8rem;
  color: #b0b0b0;
  margin-top: 10px;
  text-align: center;
}

.terms a {
  color: #00ddeb;
  text-decoration: none;
  transition: all 0.3s ease;
}

.terms a:hover {
  color: #33e6f2;
  text-shadow: 0 0 4px rgba(0, 221, 235, 0.5);
}

/* Notifications */
.error-notification {
  background: rgba(255, 75, 75, 0.9);
  color: #ffffff;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 0.8rem;
  box-shadow: 0 4px 12px rgba(255, 75, 75, 0.4);
  margin-top: 10px;
  text-align: center;
}

.success-notification {
  background: rgba(75, 255, 75, 0.9);
  color: #1e1e2f;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 0.8rem;
  box-shadow: 0 4px 12px rgba(75, 255, 75, 0.4);
  margin-top: 10px;
  text-align: center;
}

/* Transition cho thông báo */
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
  .registration-card {
    padding: 20px;
    max-width: 300px;
  }
  .reg-title {
    font-size: 1.4rem;
  }
  .reg-subtitle {
    font-size: 0.75rem;
  }
  .submit-btn {
    padding: 8px;
  }
  .error-notification,
  .success-notification {
    font-size: 0.7rem;
  }
}
</style>
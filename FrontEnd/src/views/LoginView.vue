<template>
  <LoadingSpinner :isLoading="loading" />
  <div class="login-container">
    <div class="login-card">
      <div class="card-header">
        <h3 class="login-title">Đăng Nhập</h3>
        <p class="login-subtitle">Tham gia thị trường tài khoản game ngay!</p>
      </div>
      <form @submit.prevent="handleLogin" class="login-form">
        <div class="input-group">
          <i class="fas fa-user input-icon"></i>
          <input placeholder="Tên tài khoản" v-model="userName" required />
        </div>
        <div class="input-group">
          <i class="fas fa-lock input-icon"></i>
          <input
            :type="showPassword ? 'text' : 'password'"
            placeholder="Mật khẩu"
            v-model="password"
            required
          />
          <i
            :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"
            class="toggle-password"
            @click="togglePassword"
          ></i>
        </div>
        <div class="forgot-password">
          <a href="#">Quên mật khẩu?</a>
        </div>
        <button type="submit" class="sign-in-btn">Đăng Nhập</button>
        <div class="or-container">
          <span>hoặc dùng</span>
        </div>
        <div class="social-buttons">
          <button type="button" class="social-btn google-btn">
            <i class="fab fa-google"></i>
          </button>
          <button type="button" class="social-btn github-btn">
            <i class="fab fa-github"></i>
          </button>
          <button type="button" class="social-btn facebook-btn">
            <i class="fab fa-facebook-f"></i>
          </button>
        </div>
        <div class="register-link">
          <span>Chưa có tài khoản?</span> <a href="/register">Đăng ký ngay</a>
        </div>
      </form>
      <!-- Thông báo lỗi -->
      <transition name="fade">
        <div v-if="errorMessage" class="error-notification">
          {{ errorMessage }}
        </div>
      </transition>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import Cookies from "js-cookie";
import authApi from "@/api/authenticate.api";
import { userStore } from "../stores/auth";
import DOMPurify from 'dompurify';

export default defineComponent({
  setup() {
    const user = userStore();
    const userName = ref("");
    const password = ref("");   
    const router = useRouter();
    const loading = ref(true);
    const errorMessage = ref<string | null>(null);
    const showPassword = ref(false);

    const fetchData = async () => {
      loading.value = true;
      await new Promise((resolve) => setTimeout(resolve, 2000));
      loading.value = false;
    };

    const handleLogin = async () => {
      if (userName.value && password.value) {
        try {
          const sanitizedUserName = DOMPurify.sanitize(userName.value);
          const sanitizedPassword = DOMPurify.sanitize(password.value);

          const loginModel = {
            userName: sanitizedUserName,
            password: sanitizedPassword,
          };

          const response = await authApi.login(loginModel);
          if (response && response.result?.isSuccess) {
            user.login({
              id: response.result.data.userId,
              userName: response.result.data.username,
              email: response.result.data.email,
              fullName: response.result.data.fullName,
              balance: response.result.data.balance,
              coin: response.result.data.coin,
              level: 0,
              status: false,
              experience: 0
            });
            Cookies.set("token", response.result.data.token);
            localStorage.setItem('token', response.result.data.token);
            router.push("/");
            setTimeout(() => {
              window.location.reload();
            }, 100);
          } else {
            errorMessage.value = "Tài khoản hoặc mật khẩu không đúng.";
            setTimeout(() => (errorMessage.value = null), 3000);
          }
        } catch (error) {
          console.error("Error during login:", error);
          errorMessage.value = "Không thể kết nối đến API.";
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } else {
        errorMessage.value = "Vui lòng nhập đầy đủ tài khoản và mật khẩu.";
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const togglePassword = () => {
      showPassword.value = !showPassword.value;
    };

    onMounted(async () => {
      fetchData();
    });

    return {
      userName,
      password,
      handleLogin,
      loading,
      errorMessage,
      showPassword,
      togglePassword,
    };
  },
});
</script>

<style scoped>
/* Tổng thể */
.login-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #1e1e2f 0%, #2a2a40 100%);
  position: relative;
  overflow: hidden;
}

.login-container::before {
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

/* Login Card - Thu nhỏ và loại bỏ scroll */
.login-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(15px);
  border-radius: 16px;
  padding: 25px; /* Giảm thêm từ 30px */
  width: 100%;
  max-width: 360px;
  box-shadow: 0 15px 30px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(0, 221, 235, 0.2);
  position: relative;
  z-index: 1;
  overflow: hidden; /* Ngăn scroll trong card */
}

/* Card Header - Thu nhỏ */
.card-header {
  text-align: center;
  margin-bottom: 15px; /* Giảm từ 20px */
}

.login-title {
  font-size: 1.6rem;
  font-weight: 700;
  color: #00ddeb;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  text-shadow: 0 0 8px rgba(0, 221, 235, 0.5);
}

.login-subtitle {
  font-size: 0.85rem;
  color: #b0b0b0;
  margin-top: 6px; /* Giảm từ 8px */
}

/* Form - Thu nhỏ và tối ưu chiều cao */
.login-form {
  display: flex;
  flex-direction: column;
  gap: 12px; /* Giảm từ 15px */
}

.input-group {
  position: relative;
}

.input-icon {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #00ddeb;
  font-size: 1rem;
}

input {
  width: 100%;
  padding: 10px 30px 10px 30px; /* Giảm từ 12px 35px */
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

.toggle-password {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #00ddeb;
  font-size: 1rem;
  cursor: pointer;
  transition: color 0.3s ease;
}

.toggle-password:hover {
  color: #33e6f2;
}

.forgot-password {
  text-align: right;
  margin: 5px 0; /* Thêm margin nhỏ để gọn */
}

.forgot-password a {
  color: #ff007a;
  font-size: 0.8rem;
  text-decoration: none;
  transition: all 0.3s ease;
}

.forgot-password a:hover {
  color: #ff3399;
  text-shadow: 0 0 4px rgba(255, 0, 122, 0.5);
}

/* Button Đăng Nhập - Thu nhỏ */
.sign-in-btn {
  width: 100%;
  padding: 10px; /* Giảm từ 12px */
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

.sign-in-btn:hover {
  background: #33e6f2;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 221, 235, 0.6);
}

/* Or Container - Thu nhỏ */
.or-container {
  display: flex;
  justify-content: center;
  align-items: center;
  color: #b0b0b0;
  font-size: 0.8rem;
  position: relative;
  margin: 5px 0; /* Thêm margin nhỏ để gọn */
}

.or-container::before,
.or-container::after {
  content: '';
  flex: 1;
  height: 1px;
  background: rgba(255, 255, 255, 0.2);
  margin: 0 8px; /* Giảm từ 10px */
}

/* Social Buttons - Thu nhỏ */
.social-buttons {
  display: flex;
  justify-content: center;
  gap: 10px; /* Giảm từ 12px */
}

.social-btn {
  width: 36px; /* Giảm từ 40px */
  height: 36px; /* Giảm từ 40px */
  border-radius: 50%;
  border: 1px solid rgba(255, 255, 255, 0.2);
  background: rgba(255, 255, 255, 0.05);
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 1.1rem; /* Giảm từ 1.2rem */
  cursor: pointer;
  transition: all 0.3s ease;
}

.google-btn { color: #db4437; }
.google-btn:hover { background: #db4437; color: #ffffff; border-color: #db4437; }

.github-btn { color: #ffffff; }
.github-btn:hover { background: #333; border-color: #333; }

.facebook-btn { color: #3b5998; }
.facebook-btn:hover { background: #3b5998; color: #ffffff; border-color: #3b5998; }

/* Register Link - Thu nhỏ */
.register-link {
  text-align: center;
  font-size: 0.8rem;
  color: #b0b0b0;
  margin-top: 5px; /* Thêm margin nhỏ để gọn */
}

.register-link a {
  color: #00ddeb;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.3s ease;
}

.register-link a:hover {
  color: #33e6f2;
  text-shadow: 0 0 4px rgba(0, 221, 235, 0.5);
}

/* Error Notification - Thu nhỏ */
.error-notification {
  position: absolute;
  bottom: 10px; /* Giảm từ 15px */
  left: 50%;
  transform: translateX(-50%);
  background: rgba(255, 75, 75, 0.9);
  color: #ffffff;
  padding: 6px 12px; /* Giảm từ 8px 15px */
  border-radius: 6px;
  font-size: 0.8rem; /* Giảm từ 0.85rem */
  box-shadow: 0 4px 12px rgba(255, 75, 75, 0.4);
  z-index: 2;
}

/* Transition cho thông báo */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.5s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateX(-50%) translateY(10px); /* Giảm từ 15px */
}

/* Responsive - Thu nhỏ */
@media (max-width: 768px) {
  .login-card { padding: 20px; max-width: 300px; } /* Giảm từ 25px */
  .login-title { font-size: 1.4rem; }
  .login-subtitle { font-size: 0.75rem; } /* Giảm từ 0.8rem */
  .sign-in-btn { padding: 8px; } /* Giảm từ 10px */
  .error-notification { width: 90%; font-size: 0.7rem; } /* Giảm từ 0.75rem */
}
</style>
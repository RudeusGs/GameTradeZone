<template>
  <div class="login-container">
    <!-- Component LoadingSpinner hiển thị khi đang tải -->
    <LoadingSpinner v-if="loading" class="spinner-overlay" />

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
        <button type="submit" class="sign-in-btn" :disabled="loading">
          Đăng Nhập
        </button>
        <div class="or-container">
          <span>hoặc dùng</span>
        </div>
        <div class="social-buttons">
          <button
            type="button"
            class="social-btn google-btn"
            @click="handleGoogleLogin"
          >
            <i class="fab fa-google"></i>
          </button>
          <button type="button" class="social-btn github-btn" disabled>
            <i class="fab fa-github"></i>
          </button>
          <button type="button" class="social-btn facebook-btn" disabled>
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

    <!-- Modal nhập thông tin bổ sung sau Google OAuth -->
    <UserInfoModal 
      v-if="showInfoModal" 
      :modalData="modalData" 
      :modalError="modalError" 
      :loading="loading" 
      :banks="banks" 
      @submit="submitUserInfo" 
      @close="showInfoModal = false"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, reactive } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import Cookies from 'js-cookie';
import authApi from '@/api/authenticate.api';
import baseApi from '@/api/base.api';
import { userStore } from '../stores/auth';
import DOMPurify from 'dompurify';
import UserInfoModal from '@/components/UserInforModal.vue';

export default defineComponent({
  name: 'Login',
  components: {
    UserInfoModal
  },
  setup() {
    const user = userStore();
    const userName = ref('');
    const password = ref('');
    const router = useRouter();
    const route = useRoute();
    const loading = ref(false);
    const errorMessage = ref<string | null>(null);
    const showPassword = ref(false);
    const showInfoModal = ref(false);
    const modalError = ref<string | null>(null);

    // Dữ liệu cho modal
    const modalData = reactive({
      accountName: '',
      email: '',
      fullName: '',
      bankName: '',
      bankNumber: '',
      token: '',
    });

    // Danh sách ngân hàng
    const banks = [
      { id: 1, name: 'Vietcombank' },
      { id: 2, name: 'Techcombank' },
      { id: 3, name: 'MB Bank' },
      { id: 4, name: 'Agribank' },
      { id: 5, name: 'TPBank' },
      { id: 6, name: 'Sacombank' },
      { id: 7, name: 'BIDV' },
      { id: 8, name: 'VPBank' },
    ];

    // Xử lý đăng nhập thông thường
    const handleLogin = async () => {
      if (userName.value && password.value) {
        loading.value = true;
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
              experience: 0,
              bankName: response.result.data.bankname,
              bankNumber: response.result.data.banknumber,
            });
            Cookies.set('token', response.result.data.token);
            localStorage.setItem('token', response.result.data.token);
            router.push('/');
            setTimeout(() => {
              window.location.reload();
            }, 100);
          } else {
            errorMessage.value = 'Tài khoản hoặc mật khẩu không đúng.';
            setTimeout(() => (errorMessage.value = null), 3000);
          }
        } catch (error) {
          console.error('Error during login:', error);
          errorMessage.value = 'Không thể kết nối đến API.';
          setTimeout(() => (errorMessage.value = null), 3000);
        } finally {
          loading.value = false;
        }
      } else {
        errorMessage.value = 'Vui lòng nhập đầy đủ tài khoản và mật khẩu.';
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    // Xử lý đăng nhập bằng Google
    const handleGoogleLogin = () => {
      try {
        const googleLoginUrl = authApi.externalLogin('Google');
        window.location.href = googleLoginUrl;
      } catch (error) {
        console.error('Error initiating Google login:', error);
        errorMessage.value = 'Không thể khởi động đăng nhập Google.';
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    // Xử lý callback sau Google OAuth
    const handleOAuthCallback = async () => {
      if (route.path === '/callback') {
        loading.value = true;
        try {
          const token = route.query.token as string;
          const email = route.query.email as string; // Lấy email từ query
          const error = route.query.error as string;

          if (error) {
            errorMessage.value = error;
            setTimeout(() => (errorMessage.value = null), 3000);
            router.push('/login');
            return;
          }

          if (token) {
            modalData.email = email || 'example@google.com';
            modalData.token = token;
            showInfoModal.value = true;
          }
        } catch (error) {
          console.error('Error handling OAuth callback:', error);
          errorMessage.value = 'Lỗi khi xử lý đăng nhập Google.';
          setTimeout(() => (errorMessage.value = null), 3000);
          router.push('/login');
        } finally {
          loading.value = false;
        }
      }
    };

    // Xử lý submit thông tin từ modal
    const submitUserInfo = async () => {
      modalError.value = null;
      loading.value = true;

      try {
        const sanitizedAccountName = DOMPurify.sanitize(modalData.accountName);
        const sanitizedEmail = DOMPurify.sanitize(modalData.email);
        const sanitizedFullName = DOMPurify.sanitize(modalData.fullName);
        const sanitizedBankName = DOMPurify.sanitize(modalData.bankName);
        const sanitizedBankNumber = DOMPurify.sanitize(modalData.bankNumber);

        // Gọi API update-user
        const response = await baseApi.updateUser(
          modalData.token,
          sanitizedAccountName,
          sanitizedFullName,
          sanitizedEmail,
          sanitizedBankName,
          sanitizedBankNumber
        );

        if (response.status === 200) {
          user.login({
            id: response.data.userId || 0,
            userName: sanitizedAccountName,
            email: sanitizedEmail,
            fullName: sanitizedFullName,
            balance: response.data.balance || 0,
            coin: response.data.coin || 0,
            level: 0,
            status: false,
            experience: 0,
            bankName: sanitizedBankName,
            bankNumber: sanitizedBankNumber,
          });
          Cookies.set('token', modalData.token);
          localStorage.setItem('token', modalData.token);
          showInfoModal.value = false;
          router.push('/');
          setTimeout(() => {
            window.location.reload();
          }, 100);
        } else {
          modalError.value = response.data?.message || 'Không thể lưu thông tin.';
        }
      } catch (error) {
        console.error('Error saving user info:', error);
        modalError.value = 'Lỗi khi lưu thông tin. Vui lòng thử lại.';
      } finally {
        loading.value = false;
      }
    };

    // Gọi handleOAuthCallback khi component được mount
    handleOAuthCallback();

    const togglePassword = () => {
      showPassword.value = !showPassword.value;
    };

    return {
      userName,
      password,
      handleLogin,
      handleGoogleLogin,
      loading,
      errorMessage,
      showPassword,
      togglePassword,
      showInfoModal,
      modalData,
      modalError,
      submitUserInfo,
      banks,
    };
  },
});
</script>

<style scoped>
/* CSS cho login form */
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
  0%, 100% {
    transform: scale(1);
    opacity: 0.5;
  }
  50% {
    transform: scale(1.2);
    opacity: 0.8;
  }
}

.login-card {
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

.card-header {
  text-align: center;
  margin-bottom: 15px;
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
  margin-top: 6px;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 12px;
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
  padding: 10px 30px 10px 30px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(0, 221, 235, 0.3);
  color: #ffffff;
  font-size: 0.9rem;
  outline: none;
  transition: all 0.3s ease;
  border-radius: 8px; /* Bo góc nhẹ */
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
  margin: 5px 0;
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

.sign-in-btn {
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

.sign-in-btn:hover {
  background: #33e6f2;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 221, 235, 0.6);
}

.or-container {
  display: flex;
  justify-content: center;
  align-items: center;
  color: #b0b0b0;
  font-size: 0.8rem;
  position: relative;
  margin: 5px 0;
}

.or-container::before,
.or-container::after {
  content: '';
  flex: 1;
  height: 1px;
  background: rgba(255, 255, 255, 0.2);
  margin: 0 8px;
}

.social-buttons {
  display: flex;
  justify-content: center;
  gap: 10px;
}

.social-btn {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: 1px solid rgba(255, 255, 255, 0.2);
  background: rgba(255, 255, 255, 0.05);
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 1.1rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.google-btn {
  color: #db4437;
}
.google-btn:hover {
  background: #db4437;
  color: #ffffff;
  border-color: #db4437;
}

.github-btn {
  color: #ffffff;
}
.github-btn:hover {
  background: #333;
  border-color: #333;
}

.facebook-btn {
  color: #3b5998;
}
.facebook-btn:hover {
  background: #3b5998;
  color: #ffffff;
  border-color: #3b5998;
}

.register-link {
  text-align: center;
  font-size: 0.8rem;
  color: #b0b0b0;
  margin-top: 5px;
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

.error-notification {
  position: absolute;
  bottom: 10px;
  left: 50%;
  transform: translateX(-50%);
  background: rgba(255, 75, 75, 0.9);
  color: #ffffff;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 0.8rem;
  box-shadow: 0 4px 12px rgba(255, 75, 75, 0.4);
  z-index: 2;
}

.fade-enter-active,
.fade-leave-active {
  transition: all 0.4s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(20px);
}

@media (max-width: 768px) {
  .login-card {
    padding: 20px;
    max-width: 320px;
  }
  .login-title {
    font-size: 1.4rem;
  }
  .login-subtitle {
    font-size: 0.75rem;
  }
  .sign-in-btn {
    padding: 8px;
  }
  .error-notification {
    width: 90%;
    font-size: 0.7rem;
  }
}

.spinner-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
  backdrop-filter: blur(5px);
}
</style>
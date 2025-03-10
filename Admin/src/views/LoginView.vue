<script lang="ts">
import { defineComponent, ref } from "vue";
import { useRouter } from "vue-router";
import Cookies from "js-cookie";
import authApi from "@/api/authenticate.api";
import { userStore } from "../stores/auth";
import DOMPurify from "dompurify";
import type { AxiosResponse } from "axios";

// Định nghĩa kiểu cho dữ liệu trả về từ API getRoleById
interface RoleResponse {
  result: string[];
}

export default defineComponent({
  name: "Login",
  setup() {
    const user = userStore();
    const userName = ref("");
    const password = ref("");
    const router = useRouter();
    const loading = ref(false);
    const errorMessage = ref<string | null>(null);
    const showPassword = ref(false);

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
            const userId = response.result.data.userId;

            // Gọi API để lấy vai trò
            const roleResponse: AxiosResponse<RoleResponse> = await authApi.getRoleById(userId);
            const roleData: RoleResponse = roleResponse.data;

            // Kiểm tra dữ liệu trả về từ API
            if (roleData && Array.isArray(roleData.result) && roleData.result.includes("Admin")) {
              // Nếu là Admin, tiến hành đăng nhập
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
              Cookies.set("token", response.result.data.token);
              localStorage.setItem("token", response.result.data.token);
              router.push("/");
              setTimeout(() => {
                window.location.reload();
              }, 100);
            } else {
              // Nếu không phải Admin, hiển thị thông báo lỗi
              throw new Error("Bạn không phải là Admin.");
            }
          } else {
            throw new Error("Tài khoản hoặc mật khẩu không đúng.");
          }
        } catch (error: any) {
          console.error("Error during login:", error);
          errorMessage.value = error.message || "Đã xảy ra lỗi khi đăng nhập.";
          setTimeout(() => (errorMessage.value = null), 3000);
        } finally {
          loading.value = false;
        }
      } else {
        errorMessage.value = "Vui lòng nhập đầy đủ tài khoản và mật khẩu.";
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const togglePassword = () => {
      showPassword.value = !showPassword.value;
    };

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

<!-- Phần template và style giữ nguyên như bạn đã cung cấp -->
<template>
  <div class="login-page">
    <!-- Background Overlay -->
    <div class="overlay"></div>

    <!-- Login Container -->
    <div class="login-container">
      <!-- Logo hoặc Tên Ứng Dụng -->
      <div class="app-branding">
        <i class="material-icons app-icon">store</i>
        <span class="app-name">GameTradeZone</span>
      </div>

      <!-- Form Đăng Nhập -->
      <form @submit.prevent="handleLogin" class="login-form">
        <!-- Tiêu đề -->
        <h2 class="form-title">Đăng Nhập</h2>

        <!-- Thông báo lỗi -->
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>

        <!-- Trường Username -->
        <div class="input-group">
          <i class="material-icons input-icon">person</i>
          <input
            type="text"
            v-model="userName"
            placeholder="Tên đăng nhập"
            class="input-field"
          />
        </div>

        <!-- Trường Password -->
        <div class="input-group">
          <i class="material-icons input-icon">lock</i>
          <input
            :type="showPassword ? 'text' : 'password'"
            v-model="password"
            placeholder="Mật khẩu"
            class="input-field"
          />
          <i class="material-icons eye-icon" @click="togglePassword">
            {{ showPassword ? "visibility_off" : "visibility" }}
          </i>
        </div>

        <!-- Nút Đăng Nhập -->
        <button type="submit" class="login-btn" :disabled="loading">
          <span v-if="!loading">Đăng Nhập</span>
          <!-- <loading-spinner v-else /> -->
        </button>

        <!-- Liên kết Quên Mật Khẩu -->
        <div class="forgot-password">
          <router-link to="/forgot-password" class="forgot-link">
            Quên mật khẩu?
          </router-link>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
/* Reset mặc định */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

/* Login Page */
.login-page {
  width: 100%;
  height: 100vh;
  background: linear-gradient(180deg, #2c3e50 0%, #1a252f 100%);
  display: flex;
  justify-content: center;
  align-items: center;
  position: relative;
}

/* Overlay */
.overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.3);
}

/* Login Container */
.login-container {
  position: relative;
  width: 100%;
  max-width: 400px;
  padding: 30px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
  backdrop-filter: blur(5px);
  z-index: 1;
}

/* App Branding */
.app-branding {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 20px;
}

.app-icon {
  font-size: 28px;
  color: #ecf0f1;
  margin-right: 10px;
}

.app-name {
  font-size: 24px;
  font-weight: 700;
  letter-spacing: 1px;
  color: #ecf0f1;
}

/* Form Đăng Nhập */
.login-form {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.form-title {
  font-size: 20px;
  font-weight: 600;
  color: #ecf0f1;
  text-align: center;
  margin-bottom: 10px;
}

/* Thông báo lỗi */
.error-message {
  color: #e74c3c;
  font-size: 14px;
  text-align: center;
  margin-bottom: 10px;
}

/* Trường Nhập Liệu */
.input-group {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 10px;
  color: #bdc3c7;
  font-size: 20px;
}

.eye-icon {
  position: absolute;
  right: 10px;
  color: #bdc3c7;
  font-size: 20px;
  cursor: pointer;
}

.input-field {
  width: 100%;
  padding: 10px 40px;
  border: 1px solid rgba(255,  Dost255, 255, 0.2);
  border-radius: 5px;
  background: rgba(255, 255, 255, 0.1);
  color: #ecf0f1;
  font-size: 14px;
  outline: none;
  transition: border-color 0.3s ease;
}

.input-field:focus {
  border-color: #3498db;
}

/* Nút Đăng Nhập */
.login-btn {
  padding: 12px;
  background: #3498db;
  border: none;
  border-radius: 5px;
  color: #ffffff;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.3s ease;
  display: flex;
  justify-content: center;
  align-items: center;
}

.login-btn:disabled {
  background: #2980b9;
  cursor: not-allowed;
}

.login-btn:hover:not(:disabled) {
  background: #2980b9;
}

/* Liên kết Quên Mật Khẩu */
.forgot-password {
  text-align: center;
}

.forgot-link {
  color: #bdc3c7;
  font-size: 14px;
  text-decoration: none;
  transition: color 0.3s ease;
}

.forgot-link:hover {
  color: #3498db;
}

/* Dark Mode Styles */
.dark-mode .login-page {
  background: linear-gradient(180deg, #34495e 0%, #2c3e50 100%);
}

.dark-mode .login-container {
  background: rgba(255, 255, 255, 0.1);
}

.dark-mode .input-field {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.3);
}
</style>
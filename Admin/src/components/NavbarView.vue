<script setup lang="ts">
import { ref, computed, watch, onUnmounted, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import userImage from '@/assets/user.png';
import logoImage from '@/assets/logo.png';

// Props
defineProps<{
  isCollapsed: boolean;
}>();

// State
const searchQuery = ref<string>('');
const username = ref<string>('Admin');
const userAvatar = ref<string>(userImage);
const currentPageTitle = ref<string>('Trang quản trị');
const showDropdown = ref<boolean>(false);
const isDarkMode = ref<boolean>(false);
const isLoading = ref<boolean>(false);

const route = useRoute();
const router = useRouter();

// Toggle dropdown
const toggleDropdown = () => {
  showDropdown.value = !showDropdown.value;
};

// Đóng dropdown khi click ra ngoài
const closeDropdown = (event: MouseEvent) => {
  const target = event.target as HTMLElement;
  if (!target.closest('.user-profile')) {
    showDropdown.value = false;
  }
};

// Thêm và xóa event listener để đóng dropdown
const addCloseListener = () => {
  document.addEventListener('click', closeDropdown);
};

const removeCloseListener = () => {
  document.removeEventListener('click', closeDropdown);
};

// Hàm xử lý tìm kiếm
const handleSearch = () => {
  if (searchQuery.value.trim()) {
    console.log('Searching for:', searchQuery.value);
    // Thực hiện logic tìm kiếm ở đây
  }
};

// Hàm xử lý đăng xuất
const handleLogout = () => {
  localStorage.removeItem('token'); // Xóa token
  router.push('/login'); // Chuyển hướng về trang login
  showDropdown.value = false; // Đóng dropdown
};

// Chuyển đổi chủ đề sáng/tối
const toggleTheme = () => {
  isDarkMode.value = !isDarkMode.value;
  document.body.classList.toggle('dark-mode', isDarkMode.value);
};

// Giả lập trạng thái tải (loading)
const simulateLoading = () => {
  isLoading.value = true;
  setTimeout(() => {
    isLoading.value = false;
  }, 2000);
};

// Hàm lấy tiêu đề trang dựa trên route
const getPageTitle = (path: string): string => {
  switch (path) {
    case '/admin/users':
      return 'Quản lý người dùng';
    case '/admin/roles':
      return 'Quản lý vai trò';
    case '/admin/products':
      return 'Quản lý sản phẩm';
    case '/admin/transactions':
      return 'Quản lý giao dịch';
    case '/admin/statistics':
      return 'Thống kê';
    default:
      return 'Trang quản trị';
  }
};

// Cập nhật tiêu đề trang khi route thay đổi
watch(
  () => route.path,
  (newPath) => {
    currentPageTitle.value = getPageTitle(newPath);
  },
  { immediate: true }
);

// Đảm bảo thêm và xóa listener đúng cách
onMounted(() => {
  addCloseListener();
  simulateLoading(); // Giả lập loading khi mount
});

onUnmounted(() => {
  removeCloseListener();
});
</script>

<template>
  <div class="navbar-wrapper">
    <div class="navbar" :class="{ collapsed: isCollapsed }">
      <!-- Navbar Header -->
      <div class="navbar-header">
        <a href="/"><img :src="logoImage" alt="Logo" class="navbar-logo" /></a>
        <span class="page-title">{{ currentPageTitle }}</span>
      </div>

      <!-- Navbar Actions -->
      <div class="navbar-actions">
        <!-- Quick Links -->
        <div class="quick-links">
          <router-link to="/admin/notifications" class="quick-link">
            <i class="material-icons">notifications</i>
            <span class="badge">3</span>
          </router-link>
          <router-link to="/admin/messages" class="quick-link">
            <i class="material-icons">message</i>
            <span class="badge">5</span>
          </router-link>
          <router-link to="/admin/calendar" class="quick-link">
            <i class="material-icons">calendar_today</i>
          </router-link>
        </div>

        <!-- Search Bar -->
        <div class="search-bar">
          <input
            type="text"
            placeholder="Tìm kiếm..."
            v-model="searchQuery"
            @keyup.enter="handleSearch"
          />
          <i class="material-icons" @click="handleSearch">search</i>
        </div>

        <!-- Theme Toggle -->
        <div class="theme-toggle" @click="toggleTheme">
          <i class="material-icons">{{
            isDarkMode ? 'brightness_7' : 'brightness_4'
          }}</i>
        </div>

        <!-- User Profile -->
        <div class="user-profile" @click="toggleDropdown">
          <img :src="userAvatar" alt="User Avatar" class="avatar" />
          <span class="username">{{ username }}</span>
          <i class="material-icons dropdown-icon">{{
            showDropdown ? 'arrow_drop_up' : 'arrow_drop_down'
          }}</i>

          <!-- Dropdown Menu -->
          <div class="dropdown-menu" :class="{ show: showDropdown }" v-if="showDropdown">
            <router-link to="/admin/profile" class="dropdown-item" @click="showDropdown = false">
              <i class="material-icons">person</i>
              <span>Hồ sơ</span>
            </router-link>
            <router-link to="/admin/settings" class="dropdown-item" @click="showDropdown = false">
              <i class="material-icons">settings</i>
              <span>Cài đặt</span>
            </router-link>
            <div class="dropdown-item" @click="handleLogout">
              <i class="material-icons">logout</i>
              <a href="/login"><span>Đăng xuất</span></a>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="loading-bar" v-if="isLoading" :class="{ collapsed: isCollapsed }"></div>
  </div>
</template>

<style scoped>
/* Reset mặc định */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

/* Navbar Wrapper */
.navbar-wrapper {
  position: relative;
}

/* Navbar */
.navbar {
  width: calc(100% - 220px);
  height: 60px;
  background: #ffffff;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  position: fixed;
  top: 0;
  left: 220px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
  z-index: 1000;
  transition: width 0.3s ease, left 0.3s ease;
}

.navbar.collapsed {
  width: calc(100% - 60px);
  left: 60px;
}

/* Navbar Header */
.navbar-header {
  display: flex;
  align-items: center;
}

.navbar-logo {
  width: 40px;
  height: 50px;
  margin-right: 10px;
  transition: transform 0.3s ease;
}

.navbar-logo:hover {
  transform: scale(1.1);
}

.page-title {
  font-size: 18px;
  font-weight: 600;
  color: #2c3e50;
}

/* Navbar Actions */
.navbar-actions {
  display: flex;
  align-items: center;
}

/* Quick Links */
.quick-links {
  display: flex;
  align-items: center;
  margin-right: 20px;
}

.quick-link {
  position: relative;
  display: flex;
  align-items: center;
  padding: 5px;
  color: #7f8c8d;
  text-decoration: none;
  transition: color 0.3s ease, transform 0.3s ease;
}

.quick-link:hover {
  color: #3498db;
  transform: scale(1.1);
}

.quick-link .material-icons {
  margin-left: 10px;
  font-size: 24px;
}

.badge {
  position: absolute;
  top: -5px;
  right: -5px;
  background: #e74c3c;
  color: #ffffff;
  font-size: 10px;
  font-weight: 600;
  padding: 2px 5px;
  border-radius: 10px;
}

/* Search Bar */
.search-bar {
  position: relative;
  margin-right: 20px;
}

.search-bar input {
  padding: 8px 35px 8px 15px;
  border: 1px solid #ddd;
  border-radius: 20px;
  outline: none;
  font-size: 14px;
  transition: all 0.3s ease;
}

.search-bar input:focus {
  border-color: #3498db;
  box-shadow: 0 0 5px rgba(52, 152, 219, 0.3);
}

.search-bar .material-icons {
  position: absolute;
  top: 50%;
  right: 10px;
  transform: translateY(-50%);
  color: #7f8c8d;
  cursor: pointer;
  transition: color 0.3s ease, transform 0.3s ease;
}

.search-bar .material-icons:hover {
  color: #3498db;
  transform: scale(1.1);
}

/* Theme Toggle */
.theme-toggle {
  display: flex;
  align-items: center;
  padding: 5px;
  cursor: pointer;
  color: #7f8c8d;
  margin-right: 20px;
  transition: color 0.3s ease, transform 0.3s ease;
}

.theme-toggle:hover {
  color: #3498db;
  transform: scale(1.1);
}

/* User Profile */
.user-profile {
  position: relative;
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 5px 10px;
  border-radius: 20px;
  transition: background 0.3s ease;
}

.user-profile:hover {
  background: #f5f5f5;
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  margin-right: 10px;
}

.username {
  font-size: 14px;
  font-weight: 500;
  color: #2c3e50;
}

.dropdown-icon {
  color: #7f8c8d;
}

/* Dropdown Menu */
.dropdown-menu {
  position: absolute;
  top: 100%;
  right: 0;
  background: #ffffff;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  border-radius: 8px;
  min-width: 200px;
  z-index: 1001;
  overflow: hidden;
  opacity: 0;
  transform: translateY(10px);
  transition: opacity 0.3s ease, transform 0.3s ease;
}

.dropdown-menu.show {
  opacity: 1;
  transform: translateY(0);
}

.dropdown-item {
  display: flex;
  align-items: center;
  padding: 10px 15px;
  color: #2c3e50;
  text-decoration: none;
  transition: background 0.3s ease;
}

.dropdown-item:hover {
  background: #f5f5f5;
}

.dropdown-item .material-icons {
  margin-right: 10px;
  font-size: 20px;
}

.dropdown-item span {
  font-size: 14px;
  font-weight: 500;
}
a{
  text-decoration: none;
  color: #2c3e50;
}
/* Loading Bar */
.loading-bar {
  position: fixed;
  top: 60px;
  left: 220px;
  width: calc(100% - 220px);
  height: 3px;
  background: #3498db;
  animation: loading 2s ease-in-out infinite;
  transition: width 0.3s ease, left 0.3s ease;
}

.loading-bar.collapsed {
  left: 60px;
  width: calc(100% - 60px);
}

@keyframes loading {
  0% {
    width: 0;
  }
  50% {
    width: 50%;
  }
  100% {
    width: 0;
  }
}

/* Dark Mode Styles */
.dark-mode .navbar {
  background: #2c3e50;
}

.dark-mode .page-title,
.dark-mode .username,
.dark-mode .dropdown-item {
  color: #ecf0f1;
}

.dark-mode .dropdown-menu {
  background: #34495e;
}

.dark-mode .dropdown-item:hover {
  background: #3e5c76;
}

.dark-mode .search-bar input {
  background: #34495e;
  color: #ecf0f1;
  border-color: #7f8c8d;
}
</style>
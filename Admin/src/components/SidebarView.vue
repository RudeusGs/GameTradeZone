<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import userImage from "@/assets/user.png"; // Giả sử bạn có ảnh avatar trong assets

// Props
defineProps<{
  isCollapsed: boolean;
}>();

// Emits
const emit = defineEmits<{
  (e: "toggle"): void;
}>();

// State
const router = useRouter();
const username = ref<string>("Admin");
const userRole = ref<string>("Quản trị viên");
const userAvatar = ref<string>(userImage);

// Quick Stats (Ví dụ)
const quickStats = ref({
  notifications: 3,
  messages: 5,
});

// Trạng thái mở/đóng của các dropdown
const dropdownStates = ref({
  gameAccount: false,
  transactions: false,
});

// Hàm toggle dropdown
const toggleDropdown = (key: "gameAccount" | "transactions") => {
  dropdownStates.value[key] = !dropdownStates.value[key];
};

// Logout
const logout = () => {
  localStorage.removeItem("token");
  router.push("/login");
  alert("Đăng xuất thành công!");
};
</script>

<template>
  <div class="sidebar" :class="{ collapsed: isCollapsed }">
    <!-- Sidebar Header -->
    <div class="sidebar-header">
      <div class="app-branding">
        <span class="app-name" v-if="!isCollapsed">GameTradeZone</span>
        <i class="material-icons app-icon" v-else>store</i>
      </div>
      <button class="toggle-btn" @click="emit('toggle')">
        <i class="material-icons">{{
          isCollapsed ? "chevron_right" : "chevron_left"
        }}</i>
      </button>
    </div>

    <!-- User Info -->
    <div class="user-info">
      <img :src="userAvatar" alt="User Avatar" class="user-avatar" />
      <div class="user-details" v-if="!isCollapsed">
        <span class="user-name">{{ username }}</span>
        <span class="user-role">{{ userRole }}</span>
      </div>
    </div>

    <!-- Sidebar Menu -->
    <ul class="sidebar-menu">
      <li>
        <router-link
          to="/website-account"
          class="menu-link"
          :class="{ active: $route.path === '/website-account' }"
        >
          <i class="material-icons">people</i>
          <span class="menu-text" v-if="!isCollapsed">Người dùng</span>
        </router-link>
      </li>
      <li>
        <router-link
          to="/admin/roles"
          class="menu-link"
          :class="{ active: $route.path === '/admin/roles' }"
        >
          <i class="material-icons">security</i>
          <span class="menu-text" v-if="!isCollapsed">Bài đăng</span>
        </router-link>
      </li>
      <li>
        <router-link
          to="/admin/products"
          class="menu-link"
          :class="{ active: $route.path === '/admin/products' }"
        >
          <i class="material-icons">store</i>
          <span class="menu-text" v-if="!isCollapsed">Dịch vụ</span>
        </router-link>
      </li>
      <!-- Đấu giá -->
      <li>
        <router-link
          to="/admin/auctions"
          class="menu-link"
          :class="{ active: $route.path === '/admin/auctions' }"
        >
          <i class="material-icons">gavel</i>
          <span class="menu-text" v-if="!isCollapsed">Đấu giá</span>
        </router-link>
      </li>
      <!-- Tài khoản game (Dropdown) -->
      <li class="dropdown" v-if="!isCollapsed">
        <div class="menu-link" @click="toggleDropdown('gameAccount')">
          <i class="material-icons">games</i>
          <span class="menu-text">Game</span>
          <i class="material-icons dropdown-icon">
            {{ dropdownStates.gameAccount ? "expand_less" : "expand_more" }}
          </i>
        </div>
        <ul class="submenu" v-show="dropdownStates.gameAccount">
          <li>
            <router-link
              to="/game-account"
              :class="{ active: $route.path === '/game-account' }"
            >
              Tài khoản người chơi
            </router-link>
          </li>
          <li>
            <router-link
              to="/game-infor"
              :class="{ active: $route.path === '/game-infor' }"
            >
              Trò chơi
            </router-link>
          </li>       
        </ul>
      </li>
      <li v-else>
        <router-link
          to="/game-account"
          class="menu-link"
          :class="{ active: $route.path === '/game-account' }"
        >
          <i class="material-icons">games</i>
        </router-link>
      </li>
      <!-- Giao dịch (Dropdown) -->
      <li class="dropdown" v-if="!isCollapsed">
        <div class="menu-link" @click="toggleDropdown('transactions')">
          <i class="material-icons">payment</i>
          <span class="menu-text">Giao dịch</span>
          <i class="material-icons dropdown-icon">
            {{ dropdownStates.transactions ? "expand_less" : "expand_more" }}
          </i>
        </div>
        <ul class="submenu" v-show="dropdownStates.transactions">
          <li>
            <router-link
              to="/admin/transactions/deposit"
              :class="{ active: $route.path === '/admin/transactions/deposit' }"
            >
              Nạp tiền
            </router-link>
          </li>
          <li>
            <router-link
              to="/withdraw-requests"
              :class="{
                active: $route.path === '/admin/transactions/withdraw',
              }"
            >
              Rút tiền
            </router-link>
          </li>
          <!-- <li>
            <router-link
              to="/admin/transactions/withdraw"
              :class="{ active: $route.path === '/admin/transactions/withdraw' }"
            >
              Giao dịch tài khoản
            </router-link>
          </li> -->
        </ul>
      </li>
      <li v-else>
        <router-link
          to="/admin/transactions"
          class="menu-link"
          :class="{ active: $route.path === '/admin/transactions' }"
        >
          <i class="material-icons">payment</i>
        </router-link>
      </li>
      <li>
        <router-link
          to="/admin/statistics"
          class="menu-link"
          :class="{ active: $route.path === '/admin/statistics' }"
        >
          <i class="material-icons">bar_chart</i>
          <span class="menu-text" v-if="!isCollapsed">Thống kê</span>
        </router-link>
      </li>
      <!-- Icon -->
      <li>
        <router-link
          to="/admin/icons"
          class="menu-link"
          :class="{ active: $route.path === '/admin/icons' }"
        >
          <i class="material-icons">image</i>
          <span class="menu-text" v-if="!isCollapsed">Icon</span>
        </router-link>
      </li>
    </ul>

    <!-- Quick Stats -->
    <div class="quick-stats" v-if="!isCollapsed">
      <div class="stat-item">
        <i class="material-icons">notifications</i>
        <span>Thông báo: {{ quickStats.notifications }}</span>
      </div>
      <div class="stat-item">
        <i class="material-icons">message</i>
        <span>Tin nhắn: {{ quickStats.messages }}</span>
      </div>
    </div>

    <!-- Logout Button -->
    <div class="logout-section">
      <button class="logout-btn" @click="logout">
        <i class="material-icons">logout</i>
        <span v-if="!isCollapsed">Đăng xuất</span>
      </button>
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

/* Sidebar */
.sidebar {
  width: 220px;
  height: 100vh;
  background: linear-gradient(180deg, #2c3e50 0%, #1a252f 100%);
  color: #ecf0f1;
  position: fixed;
  top: 0;
  left: 0;
  display: flex;
  flex-direction: column;
  padding-top: 20px;
  transition: width 0.3s ease;
}

.sidebar.collapsed {
  width: 60px;
}

/* Sidebar Header */
.sidebar-header {
  padding: 10px 15px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.app-branding {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
}

.app-name {
  font-size: 20px;
  font-weight: 700;
  letter-spacing: 1px;
}

.app-icon {
  font-size: 24px;
}

.toggle-btn {
  background: none;
  border: none;
  color: #ecf0f1;
  cursor: pointer;
  padding: 5px;
  transition: transform 0.3s ease;
}

.toggle-btn:hover {
  transform: scale(1.1);
}

.sidebar.collapsed .toggle-btn {
  margin-left: auto;
}

/* User Info */
.user-info {
  display: flex;
  align-items: center;
  padding: 10px 15px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.user-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  margin-right: 10px;
  transition: transform 0.3s ease;
}

.user-avatar:hover {
  transform: scale(1.1);
}

.user-details {
  display: flex;
  flex-direction: column;
}

.user-name {
  font-size: 14px;
  font-weight: 600;
}

.user-role {
  font-size: 11px;
  color: #bdc3c7;
}

.sidebar.collapsed .user-info {
  justify-content: center;
  padding: 10px;
}

/* Sidebar Menu */
.sidebar-menu {
  list-style: none;
  flex-grow: 1;
  margin-top: 15px;
}

.sidebar-menu li {
  margin-bottom: 5px;
}

.menu-link {
  display: flex;
  align-items: center;
  padding: 12px 15px;
  color: #bdc3c7;
  text-decoration: none;
  transition: all 0.3s ease;
}

.menu-link:hover {
  background: rgba(255, 255, 255, 0.1);
  color: #ffffff;
}

.menu-link.active {
  background: #3498db;
  color: #ffffff;
}

.menu-link .material-icons {
  margin-right: 10px;
  font-size: 20px;
}

.menu-text {
  font-size: 14px;
  font-weight: 500;
}

.sidebar.collapsed .menu-link {
  justify-content: center;
  padding: 12px;
}

.sidebar.collapsed .menu-link .material-icons {
  margin-right: 0;
}

/* Dropdown Menu */
.dropdown .menu-link {
  cursor: pointer;
  justify-content: start;
}

.dropdown-icon {
  font-size: 18px;
  margin-right: 0;
}

.submenu {
  list-style: none;
  background: rgba(255, 255, 255, 0.05);
  padding: 5px 0;
}

.submenu li {
  margin-bottom: 0;
}

.submenu a {
  display: block;
  padding: 10px 30px;
  color: #bdc3c7;
  text-decoration: none;
  font-size: 13px;
  transition: all 0.3s ease;
}

.submenu a:hover {
  background: rgba(255, 255, 255, 0.1);
  color: #ffffff;
}

.submenu a.active {
  background: #3498db;
  color: #ffffff;
}

/* Quick Stats */
.quick-stats {
  padding: 10px 15px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.stat-item {
  display: flex;
  align-items: center;
  margin-bottom: 8px;
  color: #bdc3c7;
}

.stat-item .material-icons {
  margin-right: 8px;
  font-size: 16px;
}

.stat-item span {
  font-size: 13px;
}

/* Logout Section */
.logout-section {
  padding: 10px 15px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.logout-btn {
  display: flex;
  align-items: center;
  width: 100%;
  padding: 12px;
  background: none;
  border: none;
  color: #e74c3c;
  cursor: pointer;
  transition: all 0.3s ease;
}

.logout-btn:hover {
  background: rgba(231, 76, 60, 0.2);
  color: #ffffff;
}

.logout-btn .material-icons {
  margin-right: 10px;
  font-size: 20px;
}

.logout-btn span {
  font-size: 14px;
  font-weight: 500;
}

.sidebar.collapsed .logout-btn {
  justify-content: center;
  padding: 12px;
}

.sidebar.collapsed .logout-btn .material-icons {
  margin-right: 0;
}

/* Dark Mode Styles */
.dark-mode .sidebar {
  background: linear-gradient(180deg, #34495e 0%, #2c3e50 100%);
}

.dark-mode .menu-link:hover {
  background: rgba(255, 255, 255, 0.2);
}

.dark-mode .logout-btn:hover {
  background: rgba(231, 76, 60, 0.3);
}

.dark-mode .submenu {
  background: rgba(255, 255, 255, 0.1);
}
</style>

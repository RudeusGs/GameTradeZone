<template>
  <div class="cosmo-topbar">
    <div class="topbar-container">
      <!-- Left Section -->
      <div class="topbar-left">
        <div class="logo-section">
          <span class="cosmo-logo menu-toggle" @click="toggleSlidePanel">
            <a href="/" @click.prevent>GTZ</a>
            <i :class="['fas', isSlidePanelOpen ? 'fa-caret-up' : 'fa-caret-down', 'caret-icon']"></i>
            <div class="logo-glow"></div>
          </span>
          <a href="/" class="cosmo-home">
            <img style="width: 40px; margin-left: 10px" src="../assets/logo_web.png" />
          </a>
        </div>

        <!-- Slide Panel -->
        <transition name="slide">
          <div v-if="isSlidePanelOpen" class="slide-panel">
            <div class="slide-content">
              <div class="slide-grid">
                <div class="grid-column">
                  <div class="column-header">
                    <i class="fas fa-gavel column-icon"></i>
                    <h4 class="column-title">ĐẤU GIÁ</h4>
                  </div>
                  <ul class="column-list">
                    <li><span class="hover-indicator"></span><a href="/list-auction">Danh sách</a></li>
                    <li><span class="hover-indicator"></span>Phần thưởng</li>
                  </ul>
                </div>
                <div class="grid-column">
                  <div class="column-header">
                    <i class="fas fa-comments column-icon"></i>
                    <h4 class="column-title">DIỄN ĐÀN</h4>
                  </div>
                  <ul class="column-list">
                    <li @click="$router.push('/forums')"><span class="hover-indicator"></span>Diễn đàn</li>
                    <li><span class="hover-indicator"></span>Bài viết của tôi</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </transition>
      </div>

      <!-- Right Section -->
      <div class="topbar-right">
        <!-- Add Button -->
        <div class="dropdown-container">
          <div class="icon-container menu-toggle" @click.stop="toggleAddMenu">
            <i class="fas fa-square-plus cosmo-icon"></i>
          </div>
          <transition name="dropdown">
            <div v-if="isAddMenuOpen" class="dropdown-menu add-menu">
              <div class="menu-item" @click="handleAddAccountClick">
                <i class="fas fa-user-plus"></i>
                <span>Tài khoản</span>
              </div>
              <div class="menu-item" @click="handleAddServiceClick">
                <i class="fas fa-gamepad"></i>
                <span>Dịch vụ</span>
              </div>
            </div>
          </transition>
        </div>

        <!-- Calendar Icon -->
        <div class="icon-container">
          <i
            class="fas fa-calendar-alt cosmo-icon"
            @mouseover="bounceIcon($event)"
            @mouseout="resetIcon($event)"
          ></i>
        </div>

        <!-- Transaction Icon with Dropdown -->
        <div class="dropdown-container">
          <div class="icon-container menu-toggle" @click.stop="toggleTransactionMenu">
            <i class="fas fa-handshake cosmo-icon"></i>
          </div>
          <transition name="dropdown">
            <div v-if="isTransactionMenuOpen" class="dropdown-menu transaction-menu">
              <div class="menu-item" @click="handleTransactionAccountClick">
                <i class="fas fa-user"></i>
                <span>Tài khoản</span>
              </div>
              <div class="menu-item" @click="handleTransactionServiceClick">
                <i class="fas fa-gamepad"></i>
                <span>Dịch vụ</span>
              </div>
            </div>
          </transition>
        </div>

        <!-- Notification Icon -->
        <div class="dropdown-container">
          <div class="icon-container menu-toggle" @click.stop="toggleNotifications">
            <i class="fas fa-bell cosmo-icon">
              <span v-if="notifications > 0" class="cosmo-badge">{{ notifications }}</span>
            </i>
          </div>
          <transition name="dropdown">
            <div v-if="isNotificationOpen" class="dropdown-menu notification-menu">
              <div class="menu-header">
                <h4>Thông báo</h4>
              </div>
              <div class="menu-content">
                <div v-if="isLoadingNotifications" class="loading">
                  <p>Đang tải...</p>
                </div>
                <div v-else-if="notificationList.length === 0" class="no-notifications">
                  <p>Không có thông báo mới</p>
                </div>
                <div v-else class="notification-list">
                  <div
                    v-for="notification in notificationList"
                    :key="notification.id"
                    class="notification-item"
                    :class="{ 'read': notification.isRead }"
                    @click="markAsRead(notification)"
                  >
                    <div class="notification-content">
                      <h5>{{ notification.title }}</h5>
                      <p>{{ notification.message }}</p>
                      <span class="notification-time">{{ formatTime(notification.createdAt) }}</span>
                    </div>
                    <i
                      class="fas fa-trash notification-delete"
                      @click.stop="deleteNotification(notification.id)"
                    ></i>
                  </div>
                </div>
              </div>
            </div>
          </transition>
        </div>

        <!-- Balance (if logged in) with Dropdown -->
        <div v-if="isLoggedIn" class="dropdown-container">
          <div class="cosmo-balance menu-toggle" @click.stop="toggleBalanceMenu">
            <i class="fas fa-coins"></i>
            <span>{{ formatCurrency(balance) }}</span>
          </div>
          <transition name="dropdown">
            <div v-if="isBalanceMenuOpen" class="dropdown-menu balance-menu">
              <div class="menu-header">
                <h4>Số Dư</h4>
                <div class="balance-display">
                  <i class="fas fa-coins"></i>
                  <span>{{ formatCurrency(balance) }}</span>
                </div>
              </div>
              <div class="menu-divider"></div>
              <div class="menu-actions">
                <div class="menu-item" @click="handleRechargeClick">
                  <i class="fas fa-plus-circle"></i>
                  <span>Nạp Tiền</span>
                </div>
                <div class="menu-item" @click="handleWithdrawClick">
                  <i class="fas fa-minus-circle"></i>
                  <span>Rút Tiền</span>
                </div>
              </div>
              <div class="menu-divider"></div>
              <div class="menu-item" @click="handleStatisticsClick">
                <i class="fas fa-chart-line"></i>
                <span>Thống Kê</span>
              </div>
            </div>
          </transition>
        </div>

        <!-- User Not Logged In -->
        <div v-if="!isLoggedIn" class="dropdown-container">
          <div class="user-avatar menu-toggle" @click.stop="toggleAccountMenu">
            <i class="fas fa-user cosmo-icon"></i>
            <i :class="['fas', isAccountMenuOpen ? 'fa-caret-up' : 'fa-caret-down', 'caret-icon']"></i>
          </div>
          <transition name="dropdown">
            <div v-if="isAccountMenuOpen" class="dropdown-menu user-menu">
              <div class="menu-item" @click="goToLogin">
                <i class="fas fa-sign-in-alt"></i>
                <span>Đăng nhập</span>
              </div>
              <div class="menu-item" @click="goToRegister">
                <i class="fas fa-user-plus"></i>
                <span>Đăng ký</span>
              </div>
            </div>
          </transition>
        </div>

        <!-- User Logged In -->
        <div v-else class="dropdown-container">
          <div class="user-profile menu-toggle" @click.stop="toggleUserMenu">
            <div class="user-avatar">
              <div class="avatar-circle">
                <span>{{ fullName.charAt(0) }}</span>
              </div>
            </div>
            <span class="cosmo-user-name" :title="fullName">{{ fullName }}</span>
            <i :class="['fas', isUserMenuOpen ? 'fa-caret-up' : 'fa-caret-down', 'caret-icon']"></i>
          </div>
          <transition name="dropdown">
            <div v-if="isUserMenuOpen" class="dropdown-menu user-menu">
              <div class="menu-header">
                <div class="user-info">
                  <div class="avatar-circle">
                    <span>{{ fullName.charAt(0) }}</span>
                  </div>
                  <div class="user-details">
                    <h4>{{ fullName }}</h4>
                  </div>
                </div>
              </div>
              <div class="menu-divider"></div>
              <div class="menu-item" @click="goToProfile">
                <i class="fas fa-user"></i>
                <span>Thông tin</span>
              </div>
              <div class="menu-item" @click="goToSettings">
                <i class="fas fa-cog"></i>
                <span>Cài đặt</span>
              </div>
              <div class="menu-divider"></div>
              <div class="menu-item logout" @click="logout">
                <i class="fas fa-sign-out-alt"></i>
                <span>Đăng Xuất</span>
              </div>
            </div>
          </transition>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import authenticateApi from "@/api/authenticate.api";
import userApi from "@/api/websiteaccount.api";
import notificationApi from "@/api/notification.api";
import { userStore } from "@/stores/auth.ts";

export default {
  name: "CosmoTopbar",
  setup() {
    const router = useRouter();
    const store = userStore();
    const isUserMenuOpen = ref(false);
    const notifications = ref(0);
    const notificationList = ref([]);
    const isSlidePanelOpen = ref(false);
    const isNotificationOpen = ref(false);
    const isAddMenuOpen = ref(false);
    const isAccountMenuOpen = ref(false);
    const isTransactionMenuOpen = ref(false);
    const isBalanceMenuOpen = ref(false);
    const balance = ref(0);
    const isLoadingNotifications = ref(false);

    const isLoggedIn = computed(() => !!store.user);
    const fullName = computed(() => store.user?.fullName || "");
    const userId = computed(() => store.user?.id || 0);

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
      }).format(amount);
    };

    const formatTime = (dateString) => {
      const date = new Date(dateString);
      return date.toLocaleString("vi-VN", {
        dateStyle: "short",
        timeStyle: "short",
      });
    };

    const fetchUserBalance = async () => {
      if (!isLoggedIn.value || !userId.value) return;
      try {
        const response = await userApi.getById(userId.value);
        const userData = response.data.result.data;
        balance.value = userData.balance || 0;
        store.user = userData;
        localStorage.setItem("user", JSON.stringify(userData));
      } catch (error) {
        console.error("Failed to fetch user balance:", error);
        // Không hiển thị modal, chỉ ghi log lỗi
      }
    };

    const fetchNotifications = async () => {
      if (!isLoggedIn.value || !userId.value) {
        console.log("User not logged in or userId missing:", { isLoggedIn: isLoggedIn.value, userId: userId.value });
        return;
      }
      isLoadingNotifications.value = true;
      try {
        console.log("Fetching notifications for userId:", userId.value);
        const response = await notificationApi.getAllByUserId(userId.value);
        console.log("API Response:", response);
        const rawNotifications = response.data.result.data || [];
        console.log("Raw Notifications:", rawNotifications);
        notificationList.value = rawNotifications
          .filter((noti) => !noti.isDelete)
          .map((noti) => ({
            id: noti.id,
            title: noti.typeNoti,
            message: noti.content,
            createdAt: noti.createdDate,
            isRead: noti.isRead,
          }));
        console.log("Transformed Notifications:", notificationList.value);
        notifications.value = notificationList.value.filter(
          (notification) => !notification.isRead
        ).length;
      } catch (error) {
        console.error("Failed to fetch notifications:", error);
        // Không hiển thị modal, chỉ ghi log lỗi
      } finally {
        isLoadingNotifications.value = false;
      }
    };

    const markAsRead = async (notification) => {
      if (notification.isRead) return;
      try {
        await notificationApi.read(notification.id);
        notification.isRead = true;
        notifications.value = notificationList.value.filter(
          (n) => !n.isRead
        ).length;
      } catch (error) {
        console.error("Failed to mark notification as read:", error);
        // Không hiển thị modal, chỉ ghi log lỗi
      }
    };

    const deleteNotification = async (id) => {
      try {
        await notificationApi.delete(id);
        notificationList.value = notificationList.value.filter(
          (notification) => notification.id !== id
        );
        notifications.value = notificationList.value.filter(
          (n) => !n.isRead
        ).length;
      } catch (error) {
        console.error("Failed to delete notification:", error);
        // Không hiển thị modal, chỉ ghi log lỗi
      }
    };

    onMounted(() => {
      store.init();
      fetchUserBalance();
      fetchNotifications();
      document.addEventListener("click", handleClickOutside);
    });

    const toggleAddMenu = () => {
      isAddMenuOpen.value = !isAddMenuOpen.value;
      if (isAddMenuOpen.value) {
        isUserMenuOpen.value = false;
        isNotificationOpen.value = false;
        isAccountMenuOpen.value = false;
        isTransactionMenuOpen.value = false;
        isBalanceMenuOpen.value = false;
      }
    };

    const toggleNotifications = () => {
      isNotificationOpen.value = !isNotificationOpen.value;
      if (isNotificationOpen.value) {
        fetchNotifications();
        isUserMenuOpen.value = false;
        isAddMenuOpen.value = false;
        isAccountMenuOpen.value = false;
        isTransactionMenuOpen.value = false;
        isBalanceMenuOpen.value = false;
      }
    };

    const toggleUserMenu = () => {
      isUserMenuOpen.value = !isUserMenuOpen.value;
      if (isUserMenuOpen.value) {
        isNotificationOpen.value = false;
        isAddMenuOpen.value = false;
        isAccountMenuOpen.value = false;
        isTransactionMenuOpen.value = false;
        isBalanceMenuOpen.value = false;
      }
    };

    const toggleAccountMenu = () => {
      isAccountMenuOpen.value = !isAccountMenuOpen.value;
      if (isAccountMenuOpen.value) {
        isNotificationOpen.value = false;
        isAddMenuOpen.value = false;
        isUserMenuOpen.value = false;
        isTransactionMenuOpen.value = false;
        isBalanceMenuOpen.value = false;
      }
    };

    const toggleTransactionMenu = () => {
      isTransactionMenuOpen.value = !isTransactionMenuOpen.value;
      if (isTransactionMenuOpen.value) {
        isNotificationOpen.value = false;
        isAddMenuOpen.value = false;
        isUserMenuOpen.value = false;
        isAccountMenuOpen.value = false;
        isBalanceMenuOpen.value = false;
      }
    };

    const toggleBalanceMenu = () => {
      isBalanceMenuOpen.value = !isBalanceMenuOpen.value;
      if (isBalanceMenuOpen.value) {
        isNotificationOpen.value = false;
        isAddMenuOpen.value = false;
        isUserMenuOpen.value = false;
        isAccountMenuOpen.value = false;
        isTransactionMenuOpen.value = false;
      }
    };

    const toggleSlidePanel = () => {
      isSlidePanelOpen.value = !isSlidePanelOpen.value;
    };

    const goToLogin = () => {
      router.push("/login");
      closeAllMenus();
    };

    const goToRegister = () => {
      router.push("/register");
      closeAllMenus();
    };

    const goToProfile = () => {
      router.push("/profile");
      closeAllMenus();
    };

    const goToSettings = () => {
      router.push("/");
      closeAllMenus();
    };

    const logout = async () => {
      try {
        await authenticateApi.logout();
        store.logout();
        balance.value = 0;
        notificationList.value = [];
        notifications.value = 0;
        closeAllMenus();
        router.push("/login");
      } catch (error) {
        console.error("Logout failed:", error);
      }
    };

    const handleRechargeClick = () => {
      if (!isLoggedIn.value) {
        router.push("/login");
      } else {
        router.push("/recharge");
      }
      closeAllMenus();
    };

    const handleWithdrawClick = () => {
      if (!isLoggedIn.value) {
        router.push("/login");
      } else {
        router.push("/withdraw");
      }
      closeAllMenus();
    };

    const handleStatisticsClick = () => {
      if (!isLoggedIn.value) {
        router.push("/login");
      } else {
        router.push("/statistics");
      }
      closeAllMenus();
    };

    const handleAddAccountClick = () => {
      if (!isLoggedIn.value) router.push("/login");
      else router.push("/add-account");
      closeAllMenus();
    };

    const handleAddServiceClick = () => {
      if (!isLoggedIn.value) router.push("/login");
      else router.push("/add-service");
      closeAllMenus();
    };

    const handleTransactionAccountClick = () => {
      if (!isLoggedIn.value) {
        router.push("/login");
      } else {
        router.push("/purchased");
      }
      closeAllMenus();
    };

    const handleTransactionServiceClick = () => {
      if (!isLoggedIn.value) {
        router.push("/login");
      } else {
        router.push("/transactions");
      }
      closeAllMenus();
    };

    const bounceIcon = (event) => {
      event.target.classList.add("animate-bounce");
    };

    const resetIcon = (event) => {
      event.target.classList.remove("animate-bounce");
    };

    const closeAllMenus = () => {
      isUserMenuOpen.value = false;
      isAddMenuOpen.value = false;
      isNotificationOpen.value = false;
      isSlidePanelOpen.value = false;
      isAccountMenuOpen.value = false;
      isTransactionMenuOpen.value = false;
      isBalanceMenuOpen.value = false;
    };

    const handleClickOutside = (event) => {
      if (event.target.closest(".menu-toggle")) return;
      if (event.target.closest(".dropdown-menu") || event.target.closest(".slide-panel")) return;
      closeAllMenus();
    };

    return {
      isUserMenuOpen,
      notifications,
      notificationList,
      isSlidePanelOpen,
      isNotificationOpen,
      isAddMenuOpen,
      isAccountMenuOpen,
      isTransactionMenuOpen,
      isBalanceMenuOpen,
      isLoggedIn,
      fullName,
      balance,
      formatCurrency,
      formatTime,
      isLoadingNotifications,
      toggleAddMenu,
      toggleNotifications,
      toggleUserMenu,
      toggleAccountMenu,
      toggleTransactionMenu,
      toggleBalanceMenu,
      toggleSlidePanel,
      goToLogin,
      goToRegister,
      goToProfile,
      goToSettings,
      logout,
      handleRechargeClick,
      handleWithdrawClick,
      handleStatisticsClick,
      handleAddAccountClick,
      handleAddServiceClick,
      handleTransactionAccountClick,
      handleTransactionServiceClick,
      markAsRead,
      deleteNotification,
      bounceIcon,
      resetIcon,
      closeAllMenus,
      handleClickOutside,
    };
  },
  beforeUnmount() {
    document.removeEventListener("click", this.handleClickOutside);
  },
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Rajdhani:wght@500;600;700&family=Orbitron:wght@400;500;700&display=swap');

/* Base Styles */
.cosmo-topbar {
  height: 70px;
  background: linear-gradient(90deg, #0a0a20 0%, #1a0933 50%, #0d1b2a 100%);
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  z-index: 1050;
  font-family: 'Rajdhani', sans-serif;
  box-shadow: 0 4px 20px rgba(0, 255, 255, 0.2);
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
}

.topbar-container {
  max-width: 1400px;
  margin: 0 auto;
  height: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
}

.topbar-left,
.topbar-right {
  display: flex;
  align-items: center;
}

a {
  text-decoration: none;
  color: inherit;
}

/* Logo Styles */
.logo-section {
  display: flex;
  align-items: center;
}

.cosmo-logo {
  position: relative;
  font-family: 'Orbitron', sans-serif;
  font-size: 2.2rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 3px;
  color: #e0e0e0;
  cursor: pointer;
  display: flex;
  align-items: center;
  padding: 0 10px;
  transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  text-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.logo-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(0, 255, 255, 0.2) 0%, transparent 70%);
  filter: blur(8px);
  z-index: -1;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.cosmo-logo:hover {
  color: #00ffff;
  transform: scale(1.05);
}

.cosmo-logo:hover .logo-glow {
  opacity: 1;
}

.cosmo-home {
  margin-left: 15px;
  transition: transform 0.3s ease;
}

.cosmo-home:hover {
  transform: scale(1.1);
}

/* Caret Icon for GTZ Logo */
.cosmo-logo .caret-icon {
  margin-left: 8px;
  font-size: 1rem;
  color: #e0e0e0;
  transition: all 0.3s ease;
}

.cosmo-logo:hover .caret-icon {
  color: #00ffff;
}

/* Slide Panel */
.slide-panel {
  position: fixed;
  top: 70px;
  left: 0;
  width: 100%;
  background: linear-gradient(180deg, #0a0a20 0%, #1a0933 100%);
  z-index: 1040;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
  overflow: hidden;
}

.slide-content {
  padding: 30px 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.slide-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

.grid-column {
  background: rgba(10, 10, 32, 0.6);
  border-radius: 12px;
  padding: 20px;
  border: 1px solid rgba(0, 255, 255, 0.1);
  transition: all 0.3s ease;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
  backdrop-filter: blur(10px);
}

.grid-column:hover {
  border-color: rgba(0, 255, 255, 0.3);
  box-shadow: 0 8px 25px rgba(0, 255, 255, 0.15);
  transform: translateY(-5px);
}

.column-header {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid rgba(0, 255, 255, 0.1);
}

.column-icon {
  color: #00ffff;
  font-size: 1.2rem;
  margin-right: 10px;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 50%;
}

.column-title {
  font-size: 1.1rem;
  color: #00ffff;
  margin: 0;
  font-weight: 600;
  letter-spacing: 1px;
  text-transform: uppercase;
}

.column-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.column-list li {
  position: relative;
  color: #e0e0e0;
  font-size: 1rem;
  padding: 10px 0 10px 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
}

.hover-indicator {
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 4px;
  height: 0;
  background: #00ffff;
  border-radius: 2px;
  transition: height 0.3s ease;
}

.column-list li:hover {
  color: #00ffff;
  padding-left: 25px;
}

.column-list li:hover .hover-indicator {
  height: 70%;
}

/* Buttons and Icons */
.cosmo-btn {
  background: rgba(0, 255, 255, 0.1);
  color: #00ffff;
  padding: 8px 16px;
  font-size: 0.95rem;
  font-weight: 600;
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  position: relative;
  overflow: hidden;
  letter-spacing: 0.5px;
}

.cosmo-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(0, 255, 255, 0.2), transparent);
  transition: left 0.5s ease;
}

.cosmo-btn:hover {
  background: rgba(0, 255, 255, 0.2);
  transform: translateY(-3px);
  box-shadow: 0 5px 15px rgba(0, 255, 255, 0.3);
}

.cosmo-btn:hover::before {
  left: 100%;
}

.add-btn {
  margin-left: 10px;
}

.icon-container {
  position: relative;
  margin: 0 10px;
}

.cosmo-icon {
  font-size: 1.2rem;
  color: #e0e0e0;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
}

.cosmo-icon:hover {
  color: #00ffff;
  background: rgba(0, 255, 255, 0.2);
  transform: translateY(-3px);
  box-shadow: 0 5px 15px rgba(0, 255, 255, 0.2);
}

.cosmo-badge {
  position: absolute;
  top: -5px;
  right: -5px;
  background: #ff00ff;
  color: white;
  font-size: 0.7rem;
  width: 18px;
  height: 18px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  box-shadow: 0 2px 5px rgba(255, 0, 255, 0.5);
}

/* Balance Display */
.cosmo-balance {
  display: flex;
  align-items: center;
  background: rgba(0, 255, 255, 0.1);
  padding: 8px 15px;
  border-radius: 8px;
  margin-left: 10px;
  border: 1px solid rgba(0, 255, 255, 0.3);
  transition: all 0.3s ease;
  cursor: pointer;
}

.cosmo-balance:hover {
  background: rgba(0, 255, 255, 0.15);
  box-shadow: 0 5px 15px rgba(0, 255, 255, 0.2);
}

.cosmo-balance i {
  color: #00ffff;
  margin-right: 8px;
  font-size: 1rem;
}

.cosmo-balance span {
  color: #e0e0e0;
  font-weight: 600;
  font-size: 0.95rem;
}

/* User Profile */
.user-profile {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 5px 10px;
  border-radius: 8px;
  margin-left: 10px;
  transition: all 0.3s ease;
  background: rgba(0, 255, 255, 0.05);
  border: 1px solid rgba(0, 255, 255, 0.1);
}

.user-profile:hover {
  background: rgba(0, 255, 255, 0.1);
  border-color: rgba(0, 255, 255, 0.3);
}

.user-profile .caret-icon {
  margin-left: 8px;
  font-size: 0.9rem;
  color: #e0e0e0;
  transition: all 0.3s ease;
}

.user-profile:hover .caret-icon {
  color: #00ffff;
}

.user-avatar {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  display: flex;
  align-items: center;
}

.user-avatar .caret-icon {
  margin-left: 8px;
  font-size: 0.9rem;
  color: #e0e0e0;
  transition: all 0.3s ease;
}

.user-avatar:hover .caret-icon {
  color: #00ffff;
}

.avatar-circle {
  width: 30px;
  height: 30px;
  border-radius: 4px !important;
  background: linear-gradient(135deg, #00ffff, #0088ff);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 0.9rem;
  text-transform: uppercase;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
}

.user-avatar .cosmo-icon {
  color: #fff;
  background: transparent;
}

.user-avatar:hover {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.4);
  transform: scale(1.05);
}

.user-avatar::after {
  content: '';
  position: absolute;
  top: -2px;
  right: -2px;
  width: 8px;
  height: 8px;
  background: #ff4757;
  border-radius: 50%;
  animation: ping 1.5s cubic-bezier(0, 0, 0.2, 1) infinite;
}

@keyframes ping {
  0% {
    transform: scale(0.8);
    opacity: 0.8;
  }
  75%,
  100% {
    transform: scale(2);
    opacity: 0;
  }
}

.cosmo-user-name {
  color: #e0e0e0;
  font-size: 0.95rem;
  font-weight: 600;
  margin: 0 5px;
  max-width: 120px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Dropdown Menus */
.dropdown-container {
  position: relative;
  z-index: 1100;
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  min-width: 200px;
  background: rgba(10, 10, 32, 0.95);
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3), 0 0 15px rgba(0, 255, 255, 0.2);
  border: 1px solid rgba(0, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  z-index: 1101;
  display: flex;
  flex-direction: column;
}

.notification-menu {
  min-width: 300px;
  max-height: 400px;
  overflow-y: auto;
  /* Firefox Scrollbar */
  scrollbar-width: thin;
  scrollbar-color: rgba(0, 255, 255, 0.5) rgba(10, 10, 32, 0.95);
}

/* WebKit Scrollbar (Chrome, Safari, Edge) */
.notification-menu::-webkit-scrollbar {
  width: 8px; /* Thinner scrollbar */
}

/* Track */
.notification-menu::-webkit-scrollbar-track {
  background: rgba(10, 10, 32, 0.95); /* Match dropdown background */
  border-radius: 10px;
}

/* Handle */
.notification-menu::-webkit-scrollbar-thumb {
  background: rgba(0, 255, 255, 0.5); /* Cyan thumb */
  border-radius: 10px;
  border: 2px solid rgba(10, 10, 32, 0.95); /* Match track background */
  box-shadow: inset 0 0 5px rgba(0, 255, 255, 0.3);
}

/* Handle on hover */
.notification-menu::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 255, 255, 0.8); /* Brighter cyan on hover */
  box-shadow: inset 0 0 8px rgba(0, 255, 255, 0.5);
}

/* Remove scrollbar buttons (arrows) in WebKit */
.notification-menu::-webkit-scrollbar-button {
  display: none;
}

.balance-menu {
  min-width: 400px;
}

.dropdown-menu::before {
  content: '';
  position: absolute;
  top: -5px;
  right: 20px;
  width: 10px;
  height: 10px;
  background: rgba(10, 10, 32, 0.95);
  transform: rotate(45deg);
  border-top: 1px solid rgba(0, 255, 255, 0.2);
  border-left: 1px solid rgba(0, 255, 255, 0.2);
}

.menu-header {
  padding: 15px;
}

.menu-header h4 {
  margin: 0;
  color: #00ffff;
  font-size: 1.1rem;
  font-weight: 600;
}

.balance-display {
  display: flex;
  align-items: center;
  margin-top: 10px;
  background: rgba(0, 255, 255, 0.05);
  padding: 8px 12px;
  border-radius: 6px;
}

.balance-display i {
  color: #00ffff;
  margin-right: 8px;
  font-size: 1rem;
}

.balance-display span {
  color: #e0e0e0;
  font-weight: 600;
  font-size: 1rem;
}

.menu-actions {
  display: flex;
  justify-content: space-between;
  padding: 10px 15px;
}

.menu-actions .menu-item {
  flex: 1;
  margin: 0 5px;
  padding: 8px 10px;
  justify-content: center;
  border-radius: 6px;
  background: rgba(0, 255, 255, 0.05);
}

.menu-actions .menu-item:hover {
  background: rgba(0, 255, 255, 0.15);
  padding-left: 10px;
}

.menu-content {
  padding: 15px;
  color: #e0e0e0;
  font-size: 0.95rem;
}

.no-notifications {
  text-align: center;
  color: #e0e0e0;
}

.loading {
  text-align: center;
  color: #00ffff;
}

.notification-list {
  width: 500px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.notification-item {
  display: flex;
  align-items: flex-start;
  padding: 12px 15px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
}

.notification-item.read {
  background: rgba(255, 255, 255, 0.02);
  opacity: 0.7;
}

.notification-item:hover {
  background: rgba(0, 255, 255, 0.1);
  transform: translateY(-2px);
}

.notification-content {
  flex: 1;
}

.notification-content h5 {
  margin: 0 0 5px;
  color: #00ffff;
  font-size: 1rem;
  font-weight: 600;
}

.notification-content p {
  margin: 0 0 5px;
  color: #e0e0e0;
  font-size: 0.9rem;
  line-height: 1.4;
}

.notification-time {
  color: #b0b0b0;
  font-size: 0.8rem;
}

.notification-delete {
  color: #ff5e5e;
  font-size: 0.9rem;
  padding: 5px;
  transition: all 0.3s ease;
}

.notification-delete:hover {
  color: #ff0000;
  transform: scale(1.1);
}

.menu-item {
  display: flex;
  align-items: center;
  padding: 12px 15px;
  color: #e0e0e0;
  transition: all 0.3s ease;
  cursor: pointer;
  width: 100%;
}

.menu-item i {
  width: 20px;
  margin-right: 10px;
  color: #00ffff;
  font-size: 1rem;
}

.menu-item:hover {
  background: rgba(0, 255, 255, 0.1);
  color: #00ffff;
  padding-left: 20px;
}

.menu-divider {
  height: 1px;
  background: rgba(0, 255, 255, 0.1);
  margin: 5px 0;
  width: 100%;
}

.menu-item.logout {
  color: #ff5e5e;
}

.menu-item.logout i {
  color: #ff5e5e;
}

.menu-item.logout:hover {
  background: rgba(255, 94, 94, 0.1);
  color: #ff5e5e;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 10px;
  width: 100%;
}

.user-details {
  flex: 1;
  overflow: hidden;
}

.user-details h4 {
  margin: 0;
  color: #e0e0e0;
  font-size: 1rem;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Animations */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  transform-origin: top right;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: scale(0.9);
}

.slide-enter-active,
.slide-leave-active {
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.slide-enter-from,
.slide-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}

@keyframes bounce {
  0%,
  20%,
  50%,
  80%,
  100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(-10px);
  }
  60% {
    transform: translateY(-5px);
  }
}

.animate-bounce {
  animation: bounce 1.5s infinite;
}

/* Responsive Styles */
@media (max-width: 992px) {
  .slide-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .notification-menu {
    min-width: 250px;
  }

  .balance-menu {
    min-width: 250px;
  }

  .menu-actions {
    flex-direction: column;
  }

  .menu-actions .menu-item {
    margin: 5px 0;
  }
}

@media (max-width: 768px) {
  .cosmo-topbar {
    height: 60px;
  }

  .topbar-container {
    padding: 0 10px;
  }

  .cosmo-logo {
    font-size: 1.8rem;
  }

  .cosmo-logo .caret-icon {
    font-size: 0.9rem;
  }

  .slide-panel {
    top: 60px;
  }

  .slide-content {
    padding: 20px 10px;
  }

  .slide-grid {
    gap: 10px;
  }

  .grid-column {
    padding: 15px;
  }

  .column-title {
    font-size: 1rem;
  }

  .column-list li {
    font-size: 0.9rem;
    padding: 8px 0 8px 15px;
  }

  .cosmo-btn {
    padding: 6px 12px;
    font-size: 0.85rem;
  }

  .cosmo-btn span {
    display: none;
  }

  .cosmo-icon {
    width: 35px;
    height: 35px;
    font-size: 1rem;
  }

  .cosmo-user-name {
    max-width: 80px;
  }

  .cosmo-balance span {
    font-size: 0.85rem;
  }

  .notification-menu {
    min-width: 200px;
  }

  .notification-content h5 {
    font-size: 0.9rem;
  }

  .notification-content p {
    font-size: 0.85rem;
  }

  .notification-time {
    font-size: 0.75rem;
  }

  .notification-delete {
    font-size: 0.8rem;
  }

  .balance-menu {
    min-width: 200px;
  }

  .user-profile .caret-icon,
  .user-avatar .caret-icon {
    font-size: 0.8rem;
  }
}

@media (max-width: 576px) {
  .cosmo-topbar {
    height: 50px;
  }

  .cosmo-logo {
    font-size: 1.5rem;
  }

  .cosmo-logo .caret-icon {
    font-size: 0.8rem;
  }

  .slide-panel {
    top: 50px;
  }

  .slide-grid {
    grid-template-columns: 1fr;
  }

  .cosmo-btn {
    padding: 5px 10px;
  }

  .cosmo-icon {
    width: 30px;
    height: 30px;
  }

  .notification-menu {
    min-width: 180px;
  }

  .balance-menu {
    min-width: 180px;
  }

  .user-profile .caret-icon,
  .user-avatar .caret-icon {
    font-size: 0.7rem;
  }
}
</style>
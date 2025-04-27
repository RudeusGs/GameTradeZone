<template>
  <div class="cosmo-navbar">
    <div class="navbar-container">
      <!-- Left Section -->
      <div class="navbar-left">
        <div class="logo-section">
          <span class="cosmo-logo menu-toggle" @click="toggleSlidePanel">
            <a href="/" @click.prevent>GTZ</a>
            <i
              :class="[
                'fas',
                isSlidePanelOpen ? 'fa-caret-up' : 'fa-caret-down',
                'caret-icon',
              ]"
            ></i>
            <div class="logo-glow"></div>
            <i :class="['fas', isSlidePanelOpen ? 'fa-chevron-up' : 'fa-chevron-down', 'caret-icon']"></i>
          </div>
          <a href="/" class="cosmo-home">
            <img
              style="width: 40px; margin-left: 10px"
              src="../assets/logo_web.png"
            />
          </a>
        </div>

        <!-- Slide Panel -->
        <transition name="slide">
          <div v-if="isSlidePanelOpen" class="slide-panel">
            <div class="slide-content">
              <div class="slide-grid">
                <div class="grid-column">
                  <div class="column-header">
                    <div class="column-icon-wrapper">
                      <i class="fas fa-gavel column-icon"></i>
                    </div>
                    <h4 class="column-title">ĐẤU GIÁ</h4>
                  </div>
                  <ul class="column-list">
                    <li @click="$router.push('/list-auction')">
                      <span class="hover-indicator"></span>
                      <span class="list-item-text">Danh sách</span>
                    </li>
                    <li>
                      <span class="hover-indicator"></span>
                      <span class="list-item-text">Phần thưởng</span>
                    </li>
                  </ul>
                </div>
                <div class="grid-column">
                  <div class="column-header">
                    <div class="column-icon-wrapper">
                      <i class="fas fa-comments column-icon"></i>
                    </div>
                    <h4 class="column-title">DIỄN ĐÀN</h4>
                  </div>
                  <ul class="column-list">
                    <li @click="$router.push('/forums')">
                      <span class="hover-indicator"></span>
                      <span class="list-item-text">Diễn đàn</span>
                    </li>
                    <li>
                      <span class="hover-indicator"></span>
                      <span class="list-item-text">Bài viết của tôi</span>
                    </li>
                  </ul>
                </div>
                <div class="grid-column">
                  <div class="column-header">
                    <div class="column-icon-wrapper">
                      <i class="fas fa-chart-line column-icon"></i>
                    </div>
                    <h4 class="column-title">THỐNG KÊ</h4>
                  </div>
                  <ul class="column-list">
                    <li>
                      <span class="hover-indicator"></span>
                      <span class="list-item-text">Tổng quan</span>
                    </li>
                    <li>
                      <span class="hover-indicator"></span>
                      <span class="list-item-text">Báo cáo</span>
                    </li>
                  </ul>
                </div>
                <div class="grid-column">
                  <div class="column-header">
                    <div class="column-icon-wrapper">
                      <i class="fas fa-cog column-icon"></i>
                    </div>
                    <h4 class="column-title">CÀI ĐẶT</h4>
                  </div>
                  <ul class="column-list">
                    <li>
                      <span class="hover-indicator"></span>
                      <span class="list-item-text">Tài khoản</span>
                    </li>
                    <li>
                      <span class="hover-indicator"></span>
                      <span class="list-item-text">Giao diện</span>
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </transition>
      </div>

      <!-- Center Section - Navigation Links -->
      <div class="navbar-center">
        <div class="nav-links">
          <div class="nav-link" @click="$router.push('/list-auction')">
            <i class="fas fa-gavel"></i>
            <span>Đấu giá</span>
          </div>
          <div class="nav-link" @click="$router.push('/forums')">
            <i class="fas fa-comments"></i>
            <span>Diễn đàn</span>
          </div>
        </div>
      </div>

      <!-- Right Section -->
      <div class="navbar-right">
        <!-- Add Button -->
        <div class="dropdown-container">
          <div class="icon-container menu-toggle" @click.stop="toggleAddMenu">
            <i class="fas fa-square-plus cosmo-icon"></i>
            <div class="icon-tooltip">Thêm mới</div>
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

        <!-- Transaction Icon with Dropdown -->
        <div class="dropdown-container">
          <div
            class="icon-container menu-toggle"
            @click.stop="toggleTransactionMenu"
          >
            <i class="fas fa-handshake cosmo-icon"></i>
            <div class="icon-tooltip">Giao dịch</div>
          </div>
          <transition name="dropdown">
            <div
              v-if="isTransactionMenuOpen"
              class="dropdown-menu transaction-menu"
            >
              <div class="menu-item" @click="handleTransactionAccountClick">
                <i class="fas fa-user"></i>
                <span>Tài khoản</span>
              </div>
              <div class="menu-item" @click="handleTransactionServiceClick">
                <i class="fas fa-gamepad"></i>
                <span>Dịch vụ</span>
              </div>
              <div class="menu-item" @click="handleTransactionHistoryClick">
                <i class="fas fa-history"></i>
                <span>Lịch sử giao dịch</span>
              </div>
            </div>
          </transition>
        </div>
        
        <!-- Notification Icon -->
        <div class="dropdown-container">
          <div
            class="icon-container menu-toggle"
            @click.stop="toggleNotifications"
          >
            <i class="fas fa-bell cosmo-icon">
              <span v-if="notifications > 0" class="cosmo-badge">{{
                notifications
              }}</span>
            </i>
            <div class="icon-tooltip">Thông báo</div>
          </div>
          <transition name="dropdown">
            <div
              v-if="isNotificationOpen"
              class="dropdown-menu notification-menu"
            >
              <div class="menu-header">
                <h4>Thông báo</h4>
                <div class="notification-actions">
                  <button class="action-btn" title="Đánh dấu tất cả đã đọc">
                    <i class="fas fa-check-double"></i>
                  </button>
                  <button class="action-btn" title="Làm mới">
                    <i class="fas fa-sync-alt"></i>
                  </button>
                </div>
              </div>
              <div class="menu-content">
                <div v-if="isLoadingNotifications" class="loading">
                  <div class="loading-spinner"></div>
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
                    :class="{ read: notification.isRead }"
                    @click="markAsRead(notification)"
                  >
                    <div class="notification-icon" :class="getNotificationIconClass(notification.title)">
                      <i :class="getNotificationIcon(notification.title)"></i>
                    </div>
                    <div class="notification-content">
                      <h5>{{ notification.title }}</h5>
                      <p>{{ notification.message }}</p>
                      <span class="notification-time">{{
                        formatTime(notification.createdAt)
                      }}</span>
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
          <div
            class="cosmo-balance menu-toggle"
            @click.stop="toggleBalanceMenu"
          >
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
                    <span class="user-status">
                      <span class="status-dot online"></span>
                      Online
                    </span>
                  </div>
                </div>
              </div>
              <div class="menu-divider"></div>
              <div class="menu-item" @click="goToProfile">
                <i class="fas fa-user"></i>
                <span>Thông tin</span>
              </div>
              <div class="menu-item" @click="goToTransactionHistory">
                <i class="fas fa-cog"></i>
                <span>Lịch sử giao dịch</span>
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
    <!-- Modal for Authentication and Alerts -->
    <transition name="modal">
      <div v-if="isModalOpen" class="modal-backdrop" @click="closeModal">
        <div class="modal-content" @click.stop>
          <h3>{{ modalTitle }}</h3>
          <p>{{ modalMessage }}</p>
          <button @click="modalAction">{{ modalActionText }}</button>
          <button @click="closeModal">Đóng</button>
        </div>
      </div>
    </transition>
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
  name: "CosmoNavbar",
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
    const isModalOpen = ref(false);
    const modalTitle = ref("");
    const modalMessage = ref("");
    const modalActionText = ref("");
    const modalAction = ref(() => {});

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

    const getNotificationIcon = (type) => {
      const iconMap = {
        "Thông báo": "fas fa-bell",
        "Giao dịch": "fas fa-handshake",
        "Hệ thống": "fas fa-cog",
        "Đấu giá": "fas fa-gavel"
      };
      return iconMap[type] || "fas fa-bell";
    };

    const getNotificationIconClass = (type) => {
      const classMap = {
        "Thông báo": "notification-icon-info",
        "Giao dịch": "notification-icon-success",
        "Hệ thống": "notification-icon-warning",
        "Đấu giá": "notification-icon-primary"
      };
      return classMap[type] || "notification-icon-info";
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
      }
    };

    const fetchNotifications = async () => {
      if (!isLoggedIn.value || !userId.value) {
        console.log("User not logged in or userId missing:", {
          isLoggedIn: isLoggedIn.value,
          userId: userId.value,
        });
        return;
      }
      isLoadingNotifications.value = true;
      try {
        const response = await notificationApi.getAllByUserId(userId.value);
        const rawNotifications = response.data.result.data || [];
        notificationList.value = rawNotifications
          .filter((noti) => !noti.isDelete)
          .map((noti) => ({
            id: noti.id,
            title: noti.typeNoti,
            message: noti.content,
            createdAt: noti.createdDate,
            isRead: noti.isRead,
          }));
        notifications.value = notificationList.value.filter(
          (notification) => !notification.isRead
        ).length;
      } catch (error) {
        console.error("Failed to fetch notifications:", error);
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
      if (!isLoggedIn.value) {
        router.push("/login");
      } else if (!store.user?.isAuthen) {
        showModal("Xác thực tài khoản", "Bạn cần xác thực tài khoản để thêm tài khoản mới.", "Đi đến trang hồ sơ", () => router.push("/profile?authRequired=true"));
      } else {
        router.push("/add-account");
        closeAllMenus();
      }
    };

    const handleAddServiceClick = () => {
      if (!isLoggedIn.value) {
        router.push("/login");
      } else if (!store.user?.isAuthen) {
        showModal("Xác thực tài khoản", "Bạn cần xác thực tài khoản để thêm dịch vụ mới.", "Đi đến trang hồ sơ", () => router.push("/profile?authRequired=true"));
      } else {
        router.push("/add-service");
        closeAllMenus();
      }
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
        router.push("/service-list");
      }
      closeAllMenus();
    };

    const handleTransactionHistoryClick = () => {
      if (!isLoggedIn.value) {
        router.push("/login");
      } else {
        router.push("/hpurcharsed");
      }
      closeAllMenus();
    };

    const showModal = (title, message, actionText, action) => {
      modalTitle.value = title;
      modalMessage.value = message;
      modalActionText.value = actionText;
      modalAction.value = action;
      isModalOpen.value = true;
    };

    const closeModal = () => {
      isModalOpen.value = false;
    };

    const bounceIcon = (event) => {
      event.target.classList.add("animate-bounce");
    };

    const resetIcon = (event) => {
      event.target.classList.remove("animate-bounce");
    };
    const goToTransactionHistory = () => {
      if (!isLoggedIn.value) {
        router.push("/login");
      } else {
        router.push("/transaction-history");
      }
      closeAllMenus();
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
      if (
        event.target.closest(".dropdown-menu") ||
        event.target.closest(".slide-panel")
      )
        return;
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
      getNotificationIcon,
      getNotificationIconClass,
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
      goToTransactionHistory,
      logout,
      handleRechargeClick,
      handleWithdrawClick,
      handleStatisticsClick,
      handleAddAccountClick,
      handleAddServiceClick,
      handleTransactionAccountClick,
      handleTransactionServiceClick,
      handleTransactionHistoryClick,
      markAsRead,
      deleteNotification,
      bounceIcon,
      resetIcon,
      closeAllMenus,
      handleClickOutside,
      isModalOpen,
      modalTitle,
      modalMessage,
      modalActionText,
      modalAction,
      closeModal,
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
.cosmo-navbar {
  height: 70px;
  background: linear-gradient(90deg, #0c0c1d 0%, #1e0b45 50%, #0f1e33 100%);
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  z-index: 1050;
  font-family: 'Rajdhani', sans-serif;
  box-shadow: 0 4px 20px rgba(0, 255, 255, 0.2);
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
}

.navbar-container {
  max-width: 1400px;
  margin: 0 auto;
  height: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
}

.navbar-left, .navbar-center, .navbar-right {
  display: flex;
  align-items: center;
}

.navbar-center {
  flex: 1;
  justify-content: center;
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
  font-family: "Orbitron", sans-serif;
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
}

.logo-text {
  position: relative;
  background: linear-gradient(to right, #9d4edd, #c77dff);
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
  text-shadow: 0 0 10px rgba(157, 78, 221, 0.5);
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
  color: #c77dff;
}

/* Navigation Links */
.nav-links {
  display: flex;
  gap: 20px;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  border-radius: 8px;
  color: #e0e0e0;
  font-weight: 500;
  transition: all 0.3s ease;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}

.nav-link::before {
  content: '';
  position: absolute;
  bottom: 0;
  left: 50%;
  width: 0;
  height: 2px;
  background: linear-gradient(to right, #9d4edd, #c77dff);
  transition: all 0.3s ease;
  transform: translateX(-50%);
}

.nav-link:hover {
  color: #fff;
  background: rgba(157, 78, 221, 0.1);
}

.nav-link:hover::before {
  width: 80%;
}

.nav-link i {
  color: #9d4edd;
  font-size: 1rem;
}

/* Slide Panel */
.slide-panel {
  position: fixed;
  top: 70px;
  left: 0;
  width: 100%;
  background: linear-gradient(180deg, #0c0c1d 0%, #1e0b45 100%);
  z-index: 1040;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
  border-bottom: 1px solid rgba(128, 0, 255, 0.2);
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
  border: 1px solid rgba(128, 0, 255, 0.1);
  transition: all 0.3s ease;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
  backdrop-filter: blur(10px);
}

.grid-column:hover {
  border-color: rgba(128, 0, 255, 0.3);
  box-shadow: 0 8px 25px rgba(128, 0, 255, 0.15);
  transform: translateY(-5px);
}

.column-header {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid rgba(128, 0, 255, 0.1);
}

.column-icon-wrapper {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #9d4edd, #c77dff);
  border-radius: 8px;
  margin-right: 12px;
}

.column-icon {
  color: #fff;
  font-size: 1rem;
}

.column-title {
  font-size: 1.1rem;
  color: #c77dff;
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
  background: #9d4edd;
  border-radius: 2px;
  transition: height 0.3s ease;
}

.column-list li:hover {
  color: #c77dff;
  padding-left: 25px;
}

.column-list li:hover .hover-indicator {
  height: 70%;
}

.list-item-text {
  position: relative;
  z-index: 1;
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
  background: rgba(157, 78, 221, 0.1);
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
}

.icon-tooltip {
  position: absolute;
  bottom: -30px;
  left: 50%;
  transform: translateX(-50%);
  background: rgba(10, 10, 32, 0.9);
  color: #e0e0e0;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.75rem;
  white-space: nowrap;
  opacity: 0;
  visibility: hidden;
  transition: all 0.3s ease;
  z-index: 1000;
  border: 1px solid rgba(157, 78, 221, 0.3);
}

.icon-tooltip::before {
  content: '';
  position: absolute;
  top: -4px;
  left: 50%;
  transform: translateX(-50%) rotate(45deg);
  width: 8px;
  height: 8px;
  background: rgba(10, 10, 32, 0.9);
  border-top: 1px solid rgba(157, 78, 221, 0.3);
  border-left: 1px solid rgba(157, 78, 221, 0.3);
}

.icon-container:hover .icon-tooltip {
  opacity: 1;
  visibility: visible;
  bottom: -35px;
}

.cosmo-icon:hover {
  color: #fff;
  background: rgba(157, 78, 221, 0.2);
  transform: translateY(-3px);
  box-shadow: 0 5px 15px rgba(157, 78, 221, 0.2);
}

.cosmo-badge {
  position: absolute;
  top: -5px;
  right: -5px;
  background: linear-gradient(135deg, #ff0080, #ff8c00);
  color: white;
  font-size: 0.7rem;
  width: 18px;
  height: 18px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  box-shadow: 0 2px 5px rgba(255, 0, 128, 0.5);
}

/* Balance Display */
.cosmo-balance {
  display: flex;
  align-items: center;
  background: rgba(157, 78, 221, 0.1);
  padding: 8px 15px;
  border-radius: 8px;
  margin-left: 10px;
  border: 1px solid rgba(157, 78, 221, 0.3);
  transition: all 0.3s ease;
  cursor: pointer;
}

.cosmo-balance:hover {
  background: rgba(157, 78, 221, 0.15);
  box-shadow: 0 5px 15px rgba(157, 78, 221, 0.2);
}

.cosmo-balance i {
  color: #c77dff;
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
  background: rgba(157, 78, 221, 0.05);
  border: 1px solid rgba(157, 78, 221, 0.1);
}

.user-profile:hover {
  background: rgba(157, 78, 221, 0.1);
  border-color: rgba(157, 78, 221, 0.3);
}

.user-profile .caret-icon {
  margin-left: 8px;
  font-size: 0.9rem;
  color: #e0e0e0;
  transition: all 0.3s ease;
}

.user-profile:hover .caret-icon {
  color: #c77dff;
}

.user-avatar {
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
  color: #c77dff;
}

.avatar-circle {
  width: 30px;
  height: 30px;
  border-radius: 8px !important;
  background: linear-gradient(135deg, #9d4edd, #c77dff);
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

/* User Status */
.user-status {
  display: flex;
  align-items: center;
  font-size: 0.8rem;
  color: #b0b0b0;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-right: 5px;
}

.status-dot.online {
  background: #4ade80;
  box-shadow: 0 0 5px rgba(74, 222, 128, 0.5);
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
  min-width: 250px;
  background: rgba(10, 10, 32, 0.95);
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3), 0 0 15px rgba(157, 78, 221, 0.2);
  border: 1px solid rgba(157, 78, 221, 0.2);
  backdrop-filter: blur(10px);
  z-index: 1101;
  display: flex;
  flex-direction: column;
}

.notification-menu {
  min-width: 300px;
  max-height: 400px;
  overflow-y: auto;
  scrollbar-width: thin;
  scrollbar-color: rgba(157, 78, 221, 0.5) rgba(10, 10, 32, 0.95);
}

/* WebKit Scrollbar */
.notification-menu::-webkit-scrollbar {
  width: 8px;
}

.notification-menu::-webkit-scrollbar-track {
  background: rgba(10, 10, 32, 0.95);
  border-radius: 10px;
}

.notification-menu::-webkit-scrollbar-thumb {
  background: rgba(157, 78, 221, 0.5);
  border-radius: 10px;
  border: 2px solid rgba(10, 10, 32, 0.95);
  box-shadow: inset 0 0 5px rgba(157, 78, 221, 0.3);
}

.notification-menu::-webkit-scrollbar-thumb:hover {
  background: rgba(157, 78, 221, 0.8);
  box-shadow: inset 0 0 8px rgba(157, 78, 221, 0.5);
}

.notification-menu::-webkit-scrollbar-button {
  display: none;
}

.balance-menu {
  min-width: 300px;
}

.dropdown-menu::before {
  content: "";
  position: absolute;
  top: -5px;
  right: 20px;
  width: 10px;
  height: 10px;
  background: rgba(10, 10, 32, 0.95);
  transform: rotate(45deg);
  border-top: 1px solid rgba(157, 78, 221, 0.2);
  border-left: 1px solid rgba(157, 78, 221, 0.2);
}

.menu-header {
  padding: 15px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.menu-header h4 {
  margin: 0;
  color: #c77dff;
  font-size: 1.1rem;
  font-weight: 600;
}

.notification-actions {
  display: flex;
  gap: 8px;
}

.action-btn {
  background: rgba(157, 78, 221, 0.1);
  border: 1px solid rgba(157, 78, 221, 0.3);
  color: #c77dff;
  width: 28px;
  height: 28px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.action-btn:hover {
  background: rgba(157, 78, 221, 0.2);
  transform: translateY(-2px);
}

.balance-display {
  display: flex;
  align-items: center;
  margin-top: 10px;
  background: rgba(157, 78, 221, 0.05);
  padding: 8px 12px;
  border-radius: 6px;
}

.balance-display i {
  color: #c77dff;
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
  background: rgba(157, 78, 221, 0.05);
}

.menu-actions .menu-item:hover {
  background: rgba(157, 78, 221, 0.15);
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
  padding: 20px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
}

.no-notifications i {
  font-size: 2rem;
  color: rgba(157, 78, 221, 0.5);
}

.loading {
  text-align: center;
  color: #c77dff;
  padding: 20px 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
}

.loading-spinner {
  width: 30px;
  height: 30px;
  border: 3px solid rgba(157, 78, 221, 0.3);
  border-radius: 50%;
  border-top-color: #c77dff;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.notification-list {
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

.notification-icon {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 12px;
  flex-shrink: 0;
}

.notification-icon-info {
  background: linear-gradient(135deg, #3b82f6, #60a5fa);
}

.notification-icon-success {
  background: linear-gradient(135deg, #10b981, #34d399);
}

.notification-icon-warning {
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
}

.notification-icon-primary {
  background: linear-gradient(135deg, #9d4edd, #c77dff);
}

.notification-icon i {
  color: white;
  font-size: 1rem;
}

.notification-item.read {
  background: rgba(255, 255, 255, 0.02);
  opacity: 0.7;
}

.notification-item:hover {
  background: rgba(157, 78, 221, 0.1);
  transform: translateY(-2px);
}

.notification-content {
  flex: 1;
}

.notification-content h5 {
  margin: 0 0 5px;
  color: #c77dff;
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
  color: #c77dff;
  font-size: 1rem;
}

.menu-item:hover {
  background: rgba(157, 78, 221, 0.1);
  color: #c77dff;
  padding-left: 20px;
}

.menu-divider {
  height: 1px;
  background: rgba(157, 78, 221, 0.1);
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

/* Modal Styles */
.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1200;
}

.modal-content {
  background: linear-gradient(135deg, #1e0b45, #0c0c1d);
  padding: 30px;
  border-radius: 12px;
  text-align: center;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(157, 78, 221, 0.3);
  max-width: 400px;
  width: 90%;
  color: #e0e0e0;
}

.modal-content h3 {
  color: #c77dff;
  margin-bottom: 15px;
  font-size: 1.5rem;
}

.modal-content p {
  margin-bottom: 20px;
  font-size: 1rem;
}

.modal-content button {
  padding: 10px 20px;
  margin: 5px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 1rem;
}

.modal-content button:first-child {
  background: linear-gradient(135deg, #9d4edd, #c77dff);
  color: white;
}

.modal-content button:first-child:hover {
  background: linear-gradient(135deg, #c77dff, #9d4edd);
  transform: translateY(-2px);
}

.modal-content button:last-child {
  background: rgba(157, 78, 221, 0.1);
  color: #c77dff;
}

.modal-content button:last-child:hover {
  background: rgba(157, 78, 221, 0.2);
  transform: translateY(-2px);
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

.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

@keyframes bounce {
  0%, 20%, 50%, 80%, 100% {
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
@media (max-width: 1200px) {
  .navbar-center {
    display: none;
  }
}

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
  .cosmo-navbar {
    height: 60px;
  }

  .navbar-container {
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
  .cosmo-navbar {
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

<template>
  <div class="cosmo-topbar">
    <div class="topbar-container">
      <!-- Left Section -->
      <div class="topbar-left">
        <div class="logo-section">
          <span class="cosmo-logo menu-toggle" @click="toggleSlidePanel">
            <a href="/" @click.prevent>GTZ</a>
            <div class="logo-glow"></div>
            <i :class="['fas', isSlidePanelOpen ? 'fa-caret-up' : 'fa-caret-down', 'cosmo-arrow-icon']"></i>
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
                    <i class="fas fa-gamepad column-icon"></i>
                    <h4 class="column-title">TRÒ CHƠI</h4>
                  </div>
                  <ul class="column-list">
                    <li><span class="hover-indicator"></span>Tài khoản của tôi</li>
                    <li><span class="hover-indicator"></span>Tài khoản đã mua</li>
                    <li><span class="hover-indicator"></span>Đấu Trường Chân Lý</li>
                    <li><span class="hover-indicator"></span>Valorant</li>
                    <li><span class="hover-indicator"></span>Huyền Thoại Runeterra</li>
                  </ul>
                </div>
                <div class="grid-column">
                  <div class="column-header">
                    <i class="fas fa-headset column-icon"></i>
                    <h4 class="column-title">DỊCH VỤ GAME</h4>
                  </div>
                  <ul class="column-list">
                    <li><span class="hover-indicator"></span>Dịch vụ của tôi</li>
                    <li><span class="hover-indicator"></span>Dịch vụ đã thuê</li>
                    <li><span class="hover-indicator"></span>Song of Nunu</li>
                    <li><span class="hover-indicator"></span>Trò Chơi Của Riot Forge</li>
                  </ul>
                </div>
                <div class="grid-column">
                  <div class="column-header">
                    <i class="fas fa-gavel column-icon"></i>
                    <h4 class="column-title">ĐẤU GIÁ</h4>
                  </div>
                  <ul class="column-list">
                    <li><span class="hover-indicator"></span><a href="/list-auction">Danh sách</a></li>
                    <li><span class="hover-indicator"></span>Phần thưởng</li>
                    <li><span class="hover-indicator"></span>Riot Games Music</li>
                  </ul>
                </div>
                <div class="grid-column">
                  <div class="column-header">
                    <i class="fas fa-comments column-icon"></i>
                    <h4 class="column-title">DIỄN ĐÀN</h4>
                  </div>
                  <ul class="column-list">
                    <li @click="$router.push('/forums')"><span class="hover-indicator"></span>Diễn đàn</li>
                    <li><span class="hover-indicator"></span>LOL Esports</li>
                    <li><span class="hover-indicator"></span>Valorant Esports</li>
                    <li><span class="hover-indicator"></span>Hỗ Trợ Riot</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </transition>
      </div>

      <!-- Right Section -->
      <div class="topbar-right">
        <!-- Recharge Button -->
        <button class="cosmo-btn recharge-btn" @click="handleRechargeClick">
          <i class="fas fa-coins"></i>
          <span>Nạp Tiền</span>
        </button>

        <!-- Add Button -->
        <div class="dropdown-container">
          <button class="cosmo-btn add-btn menu-toggle" @click.stop="toggleAddMenu">
            <i class="fas fa-plus"></i>
            <span>Thêm</span>
          </button>
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

        <!-- Transaction Icon -->
        <div class="icon-container" @click="handleTransactionClick">
          <i class="fas fa-credit-card cosmo-icon"></i>
        </div>

        <!-- Notification Icon -->
        <div class="dropdown-container">
          <div class="icon-container menu-toggle" @click.stop="toggleNotifications">
            <i class="fas fa-bell cosmo-icon">
              <span v-if="notifications" class="cosmo-badge">{{ notifications }}</span>
            </i>
          </div>
          <transition name="dropdown">
            <div v-if="isNotificationOpen" class="dropdown-menu notification-menu">
              <div class="menu-header">
                <h4>Thông báo</h4>
              </div>
              <div class="menu-content">
                <p>Không có thông báo mới</p>
              </div>
            </div>
          </transition>
        </div>

        <!-- Balance (if logged in) -->
        <div v-if="isLoggedIn" class="cosmo-balance">
          <i class="fas fa-coins"></i>
          <span>{{ formatCurrency(balance) }}</span>
        </div>

        <!-- User Not Logged In -->
        <div v-if="!isLoggedIn" class="dropdown-container">
          <div class="user-avatar menu-toggle" @click.stop="toggleAccountMenu">
            <i class="fas fa-user cosmo-icon"></i>
            <i :class="['fas', isAccountMenuOpen ? 'fa-caret-up' : 'fa-caret-down', 'cosmo-arrow-icon']"></i>
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
            <i :class="['fas', isUserMenuOpen ? 'fa-caret-up' : 'fa-caret-down', 'cosmo-arrow-icon']"></i>
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
                <span>Giao dịch tài khoản</span>
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

    <!-- Modal -->
    <transition name="modal">
      <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
        <div class="modal-content">
          <div class="modal-header">
            <h2 class="modal-title">{{ modalTitle }}</h2>
            <button class="close-btn" @click="closeModal">
              <i class="fas fa-times"></i>
            </button>
          </div>
          <div class="modal-body">
            <p class="modal-message">{{ modalMessage }}</p>
          </div>
          <div class="modal-footer">
            <button class="modal-btn login-btn" @click="modalAction">
              {{ modalActionText }}
            </button>
            <button class="modal-btn cancel-btn" @click="closeModal">
              Hủy
            </button>
          </div>
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
import { userStore } from "@/stores/auth.ts";

export default {
  name: "CosmoTopbar",
  setup() {
    const router = useRouter();
    const store = userStore();
    const isUserMenuOpen = ref(false);
    const notifications = ref(3);
    const isSlidePanelOpen = ref(false);
    const isNotificationOpen = ref(false);
    const isAddMenuOpen = ref(false);
    const isAccountMenuOpen = ref(false);
    const showModal = ref(false);
    const modalTitle = ref("Thông báo");
    const modalMessage = ref("");
    const modalActionText = ref("Đăng nhập");
    const modalAction = ref(() => {});
    const balance = ref(0);

    const isLoggedIn = computed(() => !!store.user);
    const fullName = computed(() => store.user?.fullName || "");
    const userId = computed(() => store.user?.id || 0);

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
      }).format(amount);
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
        showCustomModal(
          "Lỗi",
          "Không thể tải thông tin số dư. Vui lòng thử lại sau.",
          "Đóng",
          closeModal
        );
      }
    };

    onMounted(() => {
      store.init();
      fetchUserBalance();
      document.addEventListener("click", handleClickOutside);
    });

    const toggleAddMenu = () => {
      isAddMenuOpen.value = !isAddMenuOpen.value;
      if (isAddMenuOpen.value) {
        isUserMenuOpen.value = false;
        isNotificationOpen.value = false;
        isAccountMenuOpen.value = false;
      }
    };

    const toggleNotifications = () => {
      isNotificationOpen.value = !isNotificationOpen.value;
      if (isNotificationOpen.value) {
        notifications.value = 0;
        isUserMenuOpen.value = false;
        isAddMenuOpen.value = false;
        isAccountMenuOpen.value = false;
      }
    };

    const toggleUserMenu = () => {
      isUserMenuOpen.value = !isUserMenuOpen.value;
      if (isUserMenuOpen.value) {
        isNotificationOpen.value = false;
        isAddMenuOpen.value = false;
        isAccountMenuOpen.value = false;
      }
    };

    const toggleAccountMenu = () => {
      isAccountMenuOpen.value = !isAccountMenuOpen.value;
      if (isAccountMenuOpen.value) {
        isNotificationOpen.value = false;
        isAddMenuOpen.value = false;
        isUserMenuOpen.value = false;
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
      router.push("/purchased");
      closeAllMenus();
    };

    const logout = async () => {
      try {
        await authenticateApi.logout();
        store.logout();
        balance.value = 0;
        closeAllMenus();
        router.push("/login");
      } catch (error) {
        console.error("Logout failed:", error);
      }
    };

    const handleRechargeClick = () => {
      if (!isLoggedIn.value) {
        showModal.value = true;
        modalTitle.value = "Thông báo";
        modalMessage.value = "Vui lòng đăng nhập để tiếp tục nạp tiền.";
        modalActionText.value = "Đăng nhập";
        modalAction.value = () => {
          closeModal();
          router.push("/login");
        };
      } else {
        router.push("/recharge");
      }
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

    const handleTransactionClick = () => {
      if (!isLoggedIn.value) {
        showCustomModal(
          "Thông báo",
          "Vui lòng đăng nhập để xem giao dịch.",
          "Đăng nhập",
          () => {
            closeModal();
            router.push("/login");
          }
        );
      } else {
        router.push("/transactions");
      }
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
    };

    const handleClickOutside = (event) => {
      if (event.target.closest(".menu-toggle")) return;
      if (event.target.closest(".dropdown-menu") || event.target.closest(".slide-panel")) return;
      closeAllMenus();
    };

    const closeModal = () => {
      showModal.value = false;
    };

    const showCustomModal = (title, message, actionText, action) => {
      showModal.value = true;
      modalTitle.value = title;
      modalMessage.value = message;
      modalActionText.value = actionText;
      modalAction.value = action;
    };

    return {
      isUserMenuOpen,
      notifications,
      isSlidePanelOpen,
      isNotificationOpen,
      isAddMenuOpen,
      isAccountMenuOpen,
      isLoggedIn,
      fullName,
      balance,
      formatCurrency,
      showModal,
      modalTitle,
      modalMessage,
      modalActionText,
      modalAction,
      toggleAddMenu,
      toggleNotifications,
      toggleUserMenu,
      toggleAccountMenu,
      toggleSlidePanel,
      goToLogin,
      goToRegister,
      goToProfile,
      goToSettings,
      logout,
      handleRechargeClick,
      handleAddAccountClick,
      handleAddServiceClick,
      handleTransactionClick,
      bounceIcon,
      resetIcon,
      closeAllMenus,
      handleClickOutside,
      closeModal,
      showCustomModal,
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

.topbar-left, .topbar-right {
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

.cosmo-arrow-icon {
  font-size: 0.8rem;
  color: #00ffff;
  margin-left: 5px;
  transition: transform 0.3s ease;
}

.cosmo-logo:hover .cosmo-arrow-icon {
  color: #ff00ff;
}

.cosmo-home {
  margin-left: 15px;
  transition: transform 0.3s ease;
}

.cosmo-home:hover {
  transform: scale(1.1);
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

.recharge-btn {
  margin-left: 15px;
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

.user-avatar {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
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
  75%, 100% {
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
  border-bottom: 1px solid rgba(0, 255, 255, 0.1);
}

.menu-header h4 {
  margin: 0;
  color: #00ffff;
  font-size: 1.1rem;
  font-weight: 600;
}

.menu-content {
  padding: 15px;
  color: #e0e0e0;
  font-size: 0.95rem;
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

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.8);
  backdrop-filter: blur(5px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
}

.modal-content {
  background: linear-gradient(135deg, #0a0a20, #1a0933);
  border-radius: 15px;
  width: 90%;
  max-width: 450px;
  box-shadow: 0 15px 30px rgba(0, 0, 0, 0.3), 0 0 20px rgba(0, 255, 255, 0.3);
  border: 1px solid rgba(0, 255, 255, 0.3);
  overflow: hidden;
  position: relative;
}

.modal-content::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    radial-gradient(circle at 20% 30%, rgba(0, 255, 255, 0.1), transparent 70%),
    radial-gradient(circle at 80% 70%, rgba(255, 0, 255, 0.1), transparent 70%);
  pointer-events: none;
}

.modal-header {
  padding: 20px;
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
  position: relative;
  text-align: center;
}

.modal-title {
  margin: 0;
  color: #00ffff;
  font-size: 1.5rem;
  font-weight: 600;
  text-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.close-btn {
  position: absolute;
  top: 15px;
  right: 15px;
  background: rgba(255, 0, 0, 0.1);
  border: none;
  color: #ff5e5e;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.close-btn:hover {
  background: rgba(255, 0, 0, 0.2);
  transform: rotate(90deg);
  color: #ff0000;
}

.modal-body {
  padding: 20px;
  color: #e0e0e0;
  text-align: center;
}

.modal-message {
  font-size: 1.1rem;
  line-height: 1.5;
  margin: 0;
}

.modal-footer {
  padding: 20px;
  display: flex;
  justify-content: center;
  gap: 15px;
}

.modal-btn {
  padding: 10px 25px;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  border: none;
  min-width: 120px;
}

.login-btn {
  background: linear-gradient(135deg, #00ffff, #0088ff);
  color: #0a0a20;
}

.login-btn:hover {
  box-shadow: 0 5px 15px rgba(0, 255, 255, 0.4);
  transform: translateY(-3px);
}

.cancel-btn {
  background: rgba(255, 94, 94, 0.2);
  color: #ff5e5e;
  border: 1px solid rgba(255, 94, 94, 0.4);
}

.cancel-btn:hover {
  background: rgba(255, 94, 94, 0.3);
  transform: translateY(-3px);
}

.modal-enter-active,
.modal-leave-active {
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
  transform: scale(0.8);
}

/* Responsive Styles */
@media (max-width: 992px) {
  .slide-grid {
    grid-template-columns: repeat(2, 1fr);
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
}

@media (max-width: 576px) {
  .cosmo-topbar {
    height: 50px;
  }
  
  .cosmo-logo {
    font-size: 1.5rem;
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
}
</style>
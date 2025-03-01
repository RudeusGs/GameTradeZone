<template>
  <div class="cosmo-topbar d-flex justify-content-between align-items-center p-3 position-fixed w-100">
    <div class="d-flex align-items-center">   
      <span class="cosmo-logo menu-toggle" @click="toggleSlidePanel">
        <a href="/" @click.prevent>GTZ</a>
        <i :class="['fas', isSlidePanelOpen ? 'fa-caret-up' : 'fa-caret-down', 'ms-1', 'cosmo-arrow-icon']"></i>
      </span>
      <a href="/" class="cosmo-home">
        <img style="width: 40px; margin-left: 10px" src="../assets/logo_web.png"/>
      </a>
      <transition name="slide">
        <div v-if="isSlidePanelOpen" class="slide-panel">
          <div class="slide-content d-flex justify-content-center align-items-center">
            <div class="slide-grid">
              <div class="grid-column">
                <h4 class="column-title">TRÒ CHƠI</h4>
                <ul class="column-list">
                  <li>Liên Minh Huyền Thoại</li>
                  <li>LMHT: Tốc Chiến</li>
                  <li>Đấu Trường Chân Lý</li>
                  <li>Valorant</li>
                  <li>Huyền Thoại Runeterra</li>
                </ul>
              </div>
              <div class="grid-column">
                <h4 class="column-title">DỊCH VỤ GAME</h4>
                <ul class="column-list">
                  <li>CON/VRGENCE</li>
                  <li>Ruined King</li>
                  <li>Song of Nunu</li>
                  <li>Trò Chơi Của Riot Forge</li>
                </ul>
              </div>
              <div class="grid-column">
                <h4 class="column-title">ĐẤU GIÁ</h4>
                <ul class="column-list">
                  <li><a href="/list-auction">Danh sách</a></li>
                  <li>Vũ Trụ</li>
                  <li>Riot Games Music</li>
                </ul>
              </div>
              <div class="grid-column">
                <h4 class="column-title">DIỄN ĐÀN</h4>
                <ul class="column-list">
                  <li>Kinh Doanh</li>
                  <li>LOL Esports</li>
                  <li>Valorant Esports</li>
                  <li>Hỗ Trợ Riot</li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </transition>
    </div>
    <div class="d-flex align-items-center position-relative">
      <!-- Nút Nạp Tiền -->
      <button class="cosmo-btn" @click="handleRechargeClick">
        <i class="fas fa-coins me-1"></i>
        <span>Nạp Tiền</span>
      </button>
      <!-- Nút Thêm -->
      <div class="position-relative">
        <button class="cosmo-btn ms-2 menu-toggle" @click="toggleAddMenu">
          <i class="fas fa-plus me-1"></i> Thêm
        </button>
        <transition name="cosmo-fade">
          <div v-if="isAddMenuOpen" class="cosmo-add-menu position-absolute shadow rounded">
            <p class="mb-1" @click="handleAddAccountClick">
              <i class="fas fa-user-plus me-2"></i>Tài khoản
            </p>
            <p class="mb-0" @click="handleAddServiceClick">
              <i class="fas fa-gamepad me-2"></i>Dịch vụ
            </p>
          </div>
        </transition>
      </div>
      <i class="fas fa-calendar-alt fa-lg cosmo-icon text-light mx-2" @mouseover="bounceIcon($event)" @mouseout="resetIcon($event)"></i>
      <div class="position-relative">
        <i class="fas fa-bell fa-lg cosmo-icon text-light mx-2 menu-toggle" @click="toggleNotifications">
          <span v-if="notifications" class="cosmo-badge position-absolute top-0 start-100 translate-middle p-1">
            {{ notifications }}
          </span>
        </i>
        <transition name="cosmo-fade">
          <div v-if="isNotificationOpen" class="cosmo-notification-dropdown position-absolute shadow rounded">
            <p>No new notifications</p>
          </div>
        </transition>
      </div>
      <!-- Người dùng chưa đăng nhập -->
      <div v-if="!isLoggedIn" class="position-relative">
        <span class="menu-toggle" @click="toggleAccountMenu">
          <i class="fas fa-user fa-lg cosmo-icon text-light mx-2"></i>
          <i :class="['fas', isAccountMenuOpen ? 'fa-caret-up' : 'fa-caret-down', 'cosmo-arrow-icon']"></i>
        </span>
        <transition name="cosmo-fade">
          <div v-if="isAccountMenuOpen" class="cosmo-account-menu position-absolute shadow rounded">
            <p class="mb-1" @click="goToLogin">
              <i class="fas fa-sign-in-alt me-2"></i>Đăng nhập
            </p>
            <p class="mb-0" @click="goToRegister">
              <i class="fas fa-user-plus me-2"></i>Đăng ký
            </p>
          </div>
        </transition>
      </div>
      <!-- Người dùng đã đăng nhập -->
      <div v-else class="position-relative">
        <span class="menu-toggle" @click="toggleUserMenu">
          <span class="cosmo-user-name text-light" :title="fullName">{{ fullName }}</span>
          <i :class="['fas', isUserMenuOpen ? 'fa-caret-up' : 'fa-caret-down', 'cosmo-arrow-icon']"></i>
        </span>
        <transition name="cosmo-fade">
          <div v-if="isUserMenuOpen" class="cosmo-user-menu position-absolute shadow rounded">
            <hr class="dropdown-divider" style="border-color: rgba(0, 255, 255, 0.3);" />
            <p class="mb-1" @click="goToProfile">
              <i class="fas fa-user me-2"></i>Thông tin
            </p>
            <p class="mb-1" @click="goToSettings">
              <i class="fas fa-cog me-2"></i>Tài khoản
            </p>
            <p class="mb-0" @click="logout">
              <i class="fas fa-sign-out-alt me-2"></i>Đăng Xuất
            </p>
          </div>
        </transition>
      </div>
    </div>
    <!-- Modal thông báo cải tiến -->
    <transition name="modal-zoom">
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
            <button class="modal-btn login-btn" @click="modalAction">{{ modalActionText }}</button>
            <button class="modal-btn cancel-btn" @click="closeModal">Hủy</button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import authenticateApi from '@/api/authenticate.api';
import { userStore } from '@/stores/auth.ts';

export default {
  name: 'CosmoTopbar',
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
    const modalTitle = ref('Thông báo');
    const modalMessage = ref('');
    const modalActionText = ref('Đăng nhập');
    const modalAction = ref(() => {});

    const isLoggedIn = computed(() => !!store.user);
    const fullName = computed(() => store.fullname);

    onMounted(() => {
      store.init();
      document.addEventListener('click', handleClickOutside);
    });

    const toggleAddMenu = () => {
      isAddMenuOpen.value = !isAddMenuOpen.value;
    };

    const toggleNotifications = () => {
      isNotificationOpen.value = !isNotificationOpen.value;
      if (isNotificationOpen.value) notifications.value = 0;
    };

    const toggleUserMenu = () => {
      isUserMenuOpen.value = !isUserMenuOpen.value;
    };

    const toggleAccountMenu = () => {
      isAccountMenuOpen.value = !isAccountMenuOpen.value;
    };

    const toggleSlidePanel = () => {
      isSlidePanelOpen.value = !isSlidePanelOpen.value;
    };

    const goToLogin = () => {
      router.push('/login');
      closeAllMenus();
    };

    const goToRegister = () => {
      router.push('/register');
      closeAllMenus();
    };

    const goToProfile = () => {
      router.push('/profile');
      closeAllMenus();
    };

    const goToSettings = () => {
      router.push('/purchased');
      closeAllMenus();
    };

    const logout = async () => {
      try {
        await authenticateApi.logout();
        store.logout();
        closeAllMenus();
        router.push('/login');
      } catch (error) {
        console.error("Logout failed:", error);
      }
    };

    const handleRechargeClick = () => {
      if (!isLoggedIn.value) {
        showModal.value = true;
        modalTitle.value = 'Thông báo';
        modalMessage.value = 'Vui lòng đăng nhập để tiếp tục nạp tiền.';
        modalActionText.value = 'Đăng nhập';
        modalAction.value = () => {
          closeModal();
          router.push('/login');
        };
      } else {
        router.push('/recharge');
      }
    };

    const handleAddAccountClick = () => {
      if (!isLoggedIn.value) router.push('/login');
      else router.push('/add-account');
      closeAllMenus();
    };

    const handleAddServiceClick = () => {
      if (!isLoggedIn.value) router.push('/login');
      else router.push('/add-service');
      closeAllMenus();
    };

    const bounceIcon = (event) => {
      event.target.classList.add('animate-bounce');
    };

    const resetIcon = (event) => {
      event.target.classList.remove('animate-bounce');
    };

    const closeAllMenus = () => {
      isUserMenuOpen.value = false;
      isAddMenuOpen.value = false;
      isNotificationOpen.value = false;
      isSlidePanelOpen.value = false;
      isAccountMenuOpen.value = false;
    };

    const handleClickOutside = (event) => {
      if (event.target.closest('.menu-toggle')) return;
      if (
        event.target.closest('.cosmo-user-menu') ||
        event.target.closest('.cosmo-add-menu') ||
        event.target.closest('.cosmo-notification-dropdown') ||
        event.target.closest('.slide-panel') ||
        event.target.closest('.cosmo-account-menu')
      ) return;
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
      bounceIcon,
      resetIcon,
      closeAllMenus,
      handleClickOutside,
      closeModal,
      showCustomModal,
    };
  },
  beforeUnmount() {
    document.removeEventListener('click', this.handleClickOutside);
  },
};
</script>

<style scoped>
.cosmo-topbar {
  height: 60px;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
  z-index: 1050;
  font-family: 'Arial', sans-serif;
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.3);
  top: 0;
  border-bottom: 1px solid rgba(0, 255, 255, 0.1);
}

a {
  text-decoration: none;
  color: inherit;
}

.cosmo-icon {
  font-size: 1.3rem;
  cursor: pointer;
  padding: 10px;
  border-radius: 90%;
  background: rgba(255, 255, 255, 0.05);
}

.cosmo-btn {
  background: rgba(0, 255, 255, 0.1);
  color: #00ffff;
  padding: 8px 14px;
  font-size: 1rem;
  border: 1px solid #00ffff;
  border-radius: 10px;
  display: flex;
  align-items: center;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
  text-shadow: 0 0 5px rgba(0, 255, 255, 0.5);
}

.cosmo-btn:hover {
  background: #00ffff;
  color: #212121;
  box-shadow: 0 0 20px #00ffff;
  transform: translateY(-2px);
}

.cosmo-logo {
  margin-left: 20px;
  font-size: 2.2rem;
  font-weight: bold;
  text-transform: uppercase;
  letter-spacing: 3px;
  color: #e0e0e0;
  text-shadow: 0 0 15px #00ffff, 0 0 5px #ff00ff;
  position: relative;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
  cursor: pointer;
  display: flex;
  align-items: center;
}

.cosmo-logo:hover {
  color: #00ffff;
  transform: scale(1.05);
}

.cosmo-arrow-icon {
  font-size: 0.8rem;
  color: #00ffff;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
}

.cosmo-logo:hover .cosmo-arrow-icon {
  color: #ff00ff;
}

.cosmo-user-name {
  font-size: 0.8rem;
  cursor: pointer;
  padding: 8px;
  border-radius: 4px;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
  background: rgba(255, 255, 255, 0.05);
  max-width: 100px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  margin-right: 5px;
}

.cosmo-user-name:hover {
  background: rgba(0, 255, 255, 0.2);
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.cosmo-add-menu,
.cosmo-user-menu,
.cosmo-account-menu,
.cosmo-notification-dropdown {
  position: absolute;
  top: 100%;
  right: 0;
  width: 160px;
  padding: 10px;
  background: linear-gradient(135deg, #0d1b2a 0%, #1a0933 100%);
  color: #e0e0e0;
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 8px;
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.2);
  z-index: 999;
  backdrop-filter: blur(5px);
}

.cosmo-add-menu p,
.cosmo-user-menu p,
.cosmo-account-menu p {
  margin: 0;
  padding: 8px 12px;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
  border-radius: 4px;
}

.cosmo-add-menu p:hover,
.cosmo-user-menu p:hover,
.cosmo-account-menu p:hover {
  background: rgba(0, 255, 255, 0.2);
  color: #00ffff;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
  transform: translateX(2px);
}

.slide-panel {
  position: fixed;
  top: 60px;
  left: 0;
  width: 100%;
  max-height: 50vh;
  background: linear-gradient(135deg, #0d1b2a 0%, #1a0933 100%);
  z-index: 1040;
  box-shadow: 0 10px 20px rgba(0, 255, 255, 0.2);
  border-bottom: 1px solid rgba(0, 255, 255, 0.3);
  overflow: hidden;
}

.slide-content {
  height: 50vh;
  padding: 20px;
  text-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.slide-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 10px;
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
}

.grid-column {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(0, 255, 255, 0.1);
  border-radius: 8px;
  padding: 15px;
  backdrop-filter: blur(5px);
  transition: all 0.3s ease;
}

.grid-column:hover {
  background: rgba(0, 255, 255, 0.1);
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.3);
}

.column-title {
  font-size: 1.1rem;
  color: #00ffff;
  margin-bottom: 10px;
  text-transform: uppercase;
  text-shadow: 0 0 5px rgba(0, 255, 255, 0.5);
}

.column-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.column-list li {
  color: #e0e0e0;
  font-size: 0.9rem;
  padding: 5px 0;
  cursor: pointer;
  transition: all 0.3s ease;
}

.column-list li:hover {
  color: #00ffff;
  padding-left: 10px;
  text-shadow: 0 0 5px rgba(0, 255, 255, 0.5);
}

.slide-enter-active,
.slide-leave-active {
  transition: all 0.5s cubic-bezier(0.25, 0.8, 0.25, 1);
}

.slide-enter-from,
.slide-leave-to {
  max-height: 0;
  opacity: 0;
}

.slide-enter-to,
.slide-leave-from {
  max-height: 50vh;
  opacity: 1;
}

@keyframes bounce {
  0%, 20%, 50%, 80%, 100% { transform: translateY(0); }
  40% { transform: translateY(-10px); }
  60% { transform: translateY(-5px); }
}

.animate-bounce {
  animation: bounce 1.5s infinite;
}

.cosmo-badge {
  font-size: 0.7rem;
  background: #ff00ff;
  color: #fff;
  border-radius: 50%;
  padding: 3px 5px;
  border: none;
  text-align: center;
  min-width: 16px;
  height: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
}

.cosmo-home {
  margin-right: 10px;
  font-size: 1.5rem;
  color: #e0e0e0;
  transition: color 0.3s, transform 0.3s;
}

.cosmo-home:hover {
  color: #00ffff;
  transform: scale(1.1);
}

.cosmo-notification-dropdown {
  top: 100%;
  right: 0;
  width: 200px;
  padding: 10px;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 8px;
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.2);
  color: #e0e0e0;
  z-index: 1000;
}

.cosmo-fade-enter-active,
.cosmo-fade-leave-active {
  transition: opacity 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
}

.cosmo-fade-enter-from,
.cosmo-fade-leave-to {
  opacity: 0;
}

/* Modal Styles - Cải tiến mới */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
  animation: overlayFadeIn 0.5s ease;
}

.modal-content {
  background: linear-gradient(135deg, #1e1e2f, #2a2a40);
  border-radius: 15px; /* Giảm kích thước bo tròn */
  padding: 20px; /* Giảm padding */
  max-width: 400px; /* Thu nhỏ modal */
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.5), 0 0 10px rgba(255, 0, 122, 0.3);
  border: 1px solid #00ddeb;
  position: relative;
  overflow: hidden;
  animation: modalZoomIn 0.6s ease-out;
}

.modal-content::before {
  content: '';
  position: absolute;
  top: -2px;
  left: -2px;
  right: -2px;
  bottom: -2px;
  border-radius: 17px;
  background: linear-gradient(45deg, #00ffff, #ff00ff, #00ddeb);
  z-index: -1;
  filter: blur(5px); /* Giảm độ mờ để nhẹ nhàng hơn */
  opacity: 0.2;
  animation: neonPulse 1.5s infinite alternate;
}

.modal-header {
  text-align: center;
  margin-bottom: 15px; /* Giảm margin */
  position: relative;
}

.modal-title {
  font-size: 1.5rem; /* Giảm kích thước chữ */
  font-weight: bold;
  color: #00ffff;
  text-shadow: 0 0 8px rgba(0, 255, 255, 0.5), 0 0 3px rgba(255, 0, 122, 0.3);
  font-family: 'Arial', sans-serif; /* Dùng font cơ bản hơn */
}

.close-btn {
  position: absolute;
  top: 5px; /* Giảm khoảng cách */
  right: 10px; /* Giảm khoảng cách */
  background: none;
  border: none;
  font-size: 1.2rem; /* Giảm kích thước icon */
  color: #e0e0e0;
  cursor: pointer;
  transition: all 0.3s ease;
}

.close-btn:hover {
  color: #ff007a;
  transform: scale(1.1); /* Giảm hiệu ứng phóng to */
  text-shadow: 0 0 8px rgba(255, 0, 122, 0.6);
}

.modal-body {
  text-align: center;
  margin-bottom: 15px; /* Giảm margin */
}

.modal-message {
  font-size: 1rem; /* Giảm kích thước chữ */
  color: #e0e0e0;
  text-shadow: 0 0 5px rgba(0, 255, 255, 0.3);
  line-height: 1.4; /* Giảm khoảng cách dòng */
}

.modal-footer {
  display: flex;
  justify-content: space-between;
  gap: 10px; /* Giảm khoảng cách giữa nút */
}

.modal-btn {
  flex: 1;
  padding: 10px; /* Giảm padding */
  border-radius: 10px; /* Giảm kích thước bo tròn */
  font-size: 0.9rem; /* Giảm kích thước chữ */
  font-weight: bold;
  cursor: pointer;
  border: 1px solid transparent;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.modal-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(45deg, #00ffff, #ff00ff, #00ddeb);
  z-index: -1;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.modal-btn:hover::before {
  opacity: 0.2; /* Giảm độ sáng gradient */
}

.modal-btn:hover {
  transform: scale(1.03); /* Giảm hiệu ứng phóng to */
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.6), 0 0 10px rgba(255, 0, 122, 0.4);
  border-color: #00ddeb;
}

.login-btn {
  background: linear-gradient(135deg, #00ddeb, #33e6f2);
  color: #1e1e2f;
}

.cancel-btn {
  background: linear-gradient(135deg, #ff007a, #ff3399);
  color: #fff;
}

.modal-zoom-enter-active,
.modal-zoom-leave-active {
  transition: all 0.6s ease-out;
}

.modal-zoom-enter-from,
.modal-zoom-leave-to {
  opacity: 0;
  transform: scale(0.7); /* Hiệu ứng zoom từ nhỏ ra lớn */
}

@keyframes neonPulse {
  0% { opacity: 0.2; }
  100% { opacity: 0.4; }
}

@keyframes overlayFadeIn {
  from { opacity: 0; }
  to { opacity: 0.8; }
}

@media (max-width: 768px) {
  .cosmo-topbar {
    height: 50px;
    padding: 2px;
  }
  .cosmo-icon {
    font-size: 1.1rem;
    padding: 8px;
  }
  .cosmo-btn {
    padding: 6px 10px;
    font-size: 0.8rem;
  }
  .cosmo-logo {
    font-size: 1.8rem;
    letter-spacing: 2px;
  }
  .cosmo-arrow-icon {
    font-size: 0.6rem;
  }
  .slide-panel {
    top: 50px;
    max-height: 40vh;
  }
  .slide-content {
    height: 40vh;
    padding: 10px;
  }
  .slide-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 5px;
  }
  .grid-column {
    padding: 10px;
  }
  .column-title {
    font-size: 0.9rem;
  }
  .column-list li {
    font-size: 0.8rem;
  }
  .cosmo-user-menu,
  .cosmo-add-menu,
  .cosmo-account-menu {
    top: 50px;
    width: 140px;
    padding: 8px;
  }
  .cosmo-user-menu p,
  .cosmo-add-menu p,
  .cosmo-account-menu p {
    padding: 6px 10px;
    font-size: 0.8rem;
  }
  .cosmo-badge {
    font-size: 0.6rem;
    padding: 2px 4px;
    min-width: 14px;
    height: 14px;
  }
  .cosmo-home {
    font-size: 1.2rem;
  }
  .cosmo-notification-dropdown {
    width: 160px;
  }
  .modal-content {
    max-width: 80%; /* Thu nhỏ hơn trên mobile */
    padding: 15px;
  }
  .modal-title {
    font-size: 1.2rem;
  }
  .modal-message {
    font-size: 0.9rem;
  }
  .modal-btn {
    padding: 8px;
    font-size: 0.8rem;
  }
  .close-btn {
    font-size: 1rem;
  }
}
</style>
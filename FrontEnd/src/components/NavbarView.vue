<template>
  <div class="cosmo-topbar d-flex justify-content-between align-items-center p-3 position-fixed w-100">
    <div class="d-flex align-items-center">   
      <!-- Thêm class "menu-toggle" cho GTZ -->
      <span class="cosmo-logo menu-toggle" @click="toggleSlidePanel">
        <a href="/" @click.prevent>GTZ</a>
        <i :class="['fas', isSlidePanelOpen ? 'fa-caret-up' : 'fa-caret-down', 'ms-1', 'cosmo-arrow-icon']"></i>
      </span>
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
                  <li>Arcane</li>
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
      <a href="/" class="cosmo-home">
        <i class="fas fa-home"></i>
      </a>
      <button class="cosmo-btn">
        <i class="fas fa-coins me-1"></i>
        <a href="recharge">Nạp Tiền</a>
      </button>
      <!-- Thêm class "menu-toggle" cho nút Thêm -->
      <button class="cosmo-btn ms-2 menu-toggle" @click="toggleAddMenu">
        <i class="fas fa-plus me-1"></i> Thêm
      </button>
      <transition name="cosmo-fade">
        <div v-if="isAddMenuOpen" class="cosmo-add-menu position-absolute shadow rounded">
          <p class="mb-1"><i class="fas fa-user-plus me-2"></i><a href="/add-account">Tài khoản</a></p>
          <p class="mb-0"><i class="fas fa-gamepad me-2"></i><a href="/add-service">Dịch vụ</a></p>
        </div>
      </transition>
      <i class="fas fa-calendar-alt fa-lg cosmo-icon text-light mx-2" @mouseover="bounceIcon($event)" @mouseout="resetIcon($event)"></i>
      <div class="position-relative">
        <!-- Thêm class "menu-toggle" cho icon thông báo -->
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
      <!-- Thêm class "menu-toggle" cho icon user -->
      <i class="fas fa-user-circle fa-lg cosmo-icon text-light mx-2 menu-toggle" @click="toggleUserMenu"></i>
      <transition name="cosmo-fade">
        <div v-if="isUserMenuOpen" class="cosmo-user-menu position-absolute shadow rounded">
          <p class="mb-1" @click="goToProfile">
            <i class="fas fa-user me-2"></i><a href="/profile">Thông tin</a>
          </p>
          <p class="mb-1" @click="goToSettings">
            <i class="fas fa-cog me-2"></i><a href="/purchased">Tài khoản</a>
          </p>
          <p class="mb-0">
            <i class="fas fa-sign-out-alt me-2"></i> <a href="/login">Đăng Xuất</a>
          </p>
        </div>
      </transition>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CosmoTopbar',
  data() {
    return {
      isUserMenuOpen: false,
      notifications: 3,
      isSlidePanelOpen: false,
      isNotificationOpen: false,
      isAddMenuOpen: false,
    };
  },
  methods: {
    toggleAddMenu() {
      this.isAddMenuOpen = !this.isAddMenuOpen;
    },
    toggleSidebar() {
      this.$emit('toggleSidebar');
    },
    rotateIcon(event) {
      event.target.classList.add('animate-rotate');
    },
    pulseIcon(event) {
      event.target.classList.add('animate-pulse');
    },
    bounceIcon(event) {
      event.target.classList.add('animate-bounce');
    },
    resetIcon(event) {
      event.target.classList.remove('animate-rotate', 'animate-pulse', 'animate-bounce');
    },
    toggleNotifications() {
      this.isNotificationOpen = !this.isNotificationOpen;
      if (this.isNotificationOpen) {
        this.notifications = 0;
      }
    },
    toggleUserMenu() {
      this.isUserMenuOpen = !this.isUserMenuOpen;
    },
    toggleSlidePanel() {
      this.isSlidePanelOpen = !this.isSlidePanelOpen;
    },
    // Đóng tất cả các menu xổ xuống
    closeAllMenus() {
      this.isUserMenuOpen = false;
      this.isAddMenuOpen = false;
      this.isNotificationOpen = false;
      this.isSlidePanelOpen = false;
    },
    // Xử lý sự kiện click bên ngoài các dropdown
    handleClickOutside(event) {
      // Nếu click vào một phần tử có class "menu-toggle" thì không đóng menu
      if (event.target.closest('.menu-toggle')) {
        return;
      }
      // Nếu click vào bất kỳ dropdown nào thì không đóng menu
      if (
        event.target.closest('.cosmo-user-menu') ||
        event.target.closest('.cosmo-add-menu') ||
        event.target.closest('.cosmo-notification-dropdown') ||
        event.target.closest('.slide-panel')
      ) {
        return;
      }
      this.closeAllMenus();
    },
  },
  mounted() {
    document.addEventListener('click', this.handleClickOutside);
  },
  beforeDestroy() {
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
  border-radius: 50%;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
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

.cosmo-logo:hover::after {
  opacity: 1;
}

.cosmo-arrow-icon {
  font-size: 0.8rem;
  color: #00ffff;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
}

.cosmo-logo:hover .cosmo-arrow-icon {
  color: #ff00ff;
}

.cosmo-add-menu {
  right: 180px;
  top: 60px;
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

.cosmo-add-menu p {
  margin: 0;
  padding: 8px 12px;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
  border-radius: 4px;
}

.cosmo-add-menu p:hover {
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

@keyframes rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes pulse {
  0% { transform: scale(1); }
  50% { transform: scale(1.2); }
  100% { transform: scale(1); }
}

@keyframes bounce {
  0%, 20%, 50%, 80%, 100% { transform: translateY(0); }
  40% { transform: translateY(-10px); }
  60% { transform: translateY(-5px); }
}

.animate-rotate {
  animation: rotate 1.5s linear infinite;
}

.animate-pulse {
  animation: pulse 1.5s ease-in-out infinite;
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

.cosmo-user-menu {
  right: 10px;
  top: 60px;
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

.cosmo-user-menu p {
  margin: 0;
  padding: 8px 12px;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
  border-radius: 4px;
}

.cosmo-user-menu p:hover {
  background: rgba(0, 255, 255, 0.2);
  color: #00ffff;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
  transform: translateX(2px);
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

.cosmo-notification-dropdown p {
  margin: 0;
  padding: 5px 0;
}

.cosmo-fade-enter-active,
.cosmo-fade-leave-active {
  transition: opacity 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
}

.cosmo-fade-enter-from,
.cosmo-fade-leave-to {
  opacity: 0;
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
  .cosmo-user-menu {
    top: 50px;
    width: 140px;
    padding: 8px;
  }
  .cosmo-user-menu p {
    padding: 6px 10px;
    font-size: 0.8rem;
  }
  .cosmo-badge {
    font-size: 0.6rem;
    padding: 2px 4px;
    min-width: 14px;
    height: 14px;
  }
  .cosmo-icon:hover {
    transform: scale(1.1);
    border-radius: 50%;
  }
  .cosmo-home {
    font-size: 1.2rem;
  }
  .cosmo-notification-dropdown {
    width: 160px;
  }
}
</style>

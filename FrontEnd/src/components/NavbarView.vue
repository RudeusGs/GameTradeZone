<template>
  <div class="cosmo-topbar d-flex justify-content-between align-items-center p-3 position-fixed w-100">
    <div class="d-flex align-items-center">
      <span class="cosmo-logo" @click="toggleSlidePanel">
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
                <h4 class="column-title">QUẢN LÝ</h4>
                <ul class="column-list">
                  <li>Arcane</li>
                  <li>Vũ Trụ</li>
                  <li>Riot Games Music</li>
                </ul>
              </div>
              <div class="grid-column">
                <h4 class="column-title">RIOT GAMES</h4>
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
      <button class="cosmo-btn">
        <i class="fas fa-coins me-1"></i> <a href="recharge">Nạp Tiền</a>
      </button>
      <i class="fas fa-calendar-alt fa-lg cosmo-icon text-light mx-2" @mouseover="bounceIcon($event)" @mouseout="resetIcon($event)"></i>
      <div class="position-relative">
        <i class="fas fa-bell fa-lg cosmo-icon text-light mx-2" @click="toggleNotifications">
          <span v-if="notifications" class="cosmo-badge position-absolute top-0 start-100 translate-middle p-1">
            {{ notifications }}
          </span>
        </i>
      </div>
      <i class="fas fa-user-circle fa-lg cosmo-icon text-light mx-2" @click="toggleUserMenu"></i>
      <transition name="cosmo-fade">
        <div v-if="isUserMenuOpen" class="cosmo-user-menu position-absolute shadow rounded">
          <p class="mb-1" @click="goToProfile"><i class="fas fa-user me-2"></i><a href="/profile">Thông tin</a></p>
          <p class="mb-1" @click="goToSettings"><i class="fas fa-cog me-2"></i><a href="/purchased">Tài khoản</a></p>
          <p class="mb-0"><i class="fas fa-sign-out-alt me-2"></i> <a href="/login">Đăng Xuất</a></p>
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
    };
  },
  methods: {
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
      this.notifications = 0;
    },
    toggleUserMenu() {
      this.isUserMenuOpen = !this.isUserMenuOpen;
    },
    toggleSlidePanel() {
      this.isSlidePanelOpen = !this.isSlidePanelOpen;
    },
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
  color: #1a0933;
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

/* Slide Panel and Grid */
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
    grid-template-columns: repeat(2, 1fr); /* 2 cột trên mobile */
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
}
</style>
<template>
    <div class="topbar d-flex justify-content-between align-items-center p-4 bg-dark position-fixed w-100">
      <div class="d-flex align-items-center">
        <!-- Biểu tượng Menu -->
        <i class="fas fa-bars fa-lg text-white me-3" @click="toggleSidebar"></i>
        <!-- Logo với hiệu ứng ánh sáng -->
        <span class="logo">GTZ</span>
      </div>
      <div class="d-flex align-items-center position-relative">
        <!-- Các biểu tượng khác -->
        <i class="fas fa-sun fa-lg icon text-white mx-2" @mouseover="rotateIcon($event)" @mouseout="resetIcon($event)"></i>
        <i class="fas fa-brain fa-lg icon text-white mx-2" @mouseover="pulseIcon($event)" @mouseout="resetIcon($event)"></i>
        <i class="fas fa-calendar-alt fa-lg icon text-white mx-2" @mouseover="bounceIcon($event)" @mouseout="resetIcon($event)"></i>
        <div class="position-relative">
          <i class="fas fa-bell fa-lg icon text-white mx-2" @click="toggleNotifications">
            <!-- Badge thông báo -->
            <span v-if="notifications" class="badge bg-white text-dark position-absolute top-0 start-100 translate-middle p-1 border border-dark rounded-circle">
              {{ notifications }}
            </span>
          </i>
        </div>
        <i class="fas fa-user-circle fa-lg icon text-white mx-2" @click="toggleUserMenu"></i>
        <!-- Menu Người dùng -->
        <transition name="fade">
          <div v-if="isUserMenuOpen" class="user-menu position-absolute bg-white text-dark shadow rounded">
            <p class="mb-1" @click="goToProfile"><i class="fas fa-user me-2"></i>Profile</p>
            <p class="mb-1" @click="goToSettings"><i class="fas fa-cog me-2"></i>Settings</p>
            <p class="mb-0" @click="logout"><i class="fas fa-sign-out-alt me-2"></i>Logout</p>
          </div>
        </transition>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    name: 'Topbar',
    data() {
      return {
        isUserMenuOpen: false,
        notifications: 3,
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
      goToProfile() {
        alert('Đi đến trang hồ sơ!');
      },
      goToSettings() {
        alert('Đi đến trang cài đặt!');
      },
      logout() {
        alert('Bạn đã đăng xuất!');
      },
    },
  }
  </script>
  
  <style scoped>
  .topbar {
    height: 70px;
    background-color: #000;
    z-index: 1000;
    font-family: 'Arial', sans-serif;
  }
  
  .topbar .fa {
    font-size: 1.5rem;
    cursor: pointer;
    padding: 10px;
    border-radius: 50%;
    transition: background-color 0.3s, box-shadow 0.3s, transform 0.3s;
  }
  
  .topbar .fa:hover {
    color: #fff;
    background-color: #333;
  }
  
  .logo {
    position: relative;
    font-size: 40px;
    font-weight: bold;
    text-transform: uppercase;
    letter-spacing: 3px;
    color: #fff;
    overflow: hidden;
  }
  
  .logo::before {
    content: attr(data-text);
    position: absolute;
    left: 0;
    top: 0;
    color: #fff;
    width: 100%;
    height: 100%;
    clip-path: polygon(0 0, 0 0, 0 100%, 0 100%);
    animation: shine 3s infinite;
  }
  
  @keyframes shine {
    0% {
      clip-path: polygon(0% 0%, 0% 0%, 0% 100%, 0% 100%);
    }
    50% {
      clip-path: polygon(0% 0%, 100% 0%, 100% 100%, 0% 100%);
    }
    100% {
      clip-path: polygon(100% 0%, 100% 0%, 100% 100%, 100% 100%);
    }
  }
  
  /* Animation của biểu tượng */
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
    animation: rotate 1s linear infinite;
  }
  
  .animate-pulse {
    animation: pulse 1s ease-in-out infinite;
  }
  
  .animate-bounce {
    animation: bounce 1s infinite;
  }
  
  /* Định dạng Badge thông báo */
  .badge {
    font-size: 0.75rem;
  }
  
  .user-menu {
    right: 0;
    top: 70px;
    width: 160px;
    padding: 10px;
    z-index: 999;
  }
  
  .user-menu p {
    margin: 0;
    padding: 8px;
    cursor: pointer;
    transition: background-color 0.2s;
  }
  
  .user-menu p:hover {
    background-color: #e0e0e0;
  }
  
  /* Hiệu ứng Fade cho Menu Người dùng */
  .fade-enter-active, .fade-leave-active {
    transition: opacity 0.3s;
  }
  
  .fade-enter, .fade-leave-to {
    opacity: 0;
  }
  </style>
  
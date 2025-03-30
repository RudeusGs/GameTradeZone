<script setup lang="ts">
import { RouterView } from 'vue-router';
import NavbarView from './components/NavbarView.vue';
</script>

<template>
  <NavbarView />

  <div class="cosmo-content">
    <RouterView />
  </div>

  <!-- Messenger Support Icon -->
  <a href="https://m.me/your-messenger-id" target="_blank" class="messenger-btn" title="Chat hỗ trợ qua Messenger">
    <i class="fab fa-facebook-messenger"></i>
  </a>
</template>

<style scoped>
/* Tùy chỉnh thanh cuộn */
.cosmo-content {
  margin-top: 60px;
  margin-left: 0;
  transition: margin-left 0.3s ease;
  min-height: calc(100vh - 60px);
  overflow-y: auto; /* Đảm bảo nội dung có thể cuộn */
}

/* Tùy chỉnh thanh cuộn (Webkit - Chrome, Safari, Edge) */
.cosmo-content::-webkit-scrollbar {
  width: 12px; /* Độ rộng thanh cuộn */
  background: transparent; /* Nền trong suốt */
}

.cosmo-content::-webkit-scrollbar-track {
  background: rgba(10, 10, 10, 0.8); /* Nền track tối với độ mờ */
  border-radius: 10px; /* Bo góc */
}

.cosmo-content::-webkit-scrollbar-thumb {
  background: linear-gradient(45deg, #00ffff, #ff00ff); /* Gradient neon */
  border-radius: 10px; /* Bo góc thumb */
  border: 2px solid rgba(0, 0, 0, 0.5); /* Viền tối để tạo hiệu ứng nổi */
  box-shadow: 0 0 8px rgba(0, 255, 255, 0.5); /* Hiệu ứng glow neon */
}

.cosmo-content::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(45deg, #ff00ff, #00ffff); /* Thay đổi gradient khi hover */
  box-shadow: 0 0 12px rgba(255, 0, 255, 0.7); /* Tăng glow khi hover */
}

/* Tùy chỉnh thanh cuộn cho Firefox */
@supports (scrollbar-color: #00ffff #0a0a0a) {
  .cosmo-content {
    scrollbar-color: #00ffff #0a0a0a; /* Màu thumb và track */
    scrollbar-width: thin; /* Thanh cuộn mỏng */
  }
}

/* Đảm bảo body cũng có scrollbar đẹp nếu cần */
body {
  background-color: #121212;
  font-family: 'Arial', sans-serif;
  color: #e0e0e0;
  margin: 0;
  padding: 0;
  overflow-x: hidden;
}

body::-webkit-scrollbar {
  width: 12px;
}

body::-webkit-scrollbar-track {
  background: rgba(10, 10, 10, 0.8);
  border-radius: 10px;
}

body::-webkit-scrollbar-thumb {
  background: linear-gradient(45deg, #00ffff, #ff00ff);
  border-radius: 10px;
  border: 2px solid rgba(0, 0, 0, 0.5);
  box-shadow: 0 0 8px rgba(0, 255, 255, 0.5);
}

body::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(45deg, #ff00ff, #00ffff);
  box-shadow: 0 0 12px rgba(255, 0, 255, 0.7);
}

/* Messenger Button Styles */
.messenger-btn {
  position: fixed;
  bottom: 20px;
  right: 20px;
  width: 60px;
  height: 60px;
  background: linear-gradient(45deg, #0088cc, #00b7ff); /* Gradient màu Messenger */
  border-radius: 50%;
  display: flex;
  justify-content: center;
  align-items: center;
  box-shadow: 0 4px 15px rgba(0, 136, 204, 0.5);
  text-decoration: none;
  z-index: 1000;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.messenger-btn i {
  font-size: 2rem;
  color: #fff;
  text-shadow: 0 0 5px rgba(255, 255, 255, 0.8);
}

.messenger-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 20px rgba(0, 136, 204, 0.7);
}

/* Responsive */
@media (max-width: 768px) {
  .cosmo-sidebar {
    width: 220px;
    transform: translateX(-220px);
  }
  .cosmo-sidebar.active {
    transform: translateX(0);
  }
  .cosmo-topbar {
    height: 50px;
  }
  .cosmo-content {
    margin-top: 50px;
    margin-left: 0;
  }
  .cosmo-content.with-sidebar {
    margin-left: 220px;
  }

  /* Đảm bảo scrollbar responsive */
  .cosmo-content::-webkit-scrollbar {
    width: 8px; /* Thu nhỏ thanh cuộn trên màn hình nhỏ */
  }

  /* Messenger button responsive */
  .messenger-btn {
    width: 50px;
    height: 50px;
  }
  .messenger-btn i {
    font-size: 1.5rem;
  }
}
</style>
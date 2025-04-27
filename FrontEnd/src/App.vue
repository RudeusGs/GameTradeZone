<script setup lang="ts">
import { RouterView } from 'vue-router';
import NavbarView from './components/NavbarView.vue';
import HolographicBackground from '@/components/BackGround.vue';
</script>

<template>
  <div class="app-container">
    <HolographicBackground />
    <NavbarView />
    <div class="cosmo-content">
      <RouterView />
    </div>
    <!-- Messenger Support Icon -->
    <a href="https://m.me/your-messenger-id" target="_blank" class="messenger-btn" title="Chat hỗ trợ qua Messenger">
      <i class="fab fa-facebook-messenger"></i>
    </a>
  </div>
</template>

<style scoped>
.app-container {
  position: relative;
  min-height: 100vh;
  overflow: hidden;
}

.holographic-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: -1; /* Đặt background ở phía sau */
}

.cosmo-content {
  margin-top: 60px; /* Space for NavbarView */
  margin-left: 0;
  transition: margin-left 0.3s ease;
  min-height: calc(100vh - 60px); /* Ensure content fills viewport minus navbar */
  width: 100%; /* Full width */
  padding-bottom: 80px; /* Space for messenger-btn */
  position: relative;
  z-index: 1; /* Above background elements, below messenger-btn */
}

body {
  background-color: #121212;
  font-family: 'Arial', sans-serif;
  color: #e0e0e0;
  margin: 0;
  padding: 0;
  overflow-x: hidden; /* Prevent horizontal scroll */
}

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
  z-index: 1000; /* Above all content */
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
  .cosmo-content {
    margin-top: 50px; /* Adjust for smaller navbar */
    min-height: calc(100vh - 50px);
    padding-bottom: 60px; /* Slightly less padding for smaller screens */
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
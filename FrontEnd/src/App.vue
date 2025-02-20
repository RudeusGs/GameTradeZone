<script setup lang="ts">
import { ref } from 'vue'
import { RouterView } from 'vue-router'
import NavbarView from './components/NavbarView.vue'
import SidebarView from './components/SidebarView.vue'

const isSidebarOpen = ref(false)

const toggleSidebar = () => {
  isSidebarOpen.value = !isSidebarOpen.value
}
</script>

<template>
  <!-- NavbarView -->
  <NavbarView @toggleSidebar="toggleSidebar" />

  <!-- Sidebar -->
  <SidebarView :isSidebarOpen="isSidebarOpen" />

  <!-- Main Content Area -->
  <div class="content" :class="{ 'with-sidebar': isSidebarOpen }">
    <RouterView />
  </div>
</template>

<style scoped>
body {
  background-color: #121212;
  font-family: 'Roboto', sans-serif;
  color: #e0e0e0;
  margin: 0;
  padding: 0;
}

.content {
  padding: 100px 20px 20px;
  transition: margin-left 0.3s ease;
}

.content.with-sidebar {
  margin-left: 260px;
}

h1 {
  font-size: 2rem;
  font-weight: 700;
}

@media (max-width: 768px) {
  .sidebar {
    width: 220px;
    transform: translateX(-220px);
  }
  .sidebar.active {
    transform: translateX(0);
  }
  .content.with-sidebar {
    margin-left: 0;
  }
}
</style>

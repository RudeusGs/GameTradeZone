<script setup lang="ts">
import { ref } from 'vue'
import { RouterView } from 'vue-router'
import NavbarView from './components/NavbarView.vue'

</script>

<template>
  <NavbarView/>


  <div class="cosmo-content">
    <RouterView />
  </div>
</template>

<style scoped>
body {
  background-color: #121212;
  font-family: 'Arial', sans-serif;
  color: #e0e0e0;
  margin: 0;
  padding: 0;
  overflow-x: hidden;
}

.cosmo-content {
  margin-top: 60px;
  margin-left: 0;
  transition: margin-left 0.3s ease;
  min-height: calc(100vh - 60px);
}

.cosmo-content.with-sidebar {
  margin-left: 280px;
}

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
}
</style>
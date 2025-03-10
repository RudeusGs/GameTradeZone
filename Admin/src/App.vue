<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import AdminNavbar from '@/components/NavbarView.vue';
import AdminSidebar from '@/components/SidebarView.vue';

// Router
const router = useRouter();

// Trạng thái thu gọn sidebar
const isCollapsed = ref<boolean>(false);

// Hàm toggle sidebar
const toggleSidebar = () => {
  isCollapsed.value = !isCollapsed.value;
};

// Kiểm tra trạng thái đăng nhập dựa trên token trong localStorage
const isAuthenticated = computed(() => {
  return !!localStorage.getItem('token');
});

// Nếu không có token, chuyển hướng về /login khi truy cập trang khác
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  if (!token && to.path !== '/login') {
    next('/login');
  } else {
    next();
  }
});
</script>

<template>
  <div>
    <!-- Hiển thị Sidebar và Navbar chỉ khi đã đăng nhập -->
    <template v-if="isAuthenticated">
      <AdminSidebar :isCollapsed="isCollapsed" @toggle="toggleSidebar" />
      <div class="main-content" :class="{ collapsed: isCollapsed }">
        <AdminNavbar :isCollapsed="isCollapsed" />
        <RouterView />
      </div>
    </template>

    <!-- Chỉ hiển thị RouterView (trang login) khi chưa đăng nhập -->
    <RouterView v-else />
  </div>
</template>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.main-content {
  margin-left: 220px;
  transition: margin-left 0.3s ease;
}

.main-content.collapsed {
  margin-left: 70px; /* Đã điều chỉnh từ 60px lên 70px trước đó */
}

@media (max-width: 768px) {
  .main-content {
    margin-left: 70px;
  }

  .main-content.collapsed {
    margin-left: 70px;
  }
}

:deep(.router-view) {
  padding-top: 60px;
  padding-left: 20px;
  padding-right: 20px;
}
</style>
<template>
    <div :class="['sidebar', { 'active': isSidebarOpen }, 'bg-dark', 'text-light', 'p-4']">
      <h5 class="sidebar-title fs-5 text-white mb-4">Menu</h5>
      <ul class="nav flex-column">
        <li class="nav-item">
          <a class="nav-link text-light" href="#"><i class="fas fa-home me-3"></i> Dashboard</a>
        </li>
        <li class="nav-item">
          <a class="nav-link text-light" href="#"><i class="fas fa-users me-3"></i> Users</a>
        </li>
        <li class="nav-item">
          <a class="nav-link text-light" href="#"><i class="fas fa-envelope me-3"></i> Messages</a>
        </li>
        <li class="nav-item">
          <a class="nav-link text-light" href="#"><i class="fas fa-bookmark me-3"></i> Bookmark</a>
        </li>
        <li class="nav-item">
          <a class="nav-link text-light" href="#"><i class="fas fa-file me-3"></i> Files</a>
        </li>
        <li class="nav-item">
          <a class="nav-link text-light" href="#"><i class="fas fa-chart-bar me-3"></i> Stats</a>
        </li>
        <li class="nav-item">
          <a class="nav-link text-light" href="#"><i class="fas fa-sign-out-alt me-3"></i> SignOut</a>
        </li>
      </ul>
    </div>
  </template>
  
  <script>
  export default {
    name: 'Sidebar',
    props: {
      isSidebarOpen: Boolean
    }
  }
  </script>
  
  <style scoped>
  .sidebar {
    position: fixed;
    top: 70px;
    left: 0;
    width: 260px;
    height: 100%;
    background-color: #212121;
    padding: 20px;
    transition: transform 0.3s ease;
    transform: translateX(-260px);
    z-index: 999;
  }
  
  .sidebar.active {
    transform: translateX(0);
  }
  
  .sidebar .sidebar-title {
    font-size: 1.4rem;
    color: #fff;
  }
  
  .sidebar .nav-item {
    margin-bottom: 1rem;
  }
  
  .sidebar .nav-link {
    color: #d1d1d1;
    display: flex;
    align-items: center;
  }
  
  .sidebar .nav-link:hover {
    background-color: #383838;
    border-radius: 4px;
  }
  
  .sidebar .fa {
    margin-right: 15px;
  }
  
  .sidebar .nav-link .text-light {
    font-size: 1.1rem;
  }
  </style>
  
<template>
    <div class="loading-overlay" v-if="isLoading">
      <div class="loading-spinner">
        <div class="spinner-circle spinner-circle-outer"></div>
        <div class="spinner-circle spinner-circle-inner"></div>
        <p class="loading-text">Đang tải...</p>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    name: 'Loading',
    props: {
      isLoading: {
        type: Boolean,
        default: false,
      },
    },
  };
  </script>
  
  <style scoped>
  .loading-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.7);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 2000;
    backdrop-filter: blur(5px);
  }
  
  .loading-spinner {
    position: relative;
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  
  .spinner-circle {
    position: absolute;
    border: 4px solid transparent;
    border-radius: 50%;
    animation: spin 1.5s linear infinite;
  }
  
  .spinner-circle-outer {
    width: 60px;
    height: 60px;
    border-top: 4px solid #00ffff;
    border-right: 4px solid #ff00ff;
  }
  
  .spinner-circle-inner {
    width: 40px;
    height: 40px;
    border-top: 4px solid #00ffff;
    border-right: 4px solid #ff00ff;
    animation-direction: reverse;
  }
  
  .loading-text {
    margin-top: 70px;
    color: #00ffff;
    font-size: 1.2rem;
    font-weight: bold;
    text-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
    animation: pulse-text 1.5s ease-in-out infinite;
  }
  
  @keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
  }
  
  @keyframes pulse-text {
    0% { opacity: 0.6; }
    50% { opacity: 1; }
    100% { opacity: 0.6; }
  }
  </style>
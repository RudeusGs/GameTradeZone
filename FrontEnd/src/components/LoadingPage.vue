<template>
    <div class="cosmo-loading">
      <div class="energy-core">
        <div class="core-center"></div>
        <div class="core-ring ring1"></div>
        <div class="core-ring ring2"></div>
        <div class="core-ring ring3"></div>
        <div class="energy-particles">
          <div v-for="n in 8" :key="n" class="particle" :style="{ '--i': n }"></div>
        </div>
        <div class="scanning-line"></div>
      </div>
  
      <div class="loading-status">
        <div class="loading-bar">
          <div class="progress-track">
            <div class="progress-fill" :style="{ width: `${progress}%` }"></div>
          </div>
          <div class="progress-glow"></div>
        </div>
        <div class="loading-text">{{ message }}</div>
        <div class="loading-percentage">{{ Math.floor(progress) }}%</div>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted, onUnmounted, computed } from 'vue';
  
  const props = defineProps({
    message: {
      type: String,
      default: 'INITIALIZING SYSTEM'
    },
    loadingTime: {
      type: Number,
      default: 3000
    }
  });
  
  const progress = ref(0);
  const intervalId = ref(null);
  const message = computed(() => props.message.toUpperCase());
  
  onMounted(() => {
    // Simulate loading progress
    const increment = 100 / (props.loadingTime / 16);
    intervalId.value = setInterval(() => {
      progress.value += increment;
  
      if (progress.value >= 100) {
        progress.value = 100;
        clearInterval(intervalId.value);
      }
    }, 16);
  });
  
  onUnmounted(() => {
    if (intervalId.value) {
      clearInterval(intervalId.value);
    }
  });
  </script>
  
  <style scoped>
  .cosmo-loading {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 3rem;
    perspective: 1000px;
  }
  
  /* Energy Core */
  .energy-core {
    position: relative;
    width: 180px;
    height: 180px;
    transform-style: preserve-3d;
    animation: coreFloat 4s infinite ease-in-out;
  }
  
  @keyframes coreFloat {
    0%, 100% { transform: translateY(0) rotateX(15deg); }
    50% { transform: translateY(-10px) rotateX(20deg); }
  }
  
  .core-center {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: radial-gradient(circle, #ffffff 0%, #00fbff 40%, #0058ff 100%);
    box-shadow:
      0 0 15px #00fbff,
      0 0 30px #00fbff,
      0 0 50px rgba(0, 251, 255, 0.5);
    z-index: 10;
    animation: pulseCore 2s infinite alternate;
  }
  
  @keyframes pulseCore {
    0% {
      box-shadow:
        0 0 15px #00fbff,
        0 0 30px #00fbff,
        0 0 50px rgba(0, 251, 255, 0.5);
    }
    100% {
      box-shadow:
        0 0 20px #00fbff,
        0 0 40px #00fbff,
        0 0 70px rgba(0, 251, 255, 0.7);
    }
  }
  
  .core-ring {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    border-radius: 50%;
    border-style: solid;
    border-color: transparent;
    box-shadow: 0 0 20px rgba(0, 251, 255, 0.5);
    opacity: 0.8;
  }
  
  .ring1 {
    width: 70px;
    height: 70px;
    border-width: 4px;
    border-left-color: #00fbff;
    border-right-color: #0058ff;
    animation: spinRing 3s linear infinite;
  }
  
  .ring2 {
    width: 110px;
    height: 110px;
    border-width: 3px;
    border-top-color: #00fbff;
    border-bottom-color: #ff00aa;
    animation: spinRing 5s linear infinite reverse;
  }
  
  .ring3 {
    width: 150px;
    height: 150px;
    border-width: 2px;
    border-left-color: #0058ff;
    border-right-color: #ff00aa;
    animation: spinRing 7s linear infinite;
  }
  
  @keyframes spinRing {
    0% { transform: translate(-50%, -50%) rotate(0); }
    100% { transform: translate(-50%, -50%) rotate(360deg); }
  }
  
  .energy-particles .particle {
    position: absolute;
    top: 50%;
    left: 50%;
    width: 2px;
    height: 50px;
    background: linear-gradient(to top, transparent, #00fbff);
    transform-origin: bottom;
    animation: particleMove 2s infinite;
    transform: translate(-50%, -100%) rotate(calc(var(--i) * 45deg)) translateY(-20px);
  }
  
  .energy-particles .particle::before {
    content: '';
    position: absolute;
    top: 0;
    left: 50%;
    transform: translateX(-50%);
    width: 6px;
    height: 6px;
    background: #00fbff;
    border-radius: 50%;
    box-shadow: 0 0 10px #00fbff;
  }
  
  @keyframes particleMove {
    0%, 100% { height: 40px; opacity: 1; }
    50% { height: 60px; opacity: 0.5; }
  }
  
  .scanning-line {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 4px;
    background: linear-gradient(90deg,
      transparent 0%,
      #00fbff 20%,
      #ffffff 50%,
      #00fbff 80%,
      transparent 100%);
    animation: scanMove 2s infinite;
    opacity: 0.7;
    z-index: 5;
  }
  
  @keyframes scanMove {
    0% { top: 0; }
    100% { top: 100%; }
  }
  
  /* Loading Status */
  .loading-status {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1rem;
    width: 280px;
  }
  
  .loading-bar {
    position: relative;
    width: 100%;
    height: 6px;
    background: rgba(0, 0, 30, 0.3);
    border-radius: 3px;
    overflow: hidden;
  }
  
  .progress-track {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    overflow: hidden;
    border-radius: 3px;
  }
  
  .progress-fill {
    height: 100%;
    background: linear-gradient(90deg, #0058ff, #00fbff, #00fbff, #0058ff);
    background-size: 200% 100%;
    border-radius: 3px;
    animation: gradientShift 2s linear infinite;
    transition: width 0.1s linear;
    position: relative;
  }
  
  .progress-fill::after {
    content: '';
    position: absolute;
    top: 0;
    right: 0;
    width: 10px;
    height: 100%;
    background: #ffffff;
    opacity: 0.8;
    filter: blur(3px);
  }
  
  @keyframes gradientShift {
    0% { background-position: 0% 0%; }
    100% { background-position: 200% 0%; }
  }
  
  .progress-glow {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    box-shadow: 0 0 10px rgba(0, 251, 255, 0.5);
    pointer-events: none;
    opacity: 0.8;
  }
  
  .loading-text {
    font-family: 'Orbitron', 'Exo 2', sans-serif;
    font-size: 1rem;
    font-weight: 500;
    letter-spacing: 1px;
    color: #ffffff;
    text-transform: uppercase;
    text-shadow: 0 0 10px rgba(0, 251, 255, 0.7);
    animation: textPulse 2s infinite alternate;
  }
  
  @keyframes textPulse {
    0% { opacity: 0.7; text-shadow: 0 0 10px rgba(0, 251, 255, 0.7); }
    100% { opacity: 1; text-shadow: 0 0 15px rgba(0, 251, 255, 1); }
  }
  
  .loading-percentage {
    font-family: 'Orbitron', 'Exo 2', sans-serif;
    font-size: 1.8rem;
    font-weight: 700;
    color: #00fbff;
    text-shadow: 0 0 15px rgba(0, 251, 255, 0.7);
  }
  
  /* Media Queries */
  @media (max-width: 480px) {
    .energy-core {
      width: 150px;
      height: 150px;
    }
  
    .core-center {
      width: 35px;
      height: 35px;
    }
  
    .loading-status {
      width: 240px;
    }
  }
  </style>
  
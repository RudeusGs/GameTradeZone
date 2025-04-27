<template>
    <div class="holographic-background">
      <div class="cyber-grid"></div>
      <div class="floating-elements">
        <div v-for="i in 20" :key="`hex-${i}`" class="hex-element" :style="getHexStyle(i)"></div>
        <div v-for="i in 15" :key="`circle-${i}`" class="circle-element" :style="getCircleStyle(i)"></div>
      </div>
      <div class="particle-background">
        <div v-for="i in 80" :key="`particle-${i}`" class="particle" :style="getParticleStyle(i)"></div>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  
  export default defineComponent({
    name: 'HolographicBackground',
    methods: {
      getHexStyle(i: number) {
        const size = Math.random() * 30 + 20;
        const left = `${Math.random() * 100}%`;
        const top = `${Math.random() * 100}%`;
        const delay = Math.random() * 8;
        const duration = Math.random() * 15 + 10;
        const color = i % 2 === 0 ? '#00f2ff' : '#ff00ff';
        const opacity = Math.random() * 0.15 + 0.05;
        return {
          width: `${size}px`,
          height: `${size}px`,
          left,
          top,
          borderColor: color,
          opacity,
          animationDelay: `${delay}s`,
          animationDuration: `${duration}s`,
        };
      },
      getCircleStyle(i: number) {
        const size = Math.random() * 50 + 30;
        const left = `${Math.random() * 100}%`;
        const top = `${Math.random() * 100}%`;
        const delay = Math.random() * 10;
        const duration = Math.random() * 20 + 15;
        const color = i % 2 === 0 ? '#00f2ff' : '#ff00ff';
        const opacity = Math.random() * 0.1 + 0.05;
        return {
          width: `${size}px`,
          height: `${size}px`,
          left,
          top,
          borderColor: color,
          opacity,
          animationDelay: `${delay}s`,
          animationDuration: `${duration}s`,
        };
      },
      getParticleStyle(i: number) {
        const size = Math.random() * 3 + 1;
        const left = `${Math.random() * 100}%`;
        const top = `${Math.random() * 100}%`;
        const delay = Math.random() * 5;
        const duration = Math.random() * 10 + 5;
        const color = i % 3 === 0 ? '#00f2ff' : i % 3 === 1 ? '#ff00ff' : '#ffffff';
        const opacity = Math.random() * 0.5 + 0.3;
        return {
          width: `${size}px`,
          height: `${size}px`,
          left,
          top,
          backgroundColor: color,
          opacity,
          animationDelay: `${delay}s`,
          animationDuration: `${duration}s`,
        };
      },
    },
  });
  </script>
  
  <style scoped>
  .holographic-background {
    position: absolute;
    inset: 0;
    z-index: 0;
    overflow: hidden;
  }
  
  .cyber-grid {
    position: absolute;
    inset: 0;
    background-image: 
      radial-gradient(circle at center, rgba(0, 242, 255, 0.15) 0, rgba(0, 0, 0, 0) 60%),
      linear-gradient(to right, rgba(0, 242, 255, 0.05) 1px, transparent 1px),
      linear-gradient(to bottom, rgba(0, 242, 255, 0.05) 1px, transparent 1px);
    background-size: 100% 100%, 40px 40px, 40px 40px;
    animation: gridPulse 8s infinite alternate;
  }
  
  @keyframes gridPulse {
    0% { opacity: 0.5; }
    50% { opacity: 0.8; }
    100% { opacity: 0.5; }
  }
  
  .floating-elements {
    position: absolute;
    inset: 0;
    overflow: hidden;
  }
  
  .hex-element {
    position: absolute;
    clip-path: polygon(50% 0%, 100% 25%, 100% 75%, 50% 100%, 0% 75%, 0% 25%);
    border: 1px solid;
    background-color: transparent;
    animation: float 20s infinite linear;
  }
  
  .circle-element {
    position: absolute;
    border-radius: 50%;
    border: 1px solid;
    background-color: transparent;
    animation: float 25s infinite linear;
  }
  
  .particle-background {
    position: absolute;
    inset: 0;
    z-index: 0;
    overflow: hidden;
  }
  
  .particle {
    position: absolute;
    border-radius: 50%;
    animation: float 10s infinite ease-in-out;
  }
  
  @keyframes float {
    0% { transform: translateY(0) rotate(0deg); opacity: 0.8; }
    50% { transform: translateY(-100px) rotate(180deg); opacity: 0.3; }
    100% { transform: translateY(0) rotate(360deg); opacity: 0.8; }
  }
  </style>
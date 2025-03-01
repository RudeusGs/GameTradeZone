<template>
    <span>{{ timeRemaining }}</span>
  </template>
  
  <script>
  export default {
    props: {
      endDate: {
        type: String,
        required: true,
      },
    },
    data() {
      return {
        timeRemaining: '',
        interval: null,
      };
    },
    mounted() {
      this.updateTimer();
      this.interval = setInterval(this.updateTimer, 1000);
    },
    beforeDestroy() {
      clearInterval(this.interval);
    },
    methods: {
      updateTimer() {
        const now = new Date().getTime();
        const end = new Date(this.endDate).getTime();
        const distance = end - now;
        if (distance < 0) {
          this.timeRemaining = 'Đã kết thúc';
          return;
        }
        const days = Math.floor(distance / (1000 * 60 * 60 * 24));
        const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((distance % (1000 * 60)) / 1000);
        this.timeRemaining = `${days} ngày ${hours} giờ ${minutes} phút ${seconds} giây`;
      },
    },
  };
  </script>
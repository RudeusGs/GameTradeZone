<script setup lang="ts">
import { ref, computed } from 'vue';

// Interface cho Account
interface Account {
  id: number;
  seller: string;
  game: string;
  name: string;
  price: number;
  bid?: number;
  status: 'open' | 'bidding' | 'sold';
  image: string;
  rarity: 'common' | 'rare' | 'epic';
}

// Interface cho Service
interface Service {
  id: number;
  creator: string;
  game: string;
  name: string;
  price: number;
  description: string;
}

const accounts = ref<Account[]>([
  { id: 1, seller: 'UserA', game: 'Valorant', name: 'Rank Immortal', price: 50, status: 'open', image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRj-KgZqlq-0ZnUWBWWOjLtoqkM5GzynxVdVA&s', rarity: 'rare' },
  { id: 2, seller: 'UserB', game: 'Genshin Impact', name: 'AR 50 + 5*', price: 80, bid: 75, status: 'bidding', image: 'https://via.placeholder.com/400x250?text=Genshin', rarity: 'epic' },
  { id: 3, seller: 'UserA', game: 'Valorant', name: 'Rank Immortal', price: 50, status: 'open', image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRj-KgZqlq-0ZnUWBWWOjLtoqkM5GzynxVdVA&s', rarity: 'rare' },
]);

const services = ref<Service[]>([
  { id: 1, creator: 'UserC', game: 'Valorant', name: 'Cày rank lên Immortal', price: 30, description: 'Cày từ rank hiện tại lên Immortal trong 7 ngày.' },
  { id: 2, creator: 'UserD', game: 'Genshin Impact', name: 'Leo cấp AR 50', price: 40, description: 'Leo từ AR 1 lên 50, bao gồm cày nguyên liệu.' },
  { id: 3, creator: 'UserE', game: 'LoL', name: 'Vượt ải rank', price: 25, description: 'Giúp bạn vượt 5 trận rank bất kỳ.' },
]);

const selectedGame = ref<string>('All');
const searchQuery = ref<string>('');

const filteredAccounts = computed(() => {
  let result = accounts.value;
  if (selectedGame.value !== 'All') {
    result = result.filter(account => account.game === selectedGame.value);
  }
  if (searchQuery.value) {
    result = result.filter(account =>
      account.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      account.game.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }
  return result;
});

const filteredServices = computed(() => {
  let result = services.value;
  if (selectedGame.value !== 'All') {
    result = result.filter(service => service.game === selectedGame.value);
  }
  if (searchQuery.value) {
    result = result.filter(service =>
      service.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      service.game.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }
  return result;
});
</script>

<template>
  <div class="trade-container">
    <!-- Particle Background -->
    <div class="particle-background">
      <div v-for="i in 20" :key="i" class="particle"></div>
    </div>

    <!-- Header -->
    <header class="header">
      <div class="header-content">
        <h1 class="header-title">GameTradeZone</h1>
        <p class="header-subtitle">Nơi giao dịch tài khoản và dịch vụ game đỉnh cao</p>
        <div class="search-bar">
          <input v-model="searchQuery" type="text" placeholder="Tìm kiếm tài khoản hoặc dịch vụ..." class="search-input" />
          <i class="fas fa-search search-icon"></i>
        </div>
      </div>
    </header>

    <!-- Filter Bar -->
    <section class="filter-bar">
      <div class="filter-options">
        <button :class="{ 'active': selectedGame === 'All' }" @click="selectedGame = 'All'">Tất cả</button>
        <button :class="{ 'active': selectedGame === 'Valorant' }" @click="selectedGame = 'Valorant'">Valorant</button>
        <button :class="{ 'active': selectedGame === 'Genshin Impact' }" @click="selectedGame = 'Genshin Impact'">Genshin</button>
        <button :class="{ 'active': selectedGame === 'LoL' }" @click="selectedGame = 'LoL'">LoL</button>
      </div>
    </section>

    <!-- Account Gallery -->
    <section class="account-gallery">
      <h2 class="section-title">Tài khoản đang bán</h2>
      <div class="gallery-wrapper">
        <div v-for="account in filteredAccounts" :key="account.id" class="account-item" :class="account.rarity">
          <div class="item-image">
            <img :src="account.image" :alt="account.name" />
            <div class="image-overlay">
              <span class="status-badge" :class="account.status">{{ account.status }}</span>
            </div>
          </div>
          <div class="item-details">
            <h2 class="item-name">{{ account.name }}</h2>
            <p class="item-game">{{ account.game }}</p>
            <p class="item-seller">Người bán: {{ account.seller }}</p>
            <div class="price-box">
              <span class="item-price">${{ account.price }}</span>
              <span v-if="account.bid" class="item-bid">Bid: ${{ account.bid }}</span>
            </div>
            <div class="item-actions">
              <button class="bid-btn" :disabled="account.status === 'sold'">Trả giá</button>
              <button class="buy-btn" :disabled="account.status === 'sold'">Mua ngay</button>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Services Gallery -->
    <section class="services-gallery">
      <h2 class="section-title">Dịch vụ nổi bật</h2>
      <div class="gallery-wrapper">
        <div v-for="service in filteredServices" :key="service.id" class="service-item">
          <div class="service-details">
            <h3 class="service-name">{{ service.name }}</h3>
            <p class="service-game">{{ service.game }}</p>
            <p class="service-creator">Người tạo: {{ service.creator }}</p>
            <p class="service-description">{{ service.description }}</p>
            <div class="service-price-box">
              <span class="service-price">${{ service.price }}</span>
            </div>
            <button class="hire-btn">Thuê ngay</button>
          </div>
        </div>
      </div>
    </section>

    <!-- Banner bổ sung để đỡ trống -->
    <section class="banner-section">
      <div class="banner-content">
        <h3 class="banner-title">Tham gia ngay hôm nay!</h3>
        <p class="banner-text">Đăng ký để trải nghiệm dịch vụ và giao dịch tốt nhất!</p>
        <button class="banner-btn">Đăng ký ngay</button>
      </div>
    </section>
  </div>
</template>

<style scoped>
/* Tổng thể */
.trade-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%); /* Đồng bộ gradient với navbar */
  font-family: 'Arial', sans-serif; /* Đồng bộ font với navbar */
  color: #f0f0f0; /* Màu chữ xám nhạt, tương tự navbar */
  position: relative;
  overflow: hidden;
}

/* Particle Background */
.particle-background {
  position: absolute;
  inset: 0;
  z-index: -1;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%); /* Giữ gradient nhưng đồng bộ */
}

.particle {
  position: absolute;
  width: 5px;
  height: 5px;
  background: rgba(0, 179, 224, 0.5); /* Thay màu particle thành #00b3e0, đồng bộ với navbar */
  border-radius: 50%;
  animation: float 10s infinite ease-in-out;
}

.particle:nth-child(odd) {
  background: rgba(255, 0, 255, 0.5); /* Thay màu magenta để đồng bộ với navbar */
}

.particle:nth-child(1) { left: 10%; top: 20%; animation-duration: 12s; }
.particle:nth-child(2) { left: 20%; top: 80%; animation-duration: 15s; }
.particle:nth-child(3) { left: 30%; top: 50%; animation-duration: 8s; }
.particle:nth-child(4) { left: 40%; top: 10%; animation-duration: 10s; }
.particle:nth-child(5) { left: 50%; top: 70%; animation-duration: 13s; }
.particle:nth-child(6) { left: 60%; top: 30%; animation-duration: 9s; }
.particle:nth-child(7) { left: 70%; top: 90%; animation-duration: 11s; }
.particle:nth-child(8) { left: 80%; top: 40%; animation-duration: 14s; }
.particle:nth-child(9) { left: 90%; top: 60%; animation-duration: 7s; }
.particle:nth-child(10) { left: 15%; top: 25%; animation-duration: 16s; }
.particle:nth-child(11) { left: 25%; top: 85%; animation-duration: 12s; }
.particle:nth-child(12) { left: 35%; top: 45%; animation-duration: 10s; }
.particle:nth-child(13) { left: 45%; top: 15%; animation-duration: 8s; }
.particle:nth-child(14) { left: 55%; top: 75%; animation-duration: 13s; }
.particle:nth-child(15) { left: 65%; top: 35%; animation-duration: 9s; }
.particle:nth-child(16) { left: 75%; top: 95%; animation-duration: 11s; }
.particle:nth-child(17) { left: 85%; top: 55%; animation-duration: 14s; }
.particle:nth-child(18) { left: 95%; top: 65%; animation-duration: 7s; }
.particle:nth-child(19) { left: 5%; top: 40%; animation-duration: 15s; }
.particle:nth-child(20) { left: 15%; top: 60%; animation-duration: 10s; }

@keyframes float {
  0% { transform: translateY(0) scale(1); opacity: 0.8; }
  50% { transform: translateY(-100vh) scale(1.5); opacity: 0.3; }
  100% { transform: translateY(0) scale(1); opacity: 0.8; }
}

/* Header */
.header {
  padding: 60px 20px;
  text-align: center;
  background: linear-gradient(to bottom, rgba(26, 9, 51, 0.9), transparent); /* Đồng bộ gradient với navbar */
  position: relative;
  z-index: 1;
  box-shadow: 0 0 20px rgba(0, 204, 255, 0.2); /* Bóng đổ nhẹ, đồng bộ với navbar */
}

.header-content {
  max-width: 800px;
  margin: 0 auto;
}

.header-title {
  font-size: 3.5rem;
  font-weight: 800;
  letter-spacing: 3px; /* Tăng letter-spacing để đồng bộ với navbar */
  color: #f8f8f8; /* Màu chữ kem nhạt, đồng bộ với navbar */
  text-shadow: 0 0 15px #00b3e0, 0 0 5px #ff00ff; /* Glow giống logo navbar */
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55); /* Đồng bộ transition với navbar */
}

.header-title:hover {
  color: #00b3e0; /* Màu hover đồng bộ với navbar */
  transform: scale(1.05); /* Hiệu ứng zoom nhẹ, đồng bộ với navbar */
}

.header-subtitle {
  font-size: 1.4rem;
  color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
  margin-bottom: 30px;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
}

.search-bar {
  position: relative;
  max-width: 500px;
  margin: 0 auto;
}

.search-input {
  width: 100%;
  padding: 15px 50px 15px 20px;
  background: rgba(28, 37, 38, 0.9); /* Nền trong suốt hơn, đồng bộ với navbar */
  border: 1px solid #00b3e0; /* Viền cyan, đồng bộ với navbar */
  border-radius: 8px; /* Bo tròn ít hơn, đồng bộ với navbar */
  color: #f0f0f0;
  font-size: 1.1rem;
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4); /* Bóng đổ nhẹ, đồng bộ với navbar */
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
}

.search-input:focus {
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.6); /* Bóng đổ focus, đồng bộ với navbar */
  outline: none;
  border-color: #ff00ff; /* Màu focus magenta, đồng bộ với navbar */
}

.search-icon {
  position: absolute;
  right: 20px;
  top: 50%;
  transform: translateY(-50%);
  color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
  font-size: 1.2rem;
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
}

.search-icon:hover {
  color: #ff00ff; /* Màu hover magenta, đồng bộ với navbar */
}

/* Filter Bar */
.filter-bar {
  padding: 20px 40px;
  background: rgba(13, 27, 42, 0.9); /* Nền trong suốt hơn, đồng bộ với navbar */
  border-bottom: 1px solid #00b3e0; /* Viền cyan, đồng bộ với navbar */
  position: relative;
  z-index: 1;
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2); /* Bóng đổ nhẹ, đồng bộ với navbar */
}

.filter-options {
  display: flex;
  justify-content: center;
  gap: 20px;
  flex-wrap: wrap;
}

.filter-options button {
  background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
  color: #f0f0f0; /* Màu chữ xám nhạt, đồng bộ với navbar */
  padding: 10px 25px;
  border: 1px solid #00b3e0; /* Viền cyan, đồng bộ với navbar */
  border-radius: 8px; /* Bo tròn ít hơn, đồng bộ với navbar */
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
}

.filter-options button.active,
.filter-options button:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff); /* Gradient màu navbar */
  color: #f8f8f8; /* Màu chữ sáng, đồng bộ với navbar */
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4); /* Bóng đổ hover, đồng bộ với navbar */
  transform: translateY(-1px); /* Hiệu ứng nâng nhẹ, đồng bộ với navbar */
}

/* Section Titles */
.section-title {
  font-size: 2rem;
  font-weight: 700;
  color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
  text-align: center;
  margin-bottom: 30px;
  text-shadow: 0 0 10px #00b3e0, 0 0 5px #ff00ff; /* Glow giống navbar */
  position: relative;
  z-index: 1;
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
}

.section-title:hover {
  transform: scale(1.05); /* Hiệu ứng zoom nhẹ, đồng bộ với navbar */
}

/* Account Gallery */
.account-gallery {
  padding: 60px 40px;
  position: relative;
  z-index: 1;
}

.gallery-wrapper {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 30px;
  max-width: 1400px;
  margin: 0 auto;
}

.account-item {
  width: 400px;
  background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
  border-radius: 8px; /* Bo tròn ít hơn, đồng bộ với navbar */
  overflow: hidden;
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
  position: relative;
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2); /* Bóng đổ nhẹ, đồng bộ với navbar */
}

.account-item:hover {
  transform: translateY(-5px); /* Giảm hiệu ứng nâng, đồng bộ với navbar */
  box-shadow: 0 0 20px rgba(0, 204, 255, 0.4); /* Bóng đổ hover, đồng bộ với navbar */
}


.item-image {
  position: relative;
}

.item-image img {
  width: 100%;
  height: 250px;
  object-fit: cover;
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
}

.account-item:hover .item-image img {
  filter: brightness(110%);
}

.image-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(to top, rgba(26, 9, 51, 0.7), transparent); /* Đồng bộ gradient với navbar */
}

.status-badge {
  position: absolute;
  top: 15px;
  right: 15px;
  padding: 6px 12px;
  border-radius: 8px; /* Bo tròn ít hơn, đồng bộ với navbar */
  font-size: 0.9rem;
  text-transform: uppercase;
  background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.3); /* Bóng đổ nhẹ, đồng bộ với navbar */
}

.status-badge.open { color: #00b3e0; } /* Màu cyan, đồng bộ với navbar */
.status-badge.bidding { color: #ff00ff; } /* Màu magenta, đồng bộ với navbar */
.status-badge.sold { color: #808080; } /* Màu xám, đồng bộ với navbar */

.item-details {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.item-name {
  font-size: 1.6rem;
  font-weight: 700;
  color: #f8f8f8; /* Màu chữ sáng, đồng bộ với navbar */
  margin: 0;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
}

.item-game {
  font-size: 1.1rem;
  color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
}

.item-seller {
  font-size: 0.95rem;
  color: #b0b0b0; /* Màu chữ xám nhạt hơn, đồng bộ với navbar */
}

.price-box {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.item-price {
  font-size: 1.5rem;
  font-weight: 700;
  color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
}

.item-bid {
  font-size: 1.3rem;
  color: #ff00ff; /* Màu magenta, đồng bộ với navbar */
}

.item-actions {
  display: flex;
  gap: 15px;
}

.bid-btn, .buy-btn {
  flex: 1;
  padding: 12px;
  border: 1px solid #00b3e0; /* Viền cyan, đồng bộ với navbar */
  border-radius: 8px; /* Bo tròn ít hơn, đồng bộ với navbar */
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
  background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
  color: #f0f0f0; /* Màu chữ xám nhạt, đồng bộ với navbar */
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
}

.bid-btn:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff); /* Gradient màu navbar */
  color: #f8f8f8; /* Màu chữ sáng, đồng bộ với navbar */
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4); /* Bóng đổ hover, đồng bộ với navbar */
}

.buy-btn:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff); /* Gradient màu navbar */
  color: #f8f8f8; /* Màu chữ sáng, đồng bộ với navbar */
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4); /* Bóng đổ hover, đồng bộ với navbar */
}

.bid-btn:disabled, .buy-btn:disabled {
  background: rgba(28, 37, 38, 0.5); /* Nền mờ hơn khi disabled, đồng bộ với navbar */
  color: #808080; /* Màu xám, đồng bộ với navbar */
  border-color: #808080; /* Viền xám, đồng bộ với navbar */
  cursor: not-allowed;
}

/* Services Gallery */
.services-gallery {
  padding: 60px 40px;
  background: rgba(13, 27, 42, 0.9); /* Nền trong suốt hơn, đồng bộ với navbar */
  position: relative;
  z-index: 1;
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2); /* Bóng đổ nhẹ, đồng bộ với navbar */
}

.service-item {
  width: 400px;
  background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
  border-radius: 8px; /* Bo tròn ít hơn, đồng bộ với navbar */
  overflow: hidden;
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2); /* Bóng đổ nhẹ, đồng bộ với navbar */
}

.service-item:hover {
  transform: translateY(-5px); /* Giảm hiệu ứng nâng, đồng bộ với navbar */
  box-shadow: 0 0 20px rgba(0, 204, 255, 0.4); /* Bóng đổ hover, đồng bộ với navbar */
}

.service-details {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.service-name {
  font-size: 1.6rem;
  font-weight: 700;
  color: #f8f8f8; /* Màu chữ sáng, đồng bộ với navbar */
  margin: 0;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
}

.service-game {
  font-size: 1.1rem;
  color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
}

.service-creator {
  font-size: 0.95rem;
  color: #b0b0b0; /* Màu chữ xám nhạt hơn, đồng bộ với navbar */
}

.service-description {
  font-size: 0.9rem;
  color: #b0b0b0; /* Màu chữ xám nhạt, đồng bộ với navbar */
  line-height: 1.4;
}

.service-price-box {
  margin: 10px 0;
}

.service-price {
  font-size: 1.5rem;
  font-weight: 700;
  color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
}

.hire-btn {
  width: 100%;
  padding: 12px;
  background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
  color: #f0f0f0; /* Màu chữ xám nhạt, đồng bộ với navbar */
  border: 1px solid #00b3e0; /* Viền cyan, đồng bộ với navbar */
  border-radius: 8px; /* Bo tròn ít hơn, đồng bộ với navbar */
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
}

.hire-btn:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff); /* Gradient màu navbar */
  color: #f8f8f8; /* Màu chữ sáng, đồng bộ với navbar */
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4); /* Bóng đổ hover, đồng bộ với navbar */
}

/* Banner Section */
.banner-section {
  padding: 40px 40px;
  text-align: center;
  background: rgba(13, 27, 42, 0.9); /* Nền trong suốt hơn, đồng bộ với navbar */
  position: relative;
  z-index: 1;
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2); /* Bóng đổ nhẹ, đồng bộ với navbar */
}

.banner-content {
  max-width: 800px;
  margin: 0 auto;
}

.banner-title {
  font-size: 2rem;
  font-weight: 700;
  color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
  text-shadow: 0 0 10px #00b3e0, 0 0 5px #ff00ff; /* Glow giống navbar */
}

.banner-text {
  font-size: 1.2rem;
  color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
  margin: 10px 0 20px;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
}

.banner-btn {
  padding: 12px 30px;
  background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
  color: #f0f0f0; /* Màu chữ xám nhạt, đồng bộ với navbar */
  border: 1px solid #00b3e0; /* Viền cyan, đồng bộ với navbar */
  border-radius: 8px; /* Bo tròn ít hơn, đồng bộ với navbar */
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease-in-out; /* Đồng bộ transition với navbar */
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Bóng chữ nhẹ, đồng bộ với navbar */
}

.banner-btn:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff); /* Gradient màu navbar */
  color: #f8f8f8; /* Màu chữ sáng, đồng bộ với navbar */
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4); /* Bóng đổ hover, đồng bộ với navbar */
}

/* Responsive */
@media (max-width: 768px) {
  .header-title { font-size: 2.5rem; }
  .header-subtitle { font-size: 1.2rem; }
  .account-item, .service-item { width: 100%; max-width: 350px; }
  .section-title { font-size: 1.6rem; }
  .banner-title { font-size: 1.6rem; }
  .banner-text { font-size: 1rem; }
  .particle { width: 3px; height: 3px; animation-duration: 8s; }
}
</style>
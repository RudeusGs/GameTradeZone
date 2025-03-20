<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import gameApi from '@/api/gameinfor.api';
import gameAccountApi from '@/api/gameaccount.api';
import gamefieldApi from '@/api/gamefield.api';

// Interface cho GameInfor từ API
interface Game {
  id: number;
  name: string;
  image: string;
}

// Interface cho AccountGame từ API
interface Account {
  id: number;
  seller: string;
  game: string;
  name: string;
  price: number;
  status: string;
  image: string;
  fields: { fieldName: string; fieldValue: string }[];
  priceMin: number;
}

// Interface cho Service (giả định)
interface Service {
  id: number;
  creator: string;
  game: string;
  name: string;
  price: number;
  description: string;
}

const selectedGame = ref<string>('All');
const searchQuery = ref<string>('');
const isLoading = ref<boolean>(false);
const errorMessage = ref<string>('');

const games = ref<Game[]>([]);
const accounts = ref<Account[]>([]);
const services = ref<Service[]>([]);

// Hàm lấy URL hình ảnh đầy đủ
const getFullImageUrl = (imageString: string | null | undefined): string => {
  if (!imageString || imageString.trim() === '') return 'https://via.placeholder.com/400x250';
  const baseUrl = 'https://localhost:7232/';
  const images = imageString.split(';').filter(img => img.trim() !== '');
  return images.length > 0 ? `${baseUrl}${images[0]}` : 'https://via.placeholder.com/400x250';
};

// Lấy danh sách game từ API
const fetchGames = async () => {
  try {
    isLoading.value = true;
    const response = await gameApi.getAll();
    if (response.data?.result?.isSuccess && response.data.result.data) {
      games.value = response.data.result.data.map((game: any) => ({
        id: game.id,
        name: game.gameName,
        image: getFullImageUrl(game.image),
      }));
    } else {
      errorMessage.value = 'Không thể lấy danh sách game';
    }
  } catch (error) {
    errorMessage.value = 'Lỗi khi gọi API game';
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

// Lấy danh sách tài khoản game từ API và thông tin liên quan
const fetchAccounts = async () => {
  try {
    isLoading.value = true;
    const response = await gameAccountApi.getAll();
    if (response.data?.result?.isSuccess && response.data.result.data) {
      const accountPromises = response.data.result.data.map(async (account: any) => {
        let sellerName = 'Unknown';
        let gameName = 'Unknown';

        // Lấy thông tin người bán
        try {
          const userResponse = await gameAccountApi.getInforUser(account.id);
          if (userResponse.data?.result?.isSuccess && userResponse.data.result.data) {
            sellerName = userResponse.data.result.data.userName || 'Unknown';
          }
        } catch (error) {
          console.error(`Lỗi khi lấy thông tin người bán cho tài khoản ${account.id}:`, error);
        }

        // Lấy tên game từ GameInfor
        try {
          const game = games.value.find(g => g.id === account.gameInforID);
          gameName = game ? game.name : 'Unknown';
        } catch (error) {
          console.error(`Lỗi khi lấy tên game cho tài khoản ${account.id}:`, error);
        }

        // Lấy danh sách fields và giá trị cho account này
        let fields: { fieldName: string; fieldValue: string }[] = [];
        try {
          // Lấy tất cả GameField của game
          const fieldResponse = await gamefieldApi.getField(account.gameInforID);
          if (fieldResponse.data?.result?.isSuccess && fieldResponse.data.result.data) {
            const fieldPromises = fieldResponse.data.result.data.map(async (field: any) => {
              try {
                // Lấy fieldValue cho account và field cụ thể
                const valueResponse = await gameAccountApi.getByIdForGame(account.id, field.id);
                if (
                  valueResponse.data?.result?.isSuccess &&
                  valueResponse.data.result.data?.length > 0 &&
                  valueResponse.data.result.data[0].fieldValue
                ) {
                  return {
                    fieldName: field.fieldName,
                    fieldValue: valueResponse.data.result.data[0].fieldValue,
                  };
                }
                return null; // Không có giá trị, không thêm vào danh sách
              } catch (error) {
                console.error(`Lỗi khi lấy giá trị của field ${field.id} cho tài khoản ${account.id}:`, error);
                return null;
              }
            });
            const fieldResults = await Promise.all(fieldPromises);
            // Lọc bỏ các kết quả null để chỉ giữ lại field có giá trị
            fields = fieldResults.filter(result => result !== null) as { fieldName: string; fieldValue: string }[];
          }
        } catch (error) {
          console.error(`Lỗi khi lấy danh sách fields cho game ${account.gameInforID}:`, error);
        }

        return {
          id: account.id,
          seller: sellerName,
          game: gameName,
          name: account.accountName,
          price: account.price,
          status: account.status || 'Đang bán',
          image: getFullImageUrl(account.image),
          fields,
          priceMin: account.priceMin || 0,
        };
      });
      accounts.value = await Promise.all(accountPromises);
    } else {
      errorMessage.value = 'Không thể lấy danh sách tài khoản';
    }
  } catch (error) {
    errorMessage.value = 'Lỗi khi gọi API tài khoản';
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

// Dữ liệu dịch vụ tĩnh (giả định)
const fetchServices = () => {
  services.value = [
    { id: 1, creator: 'UserC', game: 'Valorant', name: 'Cày rank lên Immortal', price: 30, description: 'Cày từ rank hiện tại lên Immortal trong 7 ngày.' },
    { id: 2, creator: 'UserD', game: 'Genshin Impact', name: 'Leo cấp AR 50', price: 40, description: 'Leo từ AR 1 lên 50, bao gồm cày nguyên liệu.' },
    { id: 3, creator: 'UserE', game: 'LoL', name: 'Vượt ải rank', price: 25, description: 'Giúp bạn vượt 5 trận rank bất kỳ.' },
  ];
};

// Gọi API khi component được mounted
onMounted(() => {
  fetchGames().then(() => fetchAccounts()); // Đảm bảo lấy games trước accounts
  fetchServices();
});

// Computed properties cho filtered accounts
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

// Computed properties cho filtered services
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
        <div class="header-title-container">
          <div class="pyramid-loader">
            <div class="wrapper">
              <span class="side side1"></span>
              <span class="side side2"></span>
              <span class="side side3"></span>
              <span class="side side4"></span>
              <span class="shadow"></span>
            </div>  
          </div>
          <h1 class="header-title">GameTradeZone</h1>
        </div>
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
        <button v-for="game in games" :key="game.id" :class="{ 'active': selectedGame === game.name }" @click="selectedGame = game.name">{{ game.name }}</button>
      </div>
    </section>

    <!-- Loading/Error State -->
    <div v-if="isLoading" class="loading">Đang tải dữ liệu...</div>
    <div v-if="errorMessage" class="error">{{ errorMessage }}</div>

    <!-- Account Gallery -->
    <section class="account-gallery">
      <h2 class="section-title">Tài khoản đang bán</h2>
      <div class="gallery-wrapper">
        <div v-for="account in filteredAccounts" :key="account.id" class="account-item">
          <div class="item-image">
            <img :src="account.image" :alt="account.game" />
            <div class="image-overlay">
              <span class="status-badge" :class="account.status">{{ account.status }}</span>
            </div>
          </div>
          <div class="item-details">
            <p class="item-game">{{ account.game }}</p>
            <p class="item-seller">Người bán: {{ account.seller }}</p>
            <div class="price-box">
              <span class="item-price">{{ account.price }} VND</span>
              <span class="item-price-min">Giá nhỏ nhất: {{ account.priceMin }} VND</span>
            </div>
            <div class="item-fields">
              <p v-for="field in account.fields" :key="field.fieldName">{{ field.fieldName }}: {{ field.fieldValue }}</p>
            </div>
            <div class="item-actions">
              <button class="bid-btn" :disabled="account.status === 'Đã bán'">Trả giá</button>
              <button class="buy-btn" :disabled="account.status === 'Đã bán'">Mua ngay</button>
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
              <span class="service-price">{{ service.price }} VND</span>
            </div>
            <button class="hire-btn">Thuê ngay</button>
          </div>
        </div>
      </div>
    </section>

    <!-- Banner bổ sung -->
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
/* Pyramid Loader */
.pyramid-loader {
  width: 100px;
  height: 100px;
  display: block;
  transform-style: preserve-3d;
  transform: rotateX(-20deg);
  margin-right: 20px;
}

.wrapper {
  position: relative;
  width: 100%;
  height: 100%;
  transform-style: preserve-3d;
  animation: spin 4s linear infinite;
}

@keyframes spin {
  100% {
    transform: rotateY(360deg);
  }
}

.pyramid-loader .wrapper .side {
  width: 40px;
  height: 40px;
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  margin: auto;
  transform-origin: center top;
  clip-path: polygon(50% 0%, 0% 100%, 100% 100%);
}

.pyramid-loader .wrapper .side1 {
  transform: rotateZ(-30deg) rotateY(90deg);
  background: conic-gradient(#2BDEAC, #F028FD, #D8CCE6, #2F2585);
}

.pyramid-loader .wrapper .side2 {
  transform: rotateZ(30deg) rotateY(90deg);
  background: conic-gradient(#2F2585, #D8CCE6, #F028FD, #2BDEAC);
}

.pyramid-loader .wrapper .side3 {
  transform: rotateX(30deg);
  background: conic-gradient(#2F2585, #D8CCE6, #F028FD, #2BDEAC);
}

.pyramid-loader .wrapper .side4 {
  transform: rotateX(-30deg);
  background: conic-gradient(#2BDEAC, #F028FD, #D8CCE6, #2F2585);
}

.pyramid-loader .wrapper .shadow {
  width: 30px;
  height: 30px;
  background: #8B5AD5;
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  margin: auto;
  transform: rotateX(90deg) translateZ(-20px);
  filter: blur(8px);
}

/* Tổng thể */
.trade-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
  font-family: 'Arial', sans-serif;
  color: #f0f0f0;
  position: relative;
  overflow: hidden;
}

/* Particle Background */
.particle-background {
  position: absolute;
  inset: 0;
  z-index: -1;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
}

.particle {
  position: absolute;
  width: 5px;
  height: 5px;
  background: rgba(0, 179, 224, 0.5);
  border-radius: 50%;
  animation: float 10s infinite ease-in-out;
}

.particle:nth-child(odd) {
  background: rgba(255, 0, 255, 0.5);
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
  background: linear-gradient(to bottom, rgba(26, 9, 51, 0.9), transparent);
  position: relative;
  z-index: 1;
  box-shadow: 0 0 20px rgba(0, 204, 255, 0.2);
}

.header-content {
  max-width: 800px;
  margin: 0 auto;
}

.header-title-container {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 20px;
}

.header-title {
  font-size: 3.5rem;
  font-weight: 800;
  letter-spacing: 3px;
  color: #f8f8f8;
  text-shadow: 0 0 15px #00b3e0, 0 0 5px #ff00ff;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
  margin: 0;
}

.header-title:hover {
  color: #00b3e0;
  transform: scale(1.05);
}

.header-subtitle {
  font-size: 1.4rem;
  color: #e0e0e0;
  margin-bottom: 30px;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3);
}

.search-bar {
  position: relative;
  max-width: 500px;
  margin: 0 auto;
}

.search-input {
  width: 100%;
  padding: 15px 50px 15px 20px;
  background: rgba(28, 37, 38, 0.9);
  border: 1px solid #00b3e0;
  border-radius: 8px;
  color: #f0f0f0;
  font-size: 1.1rem;
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4);
  transition: all 0.2s ease-in-out;
}

.search-input:focus {
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.6);
  outline: none;
  border-color: #ff00ff;
}

.search-icon {
  position: absolute;
  right: 20px;
  top: 50%;
  transform: translateY(-50%);
  color: #00b3e0;
  font-size: 1.2rem;
  transition: all 0.2s ease-in-out;
}

.search-icon:hover {
  color: #ff00ff;
}

/* Filter Bar */
.filter-bar {
  padding: 20px 40px;
  background: rgba(13, 27, 42, 0.9);
  border-bottom: 1px solid #00b3e0;
  position: relative;
  z-index: 1;
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2);
}

.filter-options {
  display: flex;
  justify-content: center;
  gap: 20px;
  flex-wrap: wrap;
}

.filter-options button {
  background: rgba(28, 37, 38, 0.9);
  color: #f0f0f0;
  padding: 10px 25px;
  border: 1px solid #00b3e0;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s ease-in-out;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3);
}

.filter-options button.active,
.filter-options button:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4);
  transform: translateY(-1px);
}

/* Section Titles */
.section-title {
  font-size: 2rem;
  font-weight: 700;
  color: #00b3e0;
  text-align: center;
  margin-bottom: 30px;
  text-shadow: 0 0 10px #00b3e0, 0 0 5px #ff00ff;
  position: relative;
  z-index: 1;
  transition: all 0.2s ease-in-out;
}

.section-title:hover {
  transform: scale(1.05);
}

/* Loading/Error State */
.loading,
.error {
  text-align: center;
  padding: 20px;
  font-size: 1.2rem;
  color: #ff00ff;
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
  background: rgba(28, 37, 38, 0.9);
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.2s ease-in-out;
  position: relative;
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2);
}

.account-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 0 20px rgba(0, 204, 255, 0.4);
}

.item-image {
  position: relative;
}

.item-image img {
  width: 100%;
  height: 250px;
  object-fit: cover;
  transition: all 0.2s ease-in-out;
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
  background: linear-gradient(to top, rgba(26, 9, 51, 0.7), transparent);
}

.status-badge {
  position: absolute;
  top: 15px;
  right: 15px;
  padding: 6px 12px;
  border-radius: 8px;
  font-size: 0.9rem;
  text-transform: uppercase;
  background: rgba(28, 37, 38, 0.9);
  box-shadow: 0 0 5px rgba(0, 204, 255, 0.3);
}

.status-badge.open { color: #00b3e0; }
.status-badge.bidding { color: #ff00ff; }
.status-badge.sold { color: #808080; }

.item-details {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.item-game {
  font-size: 1.1rem;
  color: #e0e0e0;
}

.item-seller {
  font-size: 0.95rem;
  color: #b0b0b0;
}

.price-box {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.item-price {
  font-size: 1.5rem;
  font-weight: 700;
  color: #00b3e0;
}

.item-price-min {
  font-size: 1rem;
  color: #b0b0b0;
}

.item-fields {
  font-size: 0.9rem;
  color: #e0e0e0;
}

.item-actions {
  display: flex;
  gap: 15px;
}

.bid-btn, .buy-btn {
  flex: 1;
  padding: 12px;
  border: 1px solid #00b3e0;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s ease-in-out;
  background: rgba(28, 37, 38, 0.9);
  color: #f0f0f0;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3);
}

.bid-btn:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4);
}

.buy-btn:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4);
}

.bid-btn:disabled, .buy-btn:disabled {
  background: rgba(28, 37, 38, 0.5);
  color: #808080;
  border-color: #808080;
  cursor: not-allowed;
}

/* Services Gallery */
.services-gallery {
  padding: 60px 40px;
  background: rgba(13, 27, 42, 0.9);
  position: relative;
  z-index: 1;
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2);
}

.service-item {
  width: 400px;
  background: rgba(28, 37, 38, 0.9);
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.2s ease-in-out;
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2);
}

.service-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 0 20px rgba(0, 204, 255, 0.4);
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
  color: #f8f8f8;
  margin: 0;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3);
}

.service-game {
  font-size: 1.1rem;
  color: #e0e0e0;
}

.service-creator {
  font-size: 0.95rem;
  color: #b0b0b0;
}

.service-description {
  font-size: 0.9rem;
  color: #b0b0b0;
  line-height: 1.4;
}

.service-price-box {
  margin: 10px 0;
}

.service-price {
  font-size: 1.5rem;
  font-weight: 700;
  color: #00b3e0;
}

.hire-btn {
  width: 100%;
  padding: 12px;
  background: rgba(28, 37, 38, 0.9);
  color: #f0f0f0;
  border: 1px solid #00b3e0;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease-in-out;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3);
}

.hire-btn:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4);
}

/* Banner Section */
.banner-section {
  padding: 40px 40px;
  text-align: center;
  background: rgba(13, 27, 42, 0.9);
  position: relative;
  z-index: 1;
  box-shadow: 0 0 15px rgba(0, 204, 255, 0.2);
}

.banner-content {
  max-width: 800px;
  margin: 0 auto;
}

.banner-title {
  font-size: 2rem;
  font-weight: 700;
  color: #00b3e0;
  text-shadow: 0 0 10px #00b3e0, 0 0 5px #ff00ff;
}

.banner-text {
  font-size: 1.2rem;
  color: #e0e0e0;
  margin: 10px 0 20px;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3);
}

.banner-btn {
  padding: 12px 30px;
  background: rgba(28, 37, 38, 0.9);
  color: #f0f0f0;
  border: 1px solid #00b3e0;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease-in-out;
  text-shadow: 0 0 3px rgba(255, 255, 255, 0.3);
}

.banner-btn:hover {
  background: linear-gradient(45deg, #00b3e0, #ff00ff);
  color: #f8f8f8;
  box-shadow: 0 0 12px rgba(0, 204, 255, 0.4);
}

/* Responsive */
@media (max-width: 768px) {
  .header-title-container {
    flex-direction: column;
    margin-bottom: 10px;
  }
  .pyramid-loader {
    margin-right: 0;
    margin-bottom: 15px;
  }
  .header-title { 
    font-size: 2.5rem; 
  }
  .header-subtitle { 
    font-size: 1.2rem; 
  }
  .account-item, .service-item { 
    width: 100%; 
    max-width: 350px; 
  }
  .section-title { 
    font-size: 1.6rem; 
  }
  .banner-title { 
    font-size: 1.6rem; 
  }
  .banner-text { 
    font-size: 1rem; 
  }
  .particle { 
    width: 3px; 
    height: 3px; 
    animation-duration: 8s; 
  }
}
</style>
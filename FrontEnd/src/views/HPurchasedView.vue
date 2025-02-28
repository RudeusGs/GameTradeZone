<script setup lang="ts">
import { ref, computed } from 'vue';

// Interface cho Purchase History
interface Purchase {
  id: number;
  seller: string;
  accountName: string;
  password: string;
  images: string[];
  game: string;
  purchaseDate: string;
  status: 'completed' | 'processing' | 'cancelled';
}

// Dữ liệu mẫu
const purchases = ref<Purchase[]>([
  { id: 1, seller: 'UserA', accountName: 'Rank Immortal', password: 'pass123', images: ['https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRj-KgZqlq-0ZnUWBWWOjLtoqkM5GzynxVdVA&s'], game: 'Valorant', purchaseDate: '2025-02-27T14:30:00', status: 'completed' },
  { id: 2, seller: 'UserB', accountName: 'AR 50 + 5*', password: 'genshin2023', images: ['https://via.placeholder.com/400x250?text=Genshin'], game: 'Genshin Impact', purchaseDate: '2025-02-26T09:15:00', status: 'processing' },
  { id: 3, seller: 'UserC', accountName: 'Challenger LOL', password: 'lolpro99', images: ['https://via.placeholder.com/150?text=LOL'], game: 'LoL', purchaseDate: '2025-02-25T20:45:00', status: 'cancelled' },
  { id: 4, seller: 'UserD', accountName: 'Rank Radiant', password: 'rad123', images: ['https://via.placeholder.com/150?text=Val'], game: 'Valorant', purchaseDate: '2025-02-24T10:00:00', status: 'completed' },
]);

// Trạng thái hiển thị mật khẩu
const showPasswords = ref<boolean[]>(purchases.value.map(() => false));

// Tìm kiếm
const searchQuery = ref<string>('');

// Phân trang
const itemsPerPage = ref<number>(5);
const currentPage = ref<number>(1);

// Lọc dữ liệu dựa trên tìm kiếm
const filteredPurchases = computed(() => {
  let result = purchases.value;
  if (searchQuery.value) {
    result = result.filter(purchase =>
      purchase.accountName.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      purchase.seller.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      purchase.game.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }
  return result;
});

// Dữ liệu phân trang
const paginatedPurchases = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return filteredPurchases.value.slice(start, end);
});

// Tổng số trang
const totalPages = computed(() => Math.ceil(filteredPurchases.value.length / itemsPerPage.value));

// Chuyển trang
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

// Toggle hiển thị mật khẩu
const togglePassword = (index: number) => {
  showPasswords.value[index] = !showPasswords.value[index];
};

// Định dạng ngày giờ
const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
};
</script>

<template>
  <div class="purchase-history-container">
    <!-- Particle Background -->
    <div class="particle-background">
      <div v-for="i in 20" :key="i" class="particle"></div>
    </div>

    <!-- Header -->
    <header class="header">
      <h1 class="header-title">Lịch sử mua tài khoản</h1>
      <p class="header-subtitle">Xem lại các giao dịch của bạn</p>
      <div class="search-bar">
        <input v-model="searchQuery" type="text" placeholder="Tìm kiếm theo tên tài khoản, người bán, game..." class="search-input" />
        <i class="fas fa-search search-icon"></i>
      </div>
    </header>

    <!-- Purchase Table -->
    <section class="purchase-table">
      <div v-if="filteredPurchases.length === 0" class="empty-message">Không tìm thấy giao dịch nào</div>
      <div v-else class="table-wrapper">
        <table class="history-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Người đăng</th>
              <th>Tên tài khoản</th>
              <th>Mật khẩu</th>
              <th>Hình ảnh</th>
              <th>Tên game</th> <!-- Đã thêm cột Tên game -->
              <th>Ngày mua</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(purchase, index) in paginatedPurchases" :key="purchase.id">
              <td>{{ purchase.id }}</td>
              <td>{{ purchase.seller }}</td>
              <td>{{ purchase.accountName }}</td>
              <td>
                <span v-if="showPasswords[index]">{{ purchase.password }}</span>
                <span v-else>********</span>
                <i
                  :class="['fas', showPasswords[index] ? 'fa-eye-slash' : 'fa-eye', 'toggle-password']"
                  @click="togglePassword(index)"
                ></i>
              </td>
              <td>
                <div class="image-gallery">
                  <img v-for="(image, imgIndex) in purchase.images" :key="imgIndex" :src="image" :alt="`Image ${imgIndex + 1}`" />
                </div>
              </td>
              <td>{{ purchase.game }}</td>
              <td>{{ formatDate(purchase.purchaseDate) }}</td>
              <td :class="purchase.status">
                {{ purchase.status === 'completed' ? 'Hoàn tất' : purchase.status === 'processing' ? 'Đang xử lý' : 'Hủy' }}
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Pagination -->
        <div class="pagination">
          <button :disabled="currentPage === 1" @click="goToPage(currentPage - 1)">Trước</button>
          <span>Trang {{ currentPage }} / {{ totalPages }}</span>
          <button :disabled="currentPage === totalPages" @click="goToPage(currentPage + 1)">Sau</button>
        </div>
      </div>
    </section>
  </div>
</template>

<style scoped>
/* Tổng thể */
.purchase-history-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%);
  font-family: 'Arial', sans-serif;
  color: #e0e0e0;
  position: relative;
  overflow: hidden;
  padding-top: 60px; /* Để tránh chồng lên topbar */
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
  padding: 40px 20px;
  text-align: center;
  background: linear-gradient(to bottom, rgba(26, 9, 51, 0.9), transparent);
  position: relative;
  z-index: 1;
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.3);
}

.header-title {
  font-size: 3rem;
  font-weight: 700;
  letter-spacing: 3px;
  color: #e0e0e0;
  text-shadow: 0 0 15px #00ffff, 0 0 5px #ff00ff;
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.27, 1.55);
}

.header-title:hover {
  color: #00ffff;
  transform: scale(1.05);
}

.header-subtitle {
  font-size: 1.2rem;
  color: #b0b0b0;
  margin: 10px 0 20px;
}

.search-bar {
  position: relative;
  max-width: 500px;
  margin: 0 auto;
}

.search-input {
  width: 100%;
  padding: 12px 40px 12px 15px;
  background: rgba(28, 37, 38, 0.9);
  border: 1px solid #00ffff;
  border-radius: 8px;
  color: #e0e0e0;
  font-size: 1rem;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.3);
  transition: all 0.3s ease;
}

.search-input:focus {
  border-color: #ff00ff;
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.5);
  outline: none;
}

.search-icon {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: #00ffff;
  font-size: 1rem;
}

/* Purchase Table */
.purchase-table {
  padding: 40px;
  position: relative;
  z-index: 1;
}

.empty-message {
  text-align: center;
  font-size: 1.5rem;
  color: #808080;
}

.table-wrapper {
  max-width: 1400px;
  margin: 0 auto;
  background: rgba(28, 37, 38, 0.9);
  border-radius: 8px;
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.3);
  overflow: hidden;
}

.history-table {
  width: 100%;
  border-collapse: collapse;
}

.history-table th,
.history-table td {
  padding: 15px;
  text-align: left;
  border-bottom: 1px solid rgba(0, 255, 255, 0.1);
}

.history-table th {
  background: linear-gradient(135deg, #0d1b2a 0%, #1a0933 100%);
  color: #00ffff;
  font-weight: 600;
  text-shadow: 0 0 5px rgba(0, 255, 255, 0.5);
}

.history-table tr {
  transition: all 0.3s ease;
}

.history-table tr:hover {
  background: rgba(0, 255, 255, 0.1);
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.3);
}

.history-table td {
  color: #e0e0e0;
}

.toggle-password {
  cursor: pointer;
  color: #00ffff;
  margin-left: 10px;
  transition: all 0.3s ease;
}

.toggle-password:hover {
  color: #ff00ff;
}

.image-gallery {
  display: flex;
  gap: 5px;
  flex-wrap: wrap;
}

.image-gallery img {
  width: 50px;
  height: 50px;
  object-fit: cover;
  border-radius: 4px;
  transition: all 0.3s ease;
}

.image-gallery img:hover {
  transform: scale(1.2);
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.completed {
  color: #00ffff;
}

.processing {
  color: #ff00ff;
}

.cancelled {
  color: #808080;
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  padding: 20px;
}

.pagination button {
  background: rgba(0, 255, 255, 0.1);
  color: #00ffff;
  padding: 8px 16px;
  border: 1px solid #00ffff;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.pagination button:hover:not(:disabled) {
  background: #00ffff;
  color: #1a0933;
  box-shadow: 0 0 10px #00ffff;
}

.pagination button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination span {
  color: #e0e0e0;
  font-size: 1rem;
}

/* Responsive */
@media (max-width: 768px) {
  .header-title {
    font-size: 2rem;
  }
  .header-subtitle {
    font-size: 1rem;
  }
  .purchase-table {
    padding: 20px;
  }
  .history-table th,
  .history-table td {
    padding: 10px;
    font-size: 0.9rem;
  }
  .image-gallery img {
    width: 40px;
    height: 40px;
  }
}
</style>
<script setup lang="ts">
import { ref, computed, onMounted, reactive, watch } from 'vue';
import purchasedAccountApi from '@/api/purchased.api';
import gameAccountApi from '@/api/gameaccount.api';
import type { PurchasedAccount } from '@/models/purchased.model';
import type { GameAccount } from '@/models/gameaccount.model';
import { userStore } from '@/stores/auth.ts';

// Get the user ID from the store or local storage
const store = userStore();
const userId = store.user?.id || JSON.parse(localStorage.getItem('user') || '{}').id;

// Reactive variables
const purchases = ref<PurchasedAccount[]>([]);
const gameAccounts = ref<GameAccount[]>([]);
const showPasswords = reactive<{ [key: number]: boolean }>({});
const searchQuery = ref<string>('');
const itemsPerPage = ref<number>(5);
const currentPage = ref<number>(1);
const currentTab = ref<'purchased' | 'myAccounts'>('purchased');
const showModal = ref(false);
const selectedPurchase = ref<PurchasedAccount | null>(null);
const showPasswordInModal = ref(false);
const isLoading = ref(true);

// Fetch data when component mounts
onMounted(async () => {
  if (!userId) {
    console.error('User ID is not available');
    return;
  }
  
  isLoading.value = true;
  
  try {
    const responsePurchases = await purchasedAccountApi.getAllByUserID(userId);
    console.log('Purchases:', responsePurchases);
    purchases.value = Array.isArray(responsePurchases.data.result.data) ? responsePurchases.data.result.data : [];
    purchases.value.forEach(purchase => {
      showPasswords[purchase.id] = false;
    });
  } catch (error) {
    console.error('Failed to fetch purchases:', error);
    purchases.value = [];
  }
  
  try {
    const responseGameAccounts = await gameAccountApi.getAllByUserID(userId);
    gameAccounts.value = Array.isArray(responseGameAccounts.data.result.data) ? responseGameAccounts.data.result.data : [];
    gameAccounts.value.forEach(account => {
      showPasswords[account.id] = false;
    });
  } catch (error) {
    console.error('Failed to fetch game accounts:', error);
    gameAccounts.value = [];
  }
  
  isLoading.value = false;
});

// Watch currentTab to reset currentPage
watch(currentTab, () => {
  currentPage.value = 1;
});

// Computed properties for purchased accounts
const filteredPurchases = computed(() => {
  if (!purchases.value) return [];
  let result = purchases.value;
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(purchase =>
      (purchase.gameName?.toLowerCase().includes(query) || '') ||
      (purchase.accountName?.toLowerCase().includes(query) || '') ||
      (purchase.statusBuyer?.toLowerCase().includes(query) || '')
    );
  }
  return result;
});

const paginatedPurchases = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return filteredPurchases.value.slice(start, end);
});

// Computed properties for game accounts
const filteredGameAccounts = computed(() => {
  if (!gameAccounts.value) return [];
  let result = gameAccounts.value;
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(account =>
      (account.accountName?.toLowerCase().includes(query) || '') ||
      (account.status?.toLowerCase().includes(query) || '')
    );
  }
  return result;
});

const paginatedGameAccounts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return filteredGameAccounts.value.slice(start, end);
});

// Total pages based on current tab
const totalPages = computed(() => {
  const items = currentTab.value === 'purchased' ? filteredPurchases.value : filteredGameAccounts.value;
  return Math.ceil(items.length / itemsPerPage.value);
});

// Function to navigate between pages
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

// Toggle password visibility
const togglePassword = (id: number) => {
  showPasswords[id] = !showPasswords[id];
};

// Open the modal with the selected purchase
const openModal = (purchase: PurchasedAccount) => {
  selectedPurchase.value = purchase;
  showPasswordInModal.value = false;
  showModal.value = true;
};

// Close the modal
const closeModal = () => {
  showModal.value = false;
  selectedPurchase.value = null;
};

// Toggle password visibility in the modal
const togglePasswordInModal = () => {
  showPasswordInModal.value = !showPasswordInModal.value;
};

// Get status label
const getStatusLabel = (status: string) => {
  if (status === 'Mua thành công') return 'Hoàn tất';
  if (status === 'Đã từ chối') return 'Thất bại';
  return 'Hủy';
};

// Get status class
const getStatusClass = (status: string) => {
  if (status === 'completed') return 'status-completed';
  if (status === 'processing') return 'status-processing';
  return 'status-cancelled';
};
</script>

<template>
  <div class="purchase-history-container">
    <div class="content-wrapper">
      <!-- Sidebar -->
      <div class="sidebar">
        <div class="sidebar-menu">
          <div 
            class="menu-item" 
            :class="{ active: currentTab === 'purchased' }" 
            @click="currentTab = 'purchased'"
          >
            <i class="fas fa-shopping-cart"></i>
            <span>Tài khoản đã mua</span>
          </div>
          <div 
            class="menu-item" 
            :class="{ active: currentTab === 'myAccounts' }" 
            @click="currentTab = 'myAccounts'"
          >
            <i class="fas fa-user"></i>
            <span>Tài khoản của tôi</span>
          </div>
        </div>
      </div>

      <!-- Main Content -->
      <div class="main-content">
        <!-- Header -->
        <header class="header">
          <div class="header-content">
            <h1 class="header-title">
              {{ currentTab === 'purchased' ? 'Tài khoản đã mua' : 'Tài khoản của tôi' }}
            </h1>
            <p class="header-subtitle">
              {{ currentTab === 'purchased' ? 'Quản lý các tài khoản game bạn đã mua' : 'Quản lý các tài khoản game của bạn' }}
            </p>
          </div>
          <div class="search-bar">
            <i class="fas fa-search search-icon"></i>
            <input 
              v-model="searchQuery" 
              type="text" 
              placeholder="Tìm kiếm theo tên game, tài khoản hoặc trạng thái..." 
              class="search-input" 
            />
          </div>
        </header>

        <!-- Loading State -->
        <div v-if="isLoading" class="loading-container">
          <div class="loader"></div>
          <p>Đang tải dữ liệu...</p>
        </div>

        <!-- Content Area -->
        <div v-else class="content-area">
          <!-- Purchased Accounts Tab -->
          <div v-if="currentTab === 'purchased'" class="tab-content">
            <div v-if="filteredPurchases.length === 0" class="empty-state">
              <i class="fas fa-shopping-bag empty-icon"></i>
              <h3>Không tìm thấy tài khoản nào</h3>
              <p>Bạn chưa mua tài khoản nào hoặc không có kết quả phù hợp với tìm kiếm của bạn.</p>
            </div>
            
            <div v-else class="table-container">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>STT</th>
                    <th>Tên game</th>
                    <th>Tên tài khoản</th>
                    <th>Mật khẩu</th>
                    <th>Giá</th>
                    <th>Trạng thái</th>
                    <th>Hành động</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(purchase, index) in paginatedPurchases" :key="purchase.id" class="table-row">
                    <td class="text-center">{{ (currentPage - 1) * itemsPerPage + index + 1 }}</td>
                    <td>
                      <div class="game-info">
                        <div class="game-icon">
                          <i class="fas fa-gamepad"></i>
                        </div>
                        <span>{{ purchase.gameName }}</span>
                      </div>
                    </td>
                    <td>{{ purchase.accountName }}</td>
                    <td>
                      <div class="password-field">
                        <span v-if="showPasswords[purchase.id]">{{ purchase.password }}</span>
                        <span v-else class="password-hidden">••••••••</span>
                        <button class="password-toggle" @click="togglePassword(purchase.id)">
                          <i :class="['fas', showPasswords[purchase.id] ? 'fa-eye-slash' : 'fa-eye']"></i>
                        </button>
                      </div>
                    </td>
                    <td class="price">{{ purchase.price }}</td>
                    <td>
                      <span class="status-badge" :class="getStatusClass(purchase.statusBuyer || '')">
                        {{ getStatusLabel(purchase.statusBuyer || '') }}
                      </span>
                    </td>
                    <td>
                      <button class="action-button" @click="openModal(purchase)">
                        <i class="fas fa-info-circle"></i>
                        <span>Chi tiết</span>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- My Accounts Tab -->
          <div v-else-if="currentTab === 'myAccounts'" class="tab-content">
            <div v-if="filteredGameAccounts.length === 0" class="empty-state">
              <i class="fas fa-user-circle empty-icon"></i>
              <h3>Không tìm thấy tài khoản nào</h3>
              <p>Bạn chưa có tài khoản nào hoặc không có kết quả phù hợp với tìm kiếm của bạn.</p>
            </div>
            
            <div v-else class="table-container">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>STT</th>
                    <th>Tên tài khoản</th>
                    <th>Mật khẩu</th>
                    <th>Giá min</th>
                    <th>Giá</th>
                    <th>Trạng thái</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(account, index) in paginatedGameAccounts" :key="account.id" class="table-row">
                    <td class="text-center">{{ (currentPage - 1) * itemsPerPage + index + 1 }}</td>
                    <td>
                      <div class="account-info">
                        <div class="account-icon">
                          <i class="fas fa-user"></i>
                        </div>
                        <span>{{ account.accountName }}</span>
                      </div>
                    </td>
                    <td>
                      <div class="password-field">
                        <span v-if="showPasswords[account.id]">{{ account.password }}</span>
                        <span v-else class="password-hidden">••••••••</span>
                        <button class="password-toggle" @click="togglePassword(account.id)">
                          <i :class="['fas', showPasswords[account.id] ? 'fa-eye-slash' : 'fa-eye']"></i>
                        </button>
                      </div>
                    </td>
                    <td class="price">{{ account.priceMin }}</td>
                    <td class="price">{{ account.price }}</td>
                    <td>
                      <span class="status-badge" :class="getStatusClass(account.status || '')">
                        {{ account.status }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Pagination -->
          <div v-if="totalPages > 0" class="pagination">
            <button 
              class="pagination-button" 
              :disabled="currentPage === 1" 
              @click="goToPage(currentPage - 1)"
            >
              <i class="fas fa-chevron-left"></i>
            </button>
            
            <div class="pagination-info">
              <span>Trang {{ currentPage }} / {{ totalPages }}</span>
            </div>
            
            <button 
              class="pagination-button" 
              :disabled="currentPage === totalPages" 
              @click="goToPage(currentPage + 1)"
            >
              <i class="fas fa-chevron-right"></i>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Detail Modal -->
    <transition name="fade">
      <div v-if="showModal" class="modal-overlay" @click="closeModal">
        <div class="modal-container" @click.stop>
          <div class="modal-header">
            <h2>Chi tiết giao dịch</h2>
            <button class="modal-close" @click="closeModal">
              <i class="fas fa-times"></i>
            </button>
          </div>
          
          <div class="modal-body">
            <div class="modal-info-row">
              <div class="info-label">Mã giao dịch:</div>
              <div class="info-value">#{{ selectedPurchase?.id }}</div>
            </div>
            
            <div class="modal-info-row">
              <div class="info-label">Tên game:</div>
              <div class="info-value">{{ selectedPurchase?.gameName }}</div>
            </div>
            
            <div class="modal-info-row">
              <div class="info-label">Tên tài khoản:</div>
              <div class="info-value">{{ selectedPurchase?.accountName }}</div>
            </div>
            
            <div class="modal-info-row">
              <div class="info-label">Mật khẩu:</div>
              <div class="info-value password-field">
                <span v-if="showPasswordInModal">{{ selectedPurchase?.password }}</span>
                <span v-else class="password-hidden">••••••••</span>
                <button class="password-toggle" @click="togglePasswordInModal">
                  <i :class="['fas', showPasswordInModal ? 'fa-eye-slash' : 'fa-eye']"></i>
                </button>
              </div>
            </div>
            
            <div class="modal-info-row">
              <div class="info-label">Giá:</div>
              <div class="info-value price">{{ selectedPurchase?.price }}</div>
            </div>
            
            <div class="modal-info-row">
              <div class="info-label">Trạng thái:</div>
              <div class="info-value">
                <span 
                  class="status-badge" 
                  :class="getStatusClass(selectedPurchase?.statusBuyer || '')"
                >
                  {{ getStatusLabel(selectedPurchase?.statusBuyer || '') }}
                </span>
              </div>
            </div>
          </div>
          
          <div class="modal-footer">
            <button class="modal-button" @click="closeModal">Đóng</button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<style scoped>
/* Base Styles */
.purchase-history-container {
  font-family: 'Poppins', 'Roboto', sans-serif;
  color: #ffffff;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.content-wrapper {
  display: flex;
  flex: 1;
  height: 100vh;
}

/* Sidebar Styles */
.sidebar {
  width: 260px;
  color: white;
  display: flex;
  flex-direction: column;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
  z-index: 10;
}

.sidebar-header {
  padding: 24px 20px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.logo {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 20px;
  font-weight: 600;
}

.logo i {
  font-size: 24px;
}

.sidebar-menu {
  padding: 20px 0;
  flex: 1;
}

.menu-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 14px 20px;
  cursor: pointer;
  transition: all 0.2s ease;
  border-left: 4px solid transparent;
}

.menu-item i {
  font-size: 18px;
  width: 24px;
  text-align: center;
}

.menu-item:hover {
  background: rgba(255, 255, 255, 0.1);
}

.menu-item.active {
  background: rgba(255, 255, 255, 0.2);
  border-left: 4px solid #fff;
}

/* Main Content Styles */
.main-content {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}

/* Header Styles */
.header {
  padding: 24px 32px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
}

.header-content {
  flex: 1;
}

.header-title {
  font-size: 28px;
  font-weight: 700;
  color: #333;
  margin: 0 0 8px 0;
  background: linear-gradient(135deg, #6e8efb, #a777e3);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.header-subtitle {
  color: #6c757d;
  margin: 0;
  font-size: 16px;
}

.search-bar {
  position: relative;
  width: 300px;
}

.search-input {
  width: 100%;
  padding: 12px 16px 12px 40px;
  border: 1px solid #e0e0e0;
  border-radius: 50px;
  font-size: 14px;
  transition: all 0.2s ease;
}

.search-input:focus {
  outline: none;
  border-color: #a777e3;
  box-shadow: 0 0 0 3px rgba(167, 119, 227, 0.2);
}

.search-icon {
  position: absolute;
  left: 16px;
  top: 50%;
  transform: translateY(-50%);
  color: #a777e3;
}

/* Loading State */
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 300px;
  color: #6c757d;
}

.loader {
  border: 4px solid #f3f3f3;
  border-top: 4px solid #a777e3;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Content Area */
.content-area {
  padding: 24px 32px;
  flex: 1;
  display: flex;
  flex-direction: column;
}

.tab-content {
  border-radius: 12px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
  flex: 1;
  overflow: hidden;
}

/* Empty State */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
  color: #6c757d;
}

.empty-icon {
  font-size: 64px;
  color: #e0e0e0;
  margin-bottom: 16px;
}

.empty-state h3 {
  font-size: 20px;
  margin: 0 0 8px 0;
  color: #333;
}

.empty-state p {
  max-width: 400px;
  margin: 0;
}

/* Table Styles */
.table-container {
  overflow-x: auto;
  width: 100%;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th {
  color: #ffffff;
  font-weight: 600;
  text-align: left;
  padding: 16px;
  border-bottom: 2px solid #e9ecef;
  font-size: 14px;
}

.data-table td {
  padding: 16px;
  border-bottom: 1px solid #e9ecef;
  color: #ffffff;
  font-size: 14px;
  vertical-align: middle;
}

.table-row {
  transition: background 0.2s ease;
}


.text-center {
  text-align: center;
}

/* Game and Account Info */
.game-info, .account-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.game-icon, .account-icon {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  background: linear-gradient(135deg, #6e8efb, #a777e3);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

/* Password Field */
.password-field {
  display: flex;
  align-items: center;
  gap: 8px;
}

.password-hidden {
  letter-spacing: 2px;
  font-weight: bold;
}

.password-toggle {
  background: none;
  border: none;
  color: #a777e3;
  cursor: pointer;
  padding: 4px;
  transition: color 0.2s ease;
}

.password-toggle:hover {
  color: #6e8efb;
}

/* Price */
.price {
  font-weight: 600;
  color: #333;
}

/* Status Badge */
.status-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 6px 12px;
  border-radius: 50px;
  font-size: 12px;
  font-weight: 600;
}

.status-completed {
  background: rgba(25, 135, 84, 0.1);
  color: #198754;
}

.status-processing {
  background: rgba(255, 193, 7, 0.1);
  color: #ffc107;
}

.status-cancelled {
  background: rgba(220, 53, 69, 0.1);
  color: #dc3545;
}

/* Action Button */
.action-button {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  border-radius: 6px;
  background: rgba(110, 142, 251, 0.1);
  color: #6e8efb;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 13px;
  font-weight: 500;
}

.action-button:hover {
  background: rgba(110, 142, 251, 0.2);
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 16px;
  margin-top: 24px;
}

.pagination-button {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border: 1px solid #e0e0e0;
  color: #495057;
  cursor: pointer;
  transition: all 0.2s ease;
}

.pagination-button:hover:not(:disabled) {
  background: #6e8efb;
  color: white;
  border-color: #6e8efb;
}

.pagination-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  font-size: 14px;
  color: #6c757d;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-container {
  background: white;
  border-radius: 12px;
  width: 100%;
  max-width: 500px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.modal-header {
  padding: 20px 24px;
  border-bottom: 1px solid #e9ecef;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.modal-header h2 {
  margin: 0;
  font-size: 20px;
  color: #333;
}

.modal-close {
  background: none;
  border: none;
  color: #6c757d;
  cursor: pointer;
  font-size: 18px;
  transition: color 0.2s ease;
}

.modal-close:hover {
  color: #333;
}

.modal-body {
  padding: 24px;
}

.modal-info-row {
  display: flex;
  margin-bottom: 16px;
}

.info-label {
  width: 120px;
  font-weight: 600;
  color: #6c757d;
}

.info-value {
  flex: 1;
  color: #333;
}

.modal-footer {
  padding: 16px 24px;
  border-top: 1px solid #e9ecef;
  display: flex;
  justify-content: flex-end;
}

.modal-button {
  padding: 10px 20px;
  border-radius: 6px;
  background: linear-gradient(135deg, #6e8efb, #a777e3);
  color: white;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
  font-weight: 500;
}

.modal-button:hover {
  opacity: 0.9;
}

/* Transitions */
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

/* Responsive Styles */
@media (max-width: 992px) {
  .header {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .search-bar {
    width: 100%;
  }
}

@media (max-width: 768px) {
  .content-wrapper {
    flex-direction: column;
    height: auto;
  }
  
  .sidebar {
    width: 100%;
    height: auto;
  }
  
  .sidebar-menu {
    display: flex;
    padding: 0;
  }
  
  .menu-item {
    flex: 1;
    justify-content: center;
    border-left: none;
    border-bottom: 4px solid transparent;
  }
  
  .menu-item.active {
    border-left: none;
    border-bottom: 4px solid #fff;
  }
  
  .menu-item span {
    display: none;
  }
  
  .menu-item i {
    font-size: 20px;
  }
  
  .modal-container {
    width: 90%;
  }
}

@media (max-width: 576px) {
  .header-title {
    font-size: 24px;
  }
  
  .header-subtitle {
    font-size: 14px;
  }
  
  .data-table th, 
  .data-table td {
    padding: 12px 8px;
    font-size: 13px;
  }
  
  .action-button span {
    display: none;
  }
  
  .action-button {
    padding: 8px;
  }
  
  .info-label {
    width: 100px;
  }
}
</style>




<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import transactionApi from "@/api/transaction.api";

// Định nghĩa kiểu dữ liệu với thêm currentBalance
interface UserIncomeOutcome {
  userId: number;
  userName: string;
  currentBalance: number; // Thêm trường này
  totalIncome: number;
  totalOutcome: number;
}

interface StatisticsData {
  totalUsers: number;
  totalWithdrawals: number;
  totalRechargeTransactions: number;
  userIncomeOutcome: UserIncomeOutcome[];
}

// State
const statistics = ref<StatisticsData | null>(null);
const isLoading = ref(true);
const error = ref<string | null>(null);
const searchQuery = ref("");
const currentPage = ref(1);
const pageSize = ref(10);

// Computed properties
const filteredUsers = computed(() => {
  if (!statistics.value?.userIncomeOutcome) return [];

  return statistics.value.userIncomeOutcome.filter(
    (user) =>
      user.userName.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      user.userId.toString().includes(searchQuery.value)
  );
});

const totalPages = computed(() => {
  return Math.ceil(filteredUsers.value.length / pageSize.value);
});

const paginatedUsers = computed(() => {
  const startIndex = (currentPage.value - 1) * pageSize.value;
  const endIndex = startIndex + pageSize.value;
  return filteredUsers.value.slice(startIndex, endIndex);
});

const totalIncome = computed(() => {
  if (!statistics.value?.userIncomeOutcome) return 0;
  return statistics.value.userIncomeOutcome.reduce(
    (sum, user) => sum + user.totalIncome,
    0
  );
});

const totalOutcome = computed(() => {
  if (!statistics.value?.userIncomeOutcome) return 0;
  return statistics.value.userIncomeOutcome.reduce(
    (sum, user) => sum + user.totalOutcome,
    0
  );
});

// Thêm tính tổng số dư hiện tại của tất cả người dùng
const totalBalance = computed(() => {
  if (!statistics.value?.userIncomeOutcome) return 0;
  return statistics.value.userIncomeOutcome.reduce(
    (sum, user) => sum + user.currentBalance,
    0
  );
});

// Methods
const fetchStatistics = async () => {
  isLoading.value = true;
  error.value = null;

  try {
    // Sử dụng API service thay vì gọi axios trực tiếp
    const response = await transactionApi.getUserStatics();

    console.log("API Response:", response.data); // Ghi log để debug

    // Kiểm tra và xử lý cấu trúc phản hồi
    if (response.data && response.data.result && response.data.result.data) {
      statistics.value = response.data.result.data;
      console.log("Dữ liệu thống kê đã được tải:", statistics.value);
    } else {
      // Nếu không tìm thấy cấu trúc dữ liệu mong đợi
      console.error("Cấu trúc dữ liệu API không hợp lệ:", response.data);

      // Nếu vẫn có dữ liệu trả về nhưng cấu trúc khác
      if (response.data) {
        try {
          // Cố gắng map dữ liệu trực tiếp nếu có thể
          const responseData = response.data.result || response.data;
          statistics.value = {
            totalUsers: responseData.totalUsers || 0,
            totalWithdrawals: responseData.totalWithdrawals || 0,
            totalRechargeTransactions:
              responseData.totalRechargeTransactions || 0,
            userIncomeOutcome: responseData.userIncomeOutcome || [],
          };

          if (statistics.value.userIncomeOutcome.length > 0) {
            error.value = null; // Xóa lỗi nếu map thành công
            console.log("Đã map dữ liệu từ API thành công");
          } else {
            throw new Error("Không tìm thấy dữ liệu người dùng");
          }
        } catch (mapErr) {
          console.error("Lỗi khi chuyển đổi dữ liệu:", mapErr);
          error.value = "Không thể xử lý dữ liệu từ API.";
        }
      } else {
        error.value = "Không có dữ liệu trả về từ API.";
      }
    }
  } catch (err: any) {
    console.error("Lỗi khi tải dữ liệu thống kê:", err);
    error.value = "Không thể tải dữ liệu thống kê. Vui lòng thử lại sau.";

    if (err.response) {
      console.error("API Error Status:", err.response.status);
      console.error("API Error Data:", err.response.data);
    } else if (err.request) {
      console.error("Không nhận được phản hồi từ server:", err.request);
    } else {
      console.error("Lỗi cấu hình request:", err.message);
    }
  } finally {
    isLoading.value = false;
  }
};

const handleSearch = () => {
  currentPage.value = 1;
};

const changePage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(amount);
};

// Lifecycle hooks
onMounted(() => {
  // Chỉ gọi API khi component được mount
  fetchStatistics();
});
</script>

<template>
  <div class="statistics-container">
    <h1 class="page-title">Thống kê trang web</h1>

    <!-- Thông tin thống kê tổng quát -->
    <div class="stats-overview" v-if="statistics">
      <div class="stat-card">
        <div class="stat-icon users-icon">
          <i class="material-icons">people</i>
        </div>
        <div class="stat-info">
          <span class="stat-value">{{ statistics.totalUsers }}</span>
          <span class="stat-label">Tổng người dùng</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon withdrawals-icon">
          <i class="material-icons">account_balance_wallet</i>
        </div>
        <div class="stat-info">
          <span class="stat-value">{{ statistics.totalWithdrawals }}</span>
          <span class="stat-label">Lượt rút tiền</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon recharge-icon">
          <i class="material-icons">payments</i>
        </div>
        <div class="stat-info">
          <span class="stat-value">{{
            statistics.totalRechargeTransactions
          }}</span>
          <span class="stat-label">Lượt nạp tiền</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon income-icon">
          <i class="material-icons">trending_up</i>
        </div>
        <div class="stat-info">
          <span class="stat-value">{{ formatCurrency(totalIncome) }}</span>
          <span class="stat-label">Tổng thu</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon outcome-icon">
          <i class="material-icons">trending_down</i>
        </div>
        <div class="stat-info">
          <span class="stat-value">{{ formatCurrency(totalOutcome) }}</span>
          <span class="stat-label">Tổng rút</span>
        </div>
      </div>

      <!-- Thêm card hiển thị tổng số dư -->
      <div class="stat-card">
        <div class="stat-icon balance-icon">
          <i class="material-icons">account_balance</i>
        </div>
        <div class="stat-info">
          <span class="stat-value">{{ formatCurrency(totalBalance) }}</span>
          <span class="stat-label">Tổng số dư</span>
        </div>
      </div>
    </div>

    <!-- Loading state -->
    <div class="loading-container" v-if="isLoading">
      <div class="loader"></div>
      <p>Đang tải dữ liệu thống kê...</p>
    </div>

    <!-- Error message -->
    <div class="error-message" v-if="error">
      <p>{{ error }}</p>
      <p class="error-details">Kiểm tra console để xem chi tiết lỗi.</p>
      <button @click="fetchStatistics" class="retry-button">Thử lại</button>
    </div>

    <!-- Bảng chi tiết người dùng -->
    <div class="user-stats-container" v-if="!isLoading && !error && statistics">
      <h2 class="section-title">Chi tiết thu chi theo người dùng</h2>

      <!-- Thanh tìm kiếm -->
      <div class="search-bar">
        <div class="search-input-container">
          <i class="material-icons search-icon">search</i>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Tìm kiếm theo tên người dùng hoặc ID..."
            @input="handleSearch"
          />
        </div>
      </div>

      <!-- Bảng dữ liệu -->
      <div class="table-container">
        <table class="users-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên người dùng</th>
              <th>Tổng thu</th>
              <th>Tổng rút</th>
              <th>Số dư hiện tại</th>
              <th>Chênh lệch</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in paginatedUsers" :key="user.userId">
              <td>{{ user.userId }}</td>
              <td>{{ user.userName }}</td>
              <td class="income">{{ formatCurrency(user.totalIncome) }}</td>
              <td class="outcome">{{ formatCurrency(user.totalOutcome) }}</td>
              <td class="balance">{{ formatCurrency(user.currentBalance) }}</td>
              <td
                :class="{
                  positive: user.totalIncome >= user.totalOutcome,
                  negative: user.totalIncome < user.totalOutcome,
                }"
              >
                {{ formatCurrency(user.totalIncome - user.totalOutcome) }}
              </td>
            </tr>
            <tr v-if="paginatedUsers.length === 0">
              <td colspan="6" class="no-data">Không có dữ liệu người dùng</td>
            </tr>
          </tbody>
          <tfoot>
            <tr>
              <td colspan="2" class="total-label">Tổng cộng</td>
              <td class="income total">{{ formatCurrency(totalIncome) }}</td>
              <td class="outcome total">{{ formatCurrency(totalOutcome) }}</td>
              <td class="balance total">{{ formatCurrency(totalBalance) }}</td>
              <td
                :class="{
                  positive: totalIncome >= totalOutcome,
                  negative: totalIncome < totalOutcome,
                }"
              >
                {{ formatCurrency(totalIncome - totalOutcome) }}
              </td>
            </tr>
          </tfoot>
        </table>
      </div>

      <!-- Phân trang -->
      <div class="pagination" v-if="totalPages > 1">
        <button
          :disabled="currentPage === 1"
          @click="changePage(currentPage - 1)"
          class="pagination-button"
        >
          <i class="material-icons">chevron_left</i>
        </button>

        <span class="page-info"
          >Trang {{ currentPage }} / {{ totalPages }}</span
        >

        <button
          :disabled="currentPage === totalPages"
          @click="changePage(currentPage + 1)"
          class="pagination-button"
        >
          <i class="material-icons">chevron_right</i>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.statistics-container {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  color: #1a202c;
  margin-bottom: 20px;
}

/* Stats Overview Styles */
.stats-overview {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 20px;
  margin-bottom: 40px;
}

.stat-card {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  display: flex;
  align-items: center;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.stat-icon {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 15px;
  flex-shrink: 0;
}

.stat-icon i {
  font-size: 28px;
  color: white;
}

.users-icon {
  background-color: #4299e1;
}

.withdrawals-icon {
  background-color: #ed8936;
}

.recharge-icon {
  background-color: #48bb78;
}

.income-icon {
  background-color: #38b2ac;
}

.outcome-icon {
  background-color: #ed64a6;
}

/* Thêm màu cho biểu tượng số dư */
.balance-icon {
  background-color: #805ad5;
}

.stat-info {
  display: flex;
  flex-direction: column;
}

.stat-value {
  font-size: 24px;
  font-weight: 700;
  color: #2d3748;
  line-height: 1;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 14px;
  color: #718096;
}

/* User Stats Table */
.user-stats-container {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.section-title {
  font-size: 20px;
  font-weight: 600;
  color: #2d3748;
  margin-bottom: 20px;
}

.search-bar {
  margin-bottom: 20px;
}

.search-input-container {
  position: relative;
  max-width: 400px;
}

.search-icon {
  position: absolute;
  top: 50%;
  left: 10px;
  transform: translateY(-50%);
  color: #a0aec0;
}

.search-input-container input {
  width: 100%;
  padding: 10px 10px 10px 40px;
  border: 1px solid #e2e8f0;
  border-radius: 5px;
  font-size: 14px;
  transition: border-color 0.3s ease, box-shadow 0.3s ease;
}

.search-input-container input:focus {
  outline: none;
  border-color: #4299e1;
  box-shadow: 0 0 0 3px rgba(66, 153, 225, 0.2);
}

.table-container {
  overflow-x: auto;
}

.users-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
}

.users-table th,
.users-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #e2e8f0;
}

.users-table th {
  background-color: #f7fafc;
  color: #4a5568;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  font-size: 12px;
}

.users-table tbody tr:hover {
  background-color: #f7fafc;
}

.income {
  color: #38a169;
}

.outcome {
  color: #e53e3e;
}

/* Style cho cột số dư */
.balance {
  color: #805ad5;
  font-weight: 500;
}

.positive {
  color: #38a169;
  font-weight: 600;
}

.negative {
  color: #e53e3e;
  font-weight: 600;
}

.total-label {
  font-weight: 600;
  text-align: right;
}

.total {
  font-weight: 600;
}

.no-data {
  text-align: center;
  color: #a0aec0;
  padding: 30px 0;
}

/* Pagination */
.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: 20px;
}

.pagination-button {
  background-color: #f7fafc;
  border: 1px solid #e2e8f0;
  border-radius: 5px;
  padding: 8px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.3s ease;
}

.pagination-button:hover:not(:disabled) {
  background-color: #edf2f7;
}

.pagination-button:disabled {
  cursor: not-allowed;
  opacity: 0.5;
}

.page-info {
  margin: 0 15px;
  color: #4a5568;
  font-size: 14px;
}

/* Loading and error states */
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40px 0;
}

.loader {
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin-bottom: 15px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.error-message {
  background-color: #fff5f5;
  border: 1px solid #fed7d7;
  border-radius: 5px;
  padding: 15px;
  color: #e53e3e;
  margin-bottom: 20px;
  text-align: center;
}

.error-details {
  font-size: 12px;
  margin-top: 5px;
  color: #718096;
}

.retry-button {
  background-color: #e53e3e;
  color: white;
  border: none;
  border-radius: 5px;
  padding: 8px 15px;
  margin-top: 10px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s ease;
}

.retry-button:hover {
  background-color: #c53030;
}

/* Responsive */
@media (max-width: 768px) {
  .stats-overview {
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  }

  .users-table th,
  .users-table td {
    padding: 10px;
  }

  .stat-value {
    font-size: 20px;
  }
}

@media (max-width: 576px) {
  .stats-overview {
    grid-template-columns: 1fr;
  }

  .page-title {
    font-size: 20px;
  }

  .section-title {
    font-size: 18px;
  }
}
</style>

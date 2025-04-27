@@ -0,0 +1,609 @@
<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import transactionApi from "@/api/transaction.api"; // Adjust path if needed
import { format } from "date-fns"; // For date formatting

interface WithdrawRequest {
  id: number;
  userID: number; // Or string, depending on your API/DB
  userName?: string; // Added from API
  amount: number;
  status: string;
  drawnType?: string; // Optional, if available
  createdDate: string;
  updatedDate?: string | null;
  bankName?: string | null; // Added from API (Bank Name)
  bankAccount?: string | null; // Renamed from bankNumber (Account Number from API's BankNumber)
  // Add other relevant fields from your API response
}

// --- State ---
const allWithdrawRequests = ref<WithdrawRequest[]>([]); // Store all fetched requests
const isLoading = ref<boolean>(false);
const error = ref<string | null>(null);
const isError = ref(true); // To control message styling (error vs success)
const successMessage = ref<string | null>(null); // For confirmation feedback

// --- Search and Pagination State ---
const searchQuery = ref("");
const currentPage = ref(1);
const pageSize = ref(10); // Number of items per page

// --- Computed Properties for Filtering and Pagination ---
const filteredRequests = computed(() => {
  if (!searchQuery.value) {
    return allWithdrawRequests.value;
  }
  const lowerQuery = searchQuery.value.toLowerCase();
  return allWithdrawRequests.value.filter(
    (req) =>
      (req.userID?.toString() ?? "").includes(lowerQuery) ||
      (req.bankName?.toLowerCase() ?? "").includes(lowerQuery) || // Search by Bank Name
      (req.bankAccount?.toLowerCase() ?? "").includes(lowerQuery) || // Search by Account Number
      (req.status?.toLowerCase() ?? "").includes(lowerQuery) ||
      (req.userName?.toLowerCase() ?? "").includes(lowerQuery) // Search by User Name
  );
});

const totalPages = computed(() => {
  return Math.ceil(filteredRequests.value.length / pageSize.value);
});

const paginatedRequests = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  // Sort by createdDate descending before slicing for pagination
  return filteredRequests.value
    .sort(
      (a, b) =>
        new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime()
    )
    .slice(start, end);
});

// --- Methods ---

// Fetch all withdrawal requests
const fetchWithdrawRequests = async () => {
  isLoading.value = true;
  error.value = null;
  successMessage.value = null; // Clear previous success message
  try {
    const response = await transactionApi.getAllWithdrawRequests();
    console.log("API Response:", response.data); // Log to check structure

    // Adjust based on your actual API response structure
    let dataToProcess: WithdrawRequest[] = [];
    if (
      response.data &&
      response.data.result &&
      Array.isArray(response.data.result.data)
    ) {
      // Map API's BankNumber to bankAccount
      dataToProcess = response.data.result.data.map((item: any) => ({
        ...item,
        bankAccount: item.bankNumber, // Map bankNumber from API to bankAccount in interface
      }));
    } else if (Array.isArray(response.data)) {
      // Fallback if data is directly in response.data
      dataToProcess = response.data.map((item: any) => ({
        ...item,
        bankAccount: item.bankNumber, // Map bankNumber from API to bankAccount in interface
      }));
    } else {
      console.warn("Unexpected API response structure:", response.data);
      // error.value = "Dữ liệu trả về không đúng định dạng."; // Commented out to avoid persistent error message if empty
    }
    // Sort all requests initially by date descending
    allWithdrawRequests.value = dataToProcess.sort(
      (a, b) =>
        new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime()
    );
    currentPage.value = 1; // Reset to first page after fetching
  } catch (err: any) {
    console.error("Error fetching withdrawal requests:", err);
    error.value = "Không thể tải danh sách yêu cầu rút tiền.";
    isError.value = true;
    if (err.response) {
      console.error("API Error:", err.response.data);
    }
    setTimeout(() => (error.value = null), 5000); // Auto-clear error message
  } finally {
    isLoading.value = false;
  }
};

// Confirm a withdrawal request
const confirmWithdrawal = async (id: number) => {
  if (
    !confirm(`Bạn có chắc chắn muốn xác nhận yêu cầu rút tiền #${id} không?`)
  ) {
    return;
  }
  // Indicate loading specifically for this action if needed, or rely on global isLoading
  // const confirmingId = ref(id); // Example for specific loading state
  error.value = null;
  successMessage.value = null;
  try {
    await transactionApi.confirmWithdrawal(id);
    successMessage.value = `Đã xác nhận thành công yêu cầu #${id}.`;
    isError.value = false; // It's a success message
    setTimeout(() => (successMessage.value = null), 3000); // Auto-clear success message
    // Refresh the list after confirmation
    await fetchWithdrawRequests();
  } catch (err: any) {
    console.error(`Error confirming withdrawal #${id}:`, err);
    error.value = `Lỗi khi xác nhận yêu cầu #${id}. Vui lòng thử lại.`;
    isError.value = true;
    if (err.response) {
      console.error("API Error:", err.response.data);
      // Potentially add more specific error message from err.response.data
    }
    setTimeout(() => (error.value = null), 5000); // Auto-clear error message
  } finally {
    // confirmingId.value = null; // Reset specific loading state if used
  }
};

// Change page for pagination
const changePage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

// Trigger search (resets to page 1)
const handleSearch = () => {
  currentPage.value = 1;
  // No need to refetch, filtering is client-side via computed properties
};

// Format currency (Example for VND)
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(amount);
};

// Format date
const formatDate = (dateString: string | null | undefined) => {
  if (!dateString) return "N/A";
  try {
    // Use date-fns for reliable formatting
    return format(new Date(dateString), "dd/MM/yyyy HH:mm:ss"); // Re-enabled formatting
  } catch (e) {
    console.error("Error formatting date:", dateString, e);
    return dateString; // Return original string if formatting fails
  }
};

// Get status class for styling
const getStatusClass = (status: string) => {
  const lowerStatus = status?.toLowerCase() || "";
  if (
    lowerStatus === "completed" ||
    lowerStatus === "hoàn thành" ||
    lowerStatus === "approved"
  ) {
    return "status-completed";
  }
  if (
    lowerStatus === "pending" ||
    lowerStatus === "đang xử lý" ||
    lowerStatus === "đang chờ xác thực"
  ) {
    return "status-pending";
  }
  if (
    lowerStatus === "rejected" ||
    lowerStatus === "failed" ||
    lowerStatus === "thất bại"
  ) {
    return "status-failed";
  }
  return ""; // Default
};

// Lifecycle hook
onMounted(() => {
  fetchWithdrawRequests();
});
</script>

<template>
  <div class="withdraw-requests-container">
    <h1 class="view-title">Yêu cầu Rút tiền</h1>

    <!-- Search Bar -->
    <div class="search-filter">
      <input
        type="text"
        v-model="searchQuery"
        placeholder="Tìm ID/Tên User, Bank, STK, Status..."
        class="search-input"
        @keyup.enter="handleSearch"
      />
      <button @click="handleSearch" class="search-btn">Tìm kiếm</button>
    </div>

    <!-- Messages (Error/Success) -->
    <div
      v-if="error || successMessage"
      :class="['message', { error: isError, success: !isError }]"
    >
      {{ error || successMessage }}
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="loading-state">
      <p>Đang tải dữ liệu...</p>
    </div>

    <!-- Empty State (after loading, considering filters) -->
    <div v-else-if="!paginatedRequests.length" class="empty-state">
      <p>
        {{
          searchQuery
            ? "Không tìm thấy yêu cầu nào khớp với tìm kiếm."
            : "Không có yêu cầu rút tiền nào."
        }}
      </p>
    </div>

    <!-- Data Table -->
    <div v-else class="table-container">
      <table>
        <thead>
          <tr>
            <th>ID Yêu cầu</th>
            <th>ID Người dùng</th>
            <th>Tên Người dùng</th>
            <!-- Added Column -->
            <th>Số tiền</th>
            <th>Tên Ngân hàng</th>
            <!-- Added Column -->
            <th>Số tài khoản</th>
            <!-- Renamed Header -->
            <th>Ngày tạo</th>
            <th>Trạng thái</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          <!-- Loop over paginated requests -->
          <tr v-for="req in paginatedRequests" :key="req.id">
            <td>{{ req.id }}</td>
            <td>{{ req.userID }}</td>
            <td>{{ req.userName || "N/A" }}</td>
            <!-- Display User Name -->
            <td class="amount">{{ formatCurrency(req.amount) }}</td>
            <td>{{ req.bankName || "N/A" }}</td>
            <!-- Display Bank Name -->
            <td>{{ req.bankAccount || "N/A" }}</td>
            <!-- Display Account Number -->
            <td>{{ formatDate(req.createdDate) }}</td>
            <td>
              <span :class="['status-badge', getStatusClass(req.status)]">
                {{ req.status }}
              </span>
            </td>
            <td>
              <button
                v-if="getStatusClass(req.status) === 'status-pending'"
                @click="confirmWithdrawal(req.id)"
                class="action-button confirm-button"
                :disabled="isLoading"
              >
                Xác nhận
              </button>
              <span v-else>-</span>
              <!-- Add other actions like 'Reject' or 'View Details' if needed -->
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div class="pagination" v-if="totalPages > 1">
        <button
          @click="changePage(currentPage - 1)"
          :disabled="currentPage === 1"
        >
          Trước
        </button>
        <span>Trang {{ currentPage }} / {{ totalPages }}</span>
        <button
          @click="changePage(currentPage + 1)"
          :disabled="currentPage === totalPages"
        >
          Sau
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Import styles similar to AdminWebsiteAccount.vue */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.withdraw-requests-container {
  padding: 30px;
  max-width: 1600px; /* Adjust max-width as needed */
  margin: 0 auto;
  background: #f9fafb;
  min-height: 100vh;
}

.view-title {
  color: #1f2937; /* Darker text */
  margin-bottom: 1.5rem;
  font-size: 1.8rem;
  font-weight: 600;
}

/* Search Bar */
.search-filter {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  justify-content: flex-end;
  align-items: center;
}

.search-input {
  padding: 10px 10px 10px 35px;
  width: 350px; /* Adjust width */
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
  background: url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="%236b7280" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>')
    no-repeat 10px center;
  background-size: 20px;
  transition: border-color 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.search-btn {
  padding: 10px 20px;
  background: #3b82f6;
  color: #fff;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background 0.3s ease;
}

.search-btn:hover {
  background: #2563eb;
}

/* Messages */
.message {
  text-align: center;
  margin-bottom: 20px;
  font-size: 14px;
  padding: 12px;
  border-radius: 6px;
}

.message.error {
  color: #dc2626;
  background: #fee2e2;
  border: 1px solid #fca5a5;
}

.message.success {
  color: #059669;
  background: #d1fae5;
  border: 1px solid #6ee7b7;
}

/* States */
.loading-state,
.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  margin-top: 2rem;
  border-radius: 8px;
  background-color: #fff; /* Optional background for states */
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.loading-state p,
.empty-state p {
  color: #6b7280; /* Gray text */
  font-size: 1rem;
}

/* Table */
.table-container {
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  overflow-x: auto; /* Ensure table is scrollable on small screens */
}

table {
  width: 100%;
  border-collapse: separate; /* Use separate for border-radius effect */
  border-spacing: 0;
}

th,
td {
  padding: 14px 18px; /* Slightly more padding */
  text-align: left;
  border-bottom: 1px solid #e5e7eb; /* Lighter border */
  vertical-align: middle;
  font-size: 0.9rem; /* Slightly smaller font */
  color: #374151;
}

th {
  background: #f9fafb; /* Very light gray header */
  color: #4b5563; /* Darker gray text */
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  position: sticky; /* Make header sticky if container scrolls */
  top: 0;
  z-index: 1;
}

tbody tr:last-child td {
  border-bottom: none;
}

tbody tr:hover {
  background-color: #f3f4f6; /* Slightly darker hover */
}

.amount {
  font-weight: 500;
  color: #c0392b; /* Red for amount */
  white-space: nowrap; /* Prevent amount wrapping */
}

td:last-child {
  /* Actions column */
  white-space: nowrap;
}

/* Status Badges */
.status-badge {
  padding: 0.3rem 0.7rem;
  border-radius: 12px;
  font-size: 0.75rem; /* Smaller badge font */
  font-weight: 600;
  display: inline-block;
  white-space: nowrap;
  text-transform: capitalize;
  line-height: 1.2;
}

.status-completed {
  background-color: rgba(16, 185, 129, 0.1); /* Green tint */
  color: #059669; /* Green */
}

.status-pending {
  background-color: rgba(245, 158, 11, 0.1); /* Yellow tint */
  color: #d97706; /* Yellow */
}

.status-failed {
  background-color: rgba(239, 68, 68, 0.1); /* Red tint */
  color: #dc2626; /* Red */
}

/* Action Buttons */
.action-button {
  padding: 7px 14px; /* Adjust padding */
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.8rem; /* Smaller button font */
  font-weight: 500;
  transition: background-color 0.2s ease, opacity 0.2s ease;
  margin-right: 5px; /* Space between buttons if needed */
}
.action-button:last-child {
  margin-right: 0;
}

.confirm-button {
  background-color: #10b981; /* Green */
  color: white;
}

.confirm-button:hover {
  background-color: #059669; /* Darker green */
}
.action-button:disabled {
  background-color: #d1d5db; /* Gray when disabled */
  cursor: not-allowed;
  opacity: 0.7;
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 10px;
  margin-top: 25px; /* More space above pagination */
  padding: 15px 0;
}

.pagination button {
  padding: 8px 16px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  background: #fff;
  color: #374151;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.3s ease, color 0.3s ease, border-color 0.3s ease;
}

.pagination button:hover:not(:disabled) {
  background: #f3f4f6;
  border-color: #9ca3af;
}

.pagination button:disabled {
  background: #f9fafb;
  color: #9ca3af;
  border-color: #e5e7eb;
  cursor: not-allowed;
}

.pagination span {
  font-size: 14px;
  color: #4b5563;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 768px) {
  .withdraw-requests-container {
    padding: 15px;
  }
  .search-filter {
    flex-direction: column;
    align-items: stretch;
  }
  .search-input {
    width: 100%;
  }
  th,
  td {
    padding: 10px 12px;
    font-size: 0.85rem;
  }
  .action-button {
    padding: 6px 10px;
    font-size: 0.75rem;
  }
  .pagination {
    gap: 8px;
  }
  .pagination button {
    padding: 6px 12px;
  }
}
</style>

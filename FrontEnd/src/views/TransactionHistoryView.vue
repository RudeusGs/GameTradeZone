<script setup lang="ts">
import { ref, onMounted } from "vue";
// Import your actual API service
import transactionApi from "@/api/transactions.api"; // Adjust the path as needed

// --- Updated Interfaces based on Database Schema ---

interface RechargeTransaction {
  id: number;
  userId: string;
  accountNumber: string;
  transferAmount: number;
  transactionDate: string;
  createdDate: string;
  updatedDate?: string | null;
  deleteDate?: string | null;
  referenceCode?: string | null;
}

interface WithdrawTransaction {
  id: number;
  userID: number; // Changed to number to match API response
  amount: number;
  status: string;
  // drawnType: string; // Removed as it's not needed for display
  createdDate: string;
  updatedDate?: string | null;
  deleteDate?: string | null;
  bankNumber?: string | null; // Expecting from API (represents bank name/ID)
  bankAccount?: string | null; // Expecting from API (represents account number - currently not provided)
}

// Define a unified interface for display
interface DisplayTransaction {
  id: string;
  type: "Nạp tiền" | "Rút tiền";
  amount: number;
  currency: string;
  date: string;
  status: string;
  details?: string;
}

// --- End Updated Interfaces ---

// State
const transactions = ref<DisplayTransaction[]>([]);
const isLoading = ref<boolean>(false);
const error = ref<string | null>(null);

// Fetch transaction history from both endpoints
const fetchTransactions = async () => {
  isLoading.value = true;
  error.value = null;
  transactions.value = [];

  try {
    const [rechargeResponse, withdrawResponse] = await Promise.all([
      transactionApi.getRechargeHistory(),
      transactionApi.getWithdrawHistory(),
    ]);

    console.log("Recharge Response:", rechargeResponse.data);
    console.log("Withdraw Response:", withdrawResponse.data); // Check if bankNumber/bankAccount are present here

    const mappedRecharges: DisplayTransaction[] = [];
    if (
      rechargeResponse.data?.result?.data &&
      Array.isArray(rechargeResponse.data.result.data)
    ) {
      rechargeResponse.data.result.data.forEach((tx: RechargeTransaction) => {
        mappedRecharges.push({
          id: `recharge-${tx.id}`,
          type: "Nạp tiền",
          amount: tx.transferAmount,
          currency: "VND",
          date: tx.transactionDate || tx.createdDate,
          status: "Hoàn thành",
          details: `TK: ${tx.accountNumber || "N/A"} ${
            tx.referenceCode ? `(Code: ${tx.referenceCode})` : ""
          }`,
        });
      });
    } else {
      console.warn(
        "No recharge data or unexpected format:",
        rechargeResponse.data
      );
    }

    const mappedWithdraws: DisplayTransaction[] = [];
    if (
      withdrawResponse.data?.result?.data &&
      Array.isArray(withdrawResponse.data.result.data)
    ) {
      withdrawResponse.data.result.data.forEach((tx: WithdrawTransaction) => {
        // --- Updated details mapping (Show only Bank and Account Number if available) ---
        let withdrawDetails = "";
        if (tx.bankNumber) {
          // Assuming bankNumber holds the bank name/identifier
          withdrawDetails += `Bank: ${tx.bankNumber}`;
        }
        if (tx.bankAccount) {
          // Add account number if provided by API
          withdrawDetails += withdrawDetails
            ? ` - STK: ${tx.bankAccount}`
            : `STK: ${tx.bankAccount}`;
        }
        // --- End updated details mapping ---

        mappedWithdraws.push({
          id: `withdraw-${tx.id}`,
          type: "Rút tiền",
          amount: tx.amount,
          currency: "VND",
          date: tx.createdDate,
          status: tx.status || "Đang xử lý", // Use status from API, provide default
          // Use the updated details string. If empty, template shows "N/A".
          details: withdrawDetails,
        });
      });
    } else {
      console.warn(
        "No withdraw data or unexpected format:",
        withdrawResponse.data
      );
    }

    const combined = [...mappedRecharges, ...mappedWithdraws];
    combined.sort(
      (a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()
    );

    transactions.value = combined;
  } catch (err: any) {
    console.error("Error fetching transaction history:", err);
    error.value = "Không thể tải lịch sử giao dịch. Vui lòng thử lại.";
    if (err.response) {
      console.error("API Error Response:", err.response.data);
    }
  } finally {
    isLoading.value = false;
  }
};

// Format currency
const formatCurrency = (amount: number, currency: string) => {
  if (currency === "VND") {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  }
  return `${amount.toLocaleString()} ${currency}`;
};

// Get status class for styling
const getStatusClass = (status: string) => {
  const lowerStatus = status?.toLowerCase() || "";

  // --- Add specific check for "Đang chờ xác thực" ---
  if (lowerStatus === "đang chờ xác thực") {
    return "status-pending"; // Use the existing yellow style
  }
  // --- End specific check ---

  // Keep existing general checks
  if (
    lowerStatus.includes("hoàn thành") ||
    lowerStatus.includes("completed") ||
    lowerStatus.includes("approved")
  ) {
    return "status-completed";
  }
  // General pending check (can be removed if "Đang chờ xác thực" is the only pending state)
  if (lowerStatus.includes("đang xử lý") || lowerStatus.includes("pending")) {
    return "status-pending";
  }
  if (
    lowerStatus.includes("thất bại") ||
    lowerStatus.includes("rejected") ||
    lowerStatus.includes("failed")
  ) {
    return "status-failed";
  }
  return ""; // Default or unknown status
};

// Lifecycle hook
onMounted(() => {
  fetchTransactions();
});
</script>

<template>
  <div class="transaction-history-container">
    <h1 class="view-title">Lịch sử Giao dịch</h1>

    <div v-if="isLoading" class="loading-state">
      <p>Đang tải dữ liệu...</p>
    </div>

    <div v-else-if="error" class="error-state">
      <p>{{ error }}</p>
      <button @click="fetchTransactions" class="retry-button">Thử lại</button>
    </div>

    <div v-else-if="transactions.length === 0" class="empty-state">
      <p>Bạn chưa có giao dịch nào.</p>
    </div>

    <div v-else class="transaction-list">
      <table>
        <thead>
          <tr>
            <th>Loại</th>
            <th>Chi tiết</th>
            <th>Số tiền</th>
            <th>Ngày</th>
            <th>Trạng thái</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tx in transactions" :key="tx.id">
            <td>{{ tx.type }}</td>
            <td>{{ tx.details || "N/A" }}</td>
            <td
              :style="{
                color:
                  tx.type === 'Rút tiền'
                    ? '#fc8181' /* Red */
                    : '#68d391' /* Green */,
              }"
            >
              {{ tx.type === "Rút tiền" ? "-" : "+"
              }}{{ formatCurrency(tx.amount, tx.currency) }}
            </td>
            <td>{{ new Date(tx.date).toLocaleString() }}</td>
            <td>
              <span :class="['status-badge', getStatusClass(tx.status)]">
                {{ tx.status }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
/* Styles remain the same */
.transaction-history-container {
  max-width: 1200px;
  margin: 2rem auto;
  padding: 2rem;
  background-color: #1a202c; /* Dark background */
  border-radius: 8px;
  color: #e2e8f0; /* Light text */
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.view-title {
  color: #63b3ed; /* Blue accent */
  margin-bottom: 1.5rem;
  text-align: center;
  font-size: 1.8rem;
  font-weight: 600;
}

.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: #a0aec0; /* Gray text */
}

.error-state p {
  color: #fc8181; /* Red text for errors */
  margin-bottom: 1rem;
}

.retry-button {
  background-color: #4299e1; /* Blue button */
  color: white;
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.retry-button:hover {
  background-color: #2b6cb0; /* Darker blue on hover */
}

.transaction-list {
  overflow-x: auto; /* Allow horizontal scrolling on small screens */
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

th,
td {
  padding: 0.75rem 1rem;
  text-align: left;
  border-bottom: 1px solid #2d3748; /* Darker border */
}

th {
  background-color: #2d3748; /* Slightly darker header */
  color: #a0aec0;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

tbody tr:hover {
  background-color: #2d3748; /* Highlight row on hover */
}

td {
  font-size: 0.95rem;
}

td:nth-child(3), /* Amount */
   td:nth-child(4) {
  /* Date */
  white-space: nowrap;
}

.status-badge {
  padding: 0.25rem 0.6rem;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 600;
  display: inline-block;
  white-space: nowrap;
}

.status-completed {
  background-color: rgba(72, 187, 120, 0.2); /* Green tint */
  color: #48bb78; /* Green */
}

.status-pending {
  background-color: rgba(236, 201, 75, 0.2); /* Yellow tint */
  color: #ecc94b; /* Yellow */
}

.status-failed {
  background-color: rgba(245, 101, 101, 0.2); /* Red tint */
  color: #f56565; /* Red */
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .transaction-history-container {
    margin: 1rem;
    padding: 1rem;
  }

  th,
  td {
    padding: 0.5rem;
    font-size: 0.85rem;
  }

  .view-title {
    font-size: 1.5rem;
  }
}
</style>

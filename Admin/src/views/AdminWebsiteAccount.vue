<template>
  <div class="website-account-page" :style="{ paddingTop: `${navbarHeight}px` }" :class="{ collapsed: isCollapsed }">
    <!-- Thanh tìm kiếm và lọc -->
    <div class="search-filter">
      <input
        type="text"
        v-model="searchQuery"
        placeholder="Tìm kiếm theo họ tên..."
        class="search-input"
      />
      <button @click="fetchUsersWithPagination" class="search-btn">Tìm kiếm</button>
    </div>

    <!-- Thông báo lỗi / thành công -->
    <div v-if="errorMessage" class="error-message">
      {{ errorMessage }}
    </div>

    <!-- Trạng thái loading -->
    <div v-if="loading" class="loading">Đang tải...</div>

    <!-- Danh sách người dùng -->
    <div class="user-list" v-else>
      <table>
        <thead>
          <tr>
            <th>#</th>
            <th>Họ tên</th>
            <th>Số dư</th>
            <th>Coin</th>
            <th>Kinh nghiệm</th>
            <th>Level</th>
            <th>Vai trò</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(user, index) in paginatedUsers" :key="user.id || user.fullName">
            <td>{{ (currentPage - 1) * pageSize + index + 1 }}</td>
            <td>{{ user.fullName || "Chưa cập nhật" }}</td>
            <td>{{ formatCurrency(user.balance ?? 0) }}</td>
            <td>{{ user.coin }}</td>
            <td>{{ user.experience ?? "Chưa có" }}</td>
            <td>{{ user.level }}</td>
            <td>{{ user.roles?.join(", ") || "Chưa có" }}</td>
            <td>
              <button v-if="user.id" @click="getUserById(user.id)" class="action-btn">Xem chi tiết</button>
              <button v-if="user.id" @click="blockAccount(user.id)" class="action-btn danger">Khóa</button>
              <button v-if="user.id" @click="openRoleModal(user)" class="action-btn">Cập nhật vai trò</button>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Phân trang -->
      <div class="pagination">
        <button @click="changePage(currentPage - 1)" :disabled="currentPage === 1">Trước</button>
        <span>Trang {{ currentPage }} / {{ totalPages }}</span>
        <button @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages">Sau</button>
      </div>
    </div>

    <!-- Thông tin chi tiết người dùng -->
    <div v-if="selectedUser" class="user-detail">
      <h2>Thông tin chi tiết</h2>
      <p><strong>Họ tên:</strong> {{ selectedUser.fullName || "Chưa cập nhật" }}</p>
      <p><strong>Số dư:</strong> {{ formatCurrency(selectedUser.balance ?? 0) }}</p>
      <p><strong>Coin:</strong> {{ selectedUser.coin }}</p>
      <p><strong>Kinh nghiệm:</strong> {{ selectedUser.experience ?? "Chưa có" }}</p>
      <p><strong>Level:</strong> {{ selectedUser.level }}</p>
      <p><strong>Vai trò:</strong> {{ selectedUser.roles?.join(", ") || "Chưa có" }}</p>
      <p><strong>Ngân hàng:</strong> {{ selectedUser.bankName || "Chưa có" }}</p>
      <p><strong>Số tài khoản:</strong> {{ selectedUser.bankNumber || "Chưa có" }}</p>
      <p>
        <strong>Trạng thái:</strong>
        {{ selectedUser.status === 1 ? "Đã khóa" : "Bình thường" }}
      </p>
      <button @click="selectedUser = null" class="close-btn">Đóng</button>
    </div>

    <!-- Modal cập nhật vai trò -->
    <div v-if="showRoleModal" class="modal">
      <div class="modal-content">
        <h2>Cập nhật vai trò cho {{ selectedUser?.fullName || "Người dùng" }}</h2>
        <div class="role-selection">
          <label>
            <input type="radio" v-model="selectedRole" value="Admin" />
            Admin
          </label>
          <label>
            <input type="radio" v-model="selectedRole" value="User" />
            User
          </label>
        </div>
        <div class="modal-actions">
          <button @click="updateRoles" class="action-btn">Lưu</button>
          <button @click="showRoleModal = false" class="action-btn cancel">Hủy</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted, computed } from "vue";
import websiteAccountApi from "@/api/websiteaccount.api";
import type { AxiosResponse } from "axios";
import type { UserInfoModel } from "@/models/user-model";
import type { ApiResult } from "@/models/api-result.model";

// Mở rộng kiểu UserInfoModel để bao gồm các trường ngân hàng, trạng thái và vai trò
interface ExtendedUserInfoModel extends UserInfoModel {
  roles?: string[];
  bankName: string;
  bankNumber: string;
  status: number;
}

export default defineComponent({
  name: "AdminWebsiteAccount",
  props: {
    navbarHeight: {
      type: Number,
      default: 64,
    },
    isCollapsed: {
      type: Boolean,
      default: false,
    },
  },
  setup(props) {
    const users = ref<ExtendedUserInfoModel[]>([]);
    const selectedUser = ref<ExtendedUserInfoModel | null>(null);
    const loading = ref(false);
    const errorMessage = ref<string | null>(null);

    // Tìm kiếm và phân trang
    const searchQuery = ref("");
    const currentPage = ref(1);
    const pageSize = ref(10);

    // Phân quyền
    const showRoleModal = ref(false);
    const selectedRole = ref<string>("");

    // Tính toán phân trang
    const filteredUsers = computed(() => {
      if (!searchQuery.value) return users.value;
      return users.value.filter((user) =>
        (user.fullName || "").toLowerCase().includes(searchQuery.value.toLowerCase())
      );
    });

    const totalPages = computed(() =>
      Math.ceil(filteredUsers.value.length / pageSize.value)
    );

    const paginatedUsers = computed(() => {
      const start = (currentPage.value - 1) * pageSize.value;
      const end = start + pageSize.value;
      return filteredUsers.value.slice(start, end);
    });

    // Lấy danh sách tất cả người dùng
    const getAllUsers = async () => {
      loading.value = true;
      try {
        const response: AxiosResponse<ApiResult<UserInfoModel[]>> = await websiteAccountApi.getAll();
        if (response.data.result.isSuccess && response.data.result.data) {
          users.value = response.data.result.data.map((user: UserInfoModel) => ({
            ...user,
            id: user.id || 0,
            roles: user.roles || ["User"],
            bankName: user.bankName || "",
            bankNumber: user.bankNumber || "",
            // Nếu status của backend trả về kiểu number, thì dùng trực tiếp; nếu không, chuyển đổi từ boolean
            status: typeof user.status === "number" ? user.status : (user.status ? 1 : 0),
          }));
        } else {
          errorMessage.value =
            response.data.result.message || "Không thể lấy danh sách người dùng.";
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi lấy danh sách người dùng.";
        setTimeout(() => (errorMessage.value = null), 3000);
      } finally {
        loading.value = false;
      }
    };

    // Lấy chi tiết user theo ID
    const getUserById = async (id: number) => {
      try {
        const response = await websiteAccountApi.getById(id);
console.log("RESPONSE:", response);

          console.log(response)
        if (response.data.result.isSuccess && response.data.result.data) {
          selectedUser.value = {
            ...response.data.result.data,
            roles: response.data.result.data.roles || ["User"],
            bankName: response.data.result.data.bankName || "",
            bankNumber: response.data.result.data.bankNumber || "",
            status: typeof response.data.result.data.status === "number" ? response.data.result.data.status : (response.data.result.data.status ? 1 : 0),
          };
        } else {
          errorMessage.value =
            response.data.result.message || "Không thể lấy thông tin người dùng.";
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi lấy thông tin người dùng.";
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    // Chức năng khóa tài khoản
    const blockAccount = async (id: number) => {
      if (!confirm("Bạn có chắc chắn muốn khóa tài khoản này?")) return;
      try {
        const response: AxiosResponse<ApiResult> = await websiteAccountApi.blockAccount(id);
        if (response.data.result.isSuccess) {
          errorMessage.value = "Khóa tài khoản thành công.";
          setTimeout(() => (errorMessage.value = null), 3000);
          await getAllUsers();
        } else {
          errorMessage.value =
            response.data.result.message || "Không thể khóa tài khoản.";
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi khóa tài khoản.";
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    // Mở modal cập nhật vai trò
    const openRoleModal = (user: ExtendedUserInfoModel) => {
      selectedUser.value = user;
      selectedRole.value = user.roles?.includes("Admin") ? "Admin" : "User";
      showRoleModal.value = true;
    };

    // Cập nhật vai trò
    const updateRoles = async () => {
      if (!selectedUser.value || !selectedRole.value) {
        errorMessage.value = "Vui lòng chọn một vai trò.";
        setTimeout(() => (errorMessage.value = null), 3000);
        return;
      }
      try {
        const response: AxiosResponse<ApiResult> = await websiteAccountApi.updateRoles(
          selectedUser.value.id || 0,
          [selectedRole.value]
        );
        if (response.data.result.isSuccess) {
          errorMessage.value = "Cập nhật vai trò thành công.";
          setTimeout(() => (errorMessage.value = null), 3000);
          showRoleModal.value = false;
          await getAllUsers();
        } else {
          errorMessage.value =
            response.data.result.message || "Không thể cập nhật vai trò.";
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi cập nhật vai trò.";
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    // Chuyển trang
    const changePage = (page: number) => {
      if (page >= 1 && page <= totalPages.value) {
        currentPage.value = page;
      }
    };

    // Tìm kiếm với phân trang
    const fetchUsersWithPagination = () => {
      currentPage.value = 1;
    };

    // Format tiền tệ
    const formatCurrency = (value: number) => {
      return new Intl.NumberFormat("vi-VN", {
        style: "currency",
        currency: "VND",
      }).format(value);
    };

    onMounted(() => {
      getAllUsers();
    });

    return {
      users,
      selectedUser,
      loading,
      errorMessage,
      showRoleModal,
      selectedRole,
      searchQuery,
      currentPage,
      pageSize,
      paginatedUsers,
      totalPages,
      getUserById,
      blockAccount,
      openRoleModal,
      updateRoles,
      changePage,
      fetchUsersWithPagination,
      formatCurrency,
    };
  },
});
</script>

<style scoped>
/* Reset mặc định */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

.website-account-page {
  padding: 30px;
  max-width: 1400px;
  margin: 0 auto;
  background: #f9fafb;
  min-height: 100vh;
  transition: margin-left 0.3s ease;
}

.website-account-page.collapsed {
  margin-left: 60px;
}

/* Thanh tìm kiếm */
.search-filter {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  justify-content: flex-end;
}

.search-input {
  padding: 10px;
  width: 300px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
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

/* Thông báo lỗi */
.error-message {
  color: #dc2626;
  text-align: center;
  margin-bottom: 20px;
  font-size: 14px;
  background: #fee2e2;
  padding: 10px;
  border-radius: 6px;
}

/* Trạng thái loading */
.loading {
  text-align: center;
  font-size: 16px;
  color: #6b7280;
  padding: 20px;
}

/* Bảng danh sách người dùng */
.user-list table {
  width: 100%;
  border-collapse: collapse;
  background: #fff;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  border-radius: 8px;
  overflow: hidden;
}

.user-list th,
.user-list td {
  padding: 12px 16px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
  font-size: 14px;
  color: #374151;
}

.user-list th {
  background: #3b82f6;
  color: #fff;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.user-list tr:hover {
  background: #f9fafb;
}

/* Nút hành động */
.action-btn {
  padding: 6px 12px;
  margin-right: 5px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  background: #3b82f6;
  color: #fff;
  font-size: 13px;
  font-weight: 500;
  transition: background 0.3s ease;
}

.action-btn:hover {
  background: #2563eb;
}

.action-btn.danger {
  background: #dc2626;
}

.action-btn.danger:hover {
  background: #b91c1c;
}

.action-btn.cancel {
  background: #6b7280;
}

.action-btn.cancel:hover {
  background: #4b5563;
}

/* Phân trang */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 10px;
  margin-top: 20px;
}

.pagination button {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  background: #e5e7eb;
  color: #374151;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.3s ease;
}

.pagination button:disabled {
  background: #f3f4f6;
  cursor: not-allowed;
}

.pagination button:hover:not(:disabled) {
  background: #d1d5db;
}

.pagination span {
  font-size: 14px;
  color: #374151;
}

/* Chi tiết người dùng */
.user-detail {
  margin-top: 30px;
  padding: 20px;
  background: #fff;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  border-radius: 8px;
}

.user-detail h2 {
  margin-bottom: 20px;
  color: #1f2937;
  font-size: 22px;
  font-weight: 600;
}

.user-detail p {
  margin: 10px 0;
  font-size: 14px;
  color: #374151;
}

.close-btn {
  padding: 8px 16px;
  margin-top: 10px;
  background: #6b7280;
  color: #fff;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.3s ease;
}

.close-btn:hover {
  background: #4b5563;
}

/* Modal cập nhật vai trò */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: #fff;
  padding: 24px;
  border-radius: 8px;
  max-width: 400px;
  width: 100%;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.modal-content h2 {
  margin-bottom: 20px;
  color: #1f2937;
  font-size: 20px;
  font-weight: 600;
}

/* Radio button cho vai trò */
.role-selection {
  display: flex;
  gap: 20px;
  margin-bottom: 20px;
}

.role-selection label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #374151;
  cursor: pointer;
}

.role-selection input[type="radio"] {
  accent-color: #3b82f6;
}

/* Nút trong modal */
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

.modal-actions .action-btn {
  padding: 8px 16px;
  font-size: 14px;
}
</style>

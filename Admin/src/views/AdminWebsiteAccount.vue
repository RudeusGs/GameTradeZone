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
    <div v-if="errorMessage" :class="['message', { 'error': isError, 'success': !isError }]">
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
            <th>Email</th>
            <th>Trạng thái</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(user, index) in paginatedUsers" :key="user.id || user.fullName">
            <td>{{ (currentPage - 1) * pageSize + index + 1 }}</td>
            <td>{{ user.fullName || "Chưa cập nhật" }}</td>
            <td>{{ user.email }}</td>
            <td>
              <span :class="['status-dot', user.status ? 'blocked' : 'normal']"></span>
              {{ user.status ? "Đã khóa" : "Bình thường" }}
            </td>
            <td>
              <button v-if="user.id" @click="openUserDetailModal(user.id)" class="action-btn">Xem chi tiết</button>
              <button v-if="user.id" @click="toggleBlockAccount(user.id, user.status)" class="action-btn" :class="{ danger: !user.status }">
                {{ user.status ? "Mở khóa" : "Khóa" }}
              </button>
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

    <!-- Modal thông tin chi tiết người dùng -->
    <div v-if="showUserDetailModal" class="modal" @click.self="closeUserDetailModal">
      <div class="modal-content">
        <button class="modal-close" @click="closeUserDetailModal">&times;</button>
        <h2>Thông tin chi tiết người dùng</h2>
        <div class="user-info">
          <p><strong>Họ tên:</strong> <span>{{ selectedUser?.fullName || "Chưa cập nhật" }}</span></p>
          <p><strong>Email:</strong> <span>{{ selectedUser?.email }}</span></p>
          <p><strong>Trạng thái:</strong> <span>{{ selectedUser?.status ? "Đã khóa" : "Bình thường" }}</span></p>
          <p><strong>Kinh nghiệm:</strong> <span>{{ selectedUser?.experience ?? "Chưa có" }}</span></p>
          <p><strong>Level:</strong> <span>{{ selectedUser?.level }}</span></p>
          <p><strong>Vai trò:</strong> <span>{{ selectedUser?.roles?.join(", ") || "Chưa có" }}</span></p>
          <p><strong>Ngân hàng:</strong> <span>{{ selectedUser?.bankName || "Chưa có" }}</span></p>
          <p><strong>Số tài khoản:</strong> <span>{{ selectedUser?.bankNumber || "Chưa có" }}</span></p>
        </div>
        <button @click="closeUserDetailModal" class="close-btn">Đóng</button>
      </div>
    </div>

    <!-- Modal cập nhật vai trò -->
    <div v-if="showRoleModal" class="modal" @click.self="showRoleModal = false">
      <div class="modal-content">
        <button class="modal-close" @click="showRoleModal = false">&times;</button>
        <h2>Cập nhật vai trò cho {{ selectedUser?.fullName || "Người dùng" }}</h2>
        <div class="role-selection">
          <label class="role-option">
            <input type="radio" v-model="selectedRole" value="Admin" />
            <span>Admin</span>
          </label>
          <label class="role-option">
            <input type="radio" v-model="selectedRole" value="User" />
            <span>User</span>
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
import authenticateApi from "@/api/authenticate.api";
import type { AxiosResponse } from "axios";
import type { UserInfoModel } from "@/models/user-model";
import type { ApiResult } from "@/models/api-result.model";

interface ExtendedUserInfoModel extends UserInfoModel {
  roles?: string[];
  bankName: string;
  bankNumber: string;
  status: boolean;
  experience?: number | null;
  level: number;
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
    const isError = ref(true);
    const showUserDetailModal = ref(false);
    const showRoleModal = ref(false);
    const selectedRole = ref<string>("");
    const searchQuery = ref("");
    const currentPage = ref(1);
    const pageSize = ref(10);

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
            status: typeof user.status === "number" ? user.status === 1 : user.status || false,
            experience: user.experience || null,
            level: user.level || 0,
          }));
        } else {
          errorMessage.value = response.data.result.message || "Không thể lấy danh sách người dùng.";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi lấy danh sách người dùng.";
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      } finally {
        loading.value = false;
      }
    };

    const openUserDetailModal = async (id: number) => {
      try {
        const response = await websiteAccountApi.getById(id);
        if (response.data.result.isSuccess && response.data.result.data) {
          const userData = response.data.result.data;
          const roleResponse = await authenticateApi.getRoleById(id);
          const roles = Array.isArray(roleResponse?.data?.result)
            ? roleResponse.data.result
            : [roleResponse?.data?.result || "User"];
          selectedUser.value = {
            ...userData,
            roles: roles,
            status: typeof userData.status === "number" ? userData.status === 1 : userData.status || false,
          };
          showUserDetailModal.value = true;
        } else {
          errorMessage.value = `Không thể lấy thông tin người dùng. Lý do: ${response.data.result.message || "Dữ liệu không tồn tại"}`;
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = `Đã xảy ra lỗi khi lấy thông tin người dùng: ${error.message || "Lỗi không xác định"}`;
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const closeUserDetailModal = () => {
      showUserDetailModal.value = false;
      selectedUser.value = null;
    };

    const toggleBlockAccount = async (id: number, currentStatus: boolean) => {
      const action = currentStatus ? "mở khóa" : "khóa";
      if (!confirm(`Bạn có chắc chắn muốn ${action} tài khoản này?`)) return;
      try {
        const response: AxiosResponse<ApiResult> = await websiteAccountApi.blockAccount(id);
        if (response.data.result.isSuccess) {
          errorMessage.value = `${action.charAt(0).toUpperCase() + action.slice(1)} tài khoản thành công.`;
          isError.value = false;
          setTimeout(() => (errorMessage.value = null), 3000);
          await getAllUsers();
        } else {
          errorMessage.value = `Không thể ${action} tài khoản.`;
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = `Đã xảy ra lỗi khi ${action} tài khoản.`;
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const openRoleModal = (user: ExtendedUserInfoModel) => {
      selectedUser.value = user;
      selectedRole.value = user.roles?.includes("Admin") ? "Admin" : "User";
      showRoleModal.value = true;
    };

    const updateRoles = async () => {
      if (!selectedUser.value || !selectedRole.value) {
        errorMessage.value = "Vui lòng chọn một vai trò.";
        isError.value = true;
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
          isError.value = false;
          setTimeout(() => (errorMessage.value = null), 3000);
          showRoleModal.value = false;
          await getAllUsers();
        } else {
          errorMessage.value = response.data.result.message || "Không thể cập nhật vai trò.";
          isError.value = true;
          setTimeout(() => (errorMessage.value = null), 3000);
        }
      } catch (error: any) {
        errorMessage.value = "Đã xảy ra lỗi khi cập nhật vai trò.";
        isError.value = true;
        setTimeout(() => (errorMessage.value = null), 3000);
      }
    };

    const changePage = (page: number) => {
      if (page >= 1 && page <= totalPages.value) {
        currentPage.value = page;
      }
    };

    const fetchUsersWithPagination = () => {
      currentPage.value = 1;
    };

    onMounted(() => {
      getAllUsers();
    });

    return {
      users,
      selectedUser,
      loading,
      errorMessage,
      isError,
      showUserDetailModal,
      showRoleModal,
      selectedRole,
      searchQuery,
      currentPage,
      pageSize,
      paginatedUsers,
      totalPages,
      openUserDetailModal,
      closeUserDetailModal,
      toggleBlockAccount,
      openRoleModal,
      updateRoles,
      changePage,
      fetchUsersWithPagination,
    };
  },
});
</script>
<style scoped>
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
  align-items: center;
}

.search-input {
  padding: 10px 10px 10px 35px;
  width: 300px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 14px;
  background: url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="%236b7280" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>') no-repeat 10px center;
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

/* Thông báo lỗi/thành công */
.message {
  text-align: center;
  margin-bottom: 20px;
  font-size: 14px;
  padding: 10px;
  border-radius: 6px;
}

.message.error {
  color: #dc2626;
  background: #fee2e2;
}

.message.success {
  color: #059669;
  background: #d1fae5;
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
  border-collapse: separate;
  border-spacing: 0;
  background: #fff;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  border-radius: 8px;
  overflow: hidden;
}

.user-list th,
.user-list td {
  padding: 16px 20px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
  font-size: 15px;
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

/* Nút chấm tròn trạng thái */
.status-dot {
  display: inline-block;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  margin-right: 8px;
  vertical-align: middle;
}

.status-dot.normal {
  background-color: #10b981;
}

.status-dot.blocked {
  background-color: #ef4444;
}

/* Nút hành động */
.action-btn {
  padding: 8px 16px;
  margin-right: 8px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  background: #3b82f6;
  color: #fff;
  font-size: 14px;
  font-weight: 500;
  transition: background 0.3s ease;
}

.action-btn:hover {
  background: #2563eb;
}

.action-btn.danger {
  background: #ef4444;
}

.action-btn.danger:hover {
  background: #dc2626;
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
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  background: #e5e7eb;
  color: #374151;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.3s ease;
}

.pagination button:hover:not(:disabled) {
  background: #d1d5db;
}

.pagination button:disabled {
  background: #f3f4f6;
  cursor: not-allowed;
}

.pagination span {
  font-size: 14px;
  color: #374151;
  font-weight: 500;
}

/* Modal chung */
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
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.modal-content {
  background: #fff;
  padding: 30px;
  border-radius: 12px;
  max-width: 600px;
  width: 90%;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  position: relative;
  animation: slideUp 0.3s ease;
}

@keyframes slideUp {
  from {
    transform: translateY(20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-content h2 {
  margin-bottom: 20px;
  color: #1f2937;
  font-size: 22px;
  font-weight: 600;
  border-bottom: 1px solid #e5e7eb;
  padding-bottom: 10px;
}

/* Nút đóng modal */
.modal-close {
  position: absolute;
  top: 10px;
  right: 10px;
  background: none;
  border: none;
  font-size: 24px;
  color: #6b7280;
  cursor: pointer;
  transition: color 0.3s ease;
}

.modal-close:hover {
  color: #374151;
}

/* Thông tin chi tiết người dùng trong modal */
.user-info p {
  margin: 12px 0;
  font-size: 15px;
  color: #374151;
  display: flex;
  justify-content: space-between;
}

.user-info p strong {
  color: #1f2937;
  font-weight: 600;
  flex: 1;
}

.user-info p span {
  flex: 2;
  text-align: left;
}

/* Nút đóng modal */
.close-btn {
  padding: 10px 20px;
  margin-top: 20px;
  background: #6b7280;
  color: #fff;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.3s ease;
  width: 100%;
}

.close-btn:hover {
  background: #4b5563;
}

/* Modal cập nhật vai trò */
.role-selection {
  display: flex;
  flex-direction: column;
  gap: 15px;
  margin-bottom: 20px;
}

.role-option {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 15px;
  color: #374151;
}

.role-option input[type="radio"] {
  accent-color: #3b82f6;
  width: 18px;
  height: 18px;
  cursor: pointer;
}

.role-option span {
  font-weight: 500;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

/* Responsive */
@media (max-width: 768px) {
  .website-account-page {
    padding: 20px;
  }

  .search-filter {
    flex-direction: column;
    align-items: stretch;
  }

  .search-input {
    width: 100%;
  }

  .user-list table {
    display: block;
    overflow-x: auto;
    white-space: nowrap;
  }

  .pagination {
    flex-direction: column;
    gap: 10px;
  }

  .modal-content {
    max-width: 90%;
    padding: 20px;
  }

  .modal-content h2 {
    font-size: 18px;
  }

  .user-info p {
    font-size: 14px;
  }

  .action-btn {
    padding: 8px 12px;
    font-size: 13px;
  }
}
</style>
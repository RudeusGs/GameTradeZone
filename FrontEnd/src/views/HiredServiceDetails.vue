<template>
  <div class="hired-services-section">
    <div class="section-header">
      <div class="section-title">
        <h2>Dịch vụ đã thuê cho {{ serviceName }}</h2>
        <div class="title-underline"></div>
      </div>
      <button class="back-button" @click="goBack">
        <font-awesome-icon :icon="['fas', 'arrow-left']" />
        <span>Quay lại</span>
        <div class="button-glow"></div>
      </button>
    </div>

    <div class="service-summary">
      <div class="summary-card">
        <div class="summary-icon">
          <font-awesome-icon :icon="['fas', 'chart-line']" />
        </div>
        <div class="summary-content">
          <div class="summary-value">{{ hiredServices.length }}</div>
          <div class="summary-label">Tổng lượt thuê</div>
        </div>
      </div>

      <div class="summary-card">
        <div class="summary-icon">
          <font-awesome-icon :icon="['fas', 'check-circle']" />
        </div>
        <div class="summary-content">
          <div class="summary-value">{{ getCompletedCount }}</div>
          <div class="summary-label">Hoàn thành</div>
        </div>
      </div>

      <div class="summary-card">
        <div class="summary-icon">
          <font-awesome-icon :icon="['fas', 'spinner']" />
        </div>
        <div class="summary-content">
          <div class="summary-value">{{ getInProgressCount }}</div>
          <div class="summary-label">Đang xử lý</div>
        </div>
      </div>
    </div>

    <div v-if="hiredServices.length === 0" class="empty-message">
      <div class="empty-icon">
        <font-awesome-icon :icon="['fas', 'satellite-dish']" />
      </div>
      <span>Không có dịch vụ đã thuê cho dịch vụ này</span>
      <div class="empty-animation"></div>
    </div>
    <div v-else class="table-wrapper cyan-table">
      <table class="service-table">
        <thead>
          <tr>
            <th>STT</th>
            <th>Mô tả</th>
            <th>Trạng thái</th>
            <th>Ngày tạo</th>
            <th>Thời gian còn lại</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(service, index) in hiredServices" :key="service.id" class="table-row">
            <td>{{ index + 1 }}</td>
            <td>
              <button class="view-button" @click="openDescriptionModal(service.decriptions)">
                <font-awesome-icon :icon="['fas', 'eye']" />
                <span>Xem</span>
              </button>
            </td>
            <td>
              <span :class="['status-badge', getStatusClass(service.status)]">
                <span class="status-dot"></span>
                {{ service.status }}
              </span>
            </td>
            <td>{{ formatDate(service.createdDate) }}</td>
            <td>{{ formatRemainingTime(service.endTime) }}</td>
            <td>
              <div class="action-buttons">
                <template v-if="service.status === 'Vui lòng xác nhận'">
                  <button class="confirm-button" @click="openConfirmModal(service.id)">Xác nhận</button>
                  <button class="reject-button" @click="openRejectModal(service.id)">Từ chối</button>
                </template>
                <template v-else-if="service.status === 'Trạng thái chờ'">
                  <button class="done-button" @click="doneService(service.id)">
                    <font-awesome-icon :icon="['fas', 'check']" />
                    <span>Hoàn thành</span>
                  </button>
                </template>
                <template v-else-if="service.status === 'Đã từ chối'">
                  <button class="confirm-button" disabled>Xác nhận</button>
                  <button class="reject-button" disabled>Từ chối</button>
                  <button class="done-button" disabled>Hoàn thành</button>
                </template>
                <span v-else>Không có hành động</span>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Confirm Modal -->
    <transition name="modal">
      <div v-if="showConfirmModal" class="modal confirm-modal">
        <div class="modal-backdrop" @click="closeConfirmModal"></div>
        <div class="modal-content">
          <div class="modal-glow"></div>
          <div class="modal-hologram">
            <div class="hologram-scanline"></div>
          </div>

          <div class="modal-header">
            <h2>Xác nhận dịch vụ</h2>
            <button class="close-button" @click="closeConfirmModal">
              <font-awesome-icon :icon="['fas', 'times']" />
            </button>
          </div>

          <div class="confirm-content">
            <p>Bạn có chắc chắn muốn xác nhận dịch vụ này không?</p>
          </div>

          <div class="form-actions">
            <button class="submit-button" @click="submitConfirm">
              <font-awesome-icon :icon="['fas', 'check']" />
              <span>Xác nhận</span>
              <div class="button-glow"></div>
            </button>
            <button class="cancel-button" @click="closeConfirmModal">
              <font-awesome-icon :icon="['fas', 'times']" />
              <span>Hủy</span>
              <div class="button-glow"></div>
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- Reject Modal -->
    <transition name="modal">
      <div v-if="showRejectModal" class="modal reject-modal">
        <div class="modal-backdrop" @click="closeRejectModal"></div>
        <div class="modal-content">
          <div class="modal-glow"></div>
          <div class="modal-hologram">
            <div class="hologram-scanline"></div>
          </div>

          <div class="modal-header">
            <h2>Từ chối dịch vụ</h2>
            <button class="close-button" @click="closeRejectModal">
              <font-awesome-icon :icon="['fas', 'times']" />
            </button>
          </div>

          <div class="form-group">
            <label for="rejectReason">Lý do từ chối</label>
            <div class="input-container">
              <textarea id="rejectReason" v-model="rejectReason" required></textarea>
              <div class="input-glow"></div>
            </div>
          </div>

          <div class="form-actions">
            <button class="submit-button" @click="submitReject">
              <font-awesome-icon :icon="['fas', 'save']" />
              <span>Xác nhận từ chối</span>
              <div class="button-glow"></div>
            </button>
            <button class="cancel-button" @click="closeRejectModal">
              <font-awesome-icon :icon="['fas', 'times']" />
              <span>Hủy</span>
              <div class="button-glow"></div>
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- Description Modal -->
    <transition name="modal">
      <div v-if="showDescriptionModal" class="modal description-modal">
        <div class="modal-backdrop" @click="closeDescriptionModal"></div>
        <div class="modal-content">
          <div class="modal-glow"></div>
          <div class="modal-hologram">
            <div class="hologram-scanline"></div>
          </div>

          <div class="modal-header">
            <h2>Chi tiết mô tả</h2>
            <button class="close-button" @click="closeDescriptionModal">
              <font-awesome-icon :icon="['fas', 'times']" />
            </button>
          </div>

          <div class="description-content">
            <p>{{ currentDescription }}</p>
          </div>

          <div class="form-actions">
            <button class="cancel-button" @click="closeDescriptionModal">
              <font-awesome-icon :icon="['fas', 'times']" />
              <span>Đóng</span>
              <div class="button-glow"></div>
            </button>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted, watch, computed, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import hiredServiceApi from '@/api/hiredservice.api';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';

export default defineComponent({
  name: 'HiredServicesDetails',
  props: {
    serviceId: {
      type: Number,
      required: true
    },
    serviceName: {
      type: String,
      required: true
    }
  },
  components: {
    FontAwesomeIcon
  },
  setup(props) {
    const router = useRouter();

    interface HiredService {
      id: number;
      status: string;
      createdDate: string;
      decriptions: string;
      endTime?: string;
    }

    const hiredServices = ref<HiredService[]>([]);
    const showConfirmModal = ref(false);
    const showRejectModal = ref(false);
    const rejectReason = ref('');
    const selectedRejectId = ref<number | null>(null);
    const selectedConfirmId = ref<number | null>(null);
    const showDescriptionModal = ref(false);
    const currentDescription = ref('');
    let countdownInterval: number | null = null;

    const fetchHiredServices = async () => {
      try {
        const response = await hiredServiceApi.getAllByServiceId(props.serviceId);
        if (response.data?.result?.isSuccess && response.data.result.data) {
          hiredServices.value = response.data.result.data;
        } else {
          hiredServices.value = [];
        }
      } catch (error) {
        console.error('Lỗi khi lấy dịch vụ đã thuê:', error);
        hiredServices.value = [];
      }
    };

    // Khởi tạo đếm ngược
    const startCountdown = () => {
      countdownInterval = window.setInterval(() => {
        hiredServices.value = [...hiredServices.value];
      }, 1000); // Cập nhật mỗi giây
    };

    onUnmounted(() => {
      if (countdownInterval) {
        clearInterval(countdownInterval);
      }
    });

    const goBack = () => {
      router.push({ name: 'service-list' });
    };

    onMounted(() => {
      fetchHiredServices().then(() => {
        startCountdown();
      });
    });

    watch(() => props.serviceId, () => {
      if (countdownInterval) {
        clearInterval(countdownInterval);
      }
      fetchHiredServices().then(() => {
        startCountdown();
      });
    });

    const confirmService = async (id: number, status: string, reason = '') => {
      try {
        const model = { id, status, reason };
        const response = await hiredServiceApi.confirmService(model);
        if (response.data?.result?.isSuccess) {
          alert('Xác nhận dịch vụ thành công');
          await fetchHiredServices();
        } else {
          alert('Xác nhận dịch vụ thất bại: ' + response.data?.result?.message);
        }
      } catch (error) {
        console.error('Lỗi khi xác nhận dịch vụ:', error);
        alert('Đã xảy ra lỗi khi xác nhận dịch vụ');
      }
    };

    const doneService = async (id: number) => {
      try {
        const response = await hiredServiceApi.doneService(id);
        if (response.data?.result?.isSuccess) {
          alert('Dịch vụ đã hoàn thành');
          await fetchHiredServices();
        } else {
          alert('Hoàn thành dịch vụ thất bại: ' + response.data?.result?.message);
        }
      } catch (error) {
        console.error('Lỗi khi hoàn thành dịch vụ:', error);
        alert('Đã xảy ra lỗi khi hoàn thành dịch vụ');
      }
    };

    const openConfirmModal = (id: number) => {
      selectedConfirmId.value = id;
      showConfirmModal.value = true;
    };

    const closeConfirmModal = () => {
      showConfirmModal.value = false;
      selectedConfirmId.value = null;
    };

    const submitConfirm = async () => {
      if (selectedConfirmId.value !== null) {
        await confirmService(selectedConfirmId.value, 'Đồng ý');
      }
      closeConfirmModal();
    };

    const openRejectModal = (id: number) => {
      selectedRejectId.value = id;
      rejectReason.value = '';
      showRejectModal.value = true;
    };

    const closeRejectModal = () => {
      showRejectModal.value = false;
      selectedRejectId.value = null;
      rejectReason.value = '';
    };

    const submitReject = async () => {
      if (!rejectReason.value.trim()) {
        alert('Vui lòng nhập lý do từ chối');
        return;
      }
      await confirmService(selectedRejectId.value!, 'Từ chối', rejectReason.value);
      closeRejectModal();
    };

    const openDescriptionModal = (description: string) => {
      currentDescription.value = description || 'Không có mô tả';
      showDescriptionModal.value = true;
    };

    const closeDescriptionModal = () => {
      showDescriptionModal.value = false;
      currentDescription.value = '';
    };

    const formatDate = (dateString: string) => {
      const date = new Date(dateString);
      return date.toLocaleDateString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
      });
    };

    const formatRemainingTime = (endTime?: string) => {
      if (!endTime) return 'Không xác định';

      const end = new Date(endTime).getTime();
      const now = new Date().getTime();
      const remainingMs = Math.max(end - now, 0);

      if (remainingMs === 0) return 'Hết thời gian';

      const seconds = Math.floor((remainingMs / 1000) % 60);
      const minutes = Math.floor((remainingMs / (1000 * 60)) % 60);
      const hours = Math.floor((remainingMs / (1000 * 60 * 60)));

      return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    };

    const getStatusClass = (status: string) => {
      if (status === 'Thành công') return 'status-completed';
      if (status === 'Trạng thái chờ' || status === 'Đang thực hiện') return 'status-progress';
      if (status === 'Vui lòng xác nhận') return 'status-pending';
      return 'status-rejected'; // Cho trạng thái 'Đã từ chối'
    };

    const getCompletedCount = computed(() => {
      return hiredServices.value.filter(service => service.status === 'Thành công').length;
    });

    const getInProgressCount = computed(() => {
      return hiredServices.value.filter(service => service.status === 'Trạng thái chờ' || service.status === 'Đang thực hiện').length;
    });

    return {
      hiredServices,
      showConfirmModal,
      showRejectModal,
      rejectReason,
      selectedRejectId,
      selectedConfirmId,
      showDescriptionModal,
      currentDescription,
      confirmService,
      doneService,
      openConfirmModal,
      closeConfirmModal,
      submitConfirm,
      openRejectModal,
      closeRejectModal,
      submitReject,
      openDescriptionModal,
      closeDescriptionModal,
      formatDate,
      formatRemainingTime,
      getStatusClass,
      getCompletedCount,
      getInProgressCount,
      goBack
    };
  }
});
</script>

<style scoped>
.hired-services-section {
  background: rgba(10, 0, 31, 0.8);
  border-radius: 15px;
  padding: 30px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(0, 242, 255, 0.3);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
}

.section-title {
  position: relative;
}

.section-title h2 {
  font-size: 2rem;
  color: #00f2ff;
  margin: 0;
  font-family: 'Orbitron', sans-serif;
  text-shadow: 0 0 8px rgba(0, 242, 255, 0.5);
}

.title-underline {
  height: 3px;
  background: linear-gradient(to right, #00f2ff, transparent);
  margin-top: 8px;
  width: 100%;
}

.back-button {
  padding: 10px 20px;
  background: linear-gradient(to right, #00f2ff, #0099cc);
  color: #fff;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-family: 'Orbitron', sans-serif;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 10px;
  transition: all 0.3s ease;
  box-shadow: 0 0 10px rgba(0, 242, 255, 0.3);
  position: relative;
  overflow: hidden;
}

.back-button:hover {
  background: linear-gradient(to right, #33f5ff, #00f2ff);
  box-shadow: 0 0 15px rgba(0, 242, 255, 0.5);
  transform: translateY(-3px);
}

/* Service Summary */
.service-summary {
  display: flex;
  gap: 20px;
  margin-bottom: 25px;
  flex-wrap: wrap;
}

.summary-card {
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(0, 242, 255, 0.3);
  border-radius: 10px;
  padding: 15px;
  flex: 1;
  min-width: 150px;
  display: flex;
  align-items: center;
  gap: 15px;
  transition: all 0.3s ease;
  box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);
}

.summary-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
  border-color: #00f2ff;
  background: rgba(0, 0, 0, 0.4);
}

.summary-icon {
  font-size: 2rem;
  color: #00f2ff;
  text-shadow: 0 0 10px rgba(0, 242, 255, 0.8);
}

.summary-content {
  display: flex;
  flex-direction: column;
}

.summary-value {
  font-size: 2rem;
  font-weight: 700;
  color: #fff;
  font-family: 'Orbitron', sans-serif;
  text-shadow: 0 0 10px rgba(0, 242, 255, 0.5);
}

.summary-label {
  font-size: 0.9rem;
  color: #b0b0b0;
}

/* Empty Message */
.empty-message {
  text-align: center;
  padding: 60px 20px;
  background: rgba(0, 0, 0, 0.3);
  border-radius: 15px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  position: relative;
  overflow: hidden;
}

.empty-icon {
  font-size: 3rem;
  color: rgba(0, 242, 255, 0.3);
  margin-bottom: 20px;
  animation: float 5s infinite ease-in-out;
}

.empty-message span {
  font-size: 1.5rem;
  color: #a0a0a0;
  font-family: 'Orbitron', sans-serif;
}

.empty-animation {
  position: absolute;
  inset: 0;
  background: radial-gradient(circle at center, rgba(0, 242, 255, 0.1), transparent 70%);
  opacity: 0.5;
  animation: emptyPulse 3s infinite alternate;
}

@keyframes float {
  0% { transform: translateY(0) rotate(0deg); opacity: 0.8; }
  50% { transform: translateY(-100px) rotate(180deg); opacity: 0.3; }
  100% { transform: translateY(0) rotate(360deg); opacity: 0.8; }
}

@keyframes emptyPulse {
  0% { opacity: 0.3; transform: scale(0.95); }
  100% { opacity: 0.6; transform: scale(1.05); }
}

/* Table Styling */
.table-wrapper {
  background: rgba(10, 0, 31, 0.8);
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
}

.cyan-table {
  border: 2px solid #00f2ff;
  box-shadow: 0 0 20px rgba(0, 242, 255, 0.2);
}

.service-table {
  width: 100%;
  border-collapse: collapse;
}

.service-table th {
  background: rgba(0, 0, 0, 0.5);
  color: #00f2ff;
  font-family: 'Orbitron', sans-serif;
  font-weight: 600;
  text-align: left;
  padding: 18px 15px;
  border-bottom: 1px solid rgba(0, 242, 255, 0.3);
  position: relative;
  letter-spacing: 1px;
}

.service-table th::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 1px;
  background: linear-gradient(to right, #00f2ff, transparent);
}

.service-table td {
  padding: 15px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  color: #e0e0e0;
}

.table-row {
  transition: background-color 0.3s ease;
}

.table-row:hover {
  background-color: rgba(0, 242, 255, 0.05);
}

.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 600;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  display: inline-block;
}

.status-completed {
  background-color: rgba(0, 255, 0, 0.2);
  color: #00ff00;
  border: 1px solid #00ff00;
  box-shadow: 0 0 10px rgba(0, 255, 0, 0.3);
}

.status-completed .status-dot {
  background-color: #00ff00;
  box-shadow: 0 0 8px #00ff00;
}

.status-progress {
  background-color: rgba(0, 128, 255, 0.2);
  color: #0080ff;
  border: 1px solid #0080ff;
  box-shadow: 0 0 10px rgba(0, 128, 255, 0.3);
}

.status-progress .status-dot {
  background-color: #0080ff;
  box-shadow: 0 0 8px #0080ff;
}

.status-pending {
  background-color: rgba(255, 165, 0, 0.2);
  color: #ffa500;
  border: 1px solid #ffa500;
  box-shadow: 0 0 10px rgba(255, 165, 0, 0.3);
}

.status-pending .status-dot {
  background-color: #ffa500;
  box-shadow: 0 0 8px #ffa500;
}

.status-rejected {
  background-color: rgba(255, 0, 0, 0.2);
  color: #ff0000;
  border: 1px solid #ff0000;
  box-shadow: 0 0 10px rgba(255, 0, 0, 0.3);
}

.status-rejected .status-dot {
  background-color: #ff0000;
  box-shadow: 0 0 8px #ff0000;
}

/* Action Buttons */
.action-buttons {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.confirm-button,
.reject-button,
.done-button,
.view-button {
  padding: 8px 15px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-family: 'Orbitron', sans-serif;
  font-weight: 600;
  font-size: 0.9rem;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 5px;
}

.view-button {
  background: linear-gradient(to right, #0080ff, #0059b3);
  color: #fff;
  box-shadow: 0 0 10px rgba(0, 128, 255, 0.3);
}

.view-button:hover {
  background: linear-gradient(to right, #00b3ff, #0080ff);
  box-shadow: 0 0 15px rgba(0, 128, 255, 0.5);
  transform: translateY(-2px);
}

.confirm-button {
  background: linear-gradient(to right, #00cc00, #009900);
  color: #fff;
  box-shadow: 0 0 10px rgba(0, 204, 0, 0.3);
}

.confirm-button:hover {
  background: linear-gradient(to right, #00ff00, #00cc00);
  box-shadow: 0 0 15px rgba(0, 255, 0, 0.5);
  transform: translateY(-2px);
}

.reject-button {
  background: linear-gradient(to right, #cc0000, #990000);
  color: #fff;
  box-shadow: 0 0 10px rgba(204, 0, 0, 0.3);
}

.reject-button:hover {
  background: linear-gradient(to right, #ff0000, #cc0000);
  box-shadow: 0 0 15px rgba(255, 0, 0, 0.5);
  transform: translateY(-2px);
}

.done-button {
  background: linear-gradient(to right, #ff9900, #cc7a00);
  color: #fff;
  box-shadow: 0 0 10px rgba(255, 153, 0, 0.3);
}

.done-button:hover {
  background: linear-gradient(to right, #ffcc00, #ff9900);
  box-shadow: 0 0 15px rgba(255, 153, 0, 0.5);
  transform: translateY(-2px);
}

/* Modal Styling */
.modal {
  position: fixed;
  inset: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-backdrop {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(8px);
  z-index: 1001;
}

.modal-content {
  position: relative;
  background: linear-gradient(135deg, #0a001f 0%, #0d1b2a 100%);
  border: 2px solid #00f2ff;
  border-radius: 15px;
  padding: 30px;
  width: 90%;
  max-width: 600px;
  z-index: 1002;
  box-shadow: 0 0 30px rgba(0, 242, 255, 0.3);
}

.modal-glow {
  position: absolute;
  inset: 0;
  background: radial-gradient(circle at center, rgba(0, 242, 255, 0.2), transparent 70%);
  z-index: -1;
  border-radius: 13px;
  animation: modalGlow 3s infinite alternate;
}

@keyframes modalGlow {
  0% { opacity: 0.3; }
  100% { opacity: 0.8; }
}

.modal-hologram {
  position: absolute;
  inset: 0;
  z-index: -1;
  border-radius: 13px;
  overflow: hidden;
}

.hologram-scanline {
  position: absolute;
  inset: 0;
  background: linear-gradient(to bottom, 
    transparent 0%, 
    transparent 50%, 
    rgba(0, 242, 255, 0.1) 50%, 
    rgba(0, 242, 255, 0.1) 51%, 
    transparent 51%, 
    transparent 100%);
  background-size: 100% 8px;
  animation: scanline 8s linear infinite;
  opacity: 0.5;
}

@keyframes scanline {
  0% { background-position: 0 0; }
  100% { background-position: 0 100%; }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
}

.modal-header h2 {
  font-size: 2rem;
  color: #00f2ff;
  margin: 0;
  font-family: 'Orbitron', sans-serif;
  text-shadow: 0 0 8px rgba(0, 242, 255, 0.5);
}

.close-button {
  background: none;
  border: none;
  color: #ff0000;
  font-size: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.close-button:hover {
  transform: scale(1.2);
  color: #ff3333;
  text-shadow: 0 0 10px rgba(255, 0, 0, 0.8);
}

.form-group {
  margin-bottom: 25px;
}

.form-group label {
  display: block;
  margin-bottom: 10px;
  color: #00f2ff;
  font-family: 'Orbitron', sans-serif;
  font-size: 1.1rem;
  letter-spacing: 1px;
}

.input-container {
  position: relative;
}

.form-group textarea,
.form-group input {
  width: 100%;
  padding: 15px;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid #00f2ff;
  border-radius: 8px;
  color: #fff;
  font-family: 'Rajdhani', sans-serif;
  font-size: 1.1rem;
  transition: all 0.3s ease;
}

.form-group textarea {
  height: 100px;
  resize: vertical;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #33f5ff;
  box-shadow: 0 0 15px rgba(0, 242, 255, 0.5);
}

.input-glow {
  position: absolute;
  inset: 0;
  border-radius: 8px;
  pointer-events: none;
  opacity: 0;
  transition: opacity 0.3s ease;
  box-shadow: 0 0 15px rgba(0, 242, 255, 0.5);
}

.form-group input:focus ~ .input-glow,
.form-group textarea:focus ~ .input-glow {
  opacity: 1;
}

.form-actions {
  display: flex;
  gap: 20px;
  justify-content: flex-end;
}

.submit-button,
.cancel-button {
  padding: 12px 25px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-family: 'Orbitron', sans-serif;
  font-weight: 600;
  font-size: 1rem;
  display: flex;
  align-items: center;
  gap: 10px;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.submit-button {
  background: linear-gradient(to right, #00cc00, #009900);
  color: #fff;
  box-shadow: 0 0 10px rgba(0, 204, 0, 0.3);
}

.submit-button:hover {
  background: linear-gradient(to right, #00ff00, #00cc00);
  box-shadow: 0 0 15px rgba(0, 255, 0, 0.5);
  transform: translateY(-3px);
}

.cancel-button {
  background: linear-gradient(to right, #cc0000, #990000);
  color: #fff;
  box-shadow: 0 0 10px rgba(204, 0, 0, 0.3);
}

.cancel-button:hover {
  background: linear-gradient(to right, #ff0000, #cc0000);
  box-shadow: 0 0 15px rgba(255, 0, 0, 0.5);
  transform: translateY(-3px);
}

.button-glow {
  position: absolute;
  inset: 0;
  background: linear-gradient(45deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transform: translateX(-100%);
  transition: transform 0.6s ease;
}

.submit-button:hover .button-glow,
.cancel-button:hover .button-glow {
  transform: translateX(100%);
}

.description-content,
.confirm-content {
  margin-bottom: 25px;
  padding: 20px;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid #00f2ff;
  border-radius: 8px;
  max-height: 300px;
  overflow-y: auto;
}

.description-content p,
.confirm-content p {
  margin: 0;
  color: #e0e0e0;
  font-size: 1.1rem;
  line-height: 1.6;
}

/* Modal Transitions */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.4s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .modal-content,
.modal-leave-active .modal-content {
  transition: transform 0.4s ease;
}

.modal-enter-from .modal-content {
  transform: scale(0.9) translateY(30px);
}

.modal-leave-to .modal-content {
  transform: scale(0.9) translateY(-30px);
}

/* Responsive Design */
@media (max-width: 768px) {
  .section-header {
    flex-direction: column;
    gap: 15px;
    align-items: flex-start;
  }

  .service-table th,
  .service-table td {
    padding: 12px 10px;
    font-size: 0.9rem;
  }

  .service-summary {
    flex-direction: column;
  }
}

@media (max-width: 480px) {
  .form-actions {
    flex-direction: column;
    gap: 15px;
  }

  .submit-button,
  .cancel-button {
    width: 100%;
    justify-content: center;
  }
}
</style>
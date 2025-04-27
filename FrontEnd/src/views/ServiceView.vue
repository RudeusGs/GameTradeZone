<template>
  <div class="service-history-container">
    <!-- Holographic Background Elements -->
    <div class="holographic-background">
      <div class="cyber-grid"></div>
      <div class="floating-elements">
        <div v-for="i in 20" :key="`hex-${i}`" class="hex-element" :style="getHexStyle(i)"></div>
        <div v-for="i in 15" :key="`circle-${i}`" class="circle-element" :style="getCircleStyle(i)"></div>
      </div>
      <div class="particle-background">
        <div v-for="i in 80" :key="`particle-${i}`" class="particle" :style="getParticleStyle(i)"></div>
      </div>
    </div>

    <!-- Header with 3D effect -->
    <header class="header">
      <div class="header-hologram">
        <div class="hologram-scanline"></div>
      </div>
      <div class="header-content">
        <div class="header-icon-container">
          <font-awesome-icon :icon="['fas', 'gamepad']" class="header-icon text-neon-cyan pulse-animation" />
        </div>
        <h1 class="header-title">Quản lý dịch vụ</h1>
        <div class="header-icon-container">
          <font-awesome-icon :icon="['fas', 'gamepad']" class="header-icon text-neon-magenta pulse-animation" />
        </div>
      </div>
      <p class="header-subtitle">
        <span class="typing-text">Xem lại các dịch vụ của bạn</span>
        <span class="cursor">|</span>
      </p>
    </header>

    <!-- Animated Tabs -->
    <div class="tabs-container">
      <div class="tabs">
        <button
          :class="{ 'tab-button': true, 'tab-active': tab === 'myServices', 'tab-cyan': tab === 'myServices' }"
          @click="tab = 'myServices'"
        >
          <div class="tab-icon-container">
            <font-awesome-icon :icon="['fas', 'rocket']" class="tab-icon" />
            <div class="tab-icon-glow"></div>
          </div>
          <span>Dịch vụ của tôi</span>
        </button>
        <button
          :class="{ 'tab-button': true, 'tab-active': tab === 'hired', 'tab-magenta': tab === 'hired' }"
          @click="tab = 'hired'"
        >
          <div class="tab-icon-container">
            <font-awesome-icon :icon="['fas', 'space-shuttle']" class="tab-icon" />
            <div class="tab-icon-glow"></div>
          </div>
          <span>Dịch vụ đang thuê</span>
        </button>
      </div>
    </div>

    <!-- Content with Animated Transitions -->
    <section class="service-content">
      <transition name="fade-slide" mode="out-in">
        <div v-if="tab === 'myServices'" key="myServices">
          <div v-if="filteredMyServices.length === 0" class="empty-message">
            <div class="empty-icon">
              <font-awesome-icon :icon="['fas', 'satellite-dish']" />
            </div>
            <span>Bạn chưa có dịch vụ nào</span>
            <div class="empty-animation"></div>
          </div>
          <div v-else class="card-container">
            <div
              v-for="service in filteredMyServices"
              :key="service.id"
              class="service-card"
              @click="selectService(service.id)"
            >
              <div class="card-glow"></div>
              <div class="card-hologram">
                <div class="hologram-scanline"></div>
              </div>

              <div class="card-header">
                <h3 class="service-name">{{ service.serviceName }}</h3>
                <div :class="['service-status', service.isDelete ? 'status-deleted' : 'status-active']">
                  {{ service.isDelete ? 'Đã xóa' : 'Hoạt động' }}
                </div>
              </div>

              <div class="service-content-wrapper">
                <p class="service-description">{{ service.decription }}</p>

                <div class="service-price-container">
                  <div class="price-label">Giá dịch vụ:</div>
                  <div class="service-price">{{ service.servicePrice.toLocaleString() }} <span>VNĐ</span></div>
                </div>

                <div class="service-meta">
                  <div class="service-level">
                    <div class="meta-icon-container">
                      <font-awesome-icon :icon="['fas', 'star']" class="meta-icon" />
                      <div class="meta-icon-glow"></div>
                    </div>
                    <span>Cấp độ: <strong>{{ service.serviceLevel || 1 }}</strong></span>
                  </div>
                  <div class="service-time">
                    <div class="meta-icon-container">
                      <font-awesome-icon :icon="['fas', 'clock']" class="meta-icon" />
                      <div class="meta-icon-glow"></div>
                    </div>
                    <span>Thời gian: <strong>{{ service.serviceTime || '1 giờ' }}</strong></span>
                  </div>
                  <div class="service-rented">
                    <div class="meta-icon-container">
                      <font-awesome-icon :icon="['fas', 'users']" class="meta-icon" />
                      <div class="meta-icon-glow"></div>
                    </div>
                    <span>Số người thuê: <strong>{{ service.rentedC || 0 }}</strong> lần</span>
                  </div>
                </div>
              </div>

              <div class="card-progress">
                <div class="progress-label">Hiệu suất</div>
                <div class="progress-bar">
                  <div class="progress-fill" :style="`width: ${getServicePerformance(service)}%`"></div>
                </div>
                <div class="progress-value">{{ getServicePerformance(service) }}%</div>
              </div>

              <div v-if="!service.isDelete" class="service-actions">
                <button class="update-button" @click.stop="openUpdateModal(service)">
                  <font-awesome-icon :icon="['fas', 'edit']" />
                  <span>Cập nhật</span>
                </button>
                <button class="delete-button" @click.stop="deleteService(service.id)">
                  <font-awesome-icon :icon="['fas', 'trash']" />
                  <span>Xóa</span>
                </button>
              </div>

              <div class="card-hover-effect"></div>
            </div>
          </div>
        </div>

        <!-- Tab: Dịch vụ đang thuê -->
        <div v-else-if="tab === 'hired'" key="hired">
          <div class="hired-services-dashboard">
            <div class="dashboard-card">
              <div class="dashboard-value">{{ hiredServices.length }}</div>
              <div class="dashboard-label">Tổng dịch vụ</div>
              <div class="dashboard-icon">
                <font-awesome-icon :icon="['fas', 'shopping-cart']" />
              </div>
            </div>

            <div class="dashboard-card">
              <div class="dashboard-value">{{ getStatusCount('Thành công') }}</div>
              <div class="dashboard-label">Hoàn thành</div>
              <div class="dashboard-icon">
                <font-awesome-icon :icon="['fas', 'check-circle']" />
              </div>
            </div>

            <div class="dashboard-card">
              <div class="dashboard-value">{{ getStatusCount('Đang xử lý') }}</div>
              <div class="dashboard-label">Đang xử lý</div>
              <div class="dashboard-icon">
                <font-awesome-icon :icon="['fas', 'spinner']" />
              </div>
            </div>

            <div class="dashboard-card">
              <div class="dashboard-value">{{ getStatusCount('Chờ xử lý') }}</div>
              <div class="dashboard-label">Chờ xử lý</div>
              <div class="dashboard-icon">
                <font-awesome-icon :icon="['fas', 'clock']" />
              </div>
            </div>
          </div>

          <div v-if="hiredServices.length === 0" class="empty-message">
            <div class="empty-icon">
              <font-awesome-icon :icon="['fas', 'satellite-dish']" />
            </div>
            <span>Không có dịch vụ đang thuê</span>
            <div class="empty-animation"></div>
          </div>
          <div v-else class="table-wrapper magenta-table">
            <table class="service-table">
              <thead>
                <tr>
                  <th>STT</th>
                  <th>Tên dịch vụ</th>
                  <th>Trạng thái</th>
                  <th>Ngày tạo</th>
                  <th>Thời gian còn lại</th>
                  <th>Mô tả</th>
                  <th>Hành động</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(service, index) in hiredServices" :key="service.id" class="table-row">
                  <td>{{ index + 1 }}</td>
                  <td>{{ service.serviceName }}</td>
                  <td>
                    <span :class="['status-badge', getStatusClass(service.status)]">
                      <span class="status-dot"></span>
                      {{ service.status }}
                    </span>
                  </td>
                  <td>{{ formatDate(service.createdDate) }}</td>
                  <td>{{ formatRemainingTime(service.remainingTime) }}</td>
                  <td>
                    <button class="view-button" @click.stop="openDescriptionModal(service)">
                      <font-awesome-icon :icon="['fas', 'eye']" />
                      <span>Xem</span>
                    </button>
                  </td>
                  <td>
                    <button
                      v-if="service.status === 'Dịch vụ đã xong, vui lòng kiểm tra trước khi xác nhận'"
                      class="confirm-button"
                      @click.stop="openConfirmModal(service.id)"
                    >
                      <font-awesome-icon :icon="['fas', 'check']" />
                      <span>Xác nhận</span>
                    </button>
                    <span v-else>Không có hành động</span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </transition>
    </section>

    <!-- Update Modal -->
    <transition name="modal">
      <div v-if="showUpdateModal" class="update-modal">
        <div class="modal-backdrop" @click="closeUpdateModal"></div>
        <div class="modal-content">
          <div class="modal-header">
            <h2>Cập nhật dịch vụ</h2>
            <button class="close-button" @click="closeUpdateModal">
              <font-awesome-icon :icon="['fas', 'times']" />
            </button>
          </div>

          <div class="form-group">
            <label for="serviceName">Tên dịch vụ</label>
            <div class="input-container">
              <input type="text" id="serviceName" v-model="updateForm.Servicename" required />
            </div>
          </div>

          <div class="form-group">
            <label for="serviceDescription">Mô tả</label>
            <div class="input-container">
              <textarea id="serviceDescription" v-model="updateForm.decription"></textarea>
            </div>
          </div>

          <div class="form-group">
            <label for="servicePrice">Giá</label>
            <div class="input-container">
              <input type="number" id="servicePrice" v-model="updateForm.Serviceprice" required step="0.01" />
            </div>
          </div>

          <div class="form-group">
            <label for="serviceTime">Thời gian (HH:mm:ss)</label>
            <div class="input-container">
              <input type="text" id="serviceTime" v-model="updateForm.ServiceTime" placeholder="HH:mm:ss" />
            </div>
          </div>

          <div class="form-actions">
            <button class="submit-button" @click="updateService">
              <font-awesome-icon :icon="['fas', 'save']" />
              <span>Cập nhật</span>
            </button>
            <button class="cancel-button" @click="closeUpdateModal">
              <font-awesome-icon :icon="['fas', 'times']" />
              <span>Hủy</span>
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- Description Modal -->
    <transition name="modal">
      <div v-if="showDescriptionModal" class="description-modal">
        <div class="modal-backdrop" @click="closeDescriptionModal"></div>
        <div class="modal-content">
          <div class="modal-header">
            <h2>Chi tiết mô tả</h2>
            <button class="close-button" @click="closeDescriptionModal">
              <font-awesome-icon :icon="['fas', 'times']" />
            </button>
          </div>

          <div class="description-content">
            <p>{{ currentService?.decriptions || 'Không có mô tả' }}</p>
            <div v-if="currentService?.status === 'Từ chối nhận'">
              <h3>Lý do từ chối:</h3>
              <p>{{ currentService?.reason || 'Không có lý do' }}</p>
            </div>
          </div>

          <div class="form-actions">
            <button class="cancel-button" @click="closeDescriptionModal">
              <font-awesome-icon :icon="['fas', 'times']" />
              <span>Đóng</span>
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- Confirm Service Modal -->
    <transition name="modal">
      <div v-if="showConfirmModal" class="confirm-modal">
        <div class="modal-backdrop" @click="closeConfirmModal"></div>
        <div class="modal-content">
          <div class="modal-header">
            <h2>Xác nhận dịch vụ</h2>
            <button class="close-button" @click="closeConfirmModal">
              <font-awesome-icon :icon="['fas', 'times']" />
            </button>
          </div>

          <div class="form-group">
            <label>Trạng thái</label>
            <div class="input-container">
              <select v-model="confirmStatus">
                <option value="Đồng ý">Đồng ý</option>
                <option value="Từ chối">Từ chối</option>
              </select>
            </div>
          </div>

          <div v-if="confirmStatus === 'Từ chối'" class="form-group">
            <label for="rejectReason">Lý do từ chối</label>
            <div class="input-container">
              <textarea id="rejectReason" v-model="rejectReason" placeholder="Nhập lý do từ chối"></textarea>
            </div>
          </div>

          <div v-if="confirmStatus === 'Đồng ý'" class="form-group">
            <label for="feedback">Feedback</label>
            <div class="input-container">
              <textarea id="feedback" v-model="feedback" placeholder="Nhập feedback"></textarea>
            </div>
          </div>

          <div class="form-actions">
            <button class="submit-button" @click="submitConfirmService">
              <font-awesome-icon :icon="['fas', 'check']" />
              <span>Xác nhận</span>
            </button>
            <button class="cancel-button" @click="closeConfirmModal">
              <font-awesome-icon :icon="['fas', 'times']" />
              <span>Hủy</span>
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- Floating Action Button -->
    <div class="floating-action-button">
      <div class="fab-icon">
        <font-awesome-icon :icon="['fas', 'plus']" />
      </div>
      <div class="fab-menu">
        <button class="fab-item">
          <font-awesome-icon :icon="['fas', 'rocket']" />
          <span>Thêm dịch vụ</span>
        </button>
        <button class="fab-item">
          <font-awesome-icon :icon="['fas', 'sync']" />
          <span>Làm mới</span>
        </button>
        <button class="fab-item">
          <font-awesome-icon :icon="['fas', 'cog']" />
          <span>Cài đặt</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { userStore } from '@/stores/auth';
import serviceApi from '@/api/service.api';
import ongoingserviceApi from '@/api/ongoingservice.api';

export interface UpdateServiceModel {
  id: number;
  Servicename: string;
  decription?: string;
  Serviceprice: number;
  ServiceTime?: string;
}

export interface ConfirmServiceModel {
  id: number;
  Status: string;
  Reason?: string;
  FeedBack?: string;
}

export default defineComponent({
  name: 'ServiceList',
  setup() {
    const store = userStore();
    const userId = store.user?.id || JSON.parse(localStorage.getItem('user') || '{}').id;
    const router = useRouter();

    const tab = ref('myServices');
    const searchQuery = ref('');
    const filterStatus = ref('all');

    interface MyService {
      id: number;
      serviceName: string;
      decription: string;
      servicePrice: number;
      isDelete: boolean;
      serviceLevel?: number;
      serviceTime?: string;
      rentedC?: number;
      feedback?: string;
    }

    interface HiredService {
      id: number;
      serviceID: string;
      serviceName: string;
      decriptions: string;
      status: string;
      reason?: string;
      image?: string;
      createdDate: string;
      remainingTime?: string;
    }

    const myServices = ref<MyService[]>([]);
    const hiredServices = ref<HiredService[]>([]);
    const showUpdateModal = ref(false);
    const updateForm = ref<UpdateServiceModel>({
      id: 0,
      Servicename: '',
      decription: '',
      Serviceprice: 0,
      ServiceTime: '',
    });
    const showDescriptionModal = ref(false);
    const currentService = ref<HiredService | null>(null);
    const showConfirmModal = ref(false);
    const confirmStatus = ref('Đồng ý');
    const rejectReason = ref('');
    const feedback = ref('');
    const selectedServiceId = ref<number | null>(null);

    const filteredMyServices = computed(() => {
      let filtered = myServices.value;
      if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase();
        filtered = filtered.filter(service =>
          service.serviceName.toLowerCase().includes(query) ||
          service.decription.toLowerCase().includes(query)
        );
      }
      if (filterStatus.value !== 'all') {
        filtered = filtered.filter(service =>
          (filterStatus.value === 'active' && !service.isDelete) ||
          (filterStatus.value === 'deleted' && service.isDelete)
        );
      }
      return filtered;
    });

    const fetchMyServices = async () => {
      if (!userId) return;
      try {
        const response = await serviceApi.getAllByUserId(userId);
        if (response.data?.result?.isSuccess) {
          myServices.value = response.data.result.data;
        }
      } catch (error) {
        console.error('Lỗi khi lấy dịch vụ của tôi:', error);
      }
    };

    const fetchHiredServices = async () => {
      if (!userId) return;
      try {
        const response = await ongoingserviceApi.getAllByUserId(userId);
        if (response.data?.result?.isSuccess) {
          const services = response.data.result.data;
          for (const service of services) {
            const serviceNameResponse = await ongoingserviceApi.getServiceName(parseInt(service.serviceID));
            if (serviceNameResponse.data?.result?.isSuccess) {
              service.serviceName = serviceNameResponse.data.result.data[0]?.serviceName || 'Unknown';
            } else {
              service.serviceName = 'Unknown';
            }
            const remainingTimeResponse = await ongoingserviceApi.getRemainingTime(service.id);
            if (remainingTimeResponse.data?.result?.isSuccess) {
              service.remainingTime = remainingTimeResponse.data.result.data;
            } else {
              service.remainingTime = 'Không xác định';
            }
          }
          hiredServices.value = services;
        }
      } catch (error) {
        console.error('Lỗi khi lấy dịch vụ đang thuê:', error);
      }
    };

    const selectService = (serviceId: number) => {
      const selectedService = myServices.value.find(service => service.id === serviceId);
      router.push({
        name: 'hired-service-details',
        params: { id: serviceId.toString() },
        query: { name: selectedService?.serviceName },
      });
    };

    const deleteService = async (serviceId: number) => {
      if (confirm('Bạn có chắc chắn muốn xóa dịch vụ này?')) {
        try {
          const response = await serviceApi.delete(serviceId);
          if (response.data?.result?.isSuccess) {
            alert('Xóa dịch vụ thành công');
            await fetchMyServices();
          } else {
            alert('Xóa dịch vụ thất bại: ' + response.data?.result?.message);
          }
        } catch (error) {
          console.error('Lỗi khi xóa dịch vụ:', error);
          alert('Đã xảy ra lỗi khi xóa dịch vụ');
        }
      }
    };

    const openUpdateModal = (service: MyService) => {
      updateForm.value = {
        id: service.id,
        Servicename: service.serviceName || '',
        decription: service.decription || '',
        Serviceprice: service.servicePrice,
        ServiceTime: service.serviceTime || '',
      };
      showUpdateModal.value = true;
    };

    const closeUpdateModal = () => {
      showUpdateModal.value = false;
      updateForm.value = {
        id: 0,
        Servicename: '',
        decription: '',
        Serviceprice: 0,
        ServiceTime: '',
      };
    };

    const updateService = async () => {
      try {
        if (!updateForm.value.Servicename || !updateForm.value.Serviceprice) {
          alert('Vui lòng điền đầy đủ các trường bắt buộc: Tên dịch vụ và Giá.');
          return;
        }

        const formData = new FormData();
        formData.append('Id', updateForm.value.id.toString());
        formData.append('Servicename', updateForm.value.Servicename);
        if (updateForm.value.decription) {
          formData.append('decription', updateForm.value.decription);
        }
        formData.append('Serviceprice', updateForm.value.Serviceprice.toString());
        if (updateForm.value.ServiceTime) {
          formData.append('ServiceTime', updateForm.value.ServiceTime);
        }

        const response = await serviceApi.update(formData);

        if (response.data?.result?.isSuccess) {
          alert('Cập nhật dịch vụ thành công');
          await fetchMyServices();
          closeUpdateModal();
        } else {
          alert('Cập nhật dịch vụ thất bại: ' + response.data?.result?.message);
        }
      } catch (error) {
        console.error('Lỗi khi cập nhật dịch vụ:', error);
        alert('Đã xảy ra lỗi khi cập nhật dịch vụ');
      }
    };

    const openDescriptionModal = (service: HiredService) => {
      currentService.value = service;
      showDescriptionModal.value = true;
    };

    const closeDescriptionModal = () => {
      showDescriptionModal.value = false;
      currentService.value = null;
    };

    const openConfirmModal = (serviceId: number) => {
      selectedServiceId.value = serviceId;
      showConfirmModal.value = true;
    };

    const closeConfirmModal = () => {
      showConfirmModal.value = false;
      confirmStatus.value = 'Đồng ý';
      rejectReason.value = '';
      feedback.value = '';
      selectedServiceId.value = null;
    };

    const submitConfirmService = async () => {
      if (!selectedServiceId.value) return;

      const model: ConfirmServiceModel = {
        id: selectedServiceId.value,
        Status: confirmStatus.value,
        Reason: confirmStatus.value === 'Từ chối' ? rejectReason.value : undefined,
        FeedBack: confirmStatus.value === 'Đồng ý' ? feedback.value : undefined,
      };

      try {
        const response = await ongoingserviceApi.confirmService(model);
        if (response.data?.result?.isSuccess) {
          alert('Xác nhận dịch vụ thành công');
          await fetchHiredServices();
          closeConfirmModal();
        } else {
          alert('Xác nhận dịch vụ thất bại: ' + response.data?.result?.message);
        }
      } catch (error) {
        console.error('Lỗi khi xác nhận dịch vụ:', error);
        alert('Đã xảy ra lỗi khi xác nhận dịch vụ');
      }
    };

    const formatRemainingTime = (remainingTime?: string) => {
      if (!remainingTime || remainingTime === 'Không xác định') return 'Không xác định';
      const [hours, minutes, seconds] = remainingTime.split(':').map(Number);
      return `${hours} giờ ${minutes} phút ${seconds} giây`;
    };

    const getStatusClass = (status: string) => {
      if (status === 'Thành công') return 'status-completed';
      if (status === 'Đang xử lý') return 'status-progress';
      return 'status-pending';
    };

    const getStatusCount = (status: string) => {
      return hiredServices.value.filter(service => service.status === status).length;
    };

    const getServicePerformance = (service: MyService) => {
      const maxRented = Math.max(...myServices.value.map(s => s.rentedC || 0), 1);
      return Math.round(((service.rentedC || 0) / maxRented) * 100);
    };

    const formatDate = (dateString: string) => {
      const date = new Date(dateString);
      return date.toLocaleDateString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      });
    };

    const getParticleStyle = (i: number) => {
      const size = Math.random() * 3 + 1;
      const left = `${Math.random() * 100}%`;
      const top = `${Math.random() * 100}%`;
      const delay = Math.random() * 5;
      const duration = Math.random() * 10 + 5;
      const color = i % 3 === 0 ? '#00f2ff' : i % 3 === 1 ? '#ff00ff' : '#ffffff';
      const opacity = Math.random() * 0.5 + 0.3;
      return {
        width: `${size}px`,
        height: `${size}px`,
        left,
        top,
        backgroundColor: color,
        opacity,
        animationDelay: `${delay}s`,
        animationDuration: `${duration}s`,
      };
    };

    const getHexStyle = (i: number) => {
      const size = Math.random() * 30 + 20;
      const left = `${Math.random() * 100}%`;
      const top = `${Math.random() * 100}%`;
      const delay = Math.random() * 8;
      const duration = Math.random() * 15 + 10;
      const color = i % 2 === 0 ? '#00f2ff' : '#ff00ff';
      const opacity = Math.random() * 0.15 + 0.05;
      return {
        width: `${size}px`,
        height: `${size}px`,
        left,
        top,
        borderColor: color,
        opacity,
        animationDelay: `${delay}s`,
        animationDuration: `${duration}s`,
      };
    };

    const getCircleStyle = (i: number) => {
      const size = Math.random() * 50 + 30;
      const left = `${Math.random() * 100}%`;
      const top = `${Math.random() * 100}%`;
      const delay = Math.random() * 10;
      const duration = Math.random() * 20 + 15;
      const color = i % 2 === 0 ? '#00f2ff' : '#ff00ff';
      const opacity = Math.random() * 0.1 + 0.05;
      return {
        width: `${size}px`,
        height: `${size}px`,
        left,
        top,
        borderColor: color,
        opacity,
        animationDelay: `${delay}s`,
        animationDuration: `${duration}s`,
      };
    };

    onMounted(() => {
      fetchMyServices();
      fetchHiredServices();
    });

    return {
      tab,
      myServices,
      hiredServices,
      selectService,
      deleteService,
      showUpdateModal,
      updateForm,
      openUpdateModal,
      closeUpdateModal,
      updateService,
      showDescriptionModal,
      currentService,
      openDescriptionModal,
      closeDescriptionModal,
      showConfirmModal,
      confirmStatus,
      rejectReason,
      feedback,
      openConfirmModal,
      closeConfirmModal,
      submitConfirmService,
      formatRemainingTime,
      getStatusClass,
      getParticleStyle,
      getHexStyle,
      getCircleStyle,
      searchQuery,
      filterStatus,
      filteredMyServices,
      getStatusCount,
      getServicePerformance,
      formatDate,
    };
  },
});
</script>

<style scoped>
/* Giữ nguyên toàn bộ style từ file gốc */
@import url('https://fonts.googleapis.com/css2?family=Orbitron:wght@400;500;600;700;900&family=Rajdhani:wght@300;400;500;600;700&family=Audiowide&display=swap');

/* Main Container */
.service-history-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #0a001f 0%, #0d1b2a 100%);
  font-family: 'Rajdhani', sans-serif;
  color: #e0e0e0;
  position: relative;
  overflow-x: hidden;
  padding: 80px 20px 100px;
}

/* Holographic Background */
.holographic-background {
  position: absolute;
  inset: 0;
  z-index: 0;
  overflow: hidden;
}

.cyber-grid {
  position: absolute;
  inset: 0;
  background-image: 
    radial-gradient(circle at center, rgba(0, 242, 255, 0.15) 0, rgba(0, 0, 0, 0) 60%),
    linear-gradient(to right, rgba(0, 242, 255, 0.05) 1px, transparent 1px),
    linear-gradient(to bottom, rgba(0, 242, 255, 0.05) 1px, transparent 1px);
  background-size: 100% 100%, 40px 40px, 40px 40px;
  animation: gridPulse 8s infinite alternate;
}

@keyframes gridPulse {
  0% { opacity: 0.5; }
  50% { opacity: 0.8; }
  100% { opacity: 0.5; }
}

.floating-elements {
  position: absolute;
  inset: 0;
  overflow: hidden;
}

.hex-element {
  position: absolute;
  clip-path: polygon(50% 0%, 100% 25%, 100% 75%, 50% 100%, 0% 75%, 0% 25%);
  border: 1px solid;
  background-color: transparent;
  animation: float 20s infinite linear;
}

.circle-element {
  position: absolute;
  border-radius: 50%;
  border: 1px solid;
  background-color: transparent;
  animation: float 25s infinite linear;
}

/* Particle Background */
.particle-background {
  position: absolute;
  inset: 0;
  z-index: 0;
  overflow: hidden;
}

.particle {
  position: absolute;
  border-radius: 50%;
  animation: float 10s infinite ease-in-out;
}

@keyframes float {
  0% { transform: translateY(0) rotate(0deg); opacity: 0.8; }
  50% { transform: translateY(-100px) rotate(180deg); opacity: 0.3; }
  100% { transform: translateY(0) rotate(360deg); opacity: 0.8; }
}

/* Header */
.header {
  position: relative;
  z-index: 1;
  padding: 40px 0 20px;
  text-align: center;
  background: linear-gradient(to bottom, rgba(10, 0, 31, 0.8), transparent);
  margin-bottom: 30px;
  border-radius: 15px;
  overflow: hidden;
}

.header-hologram {
  position: absolute;
  inset: 0;
  background: radial-gradient(ellipse at center, rgba(0, 242, 255, 0.1) 0%, transparent 70%);
  z-index: -1;
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
  z-index: -1;
}

@keyframes scanline {
  0% { background-position: 0 0; }
  100% { background-position: 0 100%; }
}

.header-content {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 20px;
  margin-bottom: 15px;
}

.header-icon-container {
  position: relative;
  width: 50px;
  height: 50px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.header-icon {
  font-size: 2.2rem;
  z-index: 1;
}

.text-neon-cyan {
  color: #00f2ff;
  text-shadow: 0 0 15px #00f2ff, 0 0 25px #00f2ff;
}

.text-neon-magenta {
  color: #ff00ff;
  text-shadow: 0 0 15px #ff00ff, 0 0 25px #ff00ff;
}

.pulse-animation {
  animation: pulse 2s infinite alternate;
}

@keyframes pulse {
  0% { transform: scale(1); opacity: 0.8; }
  100% { transform: scale(1.1); opacity: 1; }
}

.header-title {
  font-size: 3rem;
  font-weight: 700;
  letter-spacing: 3px;
  color: transparent;
  background: linear-gradient(to right, #00f2ff, #ff00ff);
  -webkit-background-clip: text;
  background-clip: text;
  text-shadow: 0 0 15px rgba(0, 242, 255, 0.5);
  font-family: 'Orbitron', sans-serif;
  margin: 0;
  position: relative;
}

.header-subtitle {
  font-size: 1.2rem;
  color: #e0e0e0;
  margin: 10px 0 0;
  font-family: 'Rajdhani', sans-serif;
  letter-spacing: 1px;
  position: relative;
  display: inline-block;
}

.typing-text {
  display: inline-block;
  overflow: hidden;
  border-right: 2px solid transparent;
  white-space: nowrap;
  animation: typing 3.5s steps(40, end) 1s 1 normal both, blink-caret 0.75s step-end infinite;
}

.cursor {
  display: inline-block;
  color: #00f2ff;
  animation: blink 1s infinite;
}

@keyframes typing {
  from { width: 0 }
  to { width: 100% }
}

@keyframes blink {
  0%, 100% { opacity: 1; }
  50% { opacity: 0; }
}

/* Tabs Container */
.tabs-container {
  position: relative;
  z-index: 1;
  margin-bottom: 30px;
}

/* Tabs */
.tabs {
  display: flex;
  justify-content: center;
  gap: 20px;
  margin-bottom: 20px;
}

.tab-button {
  padding: 12px 24px;
  background: rgba(0, 0, 0, 0.4);
  color: #e0e0e0;
  border: 2px solid #333;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-family: 'Orbitron', sans-serif;
  font-weight: 600;
  font-size: 1rem;
  display: flex;
  align-items: center;
  gap: 12px;
  position: relative;
  overflow: hidden;
}

.tab-button::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(45deg, transparent, rgba(255, 255, 255, 0.1), transparent);
  transform: translateX(-100%);
  transition: transform 0.6s ease;
}

.tab-button:hover::before {
  transform: translateX(100%);
}

.tab-active {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3);
}

.tab-icon-container {
  position: relative;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.tab-icon {
  font-size: 1.2rem;
  z-index: 1;
}

.tab-icon-glow {
  position: absolute;
  inset: -5px;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(0, 242, 255, 0.5) 0%, transparent 70%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.tab-active .tab-icon-glow {
  opacity: 1;
  animation: iconPulse 2s infinite alternate;
}

@keyframes iconPulse {
  0% { transform: scale(1); opacity: 0.5; }
  100% { transform: scale(1.2); opacity: 1; }
}

.tab-cyan {
  border-color: #00f2ff;
  background: linear-gradient(to right, rgba(0, 242, 255, 0.2), rgba(0, 0, 0, 0.4));
  color: #fff;
  box-shadow: 0 0 20px rgba(0, 242, 255, 0.3);
}

.tab-magenta {
  border-color: #ff00ff;
  background: linear-gradient(to right, rgba(255, 0, 255, 0.2), rgba(0, 0, 0, 0.4));
  color: #fff;
  box-shadow: 0 0 20px rgba(255, 0, 255, 0.3);
}

/* Service Content */
.service-content {
  position: relative;
  z-index: 1;
  max-width: 1200px;
  margin: 0 auto;
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

@keyframes emptyPulse {
  0% { opacity: 0.3; transform: scale(0.95); }
  100% { opacity: 0.6; transform: scale(1.05); }
}

/* Card Container */
.card-container {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 25px;
  padding: 10px;
}

.service-card {
  background: rgba(10, 0, 31, 0.8);
  border: 2px solid #00f2ff;
  border-radius: 15px;
  padding: 25px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3), 0 0 20px rgba(0, 242, 255, 0.2);
  display: flex;
  flex-direction: column;
  gap: 15px;
  z-index: 1;
  backdrop-filter: blur(5px);
}

.service-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow: 0 15px 30px rgba(0, 242, 255, 0.2), 0 0 30px rgba(0, 242, 255, 0.4);
  border-color: #00f2ff;
}

.card-glow {
  position: absolute;
  inset: 0;
  background: radial-gradient(circle at 50% 50%, rgba(0, 242, 255, 0.15), transparent 70%);
  opacity: 0;
  transition: opacity 0.3s ease;
  z-index: -1;
}

.service-card:hover .card-glow {
  opacity: 1;
  animation: cardGlowPulse 3s infinite alternate;
}

@keyframes cardGlowPulse {
  0% { opacity: 0.3; }
  100% { opacity: 0.8; }
}

.card-hologram {
  position: absolute;
  inset: 0;
  z-index: -1;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.service-card:hover .card-hologram {
  opacity: 1;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 10px;
  border-bottom: 1px solid rgba(0, 242, 255, 0.3);
  padding-bottom: 12px;
}

.service-name {
  font-size: 1.4rem;
  font-weight: 700;
  color: #00f2ff;
  font-family: 'Orbitron', sans-serif;
  margin: 0;
  text-shadow: 0 0 8px rgba(0, 242, 255, 0.5), 0 0 15px rgba(0, 242, 255, 0.3);
  letter-spacing: 1px;
}

.service-content-wrapper {
  display: flex;
  flex-direction: column;
  gap: 15px;
  flex: 1;
}

.service-description {
  font-size: 1rem;
  color: #e0e0e0;
  margin: 0;
  line-height: 1.5;
  height: 48px;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  background: rgba(0, 0, 0, 0.2);
  padding: 10px;
  border-radius: 8px;
}

.service-price-container {
  display: flex;
  align-items: center;
  gap: 10px;
  background: rgba(0, 0, 0, 0.2);
  padding: 10px 15px;
  border-radius: 8px;
}

.price-label {
  font-size: 1rem;
  color: #b0b0b0;
}

.service-price {
  font-size: 1.4rem;
  font-weight: 700;
  color: #00ff00;
  text-shadow: 0 0 8px rgba(0, 255, 0, 0.5), 0 0 15px rgba(0, 255, 0, 0.3);
  margin: 0;
  font-family: 'Orbitron', sans-serif;
}

.service-price span {
  font-size: 1rem;
  opacity: 0.8;
}

.service-status {
  font-size: 0.9rem;
  font-weight: 600;
  padding: 5px 12px;
  border-radius: 20px;
  display: inline-block;
  margin: 0;
}

.status-active {
  background-color: rgba(0, 255, 0, 0.2);
  color: #00ff00;
  border: 1px solid #00ff00;
  box-shadow: 0 0 10px rgba(0, 255, 0, 0.3);
}

.status-deleted {
  background-color: rgba(255, 0, 0, 0.2);
  color: #ff0000;
  border: 1px solid #ff0000;
  box-shadow: 0 0 10px rgba(255, 0, 0, 0.3);
}

.service-meta {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin: 0;
  background: rgba(0, 0, 0, 0.2);
  padding: 15px;
  border-radius: 8px;
}

.service-level,
.service-time,
.service-rented {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 1rem;
  color: #e0e0e0;
}

.meta-icon-container {
  position: relative;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.meta-icon {
  color: #00f2ff;
  z-index: 1;
}

.meta-icon-glow {
  position: absolute;
  inset: -5px;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(0, 242, 255, 0.5) 0%, transparent 70%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.service-card:hover .meta-icon-glow {
  opacity: 1;
}

.card-progress {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-top: 5px;
  background: rgba(0, 0, 0, 0.2);
  padding: 10px 15px;
  border-radius: 8px;
}

.progress-label {
  font-size: 0.9rem;
  color: #b0b0b0;
  min-width: 70px;
}

.progress-bar {
  flex: 1;
  height: 8px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 4px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(to right, #00f2ff, #ff00ff);
  border-radius: 4px;
  transition: width 1s ease;
  box-shadow: 0 0 10px rgba(0, 242, 255, 0.5);
}

.progress-value {
  font-size: 0.9rem;
  color: #00f2ff;
  min-width: 40px;
  text-align: right;
  font-weight: bold;
}

.service-actions {
  display: flex;
  gap: 15px;
  margin-top: 10px;
}

.update-button,
.delete-button {
  flex: 1;
  padding: 10px 0;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-family: 'Orbitron', sans-serif;
  font-weight: 600;
  font-size: 0.9rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.update-button {
  background: linear-gradient(to right, #00cc00, #009900);
  color: #fff;
  box-shadow: 0 0 10px rgba(0, 204, 0, 0.3);
}

.update-button:hover {
  background: linear-gradient(to right, #00ff00, #00cc00);
  box-shadow: 0 0 15px rgba(0, 255, 0, 0.5);
  transform: translateY(-3px);
}

.delete-button {
  background: linear-gradient(to right, #cc0000, #990000);
  color: #fff;
  box-shadow: 0 0 10px rgba(204, 0, 0, 0.3);
}

.delete-button:hover {
  background: linear-gradient(to right, #ff0000, #cc0000);
  box-shadow: 0 0 15px rgba(255, 0, 0, 0.5);
  transform: translateY(-3px);
}

.card-hover-effect {
  position: absolute;
  inset: 0;
  background: linear-gradient(45deg, transparent, rgba(0, 242, 255, 0.1), transparent);
  transform: translateX(-100%);
  transition: transform 0.6s ease;
  z-index: -1;
}

.service-card:hover .card-hover-effect {
  transform: translateX(100%);
}

/* Table Styling */
.table-wrapper {
  background: rgba(10, 0, 31, 0.8);
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
}

.magenta-table {
  border: 2px solid #ff00ff;
  box-shadow: 0 0 20px rgba(255, 0, 255, 0.2);
}

.service-table {
  width: 100%;
  border-collapse: collapse;
}

.service-table th {
  background: rgba(0, 0, 0, 0.5);
  color: #ff00ff;
  font-family: 'Orbitron', sans-serif;
  font-weight: 600;
  text-align: left;
  padding: 18px 15px;
  border-bottom: 1px solid rgba(255, 0, 255, 0.3);
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
  background: linear-gradient(to right, #ff00ff, transparent);
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
  background-color: rgba(255, 0, 255, 0.05);
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

/* Action Buttons */
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
  background: linear-gradient(to right, #00cc00, #009900);
  color: #fff;
  box-shadow: 0 0 10px rgba(0, 204, 0, 0.3);
}

.confirm-button:hover {
  background: linear-gradient(to right, #00ff00, #00cc00);
  box-shadow: 0 0 15px rgba(0, 255, 0, 0.5);
  transform: translateY(-2px);
}

/* Hired Services Dashboard */
.hired-services-dashboard {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.dashboard-card {
  background: rgba(10, 0, 31, 0.8);
  border: 1px solid rgba(255, 0, 255, 0.3);
  border-radius: 15px;
  padding: 20px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);
}

.dashboard-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3), 0 0 20px rgba(255, 0, 255, 0.2);
  border-color: #ff00ff;
  background: rgba(10, 0, 31, 0.9);
}

.dashboard-value {
  font-size: 2.5rem;
  font-weight: 700;
  color: #fff;
  font-family: 'Orbitron', sans-serif;
  margin-bottom: 5px;
  text-shadow: 0 0 10px rgba(255, 0, 255, 0.5);
}

.dashboard-label {
  font-size: 1rem;
  color: #b0b0b0;
  margin-bottom: 15px;
}

.dashboard-icon {
  font-size: 2rem;
  color: #ff00ff;
  text-shadow: 0 0 10px rgba(255, 0, 255, 0.8);
  position: absolute;
  bottom: 15px;
  right: 15px;
  opacity: 0.5;
  transition: all 0.3s ease;
}

.dashboard-card:hover .dashboard-icon {
  opacity: 1;
  transform: scale(1.2);
}

/* Update Modal */
.update-modal {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 90vw;
  max-width: 500px;
  border: 2px solid #00f2ff;
  border-radius: 15px;
  padding: 20px;
  box-shadow: 0 0 30px rgba(0, 242, 255, 0.3);
  z-index: 1000;
}

.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.8);
  backdrop-filter: blur(8px);
  z-index: 999;
}

.modal-content {
  position: relative;
  z-index: 1001;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.modal-header h2 {
  font-size: 1.8rem;
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
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  color: #00f2ff;
  font-family: 'Orbitron', sans-serif;
  font-size: 1rem;
  letter-spacing: 1px;
}

.input-container {
  position: relative;
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: 12px;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid #00f2ff;
  border-radius: 8px;
  color: #fff;
  font-family: 'Rajdhani', sans-serif;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.form-group textarea {
  height: 100px;
  resize: vertical;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  outline: none;
  border-color: #33f5ff;
  box-shadow: 0 0 15px rgba(0, 242, 255, 0.5);
}

.form-actions {
  display: flex;
  gap: 15px;
  justify-content: flex-end;
  margin-top: 20px;
}

.submit-button,
.cancel-button {
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-family: 'Orbitron', sans-serif;
  font-weight: 600;
  font-size: 0.9rem;
  display: flex;
  align-items: center;
  gap: 8px;
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

/* Description Modal */
.description-modal {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 90vw;
  max-width: 500px;
  background: linear-gradient(135deg, #1a1a2e, #16213e);
  border: 2px solid #00f2ff;
  border-radius: 15px;
  padding: 20px;
  box-shadow: 0 0 30px rgba(0, 242, 255, 0.3);
  z-index: 1000;
}

.description-content {
  margin-bottom: 20px;
  padding: 15px;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid #00f2ff;
  border-radius: 8px;
  max-height: 200px;
  overflow-y: auto;
  color: #e0e0e0;
  font-size: 1rem;
  line-height: 1.6;
}

/* Floating Action Button */
.floating-action-button {
  position: fixed;
  bottom: 30px;
  right: 30px;
  z-index: 100;
}

.fab-icon {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #00f2ff, #ff00ff);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  font-size: 1.5rem;
  cursor: pointer;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
}

.fab-icon:hover {
  transform: scale(1.1);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.4), 0 0 20px rgba(0, 242, 255, 0.4);
}

.fab-menu {
  position: absolute;
  bottom: 70px;
  right: 0;
  display: flex;
  flex-direction: column;
  gap: 15px;
  opacity: 0;
  pointer-events: none;
  transform: translateY(20px);
  transition: all 0.3s ease;
}

.floating-action-button:hover .fab-menu {
  opacity: 1;
  pointer-events: auto;
  transform: translateY(0);
}

.fab-item {
  display: flex;
  align-items: center;
  gap: 10px;
  background: rgba(10, 0, 31, 0.9);
  border: 1px solid rgba(0, 242, 255, 0.3);
  border-radius: 30px;
  padding: 10px 20px;
  color: #fff;
  font-family: 'Orbitron', sans-serif;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
}

.fab-item:hover {
  background: linear-gradient(to right, rgba(0, 242, 255, 0.2), rgba(255, 0, 255, 0.2));
  transform: translateX(-10px);
  box-shadow: 0 5px 15px rgba(0, 242, 255, 0.3);
}

/* Transitions */
.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(30px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(-30px);
}

.modal-enter-active,
.modal-leave-active {
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .modal-content {
  transform: scale(0.9) translateY(30px);
}

.modal-leave-to .modal-content {
  transform: scale(0.9) translateY(-30px);
}

/* Responsive Design */
@media (max-width: 1024px) {
  .card-container {
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  }
}

@media (max-width: 768px) {
  .header-title {
    font-size: 2.2rem;
  }

  .header-subtitle {
    font-size: 1rem;
  }

  .tabs {
    flex-direction: column;
    gap: 15px;
  }

  .card-container {
    grid-template-columns: 1fr;
  }

  .service-table th,
  .service-table td {
    padding: 12px 10px;
    font-size: 0.9rem;
  }

  .hired-services-dashboard {
    grid-template-columns: 1fr 1fr;
  }

  .floating-action-button {
    bottom: 20px;
    right: 20px;
  }

  .fab-menu {
    right: 0;
  }
}

@media (max-width: 480px) {
  .header-content {
    gap: 10px;
  }

  .header-icon {
    font-size: 1.8rem;
  }

  .header-title {
    font-size: 1.8rem;
  }

  .hired-services-dashboard {
    grid-template-columns: 1fr;
  }

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
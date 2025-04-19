<template>
  <div class="service-history-container">
    <!-- Particle Background -->
    <div class="particle-background">
      <div v-for="i in 20" :key="i" class="particle"></div>
    </div>

    <!-- Header -->
    <header class="header">
      <h1 class="header-title">Quản lý dịch vụ</h1>
      <p class="header-subtitle">Xem lại các dịch vụ của bạn</p>
    </header>

    <!-- Tabs -->
    <div class="tabs">
      <button
        :class="{ 'tab-button': true, active: tab === 'hired' }"
        @click="tab = 'hired'"
      >
        Dịch vụ đang thuê
      </button>
      <button
        :class="{ 'tab-button': true, active: tab === 'providing' }"
        @click="tab = 'providing'"
      >
        Dịch vụ đang làm
      </button>
      <button
        :class="{ 'tab-button': true, active: tab === 'myServices' }"
        @click="tab = 'myServices'"
      >
        Dịch vụ của tôi
      </button>
    </div>

    <!-- Content -->
    <section class="service-table">
      <!-- Tab: Dịch vụ đang thuê -->
      <div v-if="tab === 'hired'">
        <div v-if="hiredServices.length === 0" class="empty-message">
          Không có dịch vụ đang thuê
        </div>
        <div v-else class="table-wrapper">
          <table class="service-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Service ID</th>
                <th>Mô tả</th>
                <th>Trạng thái</th>
                <th>Lý do</th>
                <th>Hình ảnh</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="service in hiredServices" :key="service.id">
                <td>{{ service.id }}</td>
                <td>{{ service.serviceID }}</td>
                <td>{{ service.decriptions }}</td>
                <td>{{ service.status }}</td>
                <td>{{ service.reason || 'N/A' }}</td>
                <td>
                  <img
                    v-if="service.image"
                    :src="service.image"
                    alt="Service Image"
                    class="service-image"
                  />
                  <span v-else>Không có hình ảnh</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Tab: Dịch vụ đang làm -->
      <div v-if="tab === 'providing'">
        <div v-if="providingServices.length === 0" class="empty-message">
          Không có dịch vụ đang làm
        </div>
        <div v-else class="table-wrapper">
          <table class="service-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Service ID</th>
                <th>Mô tả</th>
                <th>Trạng thái</th>
                <th>Lý do</th>
                <th>Phản hồi</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="service in providingServices" :key="service.id">
                <td>{{ service.id }}</td>
                <td>{{ service.serviceID }}</td>
                <td>{{ service.decriptions }}</td>
                <td>{{ service.status }}</td>
                <td>{{ service.reason || 'N/A' }}</td>
                <td>{{ service.feedBack || 'N/A' }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Tab: Dịch vụ của tôi -->
      <div v-if="tab === 'myServices'" class="empty-message">
        Chưa triển khai
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { userStore } from '@/stores/auth';
import hiredServiceApi from '@/api/hiredservice.api';
import ongoingServiceApi from '@/api/ongoingservice.api';

// Dữ liệu và trạng thái
const store = userStore();
const userId = store.user?.id || JSON.parse(localStorage.getItem('user') || '{}').id;
const tab = ref('hired');
interface Service {
  id: number;
  serviceID: string;
  decriptions: string;
  status: string;
  reason?: string;
  image?: string;
  feedBack?: string;
}

const hiredServices = ref<Service[]>([]);
const providingServices = ref<Service[]>([]);

// Gọi API để lấy dữ liệu
const fetchHiredServices = async () => {
  try {
    const response = await hiredServiceApi.getAllByUserId(userId);
    console.log('Hired Services Response:', response);
    if (response.data?.result?.isSuccess && response.data.result.data) {
      hiredServices.value = response.data.result.data;
    }
  } catch (error) {
    console.error('Lỗi khi lấy dịch vụ đang thuê:', error);
  }
};

const fetchProvidingServices = async () => {
  try {
    const response = await ongoingServiceApi.getAllByUserId(userId);
    console.log('Providing Services Response:', response);
    if (response.data?.result?.isSuccess && response.data.result.data) {
      providingServices.value = response.data.result.data;
    }
  } catch (error) {
    console.error('Lỗi khi lấy dịch vụ đang làm:', error);
  }
};

// Gọi dữ liệu khi component được mount
onMounted(() => {
  fetchHiredServices();
  fetchProvidingServices();
});
</script>

<style scoped>
/* Tổng thể */
.service-history-container {
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
  font-size: 2.5rem;
  font-weight: 700;
  letter-spacing: 2px;
  color: #e0e0e0;
  text-shadow: 0 0 10px #00ffff;
}

.header-subtitle {
  font-size: 1.1rem;
  color: #b0b0b0;
  margin: 10px 0 20px;
}

/* Tabs */
.tabs {
  display: flex;
  justify-content: center;
  gap: 10px;
  padding: 20px;
}

.tab-button {
  padding: 10px 20px;
  background: rgba(0, 255, 255, 0.1);
  color: #e0e0e0;
  border: 1px solid #00ffff;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.tab-button:hover {
  background: #00ffff;
  color: #1a0933;
}

.tab-button.active {
  background: #00ffff;
  color: #1a0933;
  box-shadow: 0 0 10px #00ffff;
}

/* Service Table */
.service-table {
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
  max-width: 1200px;
  margin: 0 auto;
  background: rgba(28, 37, 38, 0.9);
  border-radius: 8px;
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.3);
  overflow: hidden;
}

.service-table {
  width: 100%;
  border-collapse: collapse;
}

.service-table th,
.service-table td {
  padding: 15px;
  text-align: left;
  border-bottom: 1px solid rgba(0, 255, 255, 0.1);
}

.service-table th {
  background: linear-gradient(135deg, #0d1b2a 0%, #1a0933 100%);
  color: #00ffff;
  font-weight: 600;
  text-shadow: 0 0 5px rgba(0, 255, 255, 0.5);
}

.service-table tr:hover {
  background: rgba(0, 255, 255, 0.1);
}

.service-table td {
  color: #e0e0e0;
}

.service-image {
  width: 50px;
  height: 50px;
  object-fit: cover;
  border-radius: 4px;
}

/* Responsive */
@media (max-width: 768px) {
  .header-title {
    font-size: 2rem;
  }
  .header-subtitle {
    font-size: 1rem;
  }
  .service-table {
    padding: 20px;
  }
  .service-table th,
  .service-table td {
    padding: 10px;
    font-size: 0.9rem;
  }
  .service-image {
    width: 40px;
    height: 40px;
  }
  .tabs {
    flex-direction: column;
    gap: 10px;
  }
  .tab-button {
    padding: 8px 15px;
  }
}
</style>
<template>
  <div class="seller-tooltip-wrapper" @mouseenter="showTooltip" @mouseleave="hideTooltip">
    <slot></slot>
    <Transition name="fade">
      <div v-if="show" class="seller-tooltip">
        <div class="tooltip-content">
          <div v-if="isLoading" class="loading-state">
            <div class="loading-spinner"></div>
            <span>Đang tải...</span>
          </div>
          <div v-else-if="error" class="error-state">
            <div class="error-icon">!</div>
            <span>{{ error }}</span>
          </div>
          <div v-else class="seller-info">
            <div class="avatar-frame">
              <img :src="sellerInfo.avatar || 'https://via.placeholder.com/80'" alt="Avatar" class="avatar">
              <div class="avatar-glow"></div>
            </div>
            <div class="info-container">
              <h4 class="seller-name">{{ sellerInfo.fullName || 'Không xác định' }}</h4>
              <div class="seller-level">
                <span class="level-label">Cấp độ:</span>
                <span class="level-value">{{ sellerInfo.level }}</span>
              </div>
              <div class="seller-joined">
                <span class="joined-label">Tham gia:</span>
                <span class="joined-value">{{ formatDate(sellerInfo.createdDate) }}</span>
              </div>
            </div>
          </div>
        </div>
        <div class="tooltip-arrow"></div>
      </div>
    </Transition>
  </div>
</template>

<script>
import { ref } from 'vue';
import websiteaccountApi from '@/api/websiteaccount.api';

export default {
  props: {
    sellerId: {
      type: Number,
      required: true,
    },
  },
  setup(props) {
    const show = ref(false);
    const sellerInfo = ref(null);
    const isLoading = ref(false);
    const error = ref('');

    const fetchSellerInfo = async () => {
      if (!props.sellerId || isNaN(props.sellerId)) {
        error.value = 'ID không hợp lệ';
        return;
      }
      isLoading.value = true;
      error.value = '';
      try {
        const response = await websiteaccountApi.getById(props.sellerId);
        if (response.data?.result?.isSuccess && response.data.result.data) {
          sellerInfo.value = response.data.result.data;
        } else {
          error.value = 'Không thể lấy thông tin';
        }
      } catch (err) {
        error.value = 'Lỗi khi gọi API';
      } finally {
        isLoading.value = false;
      }
    };

    const showTooltip = () => {
      show.value = true;
      if (!sellerInfo.value && !isLoading.value) {
        fetchSellerInfo();
      }
    };

    const hideTooltip = () => {
      show.value = false;
    };

    const formatDate = (dateStr) => {
      if (!dateStr) return 'Không xác định';
      const date = new Date(dateStr);
      return date.toLocaleDateString('vi-VN');
    };

    return {
      show,
      sellerInfo,
      isLoading,
      error,
      showTooltip,
      hideTooltip,
      formatDate,
    };
  },
};
</script>

<style scoped>
.seller-tooltip-wrapper {
  position: relative;
  display: inline-block;
}

.seller-tooltip {
  position: absolute;
  top: calc(100% + 10px);
  left: 50%;
  transform: translateX(-50%);
  z-index: 2000;
  background: linear-gradient(135deg, #1e1033, #2d1b4e);
  border-radius: 12px;
  padding: 18px;
  box-shadow: 0 0 25px rgba(138, 43, 226, 0.35), 0 0 5px rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(186, 85, 255, 0.4);
  width: 280px;
  color: #e0e0ff;
  margin-top: 5px;
  backdrop-filter: blur(5px);
  transition: all 0.3s ease;
}

.tooltip-arrow {
  position: absolute;
  top: -8px;
  left: 50%;
  transform: translateX(-50%);
  width: 16px;
  height: 16px;
  background: linear-gradient(135deg, #1e1033, #2d1b4e);
  border-top: 1px solid rgba(186, 85, 255, 0.4);
  border-left: 1px solid rgba(186, 85, 255, 0.4);
  transform: translateX(-50%) rotate(45deg);
}

.tooltip-content {
  position: relative;
  z-index: 2;
}

.seller-info {
  display: flex;
  align-items: center;
}

.avatar-frame {
  position: relative;
  margin-right: 15px;
  width: 80px;
  height: 80px;
  border-radius: 50%;
  padding: 3px;
  background: linear-gradient(135deg, #9c4dff, #4a00e0);
}

.avatar {
  width: 100%;
  height: 100%;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid #1e1033;
}

.avatar-glow {
  position: absolute;
  top: -3px;
  left: -3px;
  right: -3px;
  bottom: -3px;
  border-radius: 50%;
  background: transparent;
  border: 2px solid rgba(186, 85, 255, 0.6);
  animation: pulse 2s infinite;
  z-index: -1;
}

.info-container {
  flex: 1;
}

.seller-name {
  margin: 0 0 8px 0;
  font-size: 18px;
  font-weight: 600;
  color: #bb86fc;
  text-shadow: 0 0 10px rgba(187, 134, 252, 0.5);
  letter-spacing: 0.5px;
}

.seller-level, .seller-joined {
  display: flex;
  margin: 6px 0;
  font-size: 14px;
  align-items: center;
}

.level-label, .joined-label {
  color: #a0a0d0;
  margin-right: 8px;
  font-weight: 300;
}

.level-value, .joined-value {
  color: #e9e9ff;
  font-weight: 500;
  background: linear-gradient(90deg, #9c4dff, #4a00e0);
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 13px;
}

.loading-state, .error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 20px 0;
  width: 100%;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 3px solid rgba(186, 85, 255, 0.3);
  border-top: 3px solid #bb86fc;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 12px;
}

.error-icon {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, #ff4d4d, #b30000);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  font-weight: bold;
  color: white;
  margin-bottom: 12px;
}

/* Animations */
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes pulse {
  0% { transform: scale(1); opacity: 0.8; }
  50% { transform: scale(1.05); opacity: 0.4; }
  100% { transform: scale(1); opacity: 0.8; }
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s, transform 0.3s;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
  transform: translateX(-50%) translateY(-10px);
}
</style>
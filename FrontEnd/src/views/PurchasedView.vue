<template>
  <div class="cyber-vault">
    <!-- Animated Background Elements -->
    <div class="cyber-bg">
      <div class="grid-overlay"></div>
      <div class="cyber-particles"></div>
      <div class="cyber-lines"></div>
    </div>
    
    <!-- Holographic Header -->
    <header class="vault-header">
      <div class="header-hologram">
        <div class="hologram-ring"></div>
        <div class="hologram-planet"></div>
      </div>
      <div class="header-text">
        <h1 class="glitch-text" data-text="GAME VAULT">GAME VAULT</h1>
        <div class="header-scan-line"></div>
        <p class="header-subtitle">TÀI KHOẢN QUẢN LÝ <span class="blink">_</span></p>
      </div>
    </header>

    <!-- Command Center -->
    <div class="command-center">
      <div class="command-panel">
        <div class="panel-header">
          <i class="fas fa-terminal"></i>
          <span>VAULT CONTROL</span>
        </div>
        <div class="panel-stats">
          <div class="stat-item">
            <div class="stat-icon"><i class="fas fa-gamepad"></i></div>
            <div class="stat-info">
              <div class="stat-value">{{ purchasedAccounts.length }}</div>
              <div class="stat-label">TÀI KHOẢN ĐÃ MUA</div>
            </div>
          </div>
          <div class="stat-item">
            <div class="stat-icon"><i class="fas fa-handshake"></i></div>
            <div class="stat-info">
              <div class="stat-value">{{ tradingAccounts.length }}</div>
              <div class="stat-label">ĐANG GIAO DỊCH</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="vault-loading">
      <div class="loading-container">
        <div class="loading-cube">
          <div class="cube-face front"></div>
          <div class="cube-face back"></div>
          <div class="cube-face right"></div>
          <div class="cube-face left"></div>
          <div class="cube-face top"></div>
          <div class="cube-face bottom"></div>
        </div>
        <p class="loading-text">ĐANG TẢI DỮ LIỆU<span class="dot-1">.</span><span class="dot-2">.</span><span class="dot-3">.</span></p>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="vault-error">
      <div class="error-container">
        <div class="error-icon">
          <i class="fas fa-exclamation-triangle"></i>
        </div>
        <div class="error-message">
          <h3>LỖI KẾT NỐI</h3>
          <p>{{ error }}</p>
        </div>
        <button class="cyber-button" @click="fetchAllAccounts">
          <span class="button-content">THỬ LẠI</span>
          <span class="button-glitch"></span>
        </button>
      </div>
    </div>

    <!-- Main Content -->
    <div v-else class="vault-content">
      <!-- Tabs Navigation -->
      <div class="vault-tabs">
        <button 
          class="tab-button" 
          :class="{ active: activeTab === 'purchased' }" 
          @click="activeTab = 'purchased'"
        >
          <i class="fas fa-shopping-cart"></i> TÀI KHOẢN ĐÃ MUA
        </button>
        <button 
          class="tab-button" 
          :class="{ active: activeTab === 'trading' }" 
          @click="switchToTradingTab"
        >
          <i class="fas fa-handshake"></i> ĐANG GIAO DỊCH
        </button>
      </div>

      <!-- Purchased Accounts -->
      <div v-if="activeTab === 'purchased'">
        <div v-if="purchasedAccounts.length === 0" class="vault-empty">
          <div class="empty-container">
            <div class="empty-hologram">
              <div class="hologram-rings">
                <div class="ring ring-1"></div>
                <div class="ring ring-2"></div>
                <div class="ring ring-3"></div>
              </div>
              <div class="hologram-icon">
                <i class="fas fa-shopping-cart"></i>
              </div>
            </div>
            <h3 class="empty-title">VAULT TRỐNG</h3>
            <p class="empty-desc">Bạn chưa mua tài khoản nào</p>
            <button class="cyber-button pulse-button">
              <span class="button-content">KHÁM PHÁ CỬA HÀNG</span>
              <span class="button-glitch"></span>
            </button>
          </div>
        </div>
        <div v-else class="accounts-grid">
          <div 
            v-for="account in purchasedAccounts" 
            :key="account.id" 
            class="account-card"
            :class="getStatusClass(account.statusBuyer ?? undefined)"
          >
            <div class="card-holo-effect"></div>
            <div class="game-banner">
              <div class="game-icon">
                <i class="fas fa-gamepad"></i>
              </div>
              <h3 class="game-title">{{ account.gameName }}</h3>
              <div class="status-badge" :class="getStatusClass(account.statusBuyer ?? undefined)">
                {{ getStatusText(account.statusBuyer ?? undefined) }}
              </div>
              <button 
                v-if="account.statusBuyer !== 'Mua thành công' && account.statusBuyer !== 'Đã từ chối'" 
                class="report-btn" 
                @click="openDisputeModal(account.id)"
              >
                <i class="fas fa-exclamation-triangle"></i>
              </button>
            </div>
            <div class="account-details">
              <div class="detail-group">
                <div class="detail-row">
                  <div class="detail-icon"><i class="fas fa-user"></i></div>
                  <div class="detail-content">
                    <div class="detail-label">TÀI KHOẢN</div>
                    <div class="detail-value">{{ account.accountName }}</div>
                  </div>
                </div>
                <div class="detail-row">
                  <div class="detail-icon"><i class="fas fa-key"></i></div>
                  <div class="detail-content">
                    <div class="detail-label">MẬT KHẨU</div>
                    <div class="detail-value password-value">
                      <span>{{ account.password }}</span>
                      <button class="copy-btn" @click="account.password ? copyToClipboard(account.password) : null">
                        <i class="fas fa-copy"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
              <div class="detail-group">
                <div class="detail-row">
                  <div class="detail-icon"><i class="fas fa-tag"></i></div>
                  <div class="detail-content">
                    <div class="detail-label">GIÁ TIỀN</div>
                    <div class="detail-value price-value">{{ formatPrice(account.price) }}</div>
                  </div>
                </div>
                <div v-if="account.statusBuyer === 'Chưa xác nhận'" class="detail-row">
                  <div class="detail-icon"><i class="fas fa-clock"></i></div>
                  <div class="detail-content">
                    <div class="detail-label">THỜI GIAN XÁC NHẬN</div>
                    <div class="detail-value countdown-value" @click="openCountdownModal(account.id)">
                      <i class="fas fa-clock"></i>
                      {{ getCountdown(account.id) || 'Hết hạn' }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="account-actions">
              <button
                v-if="account.statusBuyer !== 'Mua thành công' && account.statusBuyer !== 'Đã từ chối'"
                class="cyber-button direct-confirm-btn"
                @click="openDirectConfirmModal(account.id)"
              >
                <span class="button-content">XÁC NHẬN TRỰC TIẾP</span>
                <span class="button-glitch"></span>
              </button>
              <button
                v-if="account.statusBuyer !== 'Mua thành công' && account.statusBuyer !== 'Đã từ chối'"
                class="cyber-button reject-btn"
                @click="openRejectModal(account.id)"
              >
                <span class="button-content">TỪ CHỐI</span>
                <span class="button-glitch"></span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Trading Accounts -->
      <div v-if="activeTab === 'trading'">
        <div v-if="tradingAccounts.length === 0" class="vault-empty">
          <div class="empty-container">
            <div class="empty-hologram">
              <div class="hologram-rings">
                <div class="ring ring-1"></div>
                <div class="ring ring-2"></div>
                <div class="ring ring-3"></div>
              </div>
              <div class="hologram-icon">
                <i class="fas fa-handshake"></i>
              </div>
            </div>
            <h3 class="empty-title">KHÔNG CÓ GIAO DỊCH</h3>
            <p class="empty-desc">Bạn không có tài khoản nào đang giao dịch</p>
          </div>
        </div>
        <div v-else class="accounts-grid">
          <div 
            v-for="account in tradingAccounts" 
            :key="account.id" 
            class="account-card status-trading"
          >
            <div class="card-holo-effect"></div>
            <div class="game-banner">
              <div class="game-icon">
                <i class="fas fa-gamepad"></i>
              </div>
              <h3 class="game-title">{{ account.gameName }}</h3>
              <div class="status-badge status-trading">
                ĐANG GIAO DỊCH
              </div>
              <button 
                class="report-btn" 
                @click="openDisputeModal(account.id)"
              >
                <i class="fas fa-exclamation-triangle"></i>
              </button>
            </div>
            <div class="account-details">
              <div class="detail-group">
                <div class="detail-row">
                  <div class="detail-icon"><i class="fas fa-user"></i></div>
                  <div class="detail-content">
                    <div class="detail-label">TÀI KHOẢN</div>
                    <div class="detail-value">{{ account.accountName }}</div>
                  </div>
                </div>
                <div class="detail-row">
                  <div class="detail-icon"><i class="fas fa-tag"></i></div>
                  <div class="detail-content">
                    <div class="detail-label">GIÁ TIỀN</div>
                    <div class="detail-value price-value">{{ formatPrice(account.price) }}</div>
                  </div>
                </div>
                <div v-if="account.statusBuyer === 'Chưa xác nhận'" class="detail-row">
                  <div class="detail-icon"><i class="fas fa-clock"></i></div>
                  <div class="detail-content">
                    <div class="detail-label">THỜI GIAN XÁC NHẬN</div>
                    <div class="detail-value countdown-value" @click="openCountdownModal(account.id)">
                      <i class="fas fa-clock"></i>
                      {{ getCountdown(account.id) || 'Hết hạn' }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="account-actions">
              <template v-if="account.userID === userId">
                <button
                  class="cyber-button direct-confirm-btn"
                  @click="openDirectConfirmModal(account.id)"
                >
                  <span class="button-content">XÁC NHẬN TRỰC TIẾP</span>
                  <span class="button-glitch"></span>
                </button>
                <button
                  class="cyber-button reject-btn"
                  @click="openRejectModal(account.id)"
                >
                  <span class="button-content">TỪ CHỐI</span>
                  <span class="button-glitch"></span>
                </button>
              </template>
            </div>
          </div>
        </div>
      </div>

      <!-- Direct Confirm Modal -->
      <div v-if="showDirectConfirmModal" class="modal-overlay" @click.self="closeDirectConfirmModal">
        <div class="cyber-modal decision-modal">
          <div class="modal-header">
            <h3>XÁC NHẬN TRỰC TIẾP</h3>
            <button class="close-btn" @click="closeDirectConfirmModal">×</button>
          </div>
          <div class="modal-content">
            <div class="modal-hologram">
              <div class="hologram-rings">
                <div class="ring ring-1"></div>
                <div class="ring ring-2"></div>
              </div>
              <div class="hologram-icon warning-icon">
                <i class="fas fa-exclamation-triangle"></i>
              </div>
            </div>
            <p>Bạn có chắc chắn muốn xác nhận tài khoản này? Bằng cách xác nhận, bạn đồng ý rằng mọi thông tin đã được thay đổi và chúng tôi sẽ không giải quyết nếu tài khoản xảy ra vấn đề sau đó.</p>
          </div>
          <div class="modal-actions">
            <button @click="closeDirectConfirmModal" class="cyber-button cancel-btn">
              <span class="button-content">HỦY</span>
              <span class="button-glitch"></span>
            </button>
            <button @click="handleDirectConfirm" class="cyber-button confirm-btn">
              <span class="button-content">XÁC NHẬN</span>
              <span class="button-glitch"></span>
            </button>
          </div>
        </div>
      </div>

      <!-- Reject Reason Modal -->
      <div v-if="showRejectModal" class="modal-overlay" @click.self="cancelReject">
        <div class="cyber-modal reject-modal">
          <div class="modal-header">
            <h3>LÝ DO TỪ CHỐI</h3>
            <button class="close-btn" @click="cancelReject">×</button>
          </div>
          <div class="modal-content">
            <div class="modal-hologram">
              <div class="hologram-rings">
                <div class="ring ring-1"></div>
                <div class="ring ring-2"></div>
              </div>
              <div class="hologram-icon">
                <i class="fas fa-exclamation"></i>
              </div>
            </div>
            <p>Vui lòng nhập lý do từ chối tài khoản này:</p>
            <textarea 
              v-model="rejectionReason" 
              placeholder="Nhập lý do..." 
              rows="4"
              class="cyber-textarea"
            ></textarea>
          </div>
          <div class="modal-actions">
            <button @click="cancelReject" class="cyber-button cancel-btn">
              <span class="button-content">HỦY BỎ</span>
              <span class="button-glitch"></span>
            </button>
            <button @click="confirmReject" class="cyber-button confirm-reject-btn">
              <span class="button-content">XÁC NHẬN TỪ CHỐI</span>
              <span class="button-glitch"></span>
            </button>
          </div>
        </div>
      </div>

      <!-- Dispute Modal -->
      <div v-if="showDisputeModal" class="modal-overlay" @click.self="closeDisputeModal">
        <div class="cyber-modal dispute-modal">
          <div class="modal-header">
            <h3>BÁO CÁO TRANH CHẤP</h3>
            <button class="close-btn" @click="closeDisputeModal">×</button>
          </div>
          <div class="modal-content">
            <p>Vui lòng nhập lý do tranh chấp và tải lên các file liên quan (nếu có):</p>
            <textarea 
              v-model="disputeReason" 
              placeholder="Nhập lý do..." 
              rows="4"
              class="cyber-textarea"
            ></textarea>
            <input type="file" multiple @change="handleFileUpload" class="file-input" />
          </div>
          <div class="modal-actions">
            <button @click="closeDisputeModal" class="cyber-button cancel-btn">
              <span class="button-content">HỦY</span>
              <span class="button-glitch"></span>
            </button>
            <button @click="submitDispute" class="cyber-button confirm-btn">
              <span class="button-content">GỬI BÁO CÁO</span>
              <span class="button-glitch"></span>
            </button>
          </div>
        </div>
      </div>

      <!-- Countdown Modal -->
      <div v-if="showCountdownModal" class="modal-overlay" @click.self="closeCountdownModal">
        <div class="cyber-modal countdown-modal">
          <div class="modal-header">
            <h3>THỜI GIAN XÁC NHẬN</h3>
            <button class="close-btn" @click="closeCountdownModal">×</button>
          </div>
          <div class="modal-content">
            <div class="modal-hologram">
              <div class="hologram-rings">
                <div class="ring ring-1"></div>
                <div class="ring ring-2"></div>
              </div>
              <div class="hologram-icon countdown-icon">
                <i class="fas fa-clock"></i>
              </div>
            </div>
            <p v-if="countdownTime">Thời gian còn lại để xác nhận tài khoản: <strong>{{ countdownTime }}</strong></p>
            <p v-else>Thời hạn xác nhận đã hết. Hệ thống sẽ tự động hoàn tất giao dịch.</p>
            <p>Nếu không xác nhận trong thời gian quy định, giao dịch sẽ tự động hoàn tất, bạn sẽ bị trừ 100,000 điểm kinh nghiệm, và số tiền sẽ được chuyển cho người bán.</p>
          </div>
          <div class="modal-actions">
            <button @click="closeCountdownModal" class="cyber-button ok-btn">
              <span class="button-content">XÁC NHẬN</span>
              <span class="button-glitch"></span>
            </button>
          </div>
        </div>
      </div>

      <!-- Result Modal -->
      <div v-if="showResultModal" class="modal-overlay" @click.self="closeResultModal">
        <div class="cyber-modal result-modal">
          <div class="modal-header">
            <h3>THÔNG BÁO HỆ THỐNG</h3>
            <button class="close-btn" @click="closeResultModal">×</button>
          </div>
          <div class="modal-content">
            <div class="modal-hologram">
              <div class="hologram-rings">
                <div class="ring ring-1"></div>
                <div class="ring ring-2"></div>
              </div>
              <div class="hologram-icon" :class="resultIconClass">
                <i :class="getResultIcon"></i>
              </div>
            </div>
            <p>{{ resultMessage }}</p>
          </div>
          <div class="modal-actions">
            <button @click="closeResultModal" class="cyber-button ok-btn">
              <span class="button-content">XÁC NHẬN</span>
              <span class="button-glitch"></span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, onUnmounted } from 'vue';
import { userStore } from '@/stores/auth';
import purchasedApi from '@/api/purchased.api';
import disputeApi from '@/api/dispute.api';
import type { PurchasedAccount } from '@/models/purchased.model';

// Initialize store and retrieve userId
const store = userStore();
const userId = store.user?.id || JSON.parse(localStorage.getItem('user') || '{}').id;

// State
const purchasedAccounts = ref<PurchasedAccount[]>([]);
const tradingAccounts = ref<PurchasedAccount[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const showDirectConfirmModal = ref(false);
const showRejectModal = ref(false);
const showDisputeModal = ref(false);
const showResultModal = ref(false);
const showCountdownModal = ref(false);
const selectedAccountId = ref<number | null>(null);
const rejectionReason = ref('');
const disputeReason = ref('');
const disputeFiles = ref<File[]>([]);
const resultMessage = ref('');
const resultType = ref<'success' | 'error' | 'info'>('info');
const activeTab = ref('purchased');
const countdownTimers = ref<{ [key: number]: string }>({});
const countdownInterval = ref<number | null>(null);

// Computed properties
const resultIconClass = computed(() => {
  return {
    'success-icon': resultType.value === 'success',
    'error-icon': resultType.value === 'error',
    'info-icon': resultType.value === 'info'
  };
});

const getResultIcon = computed(() => {
  if (resultType.value === 'success') return 'fas fa-check';
  if (resultType.value === 'error') return 'fas fa-times';
  return 'fas fa-info';
});

const countdownTime = computed(() => {
  if (selectedAccountId.value !== null) {
    return countdownTimers.value[selectedAccountId.value] || '';
  }
  return '';
});

// Helper functions
function getStatusClass(status: string | undefined) {
  if (!status || status === 'Chưa xác nhận') return 'status-pending';
  if (status === 'Mua thành công') return 'status-success';
  if (status === 'Đã từ chối') return 'status-rejected';
  return 'status-pending';
}

function getStatusText(status: string | undefined) {
  if (!status || status === 'Chưa xác nhận') return 'ĐANG CHỜ XÁC NHẬN';
  if (status === 'Mua thành công') return 'ĐÃ XÁC NHẬN';
  if (status === 'Đã từ chối') return 'ĐÃ TỪ CHỐI';
  return 'ĐANG CHỜ XÁC NHẬN';
}

function formatPrice(price: number) {
  return new Intl.NumberFormat('vi-VN').format(price) + ' VNĐ';
}

function copyToClipboard(text: string) {
  navigator.clipboard.writeText(text);
  resultType.value = 'success';
  resultMessage.value = 'Đã sao chép mật khẩu vào bộ nhớ tạm!';
  showResultModal.value = true;
}

function getCountdown(accountId: number) {
  return countdownTimers.value[accountId];
}

function updateCountdowns() {
  const now = new Date();
  [...purchasedAccounts.value, ...tradingAccounts.value].forEach(account => {
    if (account.statusBuyer === 'Chưa xác nhận' && account.createdDate) {
      const createdStr = typeof account.createdDate === 'string' && account.createdDate.includes('+07:00') 
        ? account.createdDate 
        : `${account.createdDate}+07:00`;
      const created = new Date(createdStr);
      const elapsedTime = now.getTime() - created.getTime();
      const totalTime = 24 * 60 * 60 * 1000;
      const remainingTime = totalTime - elapsedTime;

      if (remainingTime <= 0) {
        countdownTimers.value[account.id] = '';
        checkTimeout(account.id);
      } else {
        const hours = Math.floor(remainingTime / (1000 * 60 * 60));
        const minutes = Math.floor((remainingTime % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((remainingTime % (1000 * 60)) / 1000);
        countdownTimers.value[account.id] = `${hours.toString().padStart(2, '0')}:${minutes
          .toString()
          .padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
      }
    }
  });
}

// API call to check timeout
        
        // Format remaining time and update countdown timer
async function checkTimeout(accountId: number) {
  try {
    const response = await purchasedApi.checkConfirmationTimeout(accountId);
    if (response.data.result.isSuccess) {
      resultType.value = 'info';
      resultMessage.value = response.data.result.message || 'Giao dịch đã được xử lý do hết thời gian xác nhận.';
      showResultModal.value = true;
      await fetchAllAccounts();
    } else {
      resultType.value = 'error';
      resultMessage.value = response.data.result.message || 'Lỗi khi kiểm tra thời gian xác nhận.';
      showResultModal.value = true;
    }
  } catch (err) {
    resultType.value = 'error';
    resultMessage.value = 'Lỗi khi kiểm tra thời gian xác nhận: ' + (err as Error).message;
    showResultModal.value = true;
  }
}

// Fetch all accounts
async function fetchAllAccounts() {
  try {
    loading.value = true;

    const purchasedResponse = await purchasedApi.getAllByUserID(userId);
    if (purchasedResponse.data.result.isSuccess) {
      const data = purchasedResponse.data.result.data ?? [];
      purchasedAccounts.value = Array.isArray(data) ? data.filter(account => account !== null) : [];
      // Log để kiểm tra ConfirmationDeadline
      console.log('Purchased Accounts:', purchasedAccounts.value);
      purchasedAccounts.value.forEach(account => {
        if (account.ConfirmationDeadline) {
          console.log(`Account ${account.id} - ConfirmationDeadline: ${new Date(account.ConfirmationDeadline)}`);
        }
      });
    } else {
      error.value = purchasedResponse.data.result?.message || 'Lỗi khi tải dữ liệu tài khoản đã mua';
      return;
    }

    const tradingResponse = await purchasedApi.getDontConfirm();
    if (tradingResponse.data.result.isSuccess) {
      const data = tradingResponse.data.result.data;
      if (Array.isArray(data)) {
        tradingAccounts.value = data.filter(account => account !== null);
      } else if (data && typeof data === 'object') {
        tradingAccounts.value = [data];
      } else {
        tradingAccounts.value = [];
      }
      tradingAccounts.value.forEach(account => {
        if (account.ConfirmationDeadline) {
          console.log(`Account ${account.id} - ConfirmationDeadline: ${new Date(account.ConfirmationDeadline)}`);
        }
      });
    } else {
      error.value = tradingResponse.data.result?.message || 'Lỗi khi tải dữ liệu tài khoản đang giao dịch';
    }
  } catch (err) {
    error.value = 'Lỗi khi tải dữ liệu. Vui lòng kiểm tra kết nối hoặc API.';
    console.error(err);
  } finally {
    loading.value = false;
    updateCountdowns();
  }
}

// Switch to trading tab with data refresh
async function switchToTradingTab() {
  activeTab.value = 'trading';
  await fetchAllAccounts();
}

// Modal functions
function openDirectConfirmModal(id: number) {
  selectedAccountId.value = id;
  showDirectConfirmModal.value = true;
}

function closeDirectConfirmModal() {
  showDirectConfirmModal.value = false;
  selectedAccountId.value = null;
}

function openRejectModal(id: number) {
  selectedAccountId.value = id;
  showRejectModal.value = true;
}

function cancelReject() {
  showRejectModal.value = false;
  rejectionReason.value = '';
  selectedAccountId.value = null;
}

function openCountdownModal(id: number) {
  selectedAccountId.value = id;
  showCountdownModal.value = true;
}

function closeCountdownModal() {
  showCountdownModal.value = false;
  selectedAccountId.value = null;
}

async function handleDirectConfirm() {
  if (selectedAccountId.value !== null) {
    const success = await confirmAccount(selectedAccountId.value);
    if (success) {
      showDirectConfirmModal.value = false;
      selectedAccountId.value = null;
    }
  }
}

async function confirmReject() {
  if (selectedAccountId.value !== null && rejectionReason.value.trim() !== '') {
    try {
      const model = { id: selectedAccountId.value, status: 'Từ chối', reason: rejectionReason.value };
      const response = await purchasedApi.confirmAccount(model);
      if (response.data.result.isSuccess && response.data.result.data) {
        const account = purchasedAccounts.value.find((a) => a.id === selectedAccountId.value) ||
                       tradingAccounts.value.find((a) => a.id === selectedAccountId.value);
        if (account) {
          account.statusBuyer = 'Đã từ chối';
          openDisputeModal(selectedAccountId.value!);
        }
        resultType.value = 'info';
        resultMessage.value = 'Bạn đã từ chối tài khoản này';
        showResultModal.value = true;
        await fetchAllAccounts();
      } else {
        resultType.value = 'error';
        resultMessage.value = response.data.result?.message || 'Lỗi khi từ chối tài khoản';
        showResultModal.value = true;
      }
    } catch (err) {
      resultType.value = 'error';
      resultMessage.value = 'Lỗi khi từ chối tài khoản';
      showResultModal.value = true;
    } finally {
      showRejectModal.value = false;
      rejectionReason.value = '';
    }
  } else {
    resultType.value = 'error';
    resultMessage.value = 'Vui lòng nhập lý do từ chối';
    showResultModal.value = true;
  }
}

async function confirmAccount(accountId: number) {
  try {
    const model = { id: accountId, status: 'Đồng ý' };
    const response = await purchasedApi.confirmAccount(model);
    if (response.data.result.isSuccess && response.data.result.data) {
      const account = purchasedAccounts.value.find((a) => a.id === accountId) ||
                     tradingAccounts.value.find((a) => a.id === accountId);
      if (account) account.statusBuyer = 'Mua thành công';
      resultType.value = 'success';
      resultMessage.value = 'Giao dịch đã được xác nhận thành công!';
      showResultModal.value = true;
      await fetchAllAccounts();
      return true;
    } else {
      resultType.value = 'error';
      resultMessage.value = response.data.result?.message || 'Lỗi khi xác nhận tài khoản';
      showResultModal.value = true;
      return false;
    }
  } catch (err) {
    resultType.value = 'error';
    resultMessage.value = 'Lỗi khi xác nhận tài khoản';
    showResultModal.value = true;
    return false;
  }
}

function closeResultModal() {
  showResultModal.value = false;
}

// Dispute-related functions
function openDisputeModal(id: number) {
  selectedAccountId.value = id;
  disputeReason.value = rejectionReason.value;
  showDisputeModal.value = true;
}

function closeDisputeModal() {
  showDisputeModal.value = false;
  disputeReason.value = '';
  disputeFiles.value = [];
  selectedAccountId.value = null;
}

function handleFileUpload(event: Event) {
  const target = event.target as HTMLInputElement;
  if (target.files) {
    disputeFiles.value = Array.from(target.files);
  }
}

async function submitDispute() {
  if (disputeReason.value.trim() === '') {
    resultType.value = 'error';
    resultMessage.value = 'Vui lòng nhập lý do tranh chấp!';
    showResultModal.value = true;
    return;
  }

  if (selectedAccountId.value === null) {
    resultType.value = 'error';
    resultMessage.value = 'Không thể xác định tài khoản để báo cáo!';
    showResultModal.value = true;
    return;
  }

  const formData = new FormData();
  formData.append('purchasedAccountID', selectedAccountId.value.toString());
  formData.append('reason', disputeReason.value);
  disputeFiles.value.forEach((file, index) => {
    formData.append(`files[${index}]`, file);
  });

  try {
    const response = await disputeApi.add(formData);
    if (response.data.result.isSuccess) {
      resultType.value = 'success';
      resultMessage.value = 'Báo cáo tranh chấp đã được gửi thành công!';
      showResultModal.value = true;
      closeDisputeModal();
    } else {
      resultType.value = 'error';
      resultMessage.value = response.data.result?.message || 'Lỗi khi gửi báo cáo tranh chấp';
      showResultModal.value = true;
    }
  } catch (err) {
    resultType.value = 'error';
    resultMessage.value = 'Lỗi khi gửi báo cáo tranh chấp: ' + (err as Error).message;
    showResultModal.value = true;
  }
}

function initParticles() {
  const container = document.querySelector('.cyber-particles');
  if (!container) return;
  
  for (let i = 0; i < 30; i++) {
    const particle = document.createElement('div');
    particle.classList.add('particle');
    
    const size = Math.random() * 3 + 1;
    particle.style.width = `${size}px`;
    particle.style.height = `${size}px`;
    
    particle.style.left = `${Math.random() * 100}%`;
    particle.style.top = `${Math.random() * 100}%`;
    
    const duration = Math.random() * 20 + 10;
    particle.style.animationDuration = `${duration}s`;
    
    const delay = Math.random() * 5;
    particle.style.animationDelay = `-${delay}s`;
    
    container.appendChild(particle);
  }
}

function initCyberLines() {
  const container = document.querySelector('.cyber-lines');
  if (!container) return;
  
  for (let i = 0; i < 15; i++) {
    const line = document.createElement('div');
    line.classList.add('cyber-line');
    
    const isHorizontal = Math.random() > 0.5;
    if (isHorizontal) {
      line.classList.add('horizontal');
      line.style.top = `${Math.random() * 100}%`;
      line.style.width = `${Math.random() * 30 + 10}%`;
      line.style.left = `${Math.random() * 70}%`;
    } else {
      line.classList.add('vertical');
      line.style.left = `${Math.random() * 100}%`;
      line.style.height = `${Math.random() * 30 + 10}%`;
      line.style.top = `${Math.random() * 70}%`;
    }
    
    const duration = Math.random() * 4 + 2;
    line.style.animationDuration = `${duration}s`;
    
    const delay = Math.random() * 2;
    line.style.animationDelay = `${delay}s`;
    
    container.appendChild(line);
  }
}

// Lifecycle hooks
onMounted(() => {
  fetchAllAccounts();
  initParticles();
  initCyberLines();
  countdownInterval.value = setInterval(updateCountdowns, 1000);
});

onUnmounted(() => {
  if (countdownInterval.value) {
    clearInterval(countdownInterval.value);
  }
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Rajdhani:wght@500;600;700&family=Orbitron:wght@400;500;700;900&display=swap');

/* Base Styles */
.cyber-vault {
  background-color: #050520;
  color: #e0f7ff;
  font-family: 'Rajdhani', sans-serif;
  position: relative;
  overflow: hidden;
  padding: 30px 2rem 2rem 30px;
}

/* Animated Background */
.cyber-bg {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 0;
  overflow: hidden;
}

.grid-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: 
    linear-gradient(to right, rgba(0, 255, 255, 0.1) 1px, transparent 1px),
    linear-gradient(to bottom, rgba(0, 255, 255, 0.1) 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.3;
  transform: perspective(500px) rotateX(60deg);
  transform-origin: center top;
}

.cyber-particles {
  position: absolute;
  width: 100%;
  height: 100%;
  z-index: 1;
}

.particle {
  position: absolute;
  background-color: #00ffff;
  border-radius: 50%;
  opacity: 0.5;
  animation: float-up linear infinite;
}

@keyframes float-up {
  0% { transform: translateY(100vh) translateX(0); }
  100% { transform: translateY(-100px) translateX(20px); }
}

.cyber-lines {
  position: absolute;
  width: 100%;
  height: 100%;
  z-index: 1;
}

.cyber-line {
  position: absolute;
  background: linear-gradient(90deg, transparent, #00ffff, transparent);
  opacity: 0;
  animation: line-pulse infinite;
}

.cyber-line.horizontal { height: 1px; }
.cyber-line.vertical {
  width: 1px;
  background: linear-gradient(180deg, transparent, #00ffff, transparent);
}

@keyframes line-pulse {
  0%, 100% { opacity: 0; }
  50% { opacity: 0.5; }
}

/* Header Styles */
.vault-header {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 3rem;
  padding: 2rem 0;
}

.header-hologram {
  position: relative;
  width: 100px;
  height: 100px;
  margin-right: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.hologram-ring {
  position: absolute;
  width: 100%;
  height: 100%;
  border: 2px solid #00ffff;
  border-radius: 50%;
  animation: rotate 10s linear infinite;
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.5);
}

.hologram-ring::before {
  content: '';
  position: absolute;
  top: -5px;
  left: -5px;
  right: -5px;
  bottom: -5px;
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 50%;
  animation: rotate 15s linear infinite reverse;
}

.hologram-planet {
  position: absolute;
  width: 60px;
  height: 60px;
  background: radial-gradient(circle at 30% 30%, #00ffff, #0066ff);
  border-radius: 50%;
  box-shadow: 0 0 30px rgba(0, 255, 255, 0.8);
  animation: pulse 3s ease-in-out infinite alternate;
}

.header-text { position: relative; }

.glitch-text {
  font-family: 'Orbitron', sans-serif;
  font-size: 3.5rem;
  font-weight: 900;
  color: #ffffff;
  text-shadow: 
    0 0 10px rgba(0, 255, 255, 0.8),
    0 0 20px rgba(0, 255, 255, 0.5),
    0 0 30px rgba(0, 255, 255, 0.3);
  margin: 0;
  position: relative;
}

.glitch-text::before,
.glitch-text::after {
  content: attr(data-text);
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0.8;
}

.glitch-text::before {
  color: #ff00ff;
  z-index: -1;
  animation: glitch-anim-1 2s infinite linear alternate-reverse;
}

.glitch-text::after {
  color: #00ffff;
  z-index: -2;
  animation: glitch-anim-2 3s infinite linear alternate-reverse;
}

@keyframes glitch-anim-1 {
  0%, 100% { transform: translate(0); }
  20% { transform: translate(-2px, 2px); }
  40% { transform: translate(-2px, -2px); }
  60% { transform: translate(2px, 2px); }
  80% { transform: translate(2px, -2px); }
}

@keyframes glitch-anim-2 {
  0%, 100% { transform: translate(0); }
  20% { transform: translate(2px, -2px); }
  40% { transform: translate(2px, 2px); }
  60% { transform: translate(-2px, -2px); }
  80% { transform: translate(-2px, 2px); }
}

.header-scan-line {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 2px;
  background-color: rgba(0, 255, 255, 0.5);
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.8);
  animation: scan-line 2s linear infinite;
}

@keyframes scan-line {
  0% { top: 0; }
  100% { top: 100%; }
}

.header-subtitle {
  font-size: 1.2rem;
  color: #00ffff;
  margin-top: 0.5rem;
  letter-spacing: 2px;
}

.blink { animation: blink 1s step-end infinite; }

@keyframes blink {
  0%, 100% { opacity: 1; }
  50% { opacity: 0; }
}

/* Command Center */
.command-center {
  position: relative;
  z-index: 2;
  margin-bottom: 2rem;
}

.command-panel {
  background: rgba(0, 20, 40, 0.7);
  border: 1px solid #00ffff;
  border-radius: 8px;
  padding: 1rem;
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  max-width: 1200px;
  margin: 0 auto;
}

.panel-header {
  display: flex;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid rgba(0, 255, 255, 0.3);
  color: #00ffff;
  font-size: 1.2rem;
  font-weight: 600;
}

.panel-header i { margin-right: 0.5rem; }

.panel-stats {
  display: flex;
  justify-content: space-around;
  flex-wrap: wrap;
  gap: 1rem;
}

.stat-item {
  display: flex;
  align-items: center;
  background: rgba(0, 255, 255, 0.1);
  padding: 0.8rem 1.2rem;
  border-radius: 8px;
  min-width: 200px;
  transition: all 0.3s ease;
}

.stat-item:hover {
  background: rgba(0, 255, 255, 0.2);
  transform: translateY(-3px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
}

.stat-icon {
  width: 40px;
  height: 40px;
  background: rgba(0, 255, 255, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 1rem;
  color: #00ffff;
  font-size: 1.2rem;
}

.stat-info { display: flex; flex-direction: column; }

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #ffffff;
}

.stat-label {
  font-size: 0.8rem;
  color: #00ffff;
}

/* Loading State */
.vault-loading {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
  position: relative;
  z-index: 2;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.loading-cube {
  width: 80px;
  height: 80px;
  margin-bottom: 2rem;
  position: relative;
  transform-style: preserve-3d;
  animation: cube-rotate 8s infinite linear;
}

.cube-face {
  position: absolute;
  width: 100%;
  height: 100%;
  background: rgba(0, 255, 255, 0.2);
  border: 2px solid #00ffff;
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.5);
}

.cube-face.front { transform: translateZ(40px); }
.cube-face.back { transform: rotateY(180deg) translateZ(40px); }
.cube-face.right { transform: rotateY(90deg) translateZ(40px); }
.cube-face.left { transform: rotateY(-90deg) translateZ(40px); }
.cube-face.top { transform: rotateX(90deg) translateZ(40px); }
.cube-face.bottom { transform: rotateX(-90deg) translateZ(40px); }

@keyframes cube-rotate {
  0% { transform: rotateX(0) rotateY(0); }
  100% { transform: rotateX(360deg) rotateY(360deg); }
}

.loading-text {
  font-size: 1.2rem;
  color: #00ffff;
  letter-spacing: 2px;
  text-align: center;
}

.dot-1, .dot-2, .dot-3 { animation: dot-blink 1.5s infinite; }
.dot-2 { animation-delay: 0.5s; }
.dot-3 { animation-delay: 1s; }

@keyframes dot-blink {
  0%, 100% { opacity: 0; }
  50% { opacity: 1; }
}

/* Error State */
.vault-error {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
  position: relative;
  z-index: 2;
}

.error-container {
  background: rgba(255, 0, 0, 0.1);
  border: 1px solid #ff3333;
  border-radius: 8px;
  padding: 2rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  max-width: 500px;
  box-shadow: 0 0 30px rgba(255, 0, 0, 0.2);
}

.error-icon {
  width: 80px;
  height: 80px;
  background: rgba(255, 0, 0, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1.5rem;
  color: #ff3333;
  font-size: 2.5rem;
  animation: pulse 2s infinite;
}

.error-message {
  text-align: center;
  margin-bottom: 1.5rem;
}

.error-message h3 {
  color: #ff3333;
  margin-bottom: 0.5rem;
  font-size: 1.5rem;
}

/* Empty State */
.vault-empty {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
  position: relative;
  z-index: 2;
}

.empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
}

.empty-hologram {
  position: relative;
  width: 150px;
  height: 150px;
  margin-bottom: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.hologram-rings {
  position: absolute;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.ring {
  position: absolute;
  border-radius: 50%;
  border: 2px solid transparent;
  border-top-color: #00ffff;
  border-bottom-color: #00ffff;
  animation: rotate linear infinite;
}

.ring-1 {
  width: 100%;
  height: 100%;
  animation-duration: 10s;
}

.ring-2 {
  width: 80%;
  height: 80%;
  animation-duration: 7s;
  animation-direction: reverse;
}

.ring-3 {
  width: 60%;
  height: 60%;
  animation-duration: 5s;
}

.hologram-icon {
  width: 60px;
  height: 60px;
  background: rgba(0, 255, 255, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #00ffff;
  font-size: 1.8rem;
  box-shadow: 0 0 30px rgba(0, 255, 255, 0.5);
  animation: pulse 3s infinite alternate;
}

.warning-icon {
  background: rgba(255, 165, 0, 0.2);
  color: #ffa500;
  box-shadow: 0 0 30px rgba(255, 165, 0, 0.5);
}

.countdown-icon {
  background: rgba(0, 255, 255, 0.2);
  color: #00ffff;
  box-shadow: 0 0 30px rgba(0, 255, 255, 0.5);
}

.empty-title {
  font-size: 2rem;
  color: #ffffff;
  margin-bottom: 0.5rem;
  font-family: 'Orbitron', sans-serif;
}

.empty-desc {
  color: #b0b0cc;
  margin-bottom: 2rem;
  font-size: 1.1rem;
}

/* Vault Content */
.vault-content {
  position: relative;
  z-index: 2;
  max-width: 1400px;
  margin: 0 auto;
}

/* Tabs Navigation */
.vault-tabs {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.tab-button {
  background: rgba(0, 255, 255, 0.1);
  border: 1px solid rgba(0, 255, 255, 0.3);
  color: #00ffff;
  padding: 0.8rem 1.5rem;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.tab-button:hover {
  background: rgba(0, 255, 255, 0.2);
  transform: translateY(-3px);
}

.tab-button.active {
  background: rgba(0, 255, 255, 0.3);
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.3);
}

/* Accounts Grid */
.accounts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 2rem;
}

/* Account Card */
.account-card {
  position: relative;
  background: rgba(0, 20, 40, 0.7);
  border-radius: 12px;
  overflow: hidden;
  transition: all 0.3s ease;
  border: 1px solid rgba(0, 255, 255, 0.3);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.account-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.4);
}

.card-holo-effect {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, 
    rgba(0, 255, 255, 0.1) 0%, 
    transparent 50%, 
    rgba(255, 0, 255, 0.1) 100%);
  opacity: 0.5;
  pointer-events: none;
}

.account-card.status-success { border-color: rgba(0, 255, 127, 0.5); }
.account-card.status-pending { border-color: rgba(255, 193, 7, 0.5); }
.account-card.status-rejected { border-color: rgba(255, 75, 43, 0.5); }
.account-card.status-trading { border-color: rgba(0, 255, 255, 0.5); }

/* Game Banner */
.game-banner {
  position: relative;
  padding: 1.5rem;
  display: flex;
  align-items: center;
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
}

.game-icon {
  width: 50px;
  height: 50px;
  background: linear-gradient(135deg, #00ffff, #0088ff);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 1rem;
  color: #ffffff;
  font-size: 1.5rem;
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.5);
}

.game-title {
  font-size: 1.3rem;
  color: #ffffff;
  margin: 0;
  flex: 1;
}

.status-badge {
  position: absolute;
  top: 1rem;
  right: 4rem;
  padding: 0.3rem 0.8rem;
  border-radius: 20px;
  font-size: 0.7rem;
  font-weight: 600;
}

.status-badge.status-success {
  background: rgba(0, 255, 127, 0.2);
  color: #00ff7f;
  border: 1px solid rgba(0, 255, 127, 0.5);
}

.status-badge.status-pending {
  background: rgba(255, 193, 7, 0.2);
  color: #ffc107;
  border: 1px solid rgba(255, 193, 7, 0.5);
  animation: blink 2s infinite;
}

.status-badge.status-rejected {
  background: rgba(255, 75, 43, 0.2);
  color: #ff4b2b;
  border: 1px solid rgba(255, 75, 43, 0.5);
}

.status-badge.status-trading {
  background: rgba(0, 255, 255, 0.2);
  color: #00ffff;
  border: 1px solid rgba(0, 255, 255, 0.5);
}

/* Report Button */
.report-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: none;
  border: none;
  color: #ff3333;
  font-size: 1.2rem;
  cursor: pointer;
  transition: color 0.3s ease;
}

.report-btn:hover {
  color: #ff6666;
}

/* Account Details */
.account-details {
  padding: 1.5rem;
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
}

.detail-group {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.detail-row {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
}

.detail-icon {
  width: 30px;
  height: 30px;
  background: rgba(0, 255, 255, 0.1);
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #00ffff;
  font-size: 1rem;
}

.detail-content { flex: 1; }

.detail-label {
  font-size: 0.7rem;
  color: #b0b0cc;
  margin-bottom: 0.3rem;
}

.detail-value {
  font-size: 0.9rem;
  color: #ffffff;
  word-break: break-all;
}

.password-value {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.copy-btn {
  background: rgba(0, 255, 255, 0.1);
  border: none;
  color: #00ffff;
  width: 24px;
  height: 24px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 0.8rem;
}

.copy-btn:hover {
  background: rgba(0, 255, 255, 0.3);
  transform: scale(1.1);
}

.price-value {
  color: #00ff7f;
  font-weight: 600;
}

.countdown-value {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #ffa500;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.countdown-value:hover {
  color: #ffcc00;
  text-shadow: 0 0 10px rgba(255, 165, 0, 0.5);
}

/* Account Actions */
.account-actions {
  padding: 0 1.5rem 1.5rem;
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
}

/* Cyber Button */
.cyber-button {
  position: relative;
  background: rgba(0, 255, 255, 0.1);
  border: 1px solid rgba(0, 255, 255, 0.3);
  color: #00ffff;
  padding: 0.8rem 1.5rem;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  overflow: hidden;
  display: inline-block;
}

.button-content { position: relative; z-index: 1; }

.button-glitch {
  position: absolute;
  top: -10%;
  left: -10%;
  width: 120%;
  height: 120%;
  background: linear-gradient(90deg, 
    transparent, 
    rgba(0, 255, 255, 0.2), 
    transparent);
  transform: translateX(-100%);
  transition: transform 0.5s;
}

.cyber-button:hover {
  background: rgba(0, 255, 255, 0.2);
  transform: translateY(-3px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
}

.cyber-button:hover .button-glitch { transform: translateX(100%); }

.direct-confirm-btn {
  background: rgba(255, 165, 0, 0.2);
  border-color: rgba(255, 165, 0, 0.5);
  color: #ffa500;
}

.direct-confirm-btn .button-glitch {
  background: linear-gradient(90deg, 
    transparent, 
    rgba(255, 165, 0, 0.2), 
    transparent);
}

.pulse-button { animation: button-pulse 2s infinite; }

@keyframes button-pulse {
  0% { box-shadow: 0 0 0 0 rgba(0, 255, 255, 0.7); }
  70% { box-shadow: 0 0 0 10px rgba(0, 255, 255, 0); }
  100% { box-shadow: 0 0 0 0 rgba(0, 255, 255, 0); }
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.8);
  backdrop-filter: blur(5px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
  animation: fade-in 0.3s ease;
}

@keyframes fade-in {
  from { opacity: 0; }
  to { opacity: 1; }
}

.cyber-modal {
  background: rgba(0, 20, 40, 0.9);
  border: 1px solid #00ffff;
  border-radius: 12px;
  width: 90%;
  max-width: 500px;
  box-shadow: 0 0 30px rgba(0, 255, 255, 0.3);
  animation: modal-in 0.3s ease;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  align-items: center;
}

@keyframes modal-in {
  from { transform: scale(0.9); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}

.modal-header {
  background: rgba(0, 255, 255, 0.1);
  padding: 1rem 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid rgba(0, 255, 255, 0.3);
  width: 100%;
}

.modal-header h3 {
  margin: 0;
  color: #00ffff;
  font-size: 1.2rem;
  letter-spacing: 1px;
}

.close-btn {
  background: none;
  border: none;
  color: #fff;
  font-size: 1.5rem;
  cursor: pointer;
}

.close-btn:hover { color: #00ffff; }

.modal-content {
  padding: 2rem;
  text-align: center;
  width: 100%;
}

.modal-hologram {
  position: relative;
  width: 100px;
  height: 100px;
  margin: 0 auto 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
  padding: 1rem;
  width: 100%;
}

.cyber-textarea {
  width: 100%;
  padding: 0.5rem;
  background: rgba(0, 0, 0, 0.2);
  border: 1px solid #00ffff;
  color: #fff;
  border-radius: 4px;
  margin-top: 1rem;
}

.cyber-textarea:focus {
  outline: none;
  border-color: #00ffff;
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.file-input {
  margin-top: 1rem;
  width: 100%;
  color: #00ffff;
}

/* Centering Adjustments */
.vault-loading, .vault-error, .vault-empty {
  display: flex;
  justify-content: center;
  align-items: center;
}

.loading-container, .error-container, .empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.modal-content p { margin: 1rem 0; }

@keyframes rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes pulse {
  from { transform: scale(1); }
  to { transform: scale(1.1); }
}

@keyframes blink {
  0%, 100% { opacity: 1; }
  50% { opacity: 0; }
}
</style>
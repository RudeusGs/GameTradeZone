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
            <div class="stat-icon"><i class="fas fa-shopping-cart"></i></div>
            <div class="stat-info">
              <div class="stat-value">{{ sellingAccounts.length }}</div>
              <div class="stat-label">TÀI KHOẢN ĐANG BÁN</div>
            </div>
          </div>
          <div class="stat-item">
            <div class="stat-icon"><i class="fas fa-check-circle"></i></div>
            <div class="stat-info">
              <div class="stat-value">{{ confirmedSellingAccounts }}</div>
              <div class="stat-label">ĐÃ XÁC NHẬN BÁN</div>
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
          :class="{ active: activeTab === 'selling' }" 
          @click="activeTab = 'selling'"
        >
          <i class="fas fa-store"></i> TÀI KHOẢN ĐANG BÁN
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
                      <button class="copy-btn" @click="account.password && copyToClipboard(account.password)">
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
                <div class="detail-row">
                  <div class="detail-icon"><i class="fas fa-envelope"></i></div>
                  <div class="detail-content">
                    <div class="detail-label">EMAIL</div>
                    <div class="detail-value" :class="getEmailStatusClass(account.email ?? undefined)">
                      {{ getEmailStatusText(account.email ?? undefined) }}
                    </div>
                  </div>
                </div>
                <div class="detail-row">
                  <div class="detail-icon"><i class="fas fa-shield-alt"></i></div>
                  <div class="detail-content">
                    <div class="detail-label">OTP</div>
                    <div class="detail-value" :class="getOtpStatusClass(account.otpEmail ?? undefined)">
                      {{ getOtpStatusText(account.otpEmail ?? undefined) }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="account-actions">
              <button
                v-if="account.statusBuyer !== 'Mua thành công' && account.statusBuyer !== 'Đã từ chối'"
                class="cyber-button confirm-btn"
                @click="openConfirmModal(account.id)"
              >
                <span class="button-content">XÁC NHẬN</span>
                <span class="button-glitch"></span>
              </button>
              <button
                v-if="account.statusBuyer === 'Mua thành công' && (account.email === 'Đang chờ' || !account.email)"
                class="cyber-button email-btn"
                @click="requestEmail(account.id)"
              >
                <span class="button-content">YÊU CẦU EMAIL</span>
                <span class="button-glitch"></span>
              </button>
              <button
                v-if="account.statusBuyer === 'Mua thành công' && (account.otpEmail === 'Đang chờ' || !account.otpEmail)"
                class="cyber-button otp-btn"
                @click="requestOTP(account.id)"
              >
                <span class="button-content">YÊU CẦU OTP</span>
                <span class="button-glitch"></span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Selling Accounts -->
      <div v-if="activeTab === 'selling'">
        <div v-if="sellingAccounts.length === 0" class="vault-empty">
          <div class="empty-container">
            <div class="empty-hologram">
              <div class="hologram-rings">
                <div class="ring ring-1"></div>
                <div class="ring ring-2"></div>
                <div class="ring ring-3"></div>
              </div>
              <div class="hologram-icon">
                <i class="fas fa-store"></i>
              </div>
            </div>
            <h3 class="empty-title">VAULT TRỐNG</h3>
            <p class="empty-desc">Bạn chưa đăng bán tài khoản nào</p>
            <button class="cyber-button pulse-button">
              <span class="button-content">ĐĂNG BÁN NGAY</span>
              <span class="button-glitch"></span>
            </button>
          </div>
        </div>
        <div v-else>
          <!-- Pending Selling Accounts -->
          <div class="pending-selling">
            <h3 class="section-title">TÀI KHOẢN CHỜ XÁC NHẬN</h3>
            <div class="accounts-grid">
              <div 
                v-for="account in pendingSellingAccounts" 
                :key="account.id" 
                class="account-card status-pending"
              >
                <div class="card-holo-effect"></div>
                <div class="game-banner">
                  <div class="game-icon">
                    <i class="fas fa-gamepad"></i>
                  </div>
                  <h3 class="game-title">{{ account.accountName }}</h3>
                  <div class="status-badge status-pending">
                    CHỜ XÁC NHẬN
                  </div>
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
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Confirmed/Rejected Selling Accounts Table -->
          <div class="selling-table" v-if="confirmedOrRejectedSellingAccounts.length > 0">
            <h3 class="section-title">TÀI KHOẢN ĐÃ XỬ LÝ</h3>
            <table class="cyber-table">
              <thead>
                <tr>
                  <th>TÊN GAME</th>
                  <th>TÀI KHOẢN</th>
                  <th>GIÁ TIỀN</th>
                  <th>TRẠNG THÁI</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="account in confirmedOrRejectedSellingAccounts" :key="account.id">
                  <td>{{ account.accountName }}</td>
                  <td>{{ account.accountName }}</td>
                  <td>{{ formatPrice(account.price) }}</td>
                  <td :class="getSellingStatusClass(account.status ?? undefined)">
                    {{ getSellingStatusText(account.status ?? undefined) }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <!-- Decision Modal -->
    <div v-if="showDecisionModal" class="modal-overlay" @click.self="closeDecisionModal">
      <div class="cyber-modal decision-modal">
        <div class="modal-header">
          <h3>XÁC NHẬN TÀI KHOẢN</h3>
          <button class="close-btn" @click="closeDecisionModal">×</button>
        </div>
        <div class="modal-content">
          <div class="modal-hologram">
            <div class="hologram-rings">
              <div class="ring ring-1"></div>
              <div class="ring ring-2"></div>
            </div>
            <div class="hologram-icon">
              <i class="fas fa-question"></i>
            </div>
          </div>
          <p>Bạn có muốn đồng ý hay từ chối tài khoản này?</p>
        </div>
        <div class="modal-actions">
          <button @click="handleDecision('Từ chối')" class="cyber-button reject-btn">
            <span class="button-content">TỪ CHỐI</span>
            <span class="button-glitch"></span>
          </button>
          <button @click="handleDecision('Đồng ý')" class="cyber-button accept-btn">
            <span class="button-content">ĐỒNG Ý</span>
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
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { userStore } from '@/stores/auth';
import purchasedApi from '@/api/purchased.api';
import accountGameApi from '@/api/gameaccount.api';
import type { PurchasedAccount } from '@/models/purchased.model';
import type { GameAccount } from '@/models/gameaccount.model';

// Initialize store and retrieve userId
const store = userStore();
const userId = store.user?.id || JSON.parse(localStorage.getItem('user') || '{}').id;

// State
const purchasedAccounts = ref<PurchasedAccount[]>([]);
const sellingAccounts = ref<GameAccount[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const showDecisionModal = ref(false);
const showRejectModal = ref(false);
const selectedAccountId = ref<number | null>(null);
const rejectionReason = ref('');
const showResultModal = ref(false);
const resultMessage = ref('');
const resultType = ref<'success' | 'error' | 'info'>('info');
const activeTab = ref('purchased');

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

const confirmedAccounts = computed(() => {
  return purchasedAccounts.value.filter(account => account.statusBuyer === 'Mua thành công').length;
});

const confirmedSellingAccounts = computed(() => {
  return sellingAccounts.value.filter(account => account.status === 'Confirmed').length;
});

const pendingSellingAccounts = computed(() => {
  return sellingAccounts.value.filter(account => !account.status || account.status === 'Pending');
});

const confirmedOrRejectedSellingAccounts = computed(() => {
  return sellingAccounts.value.filter(account => account.status === 'Confirmed' || account.status === 'Rejected');
});

// Helper functions
function getStatusClass(status: string | undefined) {
  if (!status) return 'status-pending';
  if (status === 'Mua thành công') return 'status-success';
  if (status === 'Đã từ chối') return 'status-rejected';
  return 'status-pending';
}

function getStatusText(status: string | undefined) {
  if (!status) return 'ĐANG CHỜ XÁC NHẬN';
  if (status === 'Mua thành công') return 'ĐÃ XÁC NHẬN';
  if (status === 'Đã từ chối') return 'ĐÃ TỪ CHỐI';
  return 'ĐANG CHỜ XÁC NHẬN';
}

function getSellingStatusClass(status: string | undefined) {
  if (!status || status === 'Pending') return 'pending';
  if (status === 'Confirmed') return 'success';
  if (status === 'Rejected') return 'rejected';
  return 'pending';
}

function getSellingStatusText(status: string | undefined) {
  if (!status || status === 'Pending') return 'CHỜ XÁC NHẬN';
  if (status === 'Confirmed') return 'ĐÃ XÁC NHẬN';
  if (status === 'Rejected') return 'ĐÃ TỪ CHỐI';
  return 'CHỜ XÁC NHẬN';
}

function getEmailStatusClass(status: string | undefined) {
  if (!status) return 'not-requested';
  if (status === 'Đang chờ') return 'pending';
  return 'received';
}

function getEmailStatusText(status: string | undefined) {
  if (!status) return 'CHƯA YÊU CẦU';
  if (status === 'Đang chờ') return 'ĐANG CHỜ';
  return status;
}

function getOtpStatusClass(status: string | undefined) {
  if (!status) return 'not-requested';
  if (status === 'Đang chờ') return 'pending';
  return 'received';
}

function getOtpStatusText(status: string | undefined) {
  if (!status) return 'CHƯA YÊU CẦU';
  if (status === 'Đang chờ') return 'ĐANG CHỜ';
  return status;
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

// Fetch all accounts
async function fetchAllAccounts() {
  try {
    loading.value = true;
    
    // Fetch purchased accounts
    const purchasedResponse = await purchasedApi.getAllByUserID(userId);
    if (purchasedResponse.data.result.isSuccess) {
      purchasedAccounts.value = purchasedResponse.data.result.data ?? [];
    } else {
      error.value = purchasedResponse.data.result?.message || 'Lỗi khi tải dữ liệu tài khoản đã mua';
      return;
    }

    // Fetch selling accounts
    const sellingResponse = await accountGameApi.getAllByUserID(userId);
    if (sellingResponse.data.result.isSuccess) {
      sellingAccounts.value = sellingResponse.data.result.data ?? [];
    } else {
      error.value = sellingResponse.data.result?.message || 'Lỗi khi tải dữ liệu tài khoản đang bán';
    }
  } catch (err) {
    console.error('API call error:', err);
    error.value = 'Lỗi khi tải dữ liệu.';
  } finally {
    loading.value = false;
  }
}

// Modal functions
function openConfirmModal(id: number) {
  selectedAccountId.value = id;
  showDecisionModal.value = true;
}

function closeDecisionModal() {
  showDecisionModal.value = false;
}

function handleDecision(decision: string) {
  showDecisionModal.value = false;
  if (decision === 'Từ chối') {
    showRejectModal.value = true;
  } else if (decision === 'Đồng ý' && selectedAccountId.value !== null) {
    confirmAccount(selectedAccountId.value).then(() => {
      resultType.value = 'success';
      resultMessage.value = 'Bạn đã xác nhận tài khoản này';
      showResultModal.value = true;
    });
  }
}

function cancelReject() {
  showRejectModal.value = false;
  rejectionReason.value = '';
}

async function confirmReject() {
  if (selectedAccountId.value !== null && rejectionReason.value.trim() !== '') {
    try {
      const model = { id: selectedAccountId.value, status: 'Từ chối', reason: rejectionReason.value };
      const response = await purchasedApi.confirmAccount(model);
      if (response.data.result.isSuccess && response.data.result.data) {
        const account = purchasedAccounts.value.find((a) => a.id === selectedAccountId.value);
        if (account) account.statusBuyer = 'Đã từ chối';
        resultType.value = 'info';
        resultMessage.value = 'Bạn đã từ chối tài khoản này';
        showResultModal.value = true;
      } else {
        resultType.value = 'error';
        resultMessage.value = response.data.result?.message || 'Lỗi khi từ chối tài khoản';
        showResultModal.value = true;
      }
    } catch (err) {
      console.error('Rejection error:', err);
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
      const account = purchasedAccounts.value.find((a) => a.id === accountId);
      if (account) account.statusBuyer = 'Mua thành công';
      return true;
    } else {
      resultType.value = 'error';
      resultMessage.value = response.data.result?.message || 'Lỗi khi xác nhận tài khoản';
      showResultModal.value = true;
      return false;
    }
  } catch (err) {
    console.error('Confirmation error:', err);
    resultType.value = 'error';
    resultMessage.value = 'Lỗi khi xác nhận tài khoản';
    showResultModal.value = true;
    return false;
  }
}

async function requestEmail(accountId: number) {
  try {
    const response = await purchasedApi.emailRequest(accountId);
    if (response.data.result.isSuccess) {
      const account = purchasedAccounts.value.find((a) => a.id === accountId);
      if (account) account.email = 'Đang chờ';
      resultType.value = 'success';
      resultMessage.value = 'Yêu cầu gửi email thành công!';
      showResultModal.value = true;
    } else {
      resultType.value = 'error';
      resultMessage.value = response.data.result?.message || 'Lỗi khi yêu cầu gửi email';
      showResultModal.value = true;
    }
  } catch (err) {
    console.error('Email request error:', err);
    resultType.value = 'error';
    resultMessage.value = 'Lỗi khi yêu cầu gửi email';
    showResultModal.value = true;
  }
}

async function requestOTP(accountId: number) {
  try {
    const response = await purchasedApi.otpRequest(accountId);
    if (response.data.result.isSuccess) {
      const account = purchasedAccounts.value.find((a) => a.id === accountId);
      if (account) account.otpEmail = 'Đang chờ';
      resultType.value = 'success';
      resultMessage.value = 'Yêu cầu gửi OTP thành công!';
      showResultModal.value = true;
    } else {
      resultType.value = 'error';
      resultMessage.value = response.data.result?.message || 'Lỗi khi yêu cầu gửi OTP';
      showResultModal.value = true;
    }
  } catch (err) {
    console.error('OTP request error:', err);
    resultType.value = 'error';
    resultMessage.value = 'Lỗi khi yêu cầu gửi OTP';
    showResultModal.value = true;
  }
}

function closeResultModal() {
  showResultModal.value = false;
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

onMounted(() => {
  fetchAllAccounts();
  initParticles();
  initCyberLines();
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Rajdhani:wght@500;600;700&family=Orbitron:wght@400;500;700;900&display=swap');

/* Base Styles */
.cyber-vault {
  min-height: 100vh;
  background-color: #050520;
  color: #e0f7ff;
  font-family: 'Rajdhani', sans-serif;
  position: relative;
  overflow: hidden;
  padding: 100px 2rem 2rem;
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
  0% {
    transform: translateY(100vh) translateX(0);
  }
  100% {
    transform: translateY(-100px) translateX(20px);
  }
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

.cyber-line.horizontal {
  height: 1px;
}

.cyber-line.vertical {
  width: 1px;
  background: linear-gradient(180deg, transparent, #00ffff, transparent);
}

@keyframes line-pulse {
  0%, 100% {
    opacity: 0;
  }
  50% {
    opacity: 0.5;
  }
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
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 60px;
  height: 60px;
  background: radial-gradient(circle at 30% 30%, #00ffff, #0066ff);
  border-radius: 50%;
  box-shadow: 0 0 30px rgba(0, 255, 255, 0.8);
  animation: pulse 3s ease-in-out infinite alternate;
}

.header-text {
  position: relative;
}

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

.blink {
  animation: blink 1s step-end infinite;
}

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

.panel-header i {
  margin-right: 0.5rem;
}

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

.stat-info {
  display: flex;
  flex-direction: column;
}

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

.cube-face.front {
  transform: translateZ(40px);
}

.cube-face.back {
  transform: rotateY(180deg) translateZ(40px);
}

.cube-face.right {
  transform: rotateY(90deg) translateZ(40px);
}

.cube-face.left {
  transform: rotateY(-90deg) translateZ(40px);
}

.cube-face.top {
  transform: rotateX(90deg) translateZ(40px);
}

.cube-face.bottom {
  transform: rotateX(-90deg) translateZ(40px);
}

@keyframes cube-rotate {
  0% {
    transform: rotateX(0) rotateY(0);
  }
  100% {
    transform: rotateX(360deg) rotateY(360deg);
  }
}

.loading-text {
  font-size: 1.2rem;
  color: #00ffff;
  letter-spacing: 2px;
}

.dot-1, .dot-2, .dot-3 {
  animation: dot-blink 1.5s infinite;
}

.dot-2 {
  animation-delay: 0.5s;
}

.dot-3 {
  animation-delay: 1s;
}

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
  text-align: center;
}

.empty-hologram {
  position: relative;
  width: 150px;
  height: 150px;
  margin-bottom: 2rem;
}

.hologram-rings {
  position: absolute;
  width: 100%;
  height: 100%;
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
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  animation-duration: 10s;
}

.ring-2 {
  top: 20%;
  left: 20%;
  right: 20%;
  bottom: 20%;
  animation-duration: 7s;
  animation-direction: reverse;
}

.ring-3 {
  top: 40%;
  left: 40%;
  right: 40%;
  bottom: 40%;
  animation-duration: 5s;
}

.hologram-icon {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
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

.account-card.status-success {
  border-color: rgba(0, 255, 127, 0.5);
}

.account-card.status-pending {
  border-color: rgba(255, 193, 7, 0.5);
}

.account-card.status-rejected {
  border-color: rgba(255, 75, 43, 0.5);
}

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
  right: 1rem;
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

/* Account Details */
.account-details {
  padding: 1.5rem;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
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

.detail-content {
  flex: 1;
}

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

.not-requested {
  color: #b0b0cc;
}

.pending {
  color: #ffc107;
  animation: blink 2s infinite;
}

.received {
  color: #00ff7f;
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

.button-content {
  position: relative;
  z-index: 1;
}

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

.cyber-button:hover .button-glitch {
  transform: translateX(100%);
}

.confirm-btn {
  background: rgba(0, 255, 255, 0.2);
  border-color: rgba(0, 255, 255, 0.5);
}

.email-btn {
  background: rgba(0, 128, 255, 0.2);
  border-color: rgba(0, 128, 255, 0.5);
  color: #0080ff;
}

.email-btn .button-glitch {
  background: linear-gradient(90deg, 
    transparent, 
    rgba(0, 128, 255, 0.2), 
    transparent);
}

.otp-btn {
  background: rgba(255, 0, 255, 0.2);
  border-color: rgba(255, 0, 255, 0.5);
  color: #ff00ff;
}

.otp-btn .button-glitch {
  background: linear-gradient(90deg, 
    transparent, 
    rgba(255, 0, 255, 0.2), 
    transparent);
}

.pulse-button {
  animation: button-pulse 2s infinite;
}

@keyframes button-pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(0, 255, 255, 0.7);
  }
  70% {
    box-shadow: 0 0 0 10px rgba(0, 255, 255, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(0, 255, 255, 0);
  }
}

/* Selling Section */
.section-title {
  font-family: 'Orbitron', sans-serif;
  font-size: 1.5rem;
  color: #00ffff;
  margin-bottom: 1.5rem;
  text-align: center;
  text-shadow: 0 0 10px rgba(0, 255, 255, 0.5);
}

.pending-selling {
  margin-bottom: 3rem;
}

/* Cyber Table */
.cyber-table {
  width: 100%;
  border-collapse: collapse;
  background: rgba(0, 20, 40, 0.7);
  border: 1px solid rgba(0, 255, 255, 0.3);
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.2);
  border-radius: 8px;
  overflow: hidden;
}

.cyber-table th,
.cyber-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
}

.cyber-table th {
  background: rgba(0, 255, 255, 0.1);
  color: #00ffff;
  font-weight: 600;
  text-transform: uppercase;
  font-size: 0.9rem;
}

.cyber-table td {
  color: #ffffff;
  font-size: 0.9rem;
}

.cyber-table tr:hover {
  background: rgba(0, 255, 255, 0.05);
}

.cyber-table td.success {
  color: #00ff7f;
}

.cyber-table td.rejected {
  color: #ff4b2b;
}

.cyber-table td.pending {
  color: #ffc107;
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
  color: rgba(255, 255, 255, 0.7);
  font-size: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.close-btn:hover {
  color: #fff;
  transform: scale(1.1);
}

.modal-content {
  padding: 1.5rem;
  text-align: center;
}

.modal-hologram {
  position: relative;
  width: 80px;
  height: 80px;
  margin: 0 auto 1.5rem;
}

.modal-hologram .hologram-icon {
  background: rgba(0, 255, 255, 0.2);
  color: #00ffff;
}

.modal-hologram .hologram-icon.success-icon {
  background: rgba(0, 255, 127, 0.2);
  color: #00ff7f;
}

.modal-hologram .hologram-icon.error-icon {
  background: rgba(255, 75, 43, 0.2);
  color: #ff4b2b;
}

.modal-content p {
  margin-bottom: 1.5rem;
  font-size: 1rem;
  line-height: 1.5;
}

.cyber-textarea {
  width: 100%;
  background: rgba(0, 255, 255, 0.05);
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 8px;
  color: #fff;
  padding: 0.8rem;
  font-family: 'Rajdhani', sans-serif;
  resize: none;
  margin-top: 1rem;
  transition: all 0.3s ease;
}

.cyber-textarea:focus {
  outline: none;
  border-color: rgba(0, 255, 255, 0.7);
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.3);
}

.modal-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
  padding: 0 1.5rem 1.5rem;
  flex-wrap: wrap;
}

.reject-btn {
  background: rgba(255, 75, 43, 0.2);
  border-color: rgba(255, 75, 43, 0.5);
  color: #ff4b2b;
}

.reject-btn .button-glitch {
  background: linear-gradient(90deg, 
    transparent, 
    rgba(255, 75, 43, 0.2), 
    transparent);
}

.accept-btn {
  background: rgba(0, 255, 127, 0.2);
  border-color: rgba(0, 255, 127, 0.5);
  color: #00ff7f;
}

.accept-btn .button-glitch {
  background: linear-gradient(90deg, 
    transparent, 
    rgba(0, 255, 127, 0.2), 
    transparent);
}

.cancel-btn {
  background: rgba(150, 150, 150, 0.2);
  border-color: rgba(150, 150, 150, 0.5);
  color: #b0b0cc;
}

.cancel-btn .button-glitch {
  background: linear-gradient(90deg, 
    transparent, 
    rgba(150, 150, 150, 0.2), 
    transparent);
}

.confirm-reject-btn {
  background: rgba(255, 75, 43, 0.2);
  border-color: rgba(255, 75, 43, 0.5);
  color: #ff4b2b;
}

.ok-btn {
  min-width: 120px;
}

/* Responsive Design */
@media (max-width: 1200px) {
  .accounts-grid {
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  }
  
  .account-details {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
  
  .glitch-text {
    font-size: 3rem;
  }
  
  .header-hologram {
    width: 80px;
    height: 80px;
  }
  
  .hologram-planet {
    width: 50px;
    height: 50px;
  }
}

@media (max-width: 768px) {
  .cyber-vault {
    padding: 80px 1rem 1rem;
  }
  
  .vault-header {
    flex-direction: column;
    text-align: center;
  }
  
  .header-hologram {
    margin-right: 0;
    margin-bottom: 1.5rem;
  }
  
  .glitch-text {
    font-size: 2.5rem;
  }
  
  .panel-stats {
    flex-direction: column;
    align-items: center;
  }
  
  .stat-item {
    width: 100%;
  }
  
  .accounts-grid {
    grid-template-columns: 1fr;
  }
  
  .account-details {
    padding: 1rem;
  }
  
  .game-banner {
    padding: 1rem;
  }
  
  .account-actions {
    padding: 0 1rem 1rem;
    justify-content: center;
  }
  
  .cyber-button {
    width: 100%;
    text-align: center;
  }
  
  .vault-tabs {
    justify-content: center;
  }
  
  .tab-button {
    padding: 0.6rem 1rem;
    font-size: 0.8rem;
  }
  
  .cyber-table th,
  .cyber-table td {
    padding: 0.8rem;
    font-size: 0.8rem;
  }
}

@media (max-width: 480px) {
  .glitch-text {
    font-size: 2rem;
  }
  
  .header-subtitle {
    font-size: 1rem;
  }
  
  .game-icon {
    width: 40px;
    height: 40px;
    font-size: 1.2rem;
  }
  
  .game-title {
    font-size: 1.1rem;
  }
  
  .detail-row {
    gap: 0.5rem;
  }
  
  .detail-icon {
    width: 24px;
    height: 24px;
    font-size: 0.8rem;
  }
  
  .modal-actions {
    flex-direction: column;
  }
  
  .cyber-modal {
    max-width: 95%;
  }
  
  .cyber-table {
    font-size: 0.7rem;
  }
}

/* Animations */
@keyframes rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes pulse {
  0%, 100% { transform: scale(1); opacity: 0.8; }
  50% { transform: scale(1.1); opacity: 1; }
}

@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}
</style>
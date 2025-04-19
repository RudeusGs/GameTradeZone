<template>
    <div class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-content">
        <div class="modal-header">
          <h3 class="modal-title">Hoàn thiện thông tin</h3>
          <p class="modal-subtitle">Vui lòng nhập thông tin để hoàn tất đăng nhập</p>
          <button class="close-btn" @click="$emit('close')">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <form @submit.prevent="$emit('submit')" class="modal-form">
          <div class="form-group">
            <div class="input-group">
              <i class="fas fa-user input-icon"></i>
              <input type="text" v-model="modalData.accountName" placeholder="Tên đăng nhập" required />
            </div>
            <div class="input-group">
              <i class="fas fa-envelope input-icon"></i>
              <input type="email" v-model="modalData.email" placeholder="Email" required readonly />
            </div>
            <div class="input-group">
              <i class="fas fa-id-card input-icon"></i>
              <input type="text" v-model="modalData.fullName" placeholder="Họ và tên" required />
            </div>
            <div class="input-group bank-select-group">
              <i class="fas fa-university input-icon"></i>
              <select v-model="modalData.bankName" class="bank-select" required>
                <option value="" disabled selected>Chọn ngân hàng</option>
                <option v-for="bank in banks" :key="bank.id" :value="bank.name">
                  {{ bank.name }}
                </option>
              </select>
            </div>
            <div class="input-group">
              <i class="fas fa-credit-card input-icon"></i>
              <input type="text" v-model="modalData.bankNumber" placeholder="Số tài khoản ngân hàng" required />
            </div>
          </div>
          <button type="submit" class="modal-submit-btn" :disabled="loading">
            <span v-if="!loading">Lưu thông tin</span>
            <span v-else class="loading-spinner-btn"></span>
          </button>
        </form>
        <p v-if="modalError" class="modal-error">{{ modalError }}</p>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent, type PropType } from 'vue';
  
  interface ModalDataInterface {
    accountName: string;
    email: string;
    fullName: string;
    bankName: string;
    bankNumber: string;
    token: string;
  }
  
  interface BankInterface {
    id: number;
    name: string;
  }
  
  export default defineComponent({
    name: 'UserInfoModal',
    props: {
      modalData: {
        type: Object as PropType<ModalDataInterface>,
        required: true
      },
      modalError: {
        type: String as PropType<string | null>,
        default: null
      },
      loading: {
        type: Boolean,
        default: false
      },
      banks: {
        type: Array as PropType<BankInterface[]>,
        required: true
      }
    },
    emits: ['submit', 'close']
  });
  </script>
  
  <style scoped>
  .modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.65);
    backdrop-filter: blur(6px);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }
  
  .modal-content {
    background: rgba(15, 20, 45, 0.95);
    padding: 32px;
    border-radius: 20px;
    border: 1px solid rgba(0, 255, 255, 0.3);
    box-shadow: 0 0 30px rgba(0, 255, 255, 0.15);
    max-width: 480px;
    width: 100%;
    color: white;
    position: relative;
    animation: popFade 0.35s ease;
    backdrop-filter: blur(10px);
    overflow: hidden;
  }
  
  @keyframes popFade {
    from {
      opacity: 0;
      transform: scale(0.95);
    }
    to {
      opacity: 1;
      transform: scale(1);
    }
  }
  
  .modal-header {
    text-align: center;
    margin-bottom: 24px;
  }
  
  .modal-title {
    font-size: 2rem;
    color: #00ffff;
    font-weight: 800;
    text-shadow: 0 0 15px rgba(0, 255, 255, 0.5);
    letter-spacing: 1px;
  }
  
  .modal-subtitle {
    font-size: 0.95rem;
    color: #cdd6f4;
    margin-top: 8px;
  }
  
  .close-btn {
    position: absolute;
    top: 14px;
    right: 14px;
    width: 36px;
    height: 36px;
    border-radius: 50%;
    background: rgba(0, 255, 255, 0.1);
    border: 1px solid rgba(0, 255, 255, 0.3);
    display: flex;
    align-items: center;
    justify-content: center;
    color: #00ffff;
    font-size: 1rem;
    cursor: pointer;
    transition: all 0.3s ease;
  }
  
  .close-btn:hover {
    background: rgba(0, 255, 255, 0.2);
    transform: rotate(90deg);
  }
  
  .modal-form {
    display: flex;
    flex-direction: column;
    gap: 16px;
  }
  
  .input-group {
    position: relative;
    margin-top: 10px;
  }
  
  .input-icon {
    position: absolute;
    left: 14px;
    top: 50%;
    transform: translateY(-50%);
    color: #00ffff;
    opacity: 0.8;
    font-size: 1rem;
  }
  
  input,
  .bank-select {
    width: 100%;
    padding: 12px 14px 12px 44px;
    background: rgba(255, 255, 255, 0.06);
    border: 1px solid rgba(0, 255, 255, 0.2);
    color: #fff;
    font-size: 0.95rem;
    border-radius: 10px;
    outline: none;
    transition: all 0.3s ease;
  }
  
  input:focus,
  .bank-select:focus {
    border-color: #00ffff;
    background: rgba(255, 255, 255, 0.1);
    box-shadow: 0 0 10px rgba(0, 255, 255, 0.4);
  }
  
  input[readonly] {
    background: rgba(255, 255, 255, 0.05);
    color: #aaa;
    cursor: not-allowed;
  }
  
  .bank-select option {
    background-color: #121c34;
    color: #fff;
  }
  
  .modal-submit-btn {
    padding: 14px;
    background: linear-gradient(135deg, #00ffff, #00b7b7);
    color: white;
    font-weight: bold;
    font-size: 1rem;
    border: none;
    border-radius: 10px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
    transition: transform 0.3s ease, box-shadow 0.3s ease;
    box-shadow: 0 4px 16px rgba(0, 255, 255, 0.3);
    min-height: 52px;
  }
  
  .modal-submit-btn:hover:not(:disabled) {
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(0, 255, 255, 0.5);
  }
  
  .modal-submit-btn:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
  
  .modal-error {
    color: #ff4d4d;
    font-size: 0.9rem;
    margin-top: 12px;
    text-shadow: 0 0 4px rgba(255, 77, 77, 0.3);
  }
  
  .loading-spinner-btn {
    display: inline-block;
    width: 22px;
    height: 22px;
    border: 3px solid rgba(255, 255, 255, 0.2);
    border-top: 3px solid #fff;
    border-radius: 50%;
    animation: spin 1s linear infinite;
  }
  
  @keyframes spin {
    to {
      transform: rotate(360deg);
    }
  }
  </style>
  
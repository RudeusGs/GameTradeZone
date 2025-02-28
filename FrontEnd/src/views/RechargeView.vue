<template>
    <div class="recharge-container">
      <!-- Particle Background -->
      <div class="particle-background">
        <div v-for="i in 15" :key="i" class="particle"></div> <!-- Giảm số lượng particle để nhẹ hơn -->
      </div>
  
      <!-- Main Content -->
      <section class="recharge-content">
        <h1 class="recharge-title">Nạp Tiền GameTradeZone</h1>
        <p class="recharge-subtitle">Chọn phương thức nạp để tiếp tục!</p>
  
        <!-- Tabs cho hai phương thức nạp tiền -->
        <div class="recharge-tabs">
          <button
            :class="{ 'active': activeTab === 'bank' }"
            @click="activeTab = 'bank'"
            class="tab-button"
          >
            <i class="fas fa-qrcode"></i> Ngân Hàng (QR)
          </button>
          <button
            :class="{ 'active': activeTab === 'card' }"
            @click="activeTab = 'card'"
            class="tab-button"
          >
            <i class="fas fa-sim-card"></i> Thẻ Cào
          </button>
        </div>
  
        <!-- Nội dung phương thức nạp -->
        <transition name="slide">
          <div v-if="activeTab === 'bank'" class="recharge-method bank-method">
            <div class="method-header">
              <h2 class="method-title">Nạp Qua QR Ngân Hàng</h2>
              <p class="method-description">
                Quét mã QR để nạp. Nhập số tiền và hoàn tất trong 5 phút.
              </p>
            </div>
  
            <div class="method-body">
              <div class="qr-section">
                <div class="qr-wrapper">
                  <img src="https://via.placeholder.com/150x150?text=QR+Bank" alt="Mã QR Ngân hàng" class="qr-image" />
                  <p class="qr-instruction">Quét mã QR</p>
                </div>
                <div class="amount-section">
                  <label for="bankAmount" class="amount-label">Số tiền (VNĐ):</label>
                  <input
                    v-model="bankAmount"
                    type="number"
                    id="bankAmount"
                    class="amount-input"
                    placeholder="Nhập số tiền"
                    min="10000"
                    required
                  />
                </div>
              </div>
  
              <button @click="confirmBankRecharge" class="confirm-btn" :disabled="!bankAmount || bankAmount < 10000">
                <i class="fas fa-check"></i> Nạp Ngay
              </button>
            </div>
  
            <p class="note-text">Giao dịch mất 1-5 phút. Hỗ trợ: <a href="mailto:support@gametradezone.com" class="note-link">support@gametradezone.com</a></p>
          </div>
  
          <div v-else class="recharge-method card-method">
            <div class="method-header">
              <h2 class="method-title">Nạp Qua Thẻ Cào</h2>
              <p class="method-description">
                Nhập thông tin thẻ cào. Đảm bảo mã chính xác.
              </p>
            </div>
  
            <div class="method-body">
              <div class="card-section">
                <div class="card-field">
                  <label for="cardProvider" class="card-label">Nhà mạng:</label>
                  <select v-model="cardProvider" id="cardProvider" class="card-select">
                    <option value="viettel">Viettel</option>
                    <option value="mobifone">Mobifone</option>
                    <option value="vinaphone">Vinaphone</option>
                  </select>
                </div>
                <div class="card-field">
                  <label for="cardCode" class="card-label">Mã thẻ:</label>
                  <input
                    v-model="cardCode"
                    type="text"
                    id="cardCode"
                    class="card-input"
                    placeholder="12-14 số"
                    maxlength="14"
                    pattern="[0-9]*"
                    required
                  />
                </div>
                <div class="card-field">
                  <label for="cardSerial" class="card-label">Serial:</label>
                  <input
                    v-model="cardSerial"
                    type="text"
                    id="cardSerial"
                    class="card-input"
                    placeholder="14 số"
                    maxlength="14"
                    pattern="[0-9]*"
                    required
                  />
                </div>
                <p class="card-amount">
                  Số tiền: <span class="amount-value">{{ calculateCardAmount() }} VNĐ</span>
                </p>
              </div>
  
              <button @click="confirmCardRecharge" class="confirm-btn" :disabled="!cardCode || !cardSerial || cardCode.length < 12 || cardSerial.length < 14">
                <i class="fas fa-check"></i> Nạp Ngay
              </button>
            </div>
  
            <p class="note-text">Kiểm tra thẻ trong 1-3 phút. Hỗ trợ: <a href="mailto:support@gametradezone.com" class="note-link">support@gametradezone.com</a></p>
          </div>
        </transition>
      </section>
  
      <!-- Footer -->
      <footer class="recharge-footer">
        <p class="footer-text">© 2025 GameTradeZone. Tất cả quyền được bảo lưu.</p>
      </footer>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref } from 'vue';
  
  const activeTab = ref<string>('bank'); // Tab mặc định là nạp qua ngân hàng
  const bankAmount = ref<number>(0); // Số tiền nạp qua ngân hàng
  const cardProvider = ref<string>('viettel'); // Nhà mạng thẻ cào
  const cardCode = ref<string>(''); // Mã thẻ cào
  const cardSerial = ref<string>(''); // Số serial thẻ cào
  
  // Hàm xác nhận nạp qua ngân hàng
  const confirmBankRecharge = () => {
    if (bankAmount.value >= 10000) {
      alert(`Nạp tiền thành công! Số tiền: ${bankAmount.value} VNĐ qua ngân hàng.`);
      bankAmount.value = 0; // Reset sau khi nạp
    } else {
      alert('Vui lòng nhập số tiền ít nhất 10,000 VNĐ!');
    }
  };
  
  // Hàm xác nhận nạp qua thẻ cào
  const confirmCardRecharge = () => {
    if (cardCode.value.length >= 12 && cardSerial.value.length === 14) {
      alert(`Nạp tiền thành công qua thẻ ${cardProvider.value}! Số tiền: ${calculateCardAmount()} VNĐ.`);
      cardCode.value = '';
      cardSerial.value = '';
    } else {
      alert('Vui lòng nhập đúng mã thẻ (12-14 số) và số serial (14 số)!');
    }
  };
  
  // Hàm tính số tiền nạp qua thẻ cào (giả định giá trị cố định)
  const calculateCardAmount = () => {
    const amounts: { [key: string]: number } = {
      viettel: 50000,
      mobifone: 100000,
      vinaphone: 200000,
    };
    return amounts[cardProvider.value] || 0;
  };
  </script>
  
  <style scoped>
  /* Tổng thể */
  .recharge-container {
    min-height: 100vh;
    background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%); /* Đồng bộ gradient với navbar */
    font-family: 'Arial', sans-serif; /* Đồng bộ font với navbar */
    color: #f0f0f0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    position: relative;
    overflow: hidden;
  }
  
  /* Particle Background */
  .particle-background {
    position: absolute;
    inset: 0;
    z-index: -1;
    background: linear-gradient(135deg, #1a0933 0%, #0d1b2a 100%); /* Giữ gradient nhưng đồng bộ */
  }
  
  .particle {
    position: absolute;
    width: 4px; /* Giảm kích thước particle */
    height: 4px;
    background: rgba(0, 179, 224, 0.5); /* Màu cyan, đồng bộ với navbar */
    border-radius: 50%;
    animation: float 10s infinite ease-in-out;
  }
  
  .particle:nth-child(odd) {
    background: rgba(255, 0, 255, 0.5); /* Màu magenta, đồng bộ với navbar */
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
  
  @keyframes float {
    0% { transform: translateY(0) scale(1); opacity: 0.8; }
    50% { transform: translateY(-100vh) scale(1.3); opacity: 0.3; } /* Giảm scale để nhẹ nhàng hơn */
    100% { transform: translateY(0) scale(1); opacity: 0.8; }
  }
  
  /* Main Content */
  .recharge-content {
    max-width: 700px; /* Giảm kích thước container để vừa khung màn hình */
    margin: 0 auto;
    padding: 40px 20px; /* Giảm padding để nhỏ gọn hơn */
    position: relative;
    z-index: 1;
  }
  
  /* Titles */
  .recharge-title {
    font-size: 2.8rem; /* Giảm kích thước tiêu đề */
    font-weight: 800;
    color: #f8f8f8; /* Màu chữ kem nhạt, đồng bộ với navbar */
    text-align: center;
    margin-bottom: 15px; /* Giảm khoảng cách */
    text-shadow: 0 0 15px #00b3e0, 0 0 5px #ff00ff; /* Giữ glow nhưng nhỏ gọn hơn */
    transition: all 0.3s ease-in-out; /* Giảm thời gian transition */
  }
  
  .recharge-title:hover {
    transform: scale(1.05); /* Giữ hiệu ứng nhưng nhỏ hơn */
    text-shadow: 0 0 20px #00b3e0, 0 0 7px #ff00ff;
  }
  
  .recharge-subtitle {
    font-size: 1.2rem; /* Giảm kích thước phụ đề */
    color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    text-align: center;
    margin-bottom: 25px; /* Giảm khoảng cách */
    text-shadow: 0 0 4px rgba(255, 255, 255, 0.3); /* Giảm bóng chữ */
  }
  
  /* Tabs */
  .recharge-tabs {
    display: flex;
    justify-content: center;
    gap: 15px; /* Giảm khoảng cách giữa các tab */
    margin-bottom: 25px; /* Giảm khoảng cách với nội dung bên dưới */
    flex-wrap: wrap;
  }
  
  .tab-button {
    background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
    color: #f0f0f0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    padding: 10px 20px; /* Giảm padding để nhỏ gọn hơn */
    border: 1px solid #00b3e0; /* Viền cyan, đồng bộ với navbar */
    border-radius: 8px; /* Giảm bo tròn để đơn giản hơn */
    cursor: pointer;
    font-weight: 600; /* Giảm độ đậm để nhẹ nhàng hơn */
    font-size: 1rem; /* Giảm kích thước chữ */
    transition: all 0.2s ease-in-out; /* Giảm thời gian transition */
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Giảm bóng chữ */
    box-shadow: 0 3px 10px rgba(0, 204, 255, 0.2); /* Giảm bóng đổ */
    display: flex;
    align-items: center;
    gap: 8px; /* Giảm khoảng cách giữa icon và text */
  }
  
  .tab-button.active,
  .tab-button:hover {
    background: linear-gradient(45deg, #00b3e0, #ff00ff); /* Gradient màu navbar */
    color: #f8f8f8; /* Màu chữ sáng, đồng bộ với navbar */
    box-shadow: 0 5px 15px rgba(0, 204, 255, 0.4); /* Giảm bóng đổ hover */
    transform: translateY(-2px); /* Giữ hiệu ứng nâng nhưng nhỏ hơn */
    border-color: #ff00ff; /* Viền magenta khi active/hover */
  }
  
  .tab-button i {
    font-size: 1rem; /* Giảm kích thước icon */
    transition: transform 0.2s ease-in-out; /* Giảm thời gian transition */
  }
  
  .tab-button:hover i {
    transform: rotate(360deg); /* Giữ hiệu ứng xoay nhưng nhẹ nhàng hơn */
  }
  
  /* Transition */
  .slide-enter-active,
  .slide-leave-active {
    transition: all 0.3s ease-in-out; /* Giảm thời gian transition */
  }
  
  .slide-enter-from,
  .slide-leave-to {
    transform: translateY(-10px); /* Giảm khoảng trượt để nhỏ gọn hơn */
    opacity: 0;
  }
  
  /* Phương thức nạp */
  .recharge-method {
    background: rgba(28, 37, 38, 0.95); /* Giữ nền trong suốt */
    border: 1px solid #00b3e0; /* Viền cyan, đồng bộ với navbar */
    border-radius: 8px; /* Giữ bo tròn đơn giản */
    padding: 25px; /* Giảm padding để nhỏ gọn hơn */
    box-shadow: 0 5px 15px rgba(0, 204, 255, 0.2); /* Giảm bóng đổ */
    transition: all 0.2s ease-in-out; /* Giữ transition */
    margin-bottom: 15px; /* Giảm khoảng cách dưới */
  }
  
  .recharge-method:hover {
    transform: translateY(-3px); /* Giảm hiệu ứng nâng */
    box-shadow: 0 8px 20px rgba(0, 204, 255, 0.3); /* Giảm bóng đổ hover */
  }
  
  .bank-method { border-color: #00b3e0; }
  .card-method { border-color: #ff00ff; } /* Phân biệt hai phương thức bằng màu magenta */
  
  /* Method Header */
  .method-header {
    margin-bottom: 20px; /* Giảm khoảng cách */
  }
  
  .method-title {
    font-size: 1.8rem; /* Giảm kích thước tiêu đề */
    font-weight: 700;
    color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
    margin-bottom: 10px; /* Giảm khoảng cách */
    text-shadow: 0 0 10px #00b3e0, 0 0 4px #ff00ff; /* Giảm glow */
    transition: all 0.2s ease-in-out; /* Giảm thời gian transition */
  }
  
  .method-title:hover {
    transform: scale(1.05); /* Giữ hiệu ứng zoom nhưng nhỏ hơn */
    text-shadow: 0 0 15px #00b3e0, 0 0 6px #ff00ff;
  }
  
  .method-description {
    font-size: 1rem; /* Giảm kích thước để nhỏ gọn hơn */
    color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    margin-bottom: 15px; /* Giảm khoảng cách */
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.3); /* Giảm bóng chữ */
    line-height: 1.5; /* Giảm khoảng cách dòng */
  }
  
  /* Method Body */
  .method-body {
    display: flex;
    flex-direction: column;
    gap: 15px; /* Giảm khoảng cách giữa các phần */
  }
  
  /* QR Section */
  .qr-section {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px; /* Giảm khoảng cách */
    background: rgba(28, 37, 38, 0.7); /* Giảm độ trong suốt cho nhẹ nhàng hơn */
    padding: 15px; /* Giảm padding */
    border-radius: 8px; /* Giữ bo tròn đơn giản */
    box-shadow: 0 3px 10px rgba(0, 204, 255, 0.1); /* Giảm bóng đổ */
    transition: all 0.2s ease-in-out; /* Giữ transition */
  }
  
  .qr-section:hover {
    box-shadow: 0 5px 15px rgba(0, 204, 255, 0.3); /* Giảm bóng đổ hover */
  }
  
  .qr-wrapper {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 5px; /* Giảm khoảng cách */
  }
  
  .qr-image {
    width: 150px; /* Giảm kích thước QR */
    height: 150px;
    border: 1px solid #00b3e0; /* Giữ viền nhưng đơn giản hơn */
    border-radius: 8px; /* Giữ bo tròn đơn giản */
    box-shadow: 0 0 8px rgba(0, 204, 255, 0.1); /* Giảm bóng đổ */
    transition: all 0.2s ease-in-out; /* Giữ transition */
  }
  
  .qr-image:hover {
    box-shadow: 0 0 12px rgba(0, 204, 255, 0.3); /* Giảm bóng đổ hover */
  }
  
  .qr-instruction {
    font-size: 0.9rem; /* Giảm kích thước để nhỏ gọn hơn */
    color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.2); /* Giảm bóng chữ */
  }
  
  /* Amount Section */
  .amount-section {
    display: flex;
    flex-direction: column;
    gap: 5px; /* Giảm khoảng cách */
    width: 100%;
    max-width: 200px; /* Giảm chiều rộng input */
  }
  
  .amount-label {
    font-size: 1rem; /* Giảm kích thước label */
    color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
    text-shadow: 0 0 3px #00b3e0; /* Giảm glow */
  }
  
  .amount-input {
    width: 100%;
    padding: 8px; /* Giảm padding */
    background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
    border: 1px solid #00b3e0; /* Giữ viền nhưng đơn giản hơn */
    border-radius: 8px; /* Giữ bo tròn đơn giản */
    color: #f0f0f0;
    font-size: 1rem; /* Giảm kích thước chữ */
    box-shadow: 0 0 5px rgba(0, 204, 255, 0.1); /* Giảm bóng đổ */
    transition: all 0.2s ease-in-out; /* Giữ transition */
  }
  
  .amount-input:focus {
    box-shadow: 0 0 10px rgba(0, 204, 255, 0.3); /* Giảm bóng đổ focus */
    outline: none;
    border-color: #ff00ff; /* Màu focus magenta, đồng bộ với navbar */
  }
  
  .amount-input:disabled {
    background: rgba(28, 37, 38, 0.5);
    color: #808080;
    cursor: not-allowed;
  }
  
  /* Card Section */
  .card-section {
    display: flex;
    flex-direction: column;
    gap: 10px; /* Giảm khoảng cách */
    background: rgba(28, 37, 38, 0.7); /* Giảm độ trong suốt cho nhẹ nhàng hơn */
    padding: 15px; /* Giảm padding */
    border-radius: 8px; /* Giữ bo tròn đơn giản */
    box-shadow: 0 3px 10px rgba(0, 204, 255, 0.1); /* Giảm bóng đổ */
    transition: all 0.2s ease-in-out; /* Giữ transition */
  }
  
  .card-section:hover {
    box-shadow: 0 5px 15px rgba(0, 204, 255, 0.3); /* Giảm bóng đổ hover */
  }
  
  .card-field {
    display: flex;
    flex-direction: column;
    gap: 5px; /* Giảm khoảng cách */
  }
  
  .card-label {
    font-size: 1rem; /* Giảm kích thước label */
    color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
    text-shadow: 0 0 3px #00b3e0; /* Giảm glow */
  }
  
  .card-select {
    width: 100%;
    padding: 8px; /* Giảm padding */
    background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
    border: 1px solid #00b3e0; /* Giữ viền nhưng đơn giản hơn */
    border-radius: 8px; /* Giữ bo tròn đơn giản */
    color: #f0f0f0;
    font-size: 1rem; /* Giảm kích thước chữ */
    box-shadow: 0 0 5px rgba(0, 204, 255, 0.1); /* Giảm bóng đổ */
    transition: all 0.2s ease-in-out; /* Giữ transition */
  }
  
  .card-select:focus {
    box-shadow: 0 0 10px rgba(0, 204, 255, 0.3); /* Giảm bóng đổ focus */
    outline: none;
    border-color: #ff00ff; /* Màu focus magenta, đồng bộ với navbar */
  }
  
  .card-input {
    width: 100%;
    padding: 8px; /* Giảm padding */
    background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
    border: 1px solid #00b3e0; /* Giữ viền nhưng đơn giản hơn */
    border-radius: 8px; /* Giữ bo tròn đơn giản */
    color: #f0f0f0;
    font-size: 1rem; /* Giảm kích thước chữ */
    box-shadow: 0 0 5px rgba(0, 204, 255, 0.1); /* Giảm bóng đổ */
    transition: all 0.2s ease-in-out; /* Giữ transition */
  }
  
  .card-input:focus {
    box-shadow: 0 0 10px rgba(0, 204, 255, 0.3); /* Giảm bóng đổ focus */
    outline: none;
    border-color: #ff00ff; /* Màu focus magenta, đồng bộ với navbar */
  }
  
  .card-amount {
    font-size: 1rem; /* Giảm kích thước */
    color: #e0e0e0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.2); /* Giảm bóng chữ */
    text-align: center; /* Giữ căn giữa */
  }
  
  .amount-value {
    color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
    font-weight: 600; /* Giảm độ đậm */
  }
  
  /* Confirm Button */
  .confirm-btn {
    width: 100%;
    padding: 10px; /* Giảm padding */
    background: rgba(28, 37, 38, 0.9); /* Nền trong suốt, đồng bộ với navbar */
    color: #f0f0f0; /* Màu chữ xám nhạt, đồng bộ với navbar */
    border: 1px solid #00b3e0; /* Giữ viền nhưng đơn giản hơn */
    border-radius: 8px; /* Giữ bo tròn đơn giản */
    font-size: 1.1rem; /* Giảm kích thước chữ */
    font-weight: 600; /* Giữ độ đậm */
    cursor: pointer;
    transition: all 0.2s ease-in-out; /* Giữ transition */
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.2); /* Giảm bóng chữ */
    box-shadow: 0 3px 10px rgba(0, 204, 255, 0.1); /* Giảm bóng đổ */
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 8px; /* Giảm khoảng cách giữa icon và text */
  }
  
  .confirm-btn:disabled {
    background: rgba(28, 37, 38, 0.5);
    color: #808080;
    border-color: #808080;
    cursor: not-allowed;
    box-shadow: none;
  }
  
  .confirm-btn:hover:not(:disabled) {
    background: linear-gradient(45deg, #00b3e0, #ff00ff); /* Gradient màu navbar */
    color: #f8f8f8; /* Màu chữ sáng, đồng bộ với navbar */
    box-shadow: 0 5px 15px rgba(0, 204, 255, 0.3); /* Giảm bóng đổ hover */
    transform: translateY(-2px); /* Giữ hiệu ứng nâng */
  }
  
  .confirm-btn i {
    font-size: 1rem; /* Giảm kích thước icon */
    transition: transform 0.2s ease-in-out; /* Giữ transition */
  }
  
  .confirm-btn:hover:not(:disabled) i {
    transform: rotate(360deg); /* Giữ hiệu ứng xoay */
  }
  
  /* Note Text */
  .note-text {
    font-size: 0.9rem; /* Giảm kích thước */
    color: #b0b0b0; /* Màu chữ xám nhạt hơn, đồng bộ với navbar */
    margin-top: 10px; /* Giảm khoảng cách với nút xác nhận */
    text-align: center;
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.2); /* Giảm bóng chữ */
  }
  
  .note-link {
    color: #00b3e0; /* Màu cyan, đồng bộ với navbar */
    text-decoration: underline;
    transition: all 0.2s ease-in-out; /* Giữ transition */
  }
  
  .note-link:hover {
    color: #ff00ff; /* Màu magenta, đồng bộ với navbar */
    text-shadow: 0 0 6px #ff00ff; /* Giảm glow khi hover */
  }
  
  /* Footer */
  .recharge-footer {
    margin-top: 20px; /* Giảm khoảng cách */
    text-align: center;
    padding: 15px; /* Giảm padding */
    background: rgba(13, 27, 42, 0.9); /* Nền trong suốt hơn, đồng bộ với navbar */
    border-radius: 8px; /* Giữ bo tròn đơn giản */
    box-shadow: 0 3px 10px rgba(0, 204, 255, 0.1); /* Giảm bóng đổ */
    transition: all 0.2s ease-in-out; /* Giữ transition */
  }
  
  .recharge-footer:hover {
    box-shadow: 0 5px 15px rgba(0, 204, 255, 0.3); /* Giảm bóng đổ hover */
  }
  
  .footer-text {
    font-size: 1rem; /* Giữ kích thước */
    color: #b0b0b0; /* Màu chữ xám nhạt hơn, đồng bộ với navbar */
    margin: 5px 0;
    text-shadow: 0 0 3px rgba(255, 255, 255, 0.2); /* Giảm bóng chữ */
  }
  
  /* Responsive */
  @media (max-width: 768px) {
    .recharge-content {
      padding: 20px 15px; /* Giảm padding thêm trên mobile */
    }
  
    .recharge-title {
      font-size: 2rem; /* Giảm kích thước tiêu đề trên mobile */
    }
  
    .recharge-subtitle {
      font-size: 1rem; /* Giảm kích thước phụ đề */
    }
  
    .recharge-tabs {
      flex-direction: column;
      gap: 10px; /* Giảm khoảng cách */
    }
  
    .tab-button {
      width: 100%;
      padding: 8px 15px; /* Giảm padding thêm */
      font-size: 0.9rem; /* Giảm kích thước chữ */
    }
  
    .recharge-method {
      padding: 15px; /* Giảm padding thêm */
    }
  
    .method-title {
      font-size: 1.4rem; /* Giảm kích thước tiêu đề */
    }
  
    .method-description {
      font-size: 0.9rem; /* Giảm kích thước */
    }
  
    .qr-image {
      width: 120px; /* Giảm kích thước QR thêm */
      height: 120px;
    }
  
    .amount-section, .card-field {
      max-width: 100%;
    }
  
    .amount-input, .card-select, .card-input {
      padding: 8px; /* Giữ padding */
      font-size: 0.9rem; /* Giảm kích thước chữ */
    }
  
    .confirm-btn {
      padding: 8px; /* Giảm padding */
      font-size: 1rem; /* Giảm kích thước chữ */
    }
  
    .note-text {
      font-size: 0.8rem; /* Giảm kích thước */
    }
  
    .footer-text {
      font-size: 0.9rem; /* Giảm kích thước */
    }
  
    .particle { 
      width: 2px; /* Giảm kích thước particle thêm */
      height: 2px; 
      animation-duration: 6s; /* Giảm thời gian animation */
    }
  }
  </style>
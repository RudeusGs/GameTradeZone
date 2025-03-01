<template>
    <div class="auction-detail-container">
      <div class="auction-detail">
        <!-- Tiêu đề -->
        <h1>{{ auction.name }}</h1>
  
        <!-- Nội dung chính: Thông tin + Chat -->
        <div class="content-wrapper">
          <!-- Thông tin phiên đấu giá -->
          <div class="auction-info">
            <div class="auction-image-wrapper">
              <img :src="auction.image" alt="Hình ảnh sản phẩm" class="auction-image" />
              <div class="image-overlay"></div>
              <span class="status-badge" :class="auction.status">
                {{ auction.status === 'ongoing' ? 'Live' : auction.status === 'ended' ? 'Đã kết thúc' : 'Sắp tới' }}
              </span>
            </div>
            <div class="info-content">
              <h2>{{ auction.item }}</h2>
              <div class="info-grid">
                <p><span>Giá khởi điểm:</span> {{ auction.startingPrice.toLocaleString() }} VNĐ</p>
                <p><span>Giá hiện tại:</span> {{ auction.currentPrice.toLocaleString() }} VNĐ</p>
                <p v-if="auction.status === 'ongoing'"><span>Thời gian còn lại:</span> <CountdownTimer :end-date="auction.endDate" /></p>
                <p v-if="auction.status === 'ended'"><span>Người thắng:</span> {{ auction.winner }}</p>
              </div>
              <div class="bid-section" v-if="auction.status === 'ongoing'">
                <input 
                  type="number" 
                  v-model="bidAmount" 
                  placeholder="Nhập số tiền đấu giá" 
                  class="bid-input" 
                />
                <button @click="placeBid" class="bid-button">Đặt giá</button>
              </div>
            </div>
          </div>
  
          <!-- Khung chat -->
          <div class="chat-box">
            <h3>Chat</h3>
            <div class="chat-messages">
              <div 
                v-for="(message, index) in chatMessages" 
                :key="index" 
                class="chat-message" 
                :class="{ 'my-message': message.isMine }"
              >
                <span class="message-sender">{{ message.sender }}</span>
                <span class="message-text">{{ message.text }}</span>
                <span class="message-time">{{ message.time }}</span>
              </div>
            </div>
            <div class="chat-input">
              <input 
                type="text" 
                v-model="newMessage" 
                placeholder="Nhập tin nhắn..." 
                @keyup.enter="sendMessage" 
                class="chat-input-field" 
              />
              <button @click="sendMessage" class="send-button">Gửi</button>
            </div>
          </div>
        </div>
  
        <!-- Banner thông báo khi có người đặt giá -->
        <transition name="banner">
          <div v-if="showNotification" class="bid-banner">
            <span class="banner-icon">⚡</span>
            <span class="banner-text">{{ notificationMessage }}</span>
            <button @click="showNotification = false" class="close-banner">X</button>
          </div>
        </transition>
      </div>
    </div>
  </template>
  
  <script>
  import CountdownTimer from '@/components/CountdownTimer.vue';
  
  export default {
    components: {
      CountdownTimer,
    },
    data() {
      return {
        auction: {
          id: 1,
          name: 'Phiên đấu giá 1',
          item: 'Vật phẩm A',
          startingPrice: 1000000,
          currentPrice: 1500000,
          status: 'ongoing',
          winner: null,
          endDate: '2023-10-10T10:00:00',
          image: 'https://via.placeholder.com/300x200?text=Vật+phẩm+A',
        },
        bidAmount: null,
        chatMessages: [
          { sender: 'Người dùng A', text: 'Tôi muốn vật phẩm này!', time: '10:00', isMine: false },
          { sender: 'Bạn', text: 'Đặt giá đi!', time: '10:01', isMine: true },
        ],
        newMessage: '',
        showNotification: false,
        notificationMessage: '',
      };
    },
    methods: {
      placeBid() {
        if (this.bidAmount && this.bidAmount > this.auction.currentPrice) {
          this.auction.currentPrice = this.bidAmount;
          this.showNotification = true;
          this.notificationMessage = `Người dùng khác vừa đặt giá ${this.bidAmount.toLocaleString()} VNĐ!`;
          setTimeout(() => (this.showNotification = false), 5000); // Ẩn sau 5 giây
          this.bidAmount = null;
        } else {
          alert('Vui lòng nhập số tiền lớn hơn giá hiện tại!');
        }
      },
      sendMessage() {
        if (this.newMessage.trim()) {
          this.chatMessages.push({
            sender: 'Bạn',
            text: this.newMessage,
            time: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
            isMine: true,
          });
          this.newMessage = '';
          // Giả lập tin nhắn từ người khác (cho demo)
          setTimeout(() => {
            this.chatMessages.push({
              sender: 'Người dùng X',
              text: 'Tôi cũng muốn đấu giá!',
              time: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
              isMine: false,
            });
          }, 1000);
        }
      },
    },
  };
  </script>
  
  <style scoped>
  /* Container tổng thể */
  .auction-detail-container {
    background: linear-gradient(180deg, #0d0d1a 0%, #1a1a33 100%);
    min-height: 100vh;
    padding: 40px 20px;
    display: flex;
    justify-content: center;
  }
  
  .auction-detail {
    max-width: 1200px;
    width: 100%;
    padding: 20px;
    background: rgba(20, 20, 40, 0.9);
    border-radius: 15px;
    box-shadow: 0 0 30px rgba(0, 255, 204, 0.3);
  }
  
  h1 {
    text-align: center;
    font-size: 2.5rem;
    font-weight: 700;
    background: linear-gradient(90deg, #00ffcc, #ff00cc);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    text-shadow: 0 0 20px rgba(0, 255, 204, 0.5);
    margin-bottom: 40px;
  }
  
  /* Content Wrapper */
  .content-wrapper {
    display: flex;
    gap: 20px;
  }
  
  /* Thông tin phiên đấu giá */
  .auction-info {
    flex: 1;
    background: rgba(0, 0, 0, 0.2);
    border-radius: 15px;
    padding: 15px;
    color: #fff;
  }
  
  .auction-image-wrapper {
    position: relative;
    height: 200px;
  }
  
  .auction-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    border-radius: 10px;
    transition: transform 0.4s ease;
  }
  
  .auction-info:hover .auction-image {
    transform: scale(1.05);
  }
  
  .image-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: linear-gradient(180deg, rgba(0, 0, 0, 0.1), rgba(0, 0, 0, 0.5));
    pointer-events: none;
  }
  
  .status-badge {
    position: absolute;
    top: 10px;
    right: 10px;
    padding: 5px 10px;
    border-radius: 15px;
    font-size: 0.75rem;
    font-weight: 600;
    text-transform: uppercase;
    backdrop-filter: blur(3px);
  }
  
  .status-badge.upcoming { background: rgba(255, 204, 0, 0.8); color: #1a1a33; }
  .status-badge.ongoing { background: rgba(0, 255, 204, 0.8); color: #1a1a33; }
  .status-badge.ended { background: rgba(255, 0, 204, 0.8); color: #ffffff; }
  
  .info-content {
    padding: 15px;
  }
  
  h2 {
    font-size: 1.5rem;
    font-weight: 600;
    color: #ffffff;
    text-shadow: 0 0 10px rgba(0, 255, 204, 0.4);
    margin-bottom: 15px;
  }
  
  .info-grid p {
    font-size: 1rem;
    color: #d0d0d0;
    margin-bottom: 8px;
  }
  
  .info-grid span {
    font-weight: 600;
    color: #00ffcc;
    text-shadow: 0 0 5px rgba(0, 255, 204, 0.4);
  }
  
  .bid-section {
    display: flex;
    gap: 10px;
    margin-top: 20px;
  }
  
  .bid-input {
    flex: 1;
    padding: 10px;
    background: rgba(255, 255, 255, 0.1);
    border: 1px solid rgba(0, 255, 204, 0.3);
    border-radius: 10px;
    color: #fff;
    font-size: 1rem;
  }
  
  .bid-button {
    padding: 10px 20px;
    background: linear-gradient(90deg, #00ffcc, #ff00cc);
    color: #ffffff;
    border: none;
    border-radius: 10px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
  }
  
  .bid-button:hover {
    background: linear-gradient(90deg, #ff00cc, #00ffcc);
    box-shadow: 0 0 15px rgba(0, 255, 204, 0.6);
  }
  
  /* Khung chat */
  .chat-box {
    flex: 1;
    background: rgba(0, 0, 0, 0.2);
    border-radius: 15px;
    padding: 15px;
    display: flex;
    flex-direction: column;
    height: 500px;
  }
  
  .chat-box h3 {
    font-size: 1.2rem;
    color: #00ffcc;
    margin-bottom: 10px;
  }
  
  .chat-messages {
    flex: 1;
    overflow-y: auto;
    padding: 10px;
  }
  
  .chat-message {
    padding: 8px 15px;
    margin-bottom: 10px;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 10px;
    font-size: 0.9rem;
    color: #d0d0d0;
  }
  
  .chat-message.my-message {
    background: rgba(0, 255, 204, 0.2);
    text-align: right;
  }
  
  .message-sender {
    font-weight: 600;
    color: #00ffcc;
    margin-right: 5px;
  }
  
  .message-time {
    font-size: 0.7rem;
    color: #888;
    margin-left: 10px;
  }
  
  .chat-input {
    display: flex;
    gap: 10px;
    margin-top: 10px;
  }
  
  .chat-input-field {
    flex: 1;
    padding: 10px;
    background: rgba(255, 255, 255, 0.1);
    border: 1px solid rgba(0, 255, 204, 0.3);
    border-radius: 10px;
    color: #fff;
    font-size: 1rem;
  }
  
  .send-button {
    padding: 10px 20px;
    background: linear-gradient(90deg, #00ffcc, #ff00cc);
    color: #ffffff;
    border: none;
    border-radius: 10px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
  }
  
  .send-button:hover {
    background: linear-gradient(90deg, #ff00cc, #00ffcc);
    box-shadow: 0 0 15px rgba(0, 255, 204, 0.6);
  }
  
  /* Banner thông báo */
  .bid-banner {
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 80%;
    max-width: 600px;
    padding: 20px;
    background: linear-gradient(90deg, #00ffcc, #ff00cc);
    color: #fff;
    border-radius: 15px;
    box-shadow: 0 0 30px rgba(0, 255, 204, 0.8);
    display: flex;
    align-items: center;
    justify-content: space-between;
    z-index: 1000;
  }
  
  .banner-icon {
    font-size: 1.5rem;
    margin-right: 10px;
  }
  
  .banner-text {
    font-size: 1.2rem;
    font-weight: 600;
  }
  
  .close-banner {
    background: none;
    border: none;
    color: #fff;
    font-size: 1.2rem;
    cursor: pointer;
    transition: all 0.3s ease;
  }
  
  .close-banner:hover {
    color: #ff2947;
  }
  
  .banner-enter-active, .banner-leave-active {
    transition: all 0.5s ease;
  }
  
  .banner-enter-from, .banner-leave-to {
    opacity: 0;
    transform: translate(-50%, -60%) scale(0.8);
  }
  
  /* Responsive */
  @media (max-width: 768px) {
    .content-wrapper {
      flex-direction: column;
    }
    .auction-info, .chat-box {
      flex: none;
      width: 100%;
      margin-bottom: 20px;
    }
    .bid-banner {
      width: 90%;
      padding: 15px;
    }
  }
  </style>
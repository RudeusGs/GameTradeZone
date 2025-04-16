<template>
  <div class="auction-detail-container">
    <div class="auction-detail">
      <!-- Tiêu đề -->
      <h1>{{ auction?.name || 'Đang tải...' }}</h1>

      <!-- Nội dung chính -->
      <div class="content-wrapper">
        <!-- Thông tin phiên đấu giá -->
        <div class="auction-info">
          <div class="auction-image-wrapper">
            <img
              :src="auction?.image || 'https://via.placeholder.com/300x200?text=Loading'"
              alt="Hình ảnh phần thưởng"
              class="auction-image"
            />
            <div class="image-overlay"></div>
            <span class="status-badge" :class="auction?.status">
              {{
                auction?.status === 'ongoing'
                  ? 'Live'
                  : auction?.status === 'ended'
                  ? 'Đã kết thúc'
                  : 'Sắp tới'
              }}
            </span>
          </div>
          <div class="info-content">
            <h2>{{ auction?.prizeName || 'Đang tải...' }}</h2>
            <p class="description">{{ auction?.description || 'Không có mô tả' }}</p>
            <div class="info-grid">
              <p>
                <span>Giá khởi điểm:</span>
                {{ auction?.startingPrice?.toLocaleString() || '0' }} VNĐ
              </p>
              <p>
                <span>Giá hiện tại:</span>
                {{
                  auction?.currentPrice?.toLocaleString() ||
                  auction?.startingPrice?.toLocaleString() ||
                  '0'
                }}
                VNĐ
              </p>
              <p>
                <span>Thời gian bắt đầu:</span>
                {{
                  auction?.startDate
                    ? new Date(auction.startDate).toLocaleString('vi-VN')
                    : 'Chưa xác định'
                }}
              </p>
              <p v-if="auction?.status === 'ongoing' && auction?.endDate">
                <span>Thời gian còn lại:</span>
                <CountdownTimer :end-date="auction?.endDate" />
              </p>
              <p v-if="auction?.status === 'ended'">
                <span>Người thắng:</span>
                {{ auction?.winner || 'Chưa có' }}
              </p>
            </div>
            <div class="bid-section" v-if="auction?.status === 'ongoing'">
              <input
                type="number"
                v-model="bidAmount"
                placeholder="Nhập số tiền đấu giá"
                class="bid-input"
                :min="auction?.currentPrice + 1"
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

      <!-- Banner thông báo -->
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

<script lang="ts">
import { defineComponent, ref, nextTick } from 'vue';
import CountdownTimer from '@/components/CountdownTimer.vue';
import auctionApi from '@/api/auction.api';
import { userStore } from '@/stores/auth'; // Import userStore
import type { UserInfoModel } from '@/models/user-model';

interface Auction {
  id: number;
  name: string;
  prizeName: string;
  description: string;
  startingPrice: number;
  currentPrice: number;
  status: 'upcoming' | 'ongoing' | 'ended';
  winner: string | null;
  startDate: string;
  endDate: string | null;
  image: string;
}

interface BackendAuction {
  userId: number;
  auctionPrizeId: number;
  gameInforsId: number;
  auctionName: string;
  startDateTime: string;
  startPrice: string;
  currentPrice: string;
  timeToEnd: string;
  endStatus: boolean;
  endDateTime: string | null;
  isApproved: boolean;
  winnerId: number;
  id: number;
  createdDate: string;
  updatedDate: string;
  deleteDate: string | null;
}

interface AuctionPrize {
  auctionId: number;
  prizeName: string;
  description: string;
  image: string;
  prizeInfo: string;
  status: boolean;
  id: number;
  createdDate: string;
  updatedDate: string;
  deleteDate: string | null;
}

interface ChatMessage {
  sender: string;
  text: string;
  time: string;
  isMine: boolean;
}

export default defineComponent({
  components: { CountdownTimer },
  props: {
    id: {
      type: String,
      required: true
    }
  },
  setup() {
    const chatMessages = ref<ChatMessage[]>([]);
    const chatMessagesRef = ref<HTMLElement | null>(null);

    const scrollToBottom = () => {
      nextTick(() => {
        if (chatMessagesRef.value) {
          chatMessagesRef.value.scrollTop = chatMessagesRef.value.scrollHeight;
        }
      });
    };

    // Khởi tạo userStore
    const authStore = userStore();

    return { chatMessages, chatMessagesRef, scrollToBottom, authStore };
  },
  data() {
    return {
      auction: null as Auction | null,
      bidAmount: null as number | null,
      newMessage: '',
      showNotification: false,
      notificationMessage: '',
      isLoading: false,
      errorMessage: null as string | null,
    };
  },
  computed: {
    // Kiểm tra trạng thái đăng nhập
    isLoggedIn(): boolean {
      return !!this.authStore.user; // Có user thì là đã đăng nhập
    },
    // Lấy thông tin user
    currentUser(): UserInfoModel | null {
      return this.authStore.user;
    },
    // Lấy số dư của user
    userBalance(): number {
      return this.currentUser?.balance || 0;
    }
  },
  async mounted() {
    // Khởi tạo userStore nếu chưa được khởi tạo
    this.authStore.init();
    console.log('Current User:', this.currentUser);
  console.log('User Balance:', this.userBalance);
    await this.fetchAuction();
  },
  methods: {
    async fetchAuction() {
      const auctionId = parseInt(this.id);
      if (isNaN(auctionId)) {
        this.errorMessage = 'ID phiên đấu giá không hợp lệ';
        return;
      }

      this.isLoading = true;
      this.errorMessage = null;
      try {
        const [auctionResponse, prizeResponse] = await Promise.all([
          auctionApi.GetAuctionById(auctionId),
          auctionApi.GetAuctionPrizeById(auctionId)
        ]);

        if (auctionResponse.data.result.isSuccess && prizeResponse.data.result.isSuccess) {
          const auctionData: BackendAuction = auctionResponse.data.result.data;
          const prizeData: AuctionPrize = prizeResponse.data.result.data;

          this.auction = {
            id: auctionData.id,
            name: auctionData.auctionName,
            prizeName: prizeData.prizeName,
            description: prizeData.description,
            startingPrice: parseInt(auctionData.startPrice) || 0,
            currentPrice: parseInt(auctionData.currentPrice) || parseInt(auctionData.startPrice) || 0,
            status: this.getAuctionStatus(auctionData),
            winner: auctionData.winnerId ? `Người dùng ${auctionData.winnerId}` : null,
            startDate: auctionData.startDateTime,
            endDate: auctionData.endDateTime || null,
            image: prizeData.image || 'https://via.placeholder.com/300x200?text=Auction'
          };
        } else {
          this.errorMessage =
            auctionResponse.data.result.message ||
            prizeResponse.data.result.message ||
            'Không thể lấy thông tin phiên đấu giá';
        }
      } catch (error) {
        this.errorMessage = 'Lỗi kết nối đến server';
        console.error('Error fetching auction:', error);
      } finally {
        this.isLoading = false;
      }
    },
    getAuctionStatus(auction: BackendAuction): 'upcoming' | 'ongoing' | 'ended' {
      const now = new Date();
      const startDate = new Date(auction.startDateTime);
      const endDate = auction.endDateTime ? new Date(auction.endDateTime) : null;

      if (
        startDate > now &&
        auction.isApproved &&
        auction.winnerId === 0 &&
        !auction.endStatus &&
        !endDate
      ) {
        return 'upcoming';
      }
      if (
        startDate <= now &&
        auction.isApproved &&
        !auction.endStatus &&
        !endDate
      ) {
        return 'ongoing';
      }
      return 'ended';
    },
    async placeBid() {
      // Sẽ triển khai sau khi có thông tin user
      console.log('User:', this.currentUser);
      console.log('Balance:', this.userBalance);
    },
    sendMessage() {
      if (!this.isLoggedIn) {
        this.$router.push({ name: 'login', query: { redirect: this.$route.fullPath } });
        return;
      }

      if (!this.newMessage.trim()) return;

      const message: ChatMessage = {
        sender: this.currentUser?.fullName || 'Bạn',
        text: this.newMessage,
        time: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
        isMine: true
      };

      this.chatMessages.push(message);
      this.newMessage = '';
      this.scrollToBottom();

      setTimeout(() => {
        this.chatMessages.push({
          sender: 'Người dùng X',
          text: 'Tôi cũng muốn đấu giá!',
          time: new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
          isMine: false
        });
        this.scrollToBottom();
      }, 1000);
    },
    handleImageError(event: Event) {
      const img = event.target as HTMLImageElement;
      img.src = 'https://via.placeholder.com/300x200?text=Auction';
    }
  }
});
</script>

<style scoped>
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
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  text-shadow: 0 0 20px rgba(0, 255, 204, 0.5);
  margin-bottom: 40px;
}

.content-wrapper {
  display: flex;
  gap: 20px;
}

.auction-info {
  flex: 1;
  background: rgba(0, 0, 0, 0.2);
  border-radius: 15px;
  padding: 15px;
  color: #fff;
}

.auction-image-wrapper {
  position: relative;
  height: 250px;
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

.status-badge.upcoming {
  background: rgba(255, 204, 0, 0.8);
  color: #1a1a33;
}

.status-badge.ongoing {
  background: rgba(0, 255, 204, 0.8);
  color: #1a1a33;
}

.status-badge.ended {
  background: rgba(255, 0, 204, 0.8);
  color: #ffffff;
}

.info-content {
  padding: 15px;
}

h2 {
  font-size: 1.8rem;
  font-weight: 600;
  color: #ffffff;
  text-shadow: 0 0 10px rgba(0, 255, 204, 0.4);
  margin-bottom: 10px;
}

.description {
  font-size: 1rem;
  color: #d0d0d0;
  margin-bottom: 15px;
  line-height: 1.5;
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

.banner-enter-active,
.banner-leave-active {
  transition: all 0.5s ease;
}

.banner-enter-from,
.banner-leave-to {
  opacity: 0;
  transform: translate(-50%, -60%) scale(0.8);
}

@media (max-width: 768px) {
  .content-wrapper {
    flex-direction: column;
  }
  .auction-info,
  .chat-box {
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
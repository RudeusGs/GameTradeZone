<template>
  <div class="auction-detail-container">
    <div class="auction-detail">
      <!-- Tiêu đề -->
      <h1>{{ auction?.name || "Đang tải..." }}</h1>

      <!-- Nội dung chính -->
      <div class="content-wrapper">
        <!-- Thông tin phiên đấu giá -->
        <div class="auction-info">
          <div class="auction-image-wrapper">
            <div class="image-carousel">
              <img
                v-for="(image, index) in auction?.images"
                :key="index"
                :src="image || 'https://via.placeholder.com/300x200?text=Loading'"
                alt="Hình ảnh phần thưởng"
                class="auction-image"
                @error="handleImageError"
              />
            </div>
            <div class="image-overlay"></div>
            <span class="status-badge" :class="auction?.status">
              {{
                auction?.status === "ongoing"
                  ? "Live"
                  : auction?.status === "ended"
                  ? "Đã kết thúc"
                  : "Sắp tới"
              }}
            </span>
          </div>
          <div class="info-content">
            <h2>{{ auction?.prizeName || "Đang tải..." }}</h2>
            <p class="description">{{ auction?.description || "Không có mô tả" }}</p>
            <div class="info-grid">
              <p>
                <span>Giá khởi điểm:</span>
                {{ auction?.startingPrice?.toLocaleString() || "0" }} VNĐ
              </p>
              <p>
                <span>Giá hiện tại:</span>
                {{
                  auction?.currentPrice?.toLocaleString() ||
                  auction?.startingPrice?.toLocaleString() ||
                  "0"
                }}
                VNĐ
              </p>
              <p>
                <span>Thời gian bắt đầu:</span>
                {{
                  auction?.startDate
                    ? new Date(auction.startDate).toLocaleString("vi-VN")
                    : "Chưa xác định"
                }}
              </p>
              <p v-if="auction?.status === 'ongoing' && auction?.endDate">
                <span>Thời gian còn lại:</span>
                <CountdownTimer :end-date="auction?.endDate" />
              </p>
              <p v-if="auction?.status === 'ended'">
                <span>Người thắng:</span>
                {{ auction?.winner || "Chưa có" }}
              </p>
            </div>
            <div class="bid-section" v-if="auction?.status === 'ongoing' && isLoggedIn">
              <input
                type="number"
                v-model.number="bidAmount"
                placeholder="Nhập số tiền đấu giá"
                class="bid-input"
                :min="minBidAmount"
                :disabled="isLoading || userBalance < minBidAmount"
              />
              <button @click="placeBid" class="bid-button">Đặt giá</button>
              <p v-if="userBalance < minBidAmount" class="balance-error">
                Số dư không đủ ({{ formatCurrency(userBalance) }})
              </p>
            </div>
            <div
              v-else-if="auction?.status === 'ongoing' && !isLoggedIn"
              class="login-prompt"
            >
              <router-link :to="{ name: 'login', query: { redirect: $route.fullPath } }">
                Đăng nhập để đặt giá
              </router-link>
            </div>
          </div>
        </div>

        <!-- Khung chat -->
        <div class="chat-box">
          <h3>Chat</h3>
          <div class="chat-messages" ref="chatMessagesRef">
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
          <div class="chat-input" v-if="isLoggedIn">
            <input
              type="text"
              v-model="newMessage"
              placeholder="Nhập tin nhắn..."
              @keyup.enter="sendMessage"
              class="chat-input-field"
            />
            <button @click="sendMessage" class="send-button">Gửi</button>
          </div>
          <div v-else class="login-prompt">
            <router-link :to="{ name: 'login', query: { redirect: $route.fullPath } }">
              Đăng nhập để chat
            </router-link>
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
import { defineComponent, ref, nextTick } from "vue";
import CountdownTimer from "@/components/CountdownTimer.vue";
import auctionApi from "@/api/auction.api";
import chatApi from "@/api/chat.api";
import SignalRService from "@/services/signalr";
import { userStore } from "@/stores/auth";
import type { UserInfoModel } from "@/models/user-model";
import websiteaccountApi from "@/api/websiteaccount.api";

interface Auction {
  id: number;
  name: string;
  prizeName: string;
  description: string;
  startingPrice: number;
  currentPrice: number;
  status: "upcoming" | "ongoing" | "ended";
  winner: string | null;
  startDate: string;
  endDate: string | null;
  images: string[];
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

interface AddAuctionDetail {
  auctionId: number;
  raisePrice: string;
}

export default defineComponent({
  components: { CountdownTimer },
  props: {
    id: {
      type: String,
      required: true,
    },
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

    const authStore = userStore();

    return { chatMessages, chatMessagesRef, scrollToBottom, authStore };
  },
  data() {
    return {
      auction: null as Auction | null,
      bidAmount: null as number | null,
      newMessage: "",
      showNotification: false,
      notificationMessage: "",
      isLoading: false,
      errorMessage: null as string | null,
      balance: 0 as number,
    };
  },
  computed: {
    isLoggedIn(): boolean {
      return !!this.authStore.user;
    },
    currentUser(): UserInfoModel | null {
      return this.authStore.user;
    },
    UserId(): number {
      const userId = this.currentUser?.id;
      return userId ? userId : 0;
    },
    userBalance(): number {
      return this.balance;
    },
    minBidAmount(): number {
      return this.auction?.currentPrice || this.auction?.startingPrice || 0;
    },
    isValidBid(): boolean {
      return !!(
        this.bidAmount &&
        this.bidAmount >= this.minBidAmount &&
        this.bidAmount <= this.userBalance &&
        Number.isInteger(this.bidAmount)
      );
    },
  },
  async mounted() {
    await this.authStore.init();
    console.log("Current User:", this.currentUser);
    console.log("Is Logged In:", this.isLoggedIn);
    if (this.isLoggedIn && this.currentUser?.id) {
      try {
        const response = await websiteaccountApi.getById(this.currentUser.id);
        this.balance = response.data?.result?.data?.balance || 0;
        console.log("User Balance:", this.balance);
      } catch (error) {
        console.error("Lỗi khi lấy số dư:", error);
        this.balance = 0;
      }

      // Khởi động SignalR và tham gia nhóm
      try {
        await SignalRService.startConnection(); // Chờ kết nối SignalR hoàn tất
        console.log("SignalR connection established, setting up message handler...");
        SignalRService.onReceiveMessage(
          (userId: number, message: string, sentAt: string) => {
            console.log("Received message from SignalR:", userId, message, sentAt);
            const isMine = userId === this.UserId;
            this.chatMessages.push({
              sender: isMine
                ? this.currentUser?.fullName || "Bạn"
                : `Người dùng ${userId}`,
              text: message,
              time: new Date(sentAt).toLocaleTimeString("vi-VN", {
                hour: "2-digit",
                minute: "2-digit",
              }),
              isMine,
            });
            this.scrollToBottom();
          }
        );

        const auctionId = parseInt(this.id);
        await SignalRService.joinGroup(auctionId); // Gọi joinGroup sau khi kết nối thành công
      } catch (error) {
        console.error("Failed to start SignalR connection:", error);
      }

      // Tải lịch sử tin nhắn
      await this.loadChatMessages();
    } else {
      console.log("Người dùng chưa đăng nhập hoặc không có ID");
      this.balance = 0;
    }
    console.log("User ID:", this.UserId);
    await this.fetchAuction();
  },
  beforeUnmount() {
    const auctionId = parseInt(this.id);
    SignalRService.leaveGroup(auctionId);
    SignalRService.stopConnection();
  },
  methods: {
    async fetchAuction() {
      const auctionId = parseInt(this.id);
      if (isNaN(auctionId)) {
        this.errorMessage = "ID phiên đấu giá không hợp lệ";
        return;
      }

      this.isLoading = true;
      this.errorMessage = null;
      try {
        const [auctionResponse, prizeResponse] = await Promise.all([
          auctionApi.GetAuctionById(auctionId),
          auctionApi.GetAuctionPrizeById(auctionId),
        ]);

        if (
          auctionResponse.data.result.isSuccess &&
          prizeResponse.data.result.isSuccess
        ) {
          const auctionData: BackendAuction = auctionResponse.data.result.data;
          const prizeData: AuctionPrize = prizeResponse.data.result.data;

          const imageUrls = prizeData.image
            ? prizeData.image
                .split(";")
                .map((url) => url.trim())
                .filter((url) => url.length > 0)
            : ["https://via.placeholder.com/300x200?text=Auction"];

          this.auction = {
            id: auctionData.id,
            name: auctionData.auctionName,
            prizeName: prizeData.prizeName,
            description: prizeData.description,
            startingPrice: parseInt(auctionData.startPrice) || 0,
            currentPrice:
              parseInt(auctionData.currentPrice) || parseInt(auctionData.startPrice) || 0,
            status: this.getAuctionStatus(auctionData),
            winner: auctionData.winnerId ? `Người dùng ${auctionData.winnerId}` : null,
            startDate: auctionData.startDateTime,
            endDate: auctionData.endDateTime || null,
            images: imageUrls,
          };
        } else {
          this.errorMessage =
            auctionResponse.data.result.message ||
            prizeResponse.data.result.message ||
            "Không thể lấy thông tin phiên đấu giá";
        }
      } catch (error) {
        this.errorMessage = "Lỗi kết nối đến server";
        console.error("Error fetching auction:", error);
      } finally {
        this.isLoading = false;
      }
    },
    getAuctionStatus(auction: BackendAuction): "upcoming" | "ongoing" | "ended" {
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
        return "upcoming";
      }
      if (
        startDate <= now &&
        auction.isApproved &&
        !auction.endStatus &&
        (!endDate || endDate > now)
      ) {
        return "ongoing";
      }
      return "ended";
    },
    async placeBid() {
      console.log("bidAmount:", this.bidAmount);
      console.log("userBalance:", this.userBalance);
      console.log("minBidAmount:", this.minBidAmount);
      console.log("UserId:", this.UserId);

      if (!this.isLoggedIn || !this.currentUser || this.UserId === 0) {
        this.$router.push({ name: "login", query: { redirect: this.$route.fullPath } });
        return;
      }

      if (!this.auction) {
        this.showNotification = true;
        this.notificationMessage = "Không tìm thấy phiên đấu giá.";
        setTimeout(() => (this.showNotification = false), 3000);
        return;
      }

      if (!this.isValidBid) {
        this.showNotification = true;
        this.notificationMessage =
          this.bidAmount && this.bidAmount < this.minBidAmount
            ? `Giá đặt phải lớn hơn ${this.formatCurrency(
                this.minBidAmount - 100
              )} ít nhất 100,000 VNĐ`
            : this.bidAmount && this.bidAmount > this.userBalance
            ? "Số dư không đủ để đặt giá"
            : this.bidAmount && !Number.isInteger(this.bidAmount)
            ? "Giá đặt phải là số nguyên."
            : "Vui lòng nhập số tiền hợp lệ";
        setTimeout(() => (this.showNotification = false), 3000);
        return;
      }

      const auctionId = this.auction.id;
      const raisePrice = this.bidAmount;

      if (!Number.isInteger(auctionId) || !Number.isInteger(raisePrice)) {
        this.showNotification = true;
        this.notificationMessage = "Dữ liệu không hợp lệ. Vui lòng thử lại.";
        setTimeout(() => (this.showNotification = false), 3000);
        return;
      }

      if (!raisePrice || raisePrice.toString().trim() === "") {
        this.showNotification = true;
        this.notificationMessage = "Giá đấu không được để trống.";
        setTimeout(() => (this.showNotification = false), 3000);
        return;
      }

      this.isLoading = true;
      try {
        const payload = {
          AuctionId: auctionId,
          RaisePrice: raisePrice.toString(),
        };
        console.log("Sending payload:", payload);

        const response = await auctionApi.AddAuctionDetail(payload);
        console.log("Response:", response.data);

        if (response.data.result.isSuccess) {
          this.auction.currentPrice = this.bidAmount!;
          this.showNotification = true;
          this.notificationMessage = `Đặt giá ${this.formatCurrency(
            this.bidAmount!
          )} thành công!`;
          this.bidAmount = null;

          try {
            const balanceResponse = await websiteaccountApi.getById(this.currentUser.id);
            this.balance = balanceResponse.data?.result?.data?.balance || 0;
          } catch (error) {
            console.error("Lỗi khi cập nhật số dư:", error);
            this.balance = 0;
          }
        } else {
          this.showNotification = true;
          this.notificationMessage = response.data.result.message || "Không thể đặt giá";
          console.log("Error message from BE:", response.data.result.message);
        }
      } catch (error) {
        this.showNotification = true;
        if ((error as { name?: string }).name === "AbortError") {
          this.notificationMessage =
            "Yêu cầu hết thời gian. Vui lòng kiểm tra kết nối và thử lại.";
        } else {
          this.notificationMessage = "Lỗi khi đặt giá. Vui lòng thử lại.";
        }
        console.error("Lỗi khi đặt giá:", error);
      } finally {
        this.isLoading = false;
        setTimeout(() => (this.showNotification = false), 3000);
      }
    },
    async loadChatMessages() {
      const auctionId = parseInt(this.id);
      try {
        const response = await chatApi.getChatMessages(auctionId);
        console.log("GetChatMessages response:", response.data);
        if (response.data.result.isSuccess && response.data.result.data) {
          this.chatMessages = response.data.result.data.map((msg: any) => ({
            sender:
              msg.userId === this.UserId
                ? this.currentUser?.fullName || "Bạn"
                : `Người dùng ${msg.userId}`,
            text: msg.messageText,
            time: new Date(msg.sentAt).toLocaleTimeString("vi-VN", {
              hour: "2-digit",
              minute: "2-digit",
            }),
            isMine: msg.userId === this.UserId,
          }));
          console.log("Loaded chat messages:", this.chatMessages); // Debug
          this.scrollToBottom();
        } else {
          console.error("Failed to load chat messages: No result data");
        }
      } catch (error) {
        console.error("Error loading chat messages:", error);
      }
    },
    async sendMessage() {
      if (!this.isLoggedIn) {
        this.$router.push({ name: "login", query: { redirect: this.$route.fullPath } });
        return;
      }

      if (!this.newMessage.trim()) return;

      const auctionId = parseInt(this.id);
      try {
        const payload = {
          auctionId: auctionId,
          userId: this.UserId,
          message: this.newMessage,
        };
        const response = await chatApi.sendChatMessage(payload);
        if (response.data.result) {
          this.newMessage = "";
        } else {
          this.showNotification = true;
          this.notificationMessage = response.data.message || "Không thể gửi tin nhắn";
          setTimeout(() => (this.showNotification = false), 3000);
        }
      } catch (error) {
        this.showNotification = true;
        this.notificationMessage = "Lỗi khi gửi tin nhắn. Vui lòng thử lại.";
        setTimeout(() => (this.showNotification = false), 3000);
        console.error("Error sending message:", error);
      }
    },
    formatCurrency(amount: number | undefined): string {
      return amount != null
        ? amount.toLocaleString("vi-VN", { style: "currency", currency: "VND" })
        : "0 VNĐ";
    },
    handleImageError(event: Event) {
      const img = event.target as HTMLImageElement;
      img.src = "https://via.placeholder.com/300x200?text=Auction";
    },
  },
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
.image-carousel {
  display: flex;
  overflow-x: auto;
  scroll-snap-type: x mandatory;
  gap: 1rem;
  padding-bottom: 0.5rem;
}

.image-carousel img {
  flex: 0 0 auto;
  width: 300px;
  height: 200px;
  object-fit: cover;
  scroll-snap-align: start;
  border-radius: 8px;
}

.image-carousel::-webkit-scrollbar {
  height: 8px;
}

.image-carousel::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 4px;
}

.image-carousel::-webkit-scrollbar-thumb:hover {
  background: #666;
}
</style>

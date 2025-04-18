<template>
  <div class="auction-container">
    <div class="auction-list">
      <h1>Danh Sách Phiên Đấu Giá</h1>
      <div class="tabs">
        <button
          class="tab-button"
          @click="selectedTab = 'upcoming'"
          :class="{ active: selectedTab === 'upcoming' }"
        >
          Sắp tới
        </button>
        <button
          class="tab-button"
          @click="selectedTab = 'ongoing'"
          :class="{ active: selectedTab === 'ongoing' }"
        >
          Đang diễn ra
        </button>
        <button
          class="tab-button"
          @click="selectedTab = 'ended'"
          :class="{ active: selectedTab === 'ended' }"
        >
          Kết thúc
        </button>
      </div>
      <div class="content-wrapper">
        <aside class="sidebar-left">
          <div class="search-box">
            <input
              type="text"
              v-model="searchKeyword"
              placeholder="Tìm kiếm phiên đấu giá..."
              class="search-input"
            />
            <i class="fas fa-search search-icon"></i>
          </div>
          <div class="filter-box">
            <h3>Bộ lọc</h3>
            <div v-for="category in categories" :key="category">
              <label>
                <input type="checkbox" v-model="selectedCategories" :value="category" /> {{ category }}
              </label>
            </div>
          </div>
        </aside>

        <div class="auction-cards">
          <div v-if="isLoading" class="loading-spinner">
            Đang tải...
          </div>
          <div v-else-if="errorMessage" class="error-message">
            {{ errorMessage }}
          </div>
          <div v-else-if="filteredAuctions.length === 0" class="no-data">
            Không có phiên đấu giá nào.
          </div>
          <div v-else v-for="auction in filteredAuctions" :key="auction.id" class="auction-card">
            <div class="auction-image-wrapper">
              <img :src="auction.image" alt="Hình ảnh sản phẩm" class="auction-image" />
              <div class="image-overlay"></div>
              <span class="status-badge" :class="auction.status">
                {{
                  auction.status === 'upcoming'
                    ? 'Sắp tới'
                    : auction.status === 'ongoing'
                    ? 'Live'
                    : 'Đã xong'
                }}
              </span>
            </div>
            <div class="auction-content">
              <div class="auction-header">
                <h2>{{ auction.name }}</h2>
                <span class="auction-id">#{{ auction.id }}</span>
              </div>
              <div class="info-grid">
                <p>
                  <span>Vật phẩm:</span> {{ auction.item }}
                </p>
                <p>
                  <span>Giá hiện tại:</span>
                  {{ (auction.currentPrice || auction.startingPrice).toLocaleString() }} VNĐ
                </p>
                <!-- <p v-if="auction.status === 'ongoing'" class="timer">
                  <span>Còn lại:</span>
                  <CountdownTimer :end-date="auction.endDate" />
                </p> -->
                <p v-if="auction.status === 'ended'">
                  <span>Người thắng:</span> {{ auction.winner || 'Chưa có' }}
                </p>
              </div>
              <button
                v-if="auction.status !== 'ended' && auction.status !== 'upcoming'"
                class="join-button"
                @click="joinAuction(auction.id)"
              >
                Tham gia
              </button>
            </div>
          </div>
        </div>

        <aside class="sidebar-right">
          <div class="top-bidders">
            <h3>Top 10 Người Đấu Giá</h3>
            <ol>
              <li
                v-for="(bidder, index) in topBidders"
                :key="index"
                :class="`rank-${index + 1}`"
              >
                <span class="bidder-name">{{ bidder.name }}</span>
                <span class="bidder-amount">{{ bidder.amount.toLocaleString() }} VNĐ</span>
              </li>
            </ol>
          </div>
        </aside>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import CountdownTimer from '@/components/CountdownTimer.vue';
import auction from '@/api/auction.api';
import gameinforApi from '@/api/gameinfor.api';

interface GameInfoResponse {
  data: {
    result: {
      data: GameInfo[];
      isSuccess: boolean;
      message?: string | null;
    };
  };
}

interface Auction {
  id: number;
  name: string;
  item: string;
  startingPrice: number;
  currentPrice: number | null;
  status: 'upcoming' | 'ongoing' | 'ended';
  winner: string | null;
  startDate: string;
  endDate: string | null;
  image: string;
  priority: boolean;
  category: string;
  gameInforId?: number;
}

interface TopBidder {
  name: string;
  amount: number;
}

interface BackendAuction {
  id: number;
  auctionName: string;
  startPrice: string;
  currentPrice: string | null;
  startDateTime: string;
  timeToEnd: string;
  endDateTime: string | null;
  status: string;
  endStatus: boolean;
  isApproved: boolean;
  winnerId: number;
  userId: number;
  createdDate: string;
  updatedDate: string;
  deleteDate: string | null;
  gameInforsId: number | null;
  auctionPrizeId: number;
  auctionPrizeImage: string;
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

interface GameInfo {
  id: number;
  gameName: string;
}

// Định nghĩa kiểu cho response từ API
interface AuctionPrizeResponse {
  data: {
    result: {
      data: AuctionPrize[];
      isSuccess: boolean;
      message?: string | null;
    };
  };
}

export default defineComponent({
  components: {
    CountdownTimer,
  },
  data() {
    return {
      selectedTab: 'upcoming' as 'upcoming' | 'ongoing' | 'ended',
      searchKeyword: '' as string,
      selectedCategories: [] as string[],
      categories: [] as string[],
      auctions: [] as Auction[],
      gameInfos: [] as GameInfo[],
      topBidders: [
        { name: 'Người dùng A', amount: 5000000 },
        { name: 'Người dùng B', amount: 4500000 },
        { name: 'Người dùng C', amount: 4000000 },
      ] as TopBidder[],
      isLoading: false as boolean,
      errorMessage: null as string | null,
    };
  },
  computed: {
    filteredAuctions(): Auction[] {
      let filtered = [...this.auctions];

      filtered = filtered.filter(
        (auction: Auction) => auction.status === this.selectedTab
      );

      if (this.searchKeyword.trim()) {
        filtered = filtered.filter((auction: Auction) =>
          auction.name.toLowerCase().includes(this.searchKeyword.toLowerCase().trim())
        );
      }

      if (this.selectedCategories.length > 0) {
        filtered = filtered.filter((auction: Auction) =>
          auction.category && this.selectedCategories.includes(auction.category)
        );
      }

      return filtered;
    },
  },
  methods: {
    joinAuction(id: number): void {
      if (isNaN(id) || typeof id !== 'number') {
        console.error('Invalid ID:', id);
        return;
      }
      console.log('Joining auction with ID:', id);
      this.$router.push({ name: 'detail-auction', params: { id: id.toString() } });
    },
    async fetchGameInfor(): Promise<void> {
      try {
        const response = await gameinforApi.getAll() as GameInfoResponse;
        console.log('Game Info:', response);
        if (response.data.result.isSuccess) {
          this.gameInfos = response.data.result.data ?? [];
          this.categories = this.gameInfos.map((game: GameInfo) => game.gameName);
        } else {
          this.errorMessage = 'Không thể lấy danh sách trò chơi';
        }
      } catch (error) {
        this.errorMessage = 'Lỗi kết nối đến server';
        console.error('Error fetching game info:', error);
      }
    },
    async fetchAuctions(): Promise<void> {
      this.isLoading = true;
      this.errorMessage = null;
      try {
        const response = await auction.getAllAuction();
        const Image = await auction.getAllAuctionPrize() as AuctionPrizeResponse;
        console.log('Response:', response);
        console.log('Image:', Image);

        if (response.data.result.isSuccess) {
          // Extract the list of prizes from Image.data.result.data
          const prizes = Image.data.result.isSuccess && Image.data.result.data ? Image.data.result.data : [];
          console.log('Prizes:', prizes);

          if (!Image.data.result.isSuccess) {
            console.error('Failed to fetch auction prizes:', Image.data.result.message);
          }

          this.auctions = response.data.result.data.map((auction: BackendAuction) => {
            const gameInfo = this.gameInfos.find(
              (game: GameInfo) => game.id === auction.gameInforsId
            );
            const category = gameInfo ? gameInfo.gameName : 'Không xác định';

            // Find the prize corresponding to this auction
            const auctionPrize = prizes.find((prize: AuctionPrize) => prize.auctionId === auction.id);

            // Get the image URL, prepend base URL, and split before semicolon if needed
            const baseUrl = 'https://your-backend.com/'; // Replace with your actual backend URL
            const imageUrl = auctionPrize && auctionPrize.image
              ? auctionPrize.image.split(';')[0]
              : 'https://via.placeholder.com/300x200?text=Auction';
            console.log('Image URL for auction', auction.id, ':', imageUrl);

            return {
              id: auction.id,
              name: auction.auctionName,
              item: auction.auctionName,
              startingPrice: isNaN(parseInt(auction.startPrice)) ? 0 : parseInt(auction.startPrice),
              currentPrice: auction.currentPrice ? parseInt(auction.currentPrice) : null,
              status: this.getAuctionStatus(auction),
              winner: auction.winnerId ? `Người dùng ${auction.winnerId}` : null,
              startDate: auction.startDateTime,
              endDate: auction.endDateTime || null,
              image: imageUrl,
              priority: false,
              category: category,
              gameInforId: auction.gameInforsId,
            };
          });
        } else {
          this.errorMessage = response.data.result.message || 'Không thể lấy danh sách phiên đấu giá';
        }
      } catch (error) {
        this.errorMessage = 'Lỗi kết nối đến server';
        console.error('Error fetching auctions:', error);
      } finally {
        this.isLoading = false;
      }
    },
    getAuctionStatus(auction: BackendAuction): 'upcoming' | 'ongoing' | 'ended' {
      const now = new Date();
      const startDate = new Date(auction.startDateTime);
      const endDate = auction.endDateTime ? new Date(auction.endDateTime) : null;

      // Kiểm tra trạng thái "Sắp diễn ra"
      if (
        startDate > now &&
        auction.isApproved &&
        auction.winnerId === 0 &&
        !auction.endStatus &&
        !endDate
      ) {
        return 'upcoming';
      }

      // Kiểm tra trạng thái "Đang diễn ra"
      if (
        startDate <= now &&
        auction.isApproved &&
        !auction.endStatus &&
        !endDate
      ) {
        return 'ongoing';
      }

      // Kiểm tra trạng thái "Kết thúc"
      if (
        auction.isApproved &&
        auction.winnerId !== 0 &&
        auction.endStatus &&
        endDate
      ) {
        return 'ended';
      }

      // Mặc định trả về 'ended' nếu không khớp với các điều kiện trên
      return 'ended';
    },
  },
  async mounted() {
    await this.fetchGameInfor();
    await this.fetchAuctions();
    
  },
});
</script>
<style scoped>
/* Container tổng thể */
.auction-container {
  background: linear-gradient(135deg, #1a1a33 0%, #0d0d1a 100%);
  min-height: 100vh;
  padding: 40px 20px;
  display: flex;
  justify-content: center;
  font-family: 'Poppins', sans-serif;
}

/* Auction List */
.auction-list {
  max-width: 1400px;
  width: 100%;
  background: rgba(30, 30, 50, 0.95);
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(0, 255, 204, 0.2);
  padding: 30px;
}

/* Tiêu đề chính */
h1 {
  text-align: center;
  font-size: 2.8rem;
  font-weight: 800;
  background: linear-gradient(90deg, #00ffcc, #ff00cc);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  text-shadow: 0 0 20px rgba(0, 255, 204, 0.5);
  margin-bottom: 40px;
  letter-spacing: 1px;
}

/* Tabs */
.tabs {
  display: flex;
  justify-content: center;
  gap: 15px;
  margin-bottom: 30px;
  flex-wrap: wrap;
}

.tab-button {
  padding: 12px 30px;
  background: rgba(255, 255, 255, 0.05);
  color: #ffffff;
  border: 2px solid rgba(0, 255, 204, 0.3);
  border-radius: 25px;
  font-size: 1rem;
  font-weight: 600;
  text-transform: uppercase;
  cursor: pointer;
  transition: all 0.3s ease;
}

.tab-button:hover {
  background: rgba(0, 255, 204, 0.2);
  box-shadow: 0 0 15px rgba(0, 255, 204, 0.5);
  transform: translateY(-2px);
}

.tab-button.active {
  background: linear-gradient(90deg, #00ffcc, #ff00cc);
  border-color: transparent;
  box-shadow: 0 0 20px rgba(0, 255, 204, 0.7);
}

/* Content Wrapper */
.content-wrapper {
  display: flex;
  gap: 25px;
}

/* Sidebar trái */
.sidebar-left {
  flex: 0 0 250px;
  background: rgba(20, 20, 40, 0.8);
  border-radius: 15px;
  padding: 20px;
  color: #fff;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
}

.search-box {
  position: relative;
  margin-bottom: 25px;
}

.search-input {
  width: 100%;
  padding: 12px 40px 12px 15px;
  background: rgba(255, 255, 255, 0.1);
  border: 2px solid rgba(0, 255, 204, 0.3);
  border-radius: 25px;
  color: #fff;
  font-size: 1rem;
  transition: all 0.3s ease;
}

.search-input:focus {
  outline: none;
  box-shadow: 0 0 15px rgba(0, 255, 204, 0.5);
  border-color: #00ffcc;
}

.search-icon {
  position: absolute;
  top: 50%;
  right: 15px;
  transform: translateY(-50%);
  color: #00ffcc;
  font-size: 1.3rem;
}

.filter-box h3 {
  margin-bottom: 15px;
  font-size: 1.3rem;
  color: #00ffcc;
  text-shadow: 0 0 5px rgba(0, 255, 204, 0.4);
}

.filter-box label {
  display: flex;
  align-items: center;
  margin-bottom: 12px;
  font-size: 0.95rem;
  cursor: pointer;
  transition: color 0.3s ease;
}

.filter-box label:hover {
  color: #00ffcc;
}

.filter-box input[type="checkbox"] {
  margin-right: 10px;
  accent-color: #00ffcc;
  cursor: pointer;
  width: 18px;
  height: 18px;
}

/* Auction Cards */
.auction-cards {
  flex: 1;
  display: grid;
  grid-template-columns: repeat(3, 1fr); /* Cố định 3 cột */
  gap: 25px;
  padding: 10px;
}

.auction-card {
  background: rgba(30, 30, 50, 0.95);
  border-radius: 15px;
  overflow: hidden;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  border: 1px solid rgba(0, 255, 204, 0.1);
}

.auction-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0, 255, 204, 0.3);
}

.auction-image-wrapper {
  position: relative;
  height: 180px;
}

.auction-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.auction-card:hover .auction-image {
  transform: scale(1.05);
}

.image-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(180deg, rgba(0, 0, 0, 0.1), rgba(0, 0, 0, 0.6));
  pointer-events: none;
}

.status-badge {
  position: absolute;
  top: 15px;
  right: 15px;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 700;
  text-transform: uppercase;
  backdrop-filter: blur(5px);
}

.status-badge.upcoming {
  background: rgba(255, 204, 0, 0.9);
  color: #1a1a33;
}

.status-badge.ongoing {
  background: rgba(0, 255, 204, 0.9);
  color: #1a1a33;
}

.status-badge.ended {
  background: rgba(255, 0, 204, 0.9);
  color: #ffffff;
}

.auction-content {
  padding: 20px;
}

.auction-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

h2 {
  font-size: 1.4rem;
  font-weight: 700;
  color: #ffffff;
  text-shadow: 0 0 10px rgba(0, 255, 204, 0.4);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 70%;
}

.auction-id {
  font-size: 1rem;
  font-weight: 600;
  color: #ff00cc;
  text-shadow: 0 0 5px rgba(255, 0, 204, 0.4);
}

.info-grid {
  display: grid;
  gap: 10px;
  margin-bottom: 15px;
}

.info-grid p {
  font-size: 0.95rem;
  color: #d0d0d0;
}

.info-grid span {
  font-weight: 600;
  color: #00ffcc;
  text-shadow: 0 0 5px rgba(0, 255, 204, 0.4);
}

.join-button {
  width: 100%;
  padding: 12px;
  background: linear-gradient(90deg, #00ffcc, #ff00cc);
  color: #ffffff;
  border: none;
  border-radius: 25px;
  font-size: 1rem;
  font-weight: 600;
  text-transform: uppercase;
  cursor: pointer;
  transition: all 0.3s ease;
}

.join-button:hover {
  background: linear-gradient(90deg, #ff00cc, #00ffcc);
  box-shadow: 0 0 20px rgba(0, 255, 204, 0.7);
  transform: translateY(-2px);
}

/* Sidebar phải: Top 10 người đấu giá */
.sidebar-right {
  flex: 0 0 250px;
  background: rgba(20, 20, 40, 0.8);
  border-radius: 15px;
  padding: 20px;
  color: #fff;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
}

.top-bidders h3 {
  font-size: 1.3rem;
  margin-bottom: 20px;
  color: #00ffcc;
  text-align: center;
  text-shadow: 0 0 5px rgba(0, 255, 204, 0.4);
}

.top-bidders ol {
  list-style: none;
  padding: 0;
  counter-reset: item;
}

.top-bidders li {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  margin-bottom: 10px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  transition: all 0.3s ease;
}

.top-bidders li:hover {
  transform: translateX(5px);
  box-shadow: 0 0 10px rgba(0, 255, 204, 0.5);
}

.top-bidders li::before {
  content: "";
  display: inline-block;
  width: 20px;
  height: 20px;
  margin-right: 10px;
  background-size: contain;
  background-repeat: no-repeat;
}

/* Icon cho Top 3 */
.top-bidders .rank-1 {
  background: linear-gradient(45deg, #ffd700, #ffaa00);
}
.top-bidders .rank-1::before {
  background-image: url('https://cdn-icons-png.flaticon.com/512/2589/2589175.png');
}

.top-bidders .rank-2 {
  background: linear-gradient(45deg, #c0c0c0, #a9a9a9);
}
.top-bidders .rank-2::before {
  background-image: url('https://cdn-icons-png.flaticon.com/512/2589/2589197.png');
}

.top-bidders .rank-3 {
  background: linear-gradient(45deg, #cd7f32, #b87333);
}
.top-bidders .rank-3::before {
  background-image: url('https://cdn-icons-png.flaticon.com/512/2589/2589189.png');
}

.top-bidders .rank-4::before,
.top-bidders .rank-5::before,
.top-bidders .rank-6::before,
.top-bidders .rank-7::before,
.top-bidders .rank-8::before,
.top-bidders .rank-9::before,
.top-bidders .rank-10::before {
  content: counter(item);
  counter-increment: item 3;
  font-size: 0.9rem;
  font-weight: bold;
  color: #00ffcc;
  background: none;
  width: auto;
  height: auto;
}

.top-bidders .bidder-name {
  flex: 1;
  font-size: 0.95rem;
}

.top-bidders .bidder-amount {
  font-weight: 600;
  color: #ff00cc;
}

/* Responsive */
@media (max-width: 1200px) {
  .auction-cards {
    grid-template-columns: repeat(2, 1fr); /* Khi màn hình nhỏ hơn 1200px, giảm còn 2 cột */
  }
}

@media (max-width: 1024px) {
  .content-wrapper {
    flex-direction: column;
  }
  .sidebar-left,
  .sidebar-right {
    flex: none;
    width: 100%;
    margin-bottom: 25px;
  }
  .auction-cards {
    grid-template-columns: repeat(2, 1fr); /* Giữ 2 cột trên màn hình nhỏ */
  }
}

@media (max-width: 768px) {
  .auction-cards {
    grid-template-columns: 1fr; /* Màn hình rất nhỏ thì chỉ còn 1 cột */
  }
}
</style>
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
  
        <!-- Wrapper chính: Sidebar trái + Auction Cards + Sidebar phải -->
        <div class="content-wrapper">
          <!-- Sidebar trái: Thanh tìm kiếm + Bộ lọc -->
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
              <label>
                <input type="checkbox" v-model="filterPriority" /> Ưu tiên hàng đầu
              </label>
              <label>
                <input type="checkbox" v-model="filterCategoryA" /> Danh mục A
              </label>
              <label>
                <input type="checkbox" v-model="filterCategoryB" /> Danh mục B
              </label>
            </div>
          </aside>
  
          <!-- Auction Cards -->
          <div class="auction-cards">
            <div 
              v-for="auction in filteredAuctions" 
              :key="auction.id" 
              class="auction-card"
            >
              <div class="auction-image-wrapper">
                <img 
                  :src="auction.image" 
                  alt="Hình ảnh sản phẩm" 
                  class="auction-image" 
                />
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
                <h2>{{ auction.name }}</h2>
                <div class="info-grid">
                  <p>
                    <span>Vật phẩm:</span> {{ auction.item }}
                  </p>
                  <p>
                    <span>Giá hiện tại:</span> 
                    {{ (auction.currentPrice || auction.startingPrice).toLocaleString() }} VNĐ
                  </p>
                  <p v-if="auction.status === 'ongoing'" class="timer">
                    <span>Còn lại:</span>
                    <CountdownTimer :end-date="auction.endDate" />
                  </p>
                  <p v-if="auction.status === 'ended'">
                    <span>Người thắng:</span> {{ auction.winner }}
                  </p>
                </div>
                <button 
                  v-if="auction.status !== 'ended'" 
                  class="join-button" 
                  @click="joinAuction(auction.id)"
                >
                  Tham gia
                </button>
              </div>
            </div>
          </div>
  
          <!-- Sidebar phải: Top 10 người đấu giá -->
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
  
  <script>
  import CountdownTimer from '@/components/CountdownTimer.vue';
  
  export default {
    components: {
      CountdownTimer,
    },
    data() {
      return {
        selectedTab: 'upcoming',
        searchKeyword: '',
        filterPriority: false,
        filterCategoryA: false,
        filterCategoryB: false,
        auctions: [
          {
            id: 1,
            name: 'Phiên đấu giá 1',
            item: 'Vật phẩm A',
            startingPrice: 1000000,
            currentPrice: 1500000,
            status: 'ongoing',
            winner: null,
            startDate: '2023-10-01T10:00:00',
            endDate: '2023-10-10T10:00:00',
            image: 'https://via.placeholder.com/300x200?text=Vật+phẩm+A',
            priority: true,
            category: 'A',
          },
          {
            id: 2,
            name: 'Phiên đấu giá 2',
            item: 'Vật phẩm B',
            startingPrice: 2000000,
            currentPrice: null,
            status: 'upcoming',
            winner: null,
            startDate: '2023-10-15T10:00:00',
            endDate: null,
            image: 'https://via.placeholder.com/300x200?text=Vật+phẩm+B',
            priority: false,
            category: 'B',
          },
          {
            id: 3,
            name: 'Phiên đấu giá 3',
            item: 'Vật phẩm C',
            startingPrice: 3000000,
            currentPrice: 3500000,
            status: 'ended',
            winner: 'Người dùng 123',
            startDate: '2023-09-01T10:00:00',
            endDate: '2023-09-10T10:00:00',
            image: 'https://via.placeholder.com/300x200?text=Vật+phẩm+C',
            priority: false,
            category: 'A',
          },
        ],
        topBidders: [
          { name: 'Người dùng A', amount: 5000000 },
          { name: 'Người dùng B', amount: 4500000 },
          { name: 'Người dùng C', amount: 4000000 },
        ],
      };
    },
    computed: {
      filteredAuctions() {
        let filtered = this.auctions.filter(
          (auction) => auction.status === this.selectedTab
        );
  
        if (this.searchKeyword) {
          filtered = filtered.filter((auction) =>
            auction.name.toLowerCase().includes(this.searchKeyword.toLowerCase())
          );
        }
  
        if (this.filterPriority) {
          filtered = filtered.filter((auction) => auction.priority);
        }
        if (this.filterCategoryA) {
          filtered = filtered.filter((auction) => auction.category === 'A');
        }
        if (this.filterCategoryB) {
          filtered = filtered.filter((auction) => auction.category === 'B');
        }
  
        return filtered;
      },
    },
    methods: {
      joinAuction(id) {
        alert(`Tham gia phiên đấu giá ID: ${id}`);
      },
    },
  };
  </script>
  
  <style scoped>
  /* Container tổng thể */
  .auction-container {
    background: linear-gradient(180deg, #0d0d1a 0%, #1a1a33 100%);
    min-height: 100vh;
    padding: 40px 20px;
    display: flex;
    justify-content: center;
  }
  
  .auction-list {
    max-width: 1400px;
    width: 100%;
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
  
  /* Tabs */
  .tabs {
    display: flex;
    justify-content: center;
    gap: 15px;
    margin-bottom: 30px;
    flex-wrap: wrap;
  }
  
  .tab-button {
    padding: 10px 25px;
    background: rgba(255, 255, 255, 0.05);
    color: #ffffff;
    border: 1px solid rgba(0, 255, 204, 0.3);
    border-radius: 20px;
    font-size: 1rem;
    font-weight: 600;
    text-transform: uppercase;
    cursor: pointer;
    transition: all 0.3s ease;
  }
  
  .tab-button:hover {
    background: rgba(0, 255, 204, 0.15);
    box-shadow: 0 0 15px rgba(0, 255, 204, 0.4);
  }
  
  .tab-button.active {
    background: linear-gradient(90deg, #00ffcc, #ff00cc);
    box-shadow: 0 0 20px rgba(0, 255, 204, 0.6);
  }
  
  /* Content Wrapper */
  .content-wrapper {
    display: flex;
    gap: 20px;
  }
  
  /* Sidebar trái */
  .sidebar-left {
    flex: 0 0 250px;
    background: rgba(0, 0, 0, 0.2);
    border-radius: 15px;
    padding: 15px;
    color: #fff;
  }
  
  .search-box {
    position: relative;
    margin-bottom: 20px;
  }
  
  .search-input {
    width: 100%;
    padding: 10px 35px 10px 15px;
    background: rgba(255, 255, 255, 0.1);
    border: 1px solid rgba(0, 255, 204, 0.3);
    border-radius: 20px;
    color: #fff;
    font-size: 1rem;
    transition: all 0.3s ease;
  }
  
  .search-input:focus {
    outline: none;
    box-shadow: 0 0 15px rgba(0, 255, 204, 0.4);
    border-color: #00ffcc;
  }
  
  .search-icon {
    position: absolute;
    top: 50%;
    right: 10px;
    transform: translateY(-50%);
    color: #00ffcc;
    font-size: 1.2rem;
  }
  
  .filter-box h3 {
    margin-bottom: 10px;
    font-size: 1.2rem;
  }
  
  .filter-box label {
    display: flex;
    align-items: center;
    margin-bottom: 10px;
    font-size: 0.9rem;
    cursor: pointer;
    transition: color 0.3s ease;
  }
  
  .filter-box label:hover {
    color: #00ffcc;
  }
  
  .filter-box input[type="checkbox"] {
    margin-right: 8px;
    accent-color: #00ffcc;
    cursor: pointer;
  }
  
  /* Auction Cards */
  .auction-cards {
    flex: 1;
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 20px;
    padding: 10px;
  }
  
  .auction-card {
    background: rgba(20, 20, 40, 0.95);
    border-radius: 15px;
    overflow: hidden;
    transition: transform 0.3s ease, box-shadow 0.3s ease;
  }
  
  .auction-card:hover {
    transform: scale(1.02);
    box-shadow: 0 0 20px rgba(0, 255, 204, 0.5);
  }
  
  .auction-image-wrapper {
    position: relative;
    height: 150px;
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
  
  .auction-content {
    padding: 15px;
  }
  
  h2 {
    font-size: 1.3rem;
    font-weight: 600;
    color: #ffffff;
    text-shadow: 0 0 10px rgba(0, 255, 204, 0.4);
    margin-bottom: 10px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
  
  .info-grid {
    display: grid;
    gap: 8px;
    margin-bottom: 12px;
  }
  
  .info-grid p {
    font-size: 0.9rem;
    color: #d0d0d0;
  }
  
  .info-grid span {
    font-weight: 600;
    color: #00ffcc;
    text-shadow: 0 0 5px rgba(0, 255, 204, 0.4);
  }
  
  .join-button {
    width: 100%;
    padding: 10px;
    background: linear-gradient(90deg, #00ffcc, #ff00cc);
    color: #ffffff;
    border: none;
    border-radius: 20px;
    font-size: 0.95rem;
    font-weight: 600;
    text-transform: uppercase;
    cursor: pointer;
    transition: all 0.3s ease;
  }
  
  .join-button:hover {
    background: linear-gradient(90deg, #ff00cc, #00ffcc);
    box-shadow: 0 0 15px rgba(0, 255, 204, 0.6);
  }
  
  /* Sidebar phải: Top 10 người đấu giá */
  .sidebar-right {
    flex: 0 0 250px;
    background: rgba(0, 0, 0, 0.2);
    border-radius: 15px;
    padding: 15px;
    color: #fff;
  }
  
  .top-bidders h3 {
    font-size: 1.2rem;
    margin-bottom: 15px;
    color: #00ffcc;
    text-align: center;
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
    padding: 10px;
    margin-bottom: 8px;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 5px;
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
    background-image: url('https://cdn-icons-png.flaticon.com/512/2589/2589175.png'); /* Icon vàng */
  }
  
  .top-bidders .rank-2 {
    background: linear-gradient(45deg, #c0c0c0, #a9a9a9);
  }
  .top-bidders .rank-2::before {
    background-image: url('https://cdn-icons-png.flaticon.com/512/2589/2589197.png'); /* Icon bạc */
  }
  
  .top-bidders .rank-3 {
    background: linear-gradient(45deg, #cd7f32, #b87333);
  }
  .top-bidders .rank-3::before {
    background-image: url('https://cdn-icons-png.flaticon.com/512/2589/2589189.png'); /* Icon đồng */
  }
  
  /* Số thứ tự cho vị trí 4-10 */
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
    font-size: 0.9rem;
  }
  
  .top-bidders .bidder-amount {
    font-weight: 600;
    color: #ff00cc;
  }
  
  /* Responsive */
  @media (max-width: 1024px) {
    .content-wrapper {
      flex-direction: column;
    }
    .sidebar-left,
    .sidebar-right {
      flex: none;
      width: 100%;
      margin-bottom: 20px;
    }
    .auction-cards {
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    }
  }
  </style>
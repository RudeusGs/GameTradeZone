<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import gameApi from '@/api/gameinfor.api';
import gameAccountApi from '@/api/gameaccount.api';
import gamefieldApi from '@/api/gamefield.api';
import { userStore } from '@/stores/auth';

interface Game {
  id: number;
  name: string;
  image: string;
}

interface Account {
  id: number;
  seller: string;
  sellerId: string | null;
  game: string;
  price: number;
  status: string;
  images: string[];
  fields: { fieldName: string; fieldValue: string }[];
  priceMin: number;
}

interface Service {
  id: number;
  creator: string;
  game: string;
  name: string;
  price: number;
  description: string;
}

interface BuyAccountGameModel {
  Id: number;
}

const router = useRouter();
const authStore = userStore();
const user = computed(() => authStore.user);

const selectedGame = ref<string>('All');
const searchQuery = ref<string>('');
const isLoading = ref<boolean>(false);
const errorMessage = ref<string>('');

const games = ref<Game[]>([]);
const accounts = ref<Account[]>([]);
const services = ref<Service[]>([]);

const showImageModal = ref(false);
const selectedAccount = ref<Account | null>(null);
const currentImageIndex = ref(0);

const showPurchaseModal = ref(false);
const purchaseStatus = ref<'success' | 'error'>('success');
const purchaseMessage = ref<string>('');

const showMoreDetails = ref<number[]>([]);

const getFullImageUrls = (imageString: string | null | undefined): string[] => {
  if (!imageString || imageString.trim() === '') {
    return ['https://via.placeholder.com/400x250'];
  }
  const baseUrl = 'https://localhost:7232/';
  const images = imageString.split(';').filter(img => img.trim() !== '');
  return images.map(img => `${baseUrl}${img}`);
};

const fetchGames = async () => {
  try {
    isLoading.value = true;
    const response = await gameApi.getAll();
    if (response.data?.result?.isSuccess && response.data.result.data) {
      games.value = response.data.result.data.map((game: any) => ({
        id: game.id,
        name: game.gameName,
        image: getFullImageUrls(game.image)[0],
      }));
    } else {
      errorMessage.value = 'Không thể lấy danh sách game';
    }
  } catch (error) {
    errorMessage.value = 'Lỗi khi gọi API game';
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

const fetchAccounts = async () => {
  try {
    isLoading.value = true;
    const response = await gameAccountApi.getAll();
    if (response.data?.result?.isSuccess && response.data.result.data) {
      const accountPromises = response.data.result.data.map(async (account: any) => {
        let sellerName = 'Không xác định';
        let sellerId: string | null = null;
        let gameName = 'Không xác định';

        try {
          const userResponse = await gameAccountApi.getInforUser(account.id);
          if (userResponse.data?.result?.isSuccess && userResponse.data.result.data) {
            sellerName = userResponse.data.result.data.userName || 'Không xác định';
            sellerId = userResponse.data.result.data.id || null;
          }
        } catch (error) {
          console.error(`Lỗi khi lấy thông tin người bán cho tài khoản ${account.id}:`, error);
        }

        try {
          const game = games.value.find(g => g.id === account.gameInforID);
          gameName = game ? game.name : 'Không xác định';
        } catch (error) {
          console.error(`Lỗi khi lấy tên game cho tài khoản ${account.id}:`, error);
        }

        let fields: { fieldName: string; fieldValue: string }[] = [];
        try {
          const fieldResponse = await gamefieldApi.getField(account.gameInforID);
          if (fieldResponse.data?.result?.isSuccess && fieldResponse.data.result.data) {
            const fieldPromises = fieldResponse.data.result.data.map(async (field: any) => {
              try {
                const valueResponse = await gameAccountApi.getByIdForGame(account.id, field.id);
                if (
                  valueResponse.data?.result?.isSuccess &&
                  valueResponse.data.result.data?.length > 0 &&
                  valueResponse.data.result.data[0].fieldValue
                ) {
                  return {
                    fieldName: field.fieldName,
                    fieldValue: valueResponse.data.result.data[0].fieldValue,
                  };
                }
                return null;
              } catch (error) {
                console.error(`Lỗi khi lấy giá trị trường ${field.id} cho tài khoản ${account.id}:`, error);
                return null;
              }
            });
            const fieldResults = await Promise.all(fieldPromises);
            fields = fieldResults.filter(result => result !== null) as { fieldName: string; fieldValue: string }[];
          }
        } catch (error) {
          console.error(`Lỗi khi lấy trường cho game ${account.gameInforID}:`, error);
        }

        return {
          id: account.id,
          seller: sellerName,
          sellerId: sellerId,
          game: gameName,
          price: account.price,
          status: account.status || 'Còn hàng',
          images: getFullImageUrls(account.image),
          fields,
          priceMin: account.priceMin || 0,
        };
      });
      accounts.value = await Promise.all(accountPromises);
    } else {
      errorMessage.value = 'Không thể lấy danh sách tài khoản';
    }
  } catch (error) {
    errorMessage.value = 'Lỗi khi gọi API tài khoản';
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

const fetchServices = () => {
  services.value = [
    { id: 1, creator: 'UserC', game: 'Valorant', name: 'Tăng hạng lên Immortal', price: 30, description: 'Tăng từ hạng hiện tại lên Immortal trong 7 ngày.' },
    { id: 2, creator: 'UserD', game: 'Genshin Impact', name: 'Lên cấp AR 50', price: 40, description: 'Lên cấp từ AR 1 đến 50, bao gồm farm tài nguyên.' },
    { id: 3, creator: 'UserE', game: 'LoL', name: 'Hỗ trợ trận hạng', price: 25, description: 'Giúp bạn thắng 5 trận hạng.' },
  ];
};

onMounted(() => {
  fetchGames().then(() => fetchAccounts());
  fetchServices();
});

const filteredAccounts = computed(() => {
  let result = accounts.value;
  if (selectedGame.value !== 'All') {
    result = result.filter(account => account.game === selectedGame.value);
  }
  if (searchQuery.value) {
    result = result.filter(account =>
      account.game.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }
  return result;
});

const filteredServices = computed(() => {
  let result = services.value;
  if (selectedGame.value !== 'All') {
    result = result.filter(service => service.game === selectedGame.value);
  }
  if (searchQuery.value) {
    result = result.filter(service =>
      service.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      service.game.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  }
  return result;
});

const isOwnAccount = (account: Account) => {
  return user.value && account.sellerId !== null && Number(account.sellerId) === user.value.id;
};

const buyAccount = async (accountId: number) => {
  if (!user.value) {
    purchaseStatus.value = 'error';
    purchaseMessage.value = 'Vui lòng đăng nhập để mua tài khoản';
    showPurchaseModal.value = true;
    return;
  }

  try {
    isLoading.value = true;
    const model: BuyAccountGameModel = { Id: accountId };
    const response = await gameAccountApi.buy(model);
    if (response.data?.result?.isSuccess) {
      await fetchAccounts();
      purchaseStatus.value = 'success';
      purchaseMessage.value = 'Mua tài khoản thành công!';
    } else {
      purchaseStatus.value = 'error';
      const errorMsg = response.data?.result?.message || 'Lỗi không xác định';
      switch (errorMsg) {
        case 'This account does not exist or has been deleted!':
          purchaseMessage.value = 'Tài khoản này không tồn tại hoặc đã bị xóa!';
          break;
        case 'This account has already been sold!':
          purchaseMessage.value = 'Tài khoản này đã được bán!';
          break;
        case 'Game information does not exist!':
          purchaseMessage.value = 'Thông tin game không tồn tại!';
          break;
        case 'Buyer information does not exist!':
          purchaseMessage.value = 'Thông tin người mua không tồn tại!';
          break;
        case 'Insufficient balance':
          purchaseMessage.value = 'Số dư không đủ';
          break;
        case 'Cannot buy your own account':
          purchaseMessage.value = 'Không thể mua tài khoản của chính bạn';
          break;
        default:
          purchaseMessage.value = `Mua thất bại: ${errorMsg}`;
      }
    }
  } catch (error) {
    purchaseStatus.value = 'error';
    purchaseMessage.value = 'Lỗi khi gọi API mua hàng';
    console.error('Lỗi mua hàng:', error);
  } finally {
    isLoading.value = false;
    showPurchaseModal.value = true;
  }
};

const closePurchaseModal = () => {
  showPurchaseModal.value = false;
  purchaseMessage.value = '';
};

const goToLogin = () => {
  router.push('/login');
};

const nextImage = () => {
  if (selectedAccount.value && currentImageIndex.value < selectedAccount.value.images.length - 1) {
    currentImageIndex.value++;
  }
};

const prevImage = () => {
  if (currentImageIndex.value > 0) {
    currentImageIndex.value--;
  }
};

const openImageModal = (account: Account) => {
  selectedAccount.value = account;
  currentImageIndex.value = 0;
  showImageModal.value = true;
};

const closeImageModal = () => {
  showImageModal.value = false;
  selectedAccount.value = null;
};

const toggleMoreDetails = (accountId: number) => {
  const index = showMoreDetails.value.indexOf(accountId);
  if (index === -1) {
    showMoreDetails.value.push(accountId);
  } else {
    showMoreDetails.value.splice(index, 1);
  }
};
</script>

<template>
  <div class="cosmo-trade-zone">
    <div class="stars-container">
      <div class="stars stars-small"></div>
      <div class="stars stars-medium"></div>
      <div class="stars stars-large"></div>
    </div>

    <div class="nebula-bg"></div>

    <div class="planet planet-1"></div>
    <div class="planet planet-2"></div>

    <section class="top-banner">
      <div class="banner-particles"></div>
      <div class="banner-hologram"></div>
      
      <div class="banner-content-wrapper">
        <div class="banner-left">
          <h1 class="banner-title">
            <span class="title-line">GAME</span>
            <span class="title-line">TRADE</span>
            <span class="title-line highlight">ZONE</span>
          </h1>
          <p class="banner-subtitle">Nền tảng giao dịch game an toàn <span class="highlight-text">số 1 Việt Nam</span></p>
          
          <div class="banner-stats">
            <div class="stat-item">
              <div class="stat-value">10,000+</div>
              <div class="stat-label">Tài khoản</div>
            </div>
            <div class="stat-item">
              <div class="stat-value">5,000+</div>
              <div class="stat-label">Người dùng</div>
            </div>
            <div class="stat-item">
              <div class="stat-value">99%</div>
              <div class="stat-label">Hài lòng</div>
            </div>
          </div>
          
          <div class="banner-buttons">
            <button class="banner-btn primary-btn">
              <i class="fas fa-rocket"></i> Khám phá ngay
            </button>
            <button class="banner-btn secondary-btn">
              <i class="fas fa-info-circle"></i> Tìm hiểu thêm
            </button>
          </div>
        </div>
        
        <div class="banner-right">
          <div class="floating-cards-container">
            <div class="floating-card card-1">
              <div class="card-glow"></div>
              <div class="card-content">
                <div class="card-game">VALORANT</div>
                <div class="card-rank">RADIANT</div>
                <div class="card-price">2.500.000 VNĐ</div>
              </div>
            </div>
            <div class="floating-card card-2">
              <div class="card-glow"></div>
              <div class="card-content">
                <div class="card-game">LEAGUE OF LEGENDS</div>
                <div class="card-rank">CHALLENGER</div>
                <div class="card-price">1.800.000 VNĐ</div>
              </div>
            </div>
            <div class="floating-card card-3">
              <div class="card-glow"></div>
              <div class="card-content">
                <div class="card-game">GENSHIN IMPACT</div>
                <div class="card-rank">AR 60</div>
                <div class="card-price">3.200.000 VNĐ</div>
              </div>
            </div>
          </div>
          <div class="holographic-sphere"></div>
        </div>
      </div>
      
      <div class="banner-bottom-curve"></div>
    </section>

    <nav class="game-nav">
      <div class="nav-container">
        <button
          class="game-filter-btn"
          :class="{ active: selectedGame === 'All' }"
          @click="selectedGame = 'All'"
        >
          <i class="fas fa-globe-asia"></i> Tất cả
        </button>
        <button
          v-for="game in games"
          :key="game.id"
          class="game-filter-btn"
          :class="{ active: selectedGame === game.name }"
          @click="selectedGame = game.name"
        >
          <i class="fas fa-gamepad"></i> {{ game.name }}
        </button>
      </div>
    </nav>

    <main class="main-content">
      <div v-if="isLoading" class="loading-container">
        <div class="space-loader">
          <div class="orbit"></div>
          <div class="core"></div>
          <p>Đang tải dữ liệu...</p>
        </div>
      </div>

      <div v-if="errorMessage && !isLoading" class="error-container">
        <div class="error-box">
          <i class="fas fa-exclamation-triangle"></i>
          <p>{{ errorMessage }}</p>
          <button class="retry-btn" @click="fetchGames().then(() => fetchAccounts())">
            <i class="fas fa-redo"></i> Thử lại
          </button>
        </div>
      </div>

      <section v-if="!isLoading && !errorMessage" class="accounts-section">
        <div class="section-header">
          <h2 class="glow-text"><i class="fas fa-user-shield"></i> Tài khoản Game</h2>
          <p class="section-desc">Mua và bán tài khoản an toàn, đáng tin cậy và nhanh chóng</p>
        </div>

        <div class="card-grid">
          <div
            v-for="account in filteredAccounts"
            :key="account.id"
            class="account-card"
            :class="{ 'sold': account.status === 'Đã bán' }"
          >
            <div class="card-banner">
              <img :src="account.images[0]" :alt="account.game" class="card-img">
              <div class="game-badge">{{ account.game }}</div>
              <div v-if="account.status === 'Đã bán'" class="sold-tag">Đã bán</div>
              <!-- Nút xem thêm cải tiến -->
              <button @click="openImageModal(account)" class="view-more-images">
                <span class="btn-glow"></span>
                <i class="fas fa-images"></i>
                <span>Xem chi tiết</span>
              </button>
            </div>

            <div class="card-content">
              <div class="seller-info">
                <i class="fas fa-user-circle"></i>
                <span>{{ account.seller }}</span>
              </div>

              <div class="account-details">
                <div v-for="field in showMoreDetails.includes(account.id) ? account.fields : account.fields.slice(0, 3)" 
                     :key="field.fieldName" 
                     class="detail-item">
                  <span class="detail-label">{{ field.fieldName }}:</span>
                  <span class="detail-value">{{ field.fieldValue }}</span>
                </div>
                <div v-if="account.fields.length > 3" class="more-details">
                  <button 
                    class="toggle-details-btn"
                    @click="toggleMoreDetails(account.id)"
                  >
                    {{ showMoreDetails.includes(account.id) ? 'Ẩn bớt' : `+${account.fields.length - 3} chi tiết nữa` }}
                  </button>
                </div>
              </div>

              <div class="price-container">
                <div class="price">
                  <span class="price-label">Giá:</span>
                  <span class="price-value">{{ account.price.toLocaleString() }} VNĐ</span>
                </div>
                <div class="min-price">
                  <span class="price-label">Giá tối thiểu:</span>
                  <span class="price-value">{{ account.priceMin.toLocaleString() }} VNĐ</span>
                </div>
              </div>

              <div class="card-actions" v-if="!isOwnAccount(account)">
                <button class="bid-btn" :disabled="account.status === 'Đã bán'">
                  <i class="fas fa-gavel"></i> Trả giá
                </button>
                <button class="buy-btn" :disabled="account.status === 'Đã bán'" @click="buyAccount(account.id)">
                  <i class="fas fa-shopping-cart"></i> Mua ngay
                </button>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section v-if="!isLoading && !errorMessage" class="services-section">
        <div class="section-header">
          <h2 class="glow-text"><i class="fas fa-hands-helping"></i> Dịch vụ Game</h2>
          <p class="section-desc">Đội ngũ hỗ trợ chuyên nghiệp đảm bảo tiến độ kịp thời</p>
        </div>

        <div class="card-grid">
          <div v-for="service in filteredServices" :key="service.id" class="service-card">
            <div class="service-header">
              <h3 class="service-title">{{ service.name }}</h3>
              <div class="service-game">{{ service.game }}</div>
            </div>

            <div class="service-content">
              <div class="creator-info">
                <i class="fas fa-user-astronaut"></i>
                <span>{{ service.creator }}</span>
              </div>
              <p class="service-desc">{{ service.description }}</p>
              <div class="service-price">
                <span class="price-label">Giá dịch vụ:</span>
                <span class="price-value">{{ service.price.toLocaleString() }} VNĐ</span>
              </div>
              <button class="hire-btn">
                <i class="fas fa-handshake"></i> Thuê ngay
              </button>
            </div>
          </div>
        </div>
      </section>
    </main>

    <section class="promotion-banner">
      <div class="banner-content">
        <div class="banner-text">
          <h2 class="banner-title">BẠN MUỐN BÁN TÀI KHOẢN?</h2>
          <p class="banner-desc">Đăng ký ngay để bắt đầu bán tài khoản game của bạn một cách an toàn</p>
        </div>
        <div class="banner-actions">
          <button class="banner-btn">
            <i class="fas fa-rocket"></i> Đăng bán tài khoản ngay
          </button>
        </div>
      </div>
    </section>

    <!-- Modal xem hình ảnh cải tiến -->
    <teleport to="body">
      <div v-if="showImageModal && selectedAccount" class="game-image-modal" @click="closeImageModal">
        <div class="game-modal-container" @click.stop>
          <!-- Header -->
          <div class="game-modal-header">
            <div class="game-modal-title">
              <div class="game-badge-modal">{{ selectedAccount.game }}</div>
              <h3>GAME SHOWCASE</h3>
            </div>
            <button class="game-close-btn" @click="closeImageModal">
              <i class="fas fa-times"></i>
            </button>
          </div>
          
          <!-- Body -->
          <div class="game-modal-body">
            <!-- Gallery Section -->
            <div class="game-gallery">
              <div class="game-main-image-wrapper">
                <div class="game-image-frame">
                  <img 
                    :src="selectedAccount.images[currentImageIndex]" 
                    :alt="'Hình ' + (currentImageIndex + 1)" 
                    class="game-main-image"
                  >
                  <div class="game-image-overlay"></div>
                  <div class="game-corner top-left"></div>
                  <div class="game-corner top-right"></div>
                  <div class="game-corner bottom-left"></div>
                  <div class="game-corner bottom-right"></div>
                </div>
                
                <button 
                  v-if="selectedAccount.images.length > 1 && currentImageIndex > 0" 
                  class="game-nav-btn prev-btn" 
                  @click.stop="prevImage"
                >
                  <i class="fas fa-chevron-left"></i>
                </button>
                <button 
                  v-if="selectedAccount.images.length > 1 && currentImageIndex < selectedAccount.images.length - 1" 
                  class="game-nav-btn next-btn" 
                  @click.stop="nextImage"
                >
                  <i class="fas fa-chevron-right"></i>
                </button>
                
                <div class="game-image-counter">
                  <span class="current-index">{{ currentImageIndex + 1 }}</span>
                  <span class="separator">/</span>
                  <span class="total-images">{{ selectedAccount.images.length }}</span>
                </div>
              </div>
              
              <div v-if="selectedAccount.images.length > 1" class="game-thumbnails">
                <div 
                  v-for="(image, index) in selectedAccount.images" 
                  :key="index"
                  class="game-thumbnail"
                  :class="{ active: index === currentImageIndex }"
                  @click.stop="currentImageIndex = index"
                >
                  <div class="game-thumb-frame">
                    <img :src="image" :alt="'Thumbnail ' + (index + 1)">
                    <div class="game-thumb-overlay"></div>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Account Details -->
            <div class="game-account-info">
              <div class="game-info-header">
                <div class="game-seller-badge">
                  <i class="fas fa-user-circle"></i>
                  <span>{{ selectedAccount.seller }}</span>
                </div>
                <div class="game-status-badge" :class="{ 'sold-status': selectedAccount.status === 'Đã bán' }">
                  {{ selectedAccount.status }}
                </div>
              </div>
              
              <div class="game-price-section">
                <div class="game-price-box">
                  <div class="game-price-label">Giá bán</div>
                  <div class="game-price-value">{{ selectedAccount.price.toLocaleString() }} <span>VNĐ</span></div>
                </div>
                <div class="game-price-box min-price">
                  <div class="game-price-label">Giá tối thiểu</div>
                  <div class="game-price-value">{{ selectedAccount.priceMin.toLocaleString() }} <span>VNĐ</span></div>
                </div>
              </div>
              
              <div class="game-fields-container">
                <div class="game-fields-header">
                  <i class="fas fa-info-circle"></i>
                  <span>Thông tin chi tiết</span>
                </div>
                <div class="game-fields-grid">
                  <div v-for="field in selectedAccount.fields" :key="field.fieldName" class="game-field-item">
                    <div class="field-name">{{ field.fieldName }}</div>
                    <div class="field-value">{{ field.fieldValue }}</div>
                  </div>
                </div>
              </div>
              
              <div class="game-action-buttons" v-if="selectedAccount.status !== 'Đã bán'">
                <button class="game-action-btn bid-btn" @click.stop>
                  <i class="fas fa-gavel"></i>
                  <span>Trả giá</span>
                  <div class="btn-glow"></div>
                </button>
                <button class="game-action-btn buy-btn" @click.stop="buyAccount(selectedAccount.id)">
                  <i class="fas fa-shopping-cart"></i>
                  <span>Mua ngay</span>
                  <div class="btn-glow"></div>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </teleport>

    <teleport to="body">
      <div v-if="showPurchaseModal" class="purchase-modal" @click="closePurchaseModal">
        <div class="modal-content animated" @click.stop>
          <button class="close-btn" @click="closePurchaseModal">×</button>
          <div class="modal-body">
            <i :class="purchaseStatus === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-triangle'"
               class="modal-icon"></i>
            <h3 class="modal-title">
              {{ purchaseStatus === 'success' ? 'Thành công!' : 'Thất bại!' }}
            </h3>
            <p class="modal-message">{{ purchaseMessage }}</p>
            <div class="modal-actions">
              <button v-if="purchaseMessage === 'Vui lòng đăng nhập để mua tài khoản'" 
                      class="modal-btn login-btn" 
                      @click="goToLogin">
                Đăng nhập
              </button>
              <button class="modal-btn" @click="closePurchaseModal">Đóng</button>
            </div>
          </div>
        </div>
      </div>
    </teleport>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@700&family=Open+Sans:wght@400&display=swap');
@import url('https://fonts.googleapis.com/css2?family=Rajdhani:wght@500;600;700&family=Orbitron:wght@400;500;700;900&display=swap');

.cosmo-trade-zone {
  font-family: 'Open Sans', sans-serif;
  color: #e1e7ef;
  min-height: 100vh;
  background-color: #050A15;
  position: relative;
  overflow-x: hidden;
}

h1, h2, h3, h4, .banner-title {
  font-family: 'Roboto', sans-serif;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.stars-container {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  overflow: hidden;
  z-index: 1;
}

.stars {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: transparent;
}

.stars-small {
  background-image:
    radial-gradient(1px 1px at 25px 5px, #fff, rgba(255, 255, 255, 0)),
    radial-gradient(1px 1px at 50px 25px, #fff, rgba(255, 255, 255, 0)),
    radial-gradient(1px 1px at 125px 20px, #fff, rgba(255, 255, 255, 0)),
    radial-gradient(1.5px 1.5px at 50px 75px, #fff, rgba(255, 255, 255, 0)),
    radial-gradient(1.5px 1.5px at 100px 10px, #fff, rgba(255, 255, 255, 0));
  background-repeat: repeat;
  background-size: 200px 200px;
  animation: animateStars 50s linear infinite;
  opacity: 0.6;
}

.stars-medium {
  background-image:
    radial-gradient(2px 2px at 50px 100px, #fff, rgba(255, 255, 255, 0)),
    radial-gradient(2px 2px at 100px 50px, #fff, rgba(255, 255, 255, 0)),
    radial-gradient(2px 2px at 150px 150px, #fff, rgba(255, 255, 255, 0)),
    radial-gradient(2px 2px at 200px 200px, #fff, rgba(255, 255, 255, 0)),
    radial-gradient(2px 2px at 250px 250px, #fff, rgba(255, 255, 255, 0));
  background-repeat: repeat;
  background-size: 300px 300px;
  animation: animateStars 75s linear infinite;
  opacity: 0.5;
}

.stars-large {
  background-image:
    radial-gradient(3px 3px at 300px 300px, rgba(255, 255, 255, 0.8), rgba(255, 255, 255, 0)),
    radial-gradient(3px 3px at 400px 400px, rgba(255, 255, 255, 0.8), rgba(255, 255, 255, 0)),
    radial-gradient(3px 3px at 500px 500px, rgba(255, 255, 255, 0.8), rgba(255, 255, 255, 0)),
    radial-gradient(3px 3px at 600px 600px, rgba(255, 255, 255, 0.8), rgba(255, 255, 255, 0));
  background-repeat: repeat;
  background-size: 600px 600px;
  animation: animateStars 100s linear infinite;
  opacity: 0.4;
}

@keyframes animateStars {
  from {
    transform: translateY(0);
  }
  to {
    transform: translateY(-200px);
  }
}

.nebula-bg {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background:
    radial-gradient(circle at 20% 30%, rgba(142, 45, 226, 0.1), transparent 80%),
    radial-gradient(circle at 80% 70%, rgba(41, 121, 255, 0.1), transparent 80%),
    radial-gradient(circle at 50% 50%, rgba(114, 9, 183, 0.05), transparent 100%);
  filter: blur(10px);
  z-index: 0;
  opacity: 0.7;
  animation: pulsate 15s ease-in-out infinite alternate;
}

@keyframes pulsate {
  0%, 100% {
    opacity: 0.7;
  }
  50% {
    opacity: 0.4;
  }
}

.planet {
  position: fixed;
  border-radius: 50%;
  z-index: 0;
}

.planet-1 {
  width: 150px;
  height: 150px;
  top: 15%;
  right: -50px;
  background: radial-gradient(circle at 30% 30%, #7A4EFE, #4A00E0);
  box-shadow: 0 0 30px rgba(122, 78, 254, 0.4);
  opacity: 0.5;
  animation: rotate1 120s linear infinite;
}

.planet-2 {
  width: 100px;
  height: 100px;
  bottom: 10%;
  left: -30px;
  background: radial-gradient(circle at 40% 40%, #00E0AA, #00807A);
  box-shadow: 0 0 20px rgba(0, 224, 170, 0.4);
  opacity: 0.5;
  animation: rotate2 80s linear infinite;
}

@keyframes rotate1 {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes rotate2 {
  0% { transform: rotate(360deg); }
  100% { transform: rotate(0deg); }
}

.nav-container,
.section-header,
.card-grid,
.banner-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 20px;
  position: relative;
  z-index: 2;
}

section {
  padding: 40px 0;
  position: relative;
  z-index: 2;
}

.glow-text {
  color: #fff;
  text-shadow: 0 0 10px rgba(41, 121, 255, 0.7),
               0 0 20px rgba(41, 121, 255, 0.4);
  transition: all 0.3s ease;
}

.glow-text:hover {
  text-shadow: 0 0 15px rgba(41, 121, 255, 0.9),
               0 0 30px rgba(41, 121, 255, 0.6);
}

.top-banner {
  position: relative;
  width: 100%;
  height: 650px;
  overflow: hidden;
  z-index: 2;
  background: linear-gradient(135deg, #0a0a20, #16213e);
  border-bottom: 1px solid rgba(122, 78, 254, 0.3);
  box-shadow: 0 5px 25px rgba(0, 0, 0, 0.3);
}

.banner-particles {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: 
    radial-gradient(circle at 20% 30%, rgba(122, 78, 254, 0.15), transparent 20%),
    radial-gradient(circle at 80% 20%, rgba(0, 224, 170, 0.1), transparent 20%),
    radial-gradient(circle at 10% 80%, rgba(255, 0, 128, 0.1), transparent 20%),
    radial-gradient(circle at 90% 80%, rgba(255, 102, 0, 0.1), transparent 20%);
  animation: particleMove 20s ease-in-out infinite alternate;
}

@keyframes particleMove {
  0% {
    background-position: 0% 0%, 0% 0%, 0% 0%, 0% 0%;
  }
  100% {
    background-position: 10% 20%, -10% 10%, 5% -5%, -5% 5%;
  }
}

.banner-hologram {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: 
    repeating-linear-gradient(
      to bottom,
      transparent,
      rgba(122, 78, 254, 0.05) 1px,
      transparent 3px
    );
  opacity: 0.5;
  animation: hologramScan 8s linear infinite;
}

@keyframes hologramScan {
  0% {
    background-position: 0 0;
  }
  100% {
    background-position: 0 100%;
  }
}

.banner-content-wrapper {
  position: relative;
  max-width: 1400px;
  height: 100%;
  margin: 0 auto;
  padding: 0 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  z-index: 3;
}

.banner-left {
  flex: 1;
  max-width: 600px;
  animation: fadeInLeft 1s ease;
}

.banner-right {
  flex: 1;
  position: relative;
  height: 500px;
  animation: fadeInRight 1s ease;
}

@keyframes fadeInLeft {
  from {
    opacity: 0;
    transform: translateX(-50px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes fadeInRight {
  from {
    opacity: 0;
    transform: translateX(50px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.banner-title {
  font-size: 5rem;
  font-weight: 900;
  line-height: 1;
  margin-bottom: 20px;
  display: flex;
  flex-direction: column;
  text-shadow: 0 0 20px rgba(122, 78, 254, 0.5);
}

.title-line {
  display: block;
}

.title-line.highlight {
  background: linear-gradient(to right, #ff00cc, #3333ff);
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
  text-shadow: none;
  position: relative;
}

.title-line.highlight::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 5px;
  background: linear-gradient(to right, #ff00cc, #3333ff);
  border-radius: 5px;
}

.banner-subtitle {
  font-size: 1.5rem;
  color: #e1e7ef;
  margin-bottom: 30px;
  line-height: 1.4;
}

.highlight-text {
  color: #00E0AA;
  font-weight: 700;
  text-shadow: 0 0 10px rgba(0, 224, 170, 0.5);
}

.banner-stats {
  display: flex;
  gap: 30px;
  margin-bottom: 30px;
}

.stat-item {
  text-align: center;
  background: rgba(255, 255, 255, 0.05);
  padding: 15px;
  border-radius: 10px;
  border: 1px solid rgba(122, 78, 254, 0.2);
  min-width: 100px;
  transition: all 0.3s ease;
}

.stat-item:hover {
  transform: translateY(-5px);
  background: rgba(122, 78, 254, 0.1);
  border-color: rgba(122, 78, 254, 0.5);
  box-shadow: 0 10px 20px rgba(122, 78, 254, 0.2);
}

.stat-value {
  font-size: 1.8rem;
  font-weight: 800;
  color: #fff;
  margin-bottom: 5px;
  background: linear-gradient(to right, #7A4EFE, #00E0AA);
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
}

.stat-label {
  font-size: 0.9rem;
  color: #b0b5c3;
}

.banner-buttons {
  display: flex;
  gap: 20px;
}

.banner-btn {
  padding: 15px 30px;
  border-radius: 50px;
  font-weight: 700;
  font-size: 1rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 10px;
  transition: all 0.3s ease;
  border: none;
}

.primary-btn {
  background: linear-gradient(to right, #ff00cc, #3333ff);
  color: white;
  box-shadow: 0 10px 20px rgba(255, 0, 204, 0.3);
}

.primary-btn:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 30px rgba(255, 0, 204, 0.5);
}

.secondary-btn {
  background: transparent;
  color: #e1e7ef;
  border: 2px solid rgba(122, 78, 254, 0.5);
}

.secondary-btn:hover {
  background: rgba(122, 78, 254, 0.1);
  transform: translateY(-5px);
  border-color: #7A4EFE;
}

.floating-cards-container {
  position: absolute;
  width: 100%;
  height: 100%;
}

.floating-card {
  position: absolute;
  width: 220px;
  height: 300px;
  background: rgba(10, 10, 32, 0.8);
  border-radius: 15px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 20px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4);
  border: 1px solid rgba(122, 78, 254, 0.3);
  backdrop-filter: blur(5px);
  transform-style: preserve-3d;
  perspective: 1000px;
}

.card-1 {
  top: 50px;
  left: 0;
  transform: rotate(-15deg);
  animation: floatCard1 6s ease-in-out infinite alternate;
  z-index: 3;
}

.card-2 {
  top: 150px;
  left: 120px;
  transform: rotate(5deg);
  animation: floatCard2 7s ease-in-out infinite alternate;
  z-index: 2;
}

.card-3 {
  top: 250px;
  left: 60px;
  transform: rotate(-5deg);
  animation: floatCard3 8s ease-in-out infinite alternate;
  z-index: 1;
}

@keyframes floatCard1 {
  0% {
    transform: rotate(-15deg) translateY(0);
  }
  100% {
    transform: rotate(-12deg) translateY(-20px);
  }
}

@keyframes floatCard2 {
  0% {
    transform: rotate(5deg) translateY(0);
  }
  100% {
    transform: rotate(8deg) translateY(-15px);
  }
}

@keyframes floatCard3 {
  0% {
    transform: rotate(-5deg) translateY(0);
  }
  100% {
    transform: rotate(-8deg) translateY(-25px);
  }
}

.card-glow {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle at 30% 30%, rgba(122, 78, 254, 0.4), transparent 70%);
  opacity: 0.7;
  z-index: -1;
}

.card-content {
  display: flex;
  flex-direction: column;
  gap: 15px;
  z-index: 1;
}

.card-game {
  font-size: 0.9rem;
  color: #b0b5c3;
  font-weight: 600;
}

.card-rank {
  font-size: 1.8rem;
  font-weight: 800;
  color: #fff;
  text-shadow: 0 0 10px rgba(122, 78, 254, 0.8);
}

.card-price {
  margin-top: auto;
  font-size: 1.2rem;
  font-weight: 700;
  color: #00E0AA;
  text-shadow: 0 0 10px rgba(0, 224, 170, 0.5);
}

.holographic-sphere {
  position: absolute;
  top: 50%;
  right: 0;
  transform: translateY(-50%);
  width: 300px;
  height: 300px;
  border-radius: 50%;
  background: radial-gradient(circle at 30% 30%, rgba(122, 78, 254, 0.1), transparent 80%);
  border: 2px solid rgba(122, 78, 254, 0.3);
  box-shadow: 0 0 50px rgba(122, 78, 254, 0.5), inset 0 0 50px rgba(122, 78, 254, 0.3);
  animation: rotateSphere 20s linear infinite;
}

.holographic-sphere::before {
  content: '';
  position: absolute;
  top: -2px;
  left: -2px;
  right: -2px;
  bottom: -2px;
  border-radius: 50%;
  background: conic-gradient(
    transparent 0deg,
    rgba(122, 78, 254, 0.5) 60deg,
    transparent 120deg,
    rgba(0, 224, 170, 0.5) 180deg,
    transparent 240deg,
    rgba(255, 0, 204, 0.5) 300deg,
    transparent 360deg
  );
  opacity: 0.3;
  animation: rotateSphereGradient 10s linear infinite;
}

@keyframes rotateSphere {
  0% {
    transform: translateY(-50%) rotate(0deg);
  }
  100% {
    transform: translateY(-50%) rotate(360deg);
  }
}

@keyframes rotateSphereGradient {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(-360deg);
  }
}

.banner-bottom-curve {
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 100%;
  height: 80px;
  background: #050A15;
  clip-path: ellipse(50% 60% at 50% 100%);
  z-index: 2;
}

.game-nav {
  padding: 15px 0;
  position: sticky;
  top: 0;
  z-index: 9;
  background: rgba(5, 10, 21, 0.8);
  backdrop-filter: blur(10px);
  border-bottom: 1px solid rgba(122, 78, 254, 0.2);
}

.nav-container {
  display: flex;
  gap: 10px;
  overflow-x: auto;
  padding-bottom: 5px;
  scrollbar-width: thin;
  scrollbar-color: #7A4EFE #0a0e17;
}

.nav-container::-webkit-scrollbar {
  height: 5px;
}

.nav-container::-webkit-scrollbar-thumb {
  background: #7A4EFE;
  border-radius: 5px;
}

.nav-container::-webkit-scrollbar-track {
  background: #0a0e17;
}

.game-filter-btn {
  background: rgba(26, 34, 52, 0.6);
  color: #e1e7ef;
  border: 1px solid rgba(122, 78, 254, 0.2);
  border-radius: 50px;
  padding: 10px 20px;
  font-weight: 600;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 8px;
  white-space: nowrap;
}

.game-filter-btn i {
  color: #00E0AA;
  font-size: 1rem;
}

.game-filter-btn:hover, .game-filter-btn.active {
  background: linear-gradient(to right, rgba(122, 78, 254, 0.2), rgba(0, 224, 170, 0.2));
  border-color: #7A4EFE;
  box-shadow: 0 5px 15px rgba(122, 78, 254, 0.3);
}

.game-filter-btn.active {
  background: linear-gradient(to right, rgba(122, 78, 254, 0.6), rgba(0, 224, 170, 0.6));
  color: white;
}

.main-content {
  min-height: 60vh;
  padding: 40px 0;
}

.section-header {
  text-align: center;
  margin-bottom: 40px;
}

.section-header h2 {
  font-size: 2.5rem;
  margin-bottom: 10px;
}

.section-desc {
  color: #b0b5c3;
  font-size: 1.1rem;
  max-width: 600px;
  margin: 0 auto;
}

.loading-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 40vh;
}

.space-loader {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
}

.orbit {
  width: 80px;
  height: 80px;
  border: 2px solid rgba(122, 78, 254, 0.3);
  border-top: 2px solid #7A4EFE;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.core {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 20px;
  height: 20px;
  background: radial-gradient(circle at 30% 30%, #00E0AA, #009B8F);
  border-radius: 50%;
  box-shadow: 0 0 15px rgba(0, 224, 170, 0.6);
  animation: pulse 1.5s infinite alternate;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes pulse {
  0% {
    transform: scale(1);
    opacity: 0.7;
  }
  100% {
    transform: scale(1.1);
    opacity: 1;
  }
}

.space-loader p {
  color: #7A4EFE;
  font-size: 1.2rem;
  letter-spacing: 2px;
  text-shadow: 0 0 10px rgba(122, 78, 254, 0.6);
}

.error-container {
  display: flex;
  justify-content: center;
  padding: 40px 0;
}

.error-box {
  background: rgba(255, 0, 76, 0.1);
  border: 1px solid rgba(255, 0, 76, 0.3);
  border-radius: 10px;
  padding: 20px;
  max-width: 600px;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
}

.error-box i {
  color: #FF004C;
  font-size: 3rem;
}

.error-box p {
  color: #e1e7ef;
  font-size: 1.1rem;
  margin-bottom: 15px;
}

.retry-btn {
  background: rgba(255, 0, 76, 0.2);
  color: #e1e7ef;
  border: 1px solid rgba(255, 0, 76, 0.5);
  border-radius: 50px;
  padding: 10px 25px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 8px;
}

.retry-btn:hover {
  background: rgba(255, 0, 76, 0.3);
  transform: translateY(-3px);
  box-shadow: 0 5px 15px rgba(255, 0, 76, 0.3);
}

.card-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 25px;
}

.account-card {
  background: rgba(26, 34, 52, 0.8);
  backdrop-filter: blur(5px);
  border-radius: 15px;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(122, 78, 254, 0.2);
  height: 100%;
  display: flex;
  flex-direction: column;
}

.account-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 15px 30px rgba(122, 78, 254, 0.3);
  border-color: #7A4EFE;
}

.account-card.sold {
  opacity: 0.7;
  filter: grayscale(0.5);
}

.card-banner {
  position: relative;
  height: 180px;
}

.card-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.game-badge {
  position: absolute;
  top: 15px;
  left: 15px;
  padding: 5px 12px;
  background: linear-gradient(to right, #7A4EFE, #4A00E0);
  color: white;
  font-weight: 600;
  font-size: 0.8rem;
  border-radius: 8px;
  box-shadow: 0 5px 10px rgba(122, 78, 254, 0.3);
}

.sold-tag {
  position: absolute;
  top: 0;
  right: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.7);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: 800;
  letter-spacing: 2px;
  text-shadow: 0 0 10px rgba(255, 0, 76, 0.8);
}

/* Nút xem thêm cải tiến */
.view-more-images {
  position: absolute;
  bottom: 15px;
  left: 50%;
  transform: translateX(-50%);
  background: rgba(0, 0, 0, 0.7);
  border: 1px solid #00ffff;
  border-radius: 30px;
  padding: 8px 20px;
  color: #00ffff;
  font-family: 'Rajdhani', sans-serif;
  font-weight: 600;
  font-size: 0.9rem;
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  overflow: hidden;
  transition: all 0.3s ease;
  z-index: 5;
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.3);
}

.view-more-images i {
  font-size: 1rem;
}

.view-more-images .btn-glow {
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle at center, rgba(0, 255, 255, 0.5), transparent 70%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.view-more-images:hover {
  transform: translateX(-50%) translateY(-5px);
  background: rgba(0, 0, 0, 0.8);
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.5);
}

.view-more-images:hover .btn-glow {
  opacity: 0.3;
  animation: rotate 10s linear infinite;
}

.seller-info {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #b0b5c3;
  font-size: 0.9rem;
}

.seller-info i {
  color: #00E0AA;
}

.account-details {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 12px;
  background: rgba(5, 10, 21, 0.5);
  border-radius: 10px;
  border: 1px solid rgba(122, 78, 254, 0.1);
}

.detail-item {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;
  font-size: 0.9rem;
}

.detail-label {
  color: #b0b5c3;
  min-width: 100px;
  font-weight: 600;
}

.detail-value {
  color: #e1e7ef;
  flex: 1;
}

.more-details {
  margin-top: 5px;
  text-align: center;
}

.toggle-details-btn {
  background: none;
  border: none;
  color: #7A4EFE;
  font-size: 0.85rem;
  font-style: italic;
  cursor: pointer;
  padding: 5px;
  transition: all 0.3s ease;
}

.toggle-details-btn:hover {
  color: #00E0AA;
  text-decoration: underline;
}

.price-container {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 12px;
  background: rgba(122, 78, 254, 0.1);
  border-radius: 10px;
  margin-top: auto;
}

.price, .min-price {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.price-label {
  color: #b0b5c3;
  font-size: 0.9rem;
}

.price-value {
  color: #00E0AA;
  font-weight: 700;
  font-size: 1.1rem;
  text-shadow: 0 0 5px rgba(0, 224, 170, 0.3);
}

.card-actions {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
  margin-top: 15px;
}

.bid-btn, .buy-btn {
  padding: 10px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  font-size: 0.9rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
  transition: all 0.3s ease;
}

.bid-btn {
  background: rgba(0, 224, 170, 0.2);
  color: #e1e7ef;
  border: 1px solid rgba(0, 224, 170, 0.4);
}

.bid-btn:hover {
  background: rgba(0, 224, 170, 0.3);
  transform: translateY(-3px);
}

.buy-btn {
  background: linear-gradient(to right, #7A4EFE, #00E0AA);
  color: white;
  box-shadow: 0 5px 10px rgba(122, 78, 254, 0.2);
}

.buy-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 15px rgba(122, 78, 254, 0.4);
}

.bid-btn:disabled, .buy-btn:disabled {
  background: #2c3e50;
  border-color: transparent;
  color: #8896ae;
  box-shadow: none;
  cursor: not-allowed;
  transform: none;
}

.service-card {
  background: rgba(0, 224, 170, 0.05);
  backdrop-filter: blur(5px);
  border-radius: 15px;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(0, 224, 170, 0.2);
  display: flex;
  flex-direction: column;
  height: 100%;
}

.service-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 15px 30px rgba(0, 224, 170, 0.3);
  border-color: #00E0AA;
}

.service-header {
  padding: 15px;
  background: linear-gradient(to right, rgba(0, 224, 170, 0.2), rgba(41, 121, 255, 0.2));
  border-bottom: 1px solid rgba(0, 224, 170, 0.2);
}

.service-title {
  font-size: 1.3rem;
  font-weight: 700;
  color: #fff;
  margin-bottom: 5px;
  letter-spacing: 1px;
}

.service-game {
  display: inline-block;
  padding: 3px 10px;
  background: rgba(0, 0, 0, 0.3);
  color: #00E0AA;
  font-size: 0.8rem;
  border-radius: 5px;
  margin-top: 5px;
}

.service-content {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 15px;
  flex: 1;
}

.creator-info {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #b0b5c3;
  font-size: 0.9rem;
}

.creator-info i {
  color: #7A4EFE;
}

.service-desc {
  color: #e1e7ef;
  font-size: 0.95rem;
  line-height: 1.5;
  margin-bottom: 10px;
  flex: 1;
}

.service-price {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  background: rgba(0, 224, 170, 0.1);
  border-radius: 8px;
  margin-top: auto;
}

.hire-btn {
  width: 100%;
  padding: 12px;
  margin-top: 15px;
  background: linear-gradient(to right, #00E0AA, #00b3e0);
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  transition: all 0.3s ease;
  box-shadow: 0 5px 10px rgba(0, 224, 170, 0.2);
}

.hire-btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 15px rgba(0, 224, 170, 0.4);
}

.promotion-banner {
  background: linear-gradient(135deg, rgba(122, 78, 254, 0.2), rgba(0, 224, 170, 0.2));
  padding: 50px 0;
  position: relative;
  overflow: hidden;
  margin: 40px 0;
}

.promotion-banner::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background:
    radial-gradient(circle at 20% 30%, rgba(122, 78, 254, 0.3), transparent 50%),
    radial-gradient(circle at 80% 70%, rgba(0, 224, 170, 0.3), transparent 50%);
  filter: blur(20px);
  z-index: 0;
}

.banner-content {
  display: flex;
  flex-direction: column;
  gap: 30px;
  align-items: center;
  text-align: center;
  position: relative;
  z-index: 2;
}

.promotion-banner .banner-title {
  font-size: 2.5rem;
  font-weight: 900;
  color: #fff;
  text-shadow: 0 0 20px rgba(122, 78, 254, 0.6);
  margin-bottom: 10px;
}

.banner-desc {
  font-size: 1.2rem;
  color: #e1e7ef;
  max-width: 700px;
  margin: 0 auto;
}

.promotion-banner .banner-btn {
  padding: 15px 40px;
  background: linear-gradient(to right, #7A4EFE, #00E0AA);
  color: white;
  border: none;
  border-radius: 50px;
  font-weight: 700;
  font-size: 1.1rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 10px;
  transition: all 0.3s ease;
  box-shadow: 0 10px 20px rgba(122, 78, 254, 0.4);
}

.promotion-banner .banner-btn:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 30px rgba(122, 78, 254, 0.6);
}

/* Game Image Modal Styles - Cải tiến Modal Xem Hình Ảnh */
.game-image-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(5, 10, 21, 0.95);
  backdrop-filter: blur(10px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
  padding: 20px;
}

.game-modal-container {
  width: 90%;
  max-width: 1200px;
  max-height: 90vh;
  background: rgba(10, 15, 30, 0.9);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 0 40px rgba(0, 255, 255, 0.3);
  border: 1px solid rgba(0, 255, 255, 0.3);
  display: flex;
  flex-direction: column;
  animation: modalFadeIn 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  position: relative;
}

@keyframes modalFadeIn {
  from {
    opacity: 0;
    transform: scale(0.8);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.game-modal-header {
  background: linear-gradient(90deg, rgba(0, 255, 255, 0.1), rgba(255, 0, 255, 0.1));
  padding: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
}

.game-modal-title {
  display: flex;
  align-items: center;
  gap: 15px;
}

.game-modal-title h3 {
  color: #ffffff;
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0;
  font-family: 'Orbitron', sans-serif;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.game-badge-modal {
  background: rgba(0, 255, 255, 0.2);
  color: #00ffff;
  padding: 5px 15px;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 600;
  border: 1px solid rgba(0, 255, 255, 0.4);
  box-shadow: 0 0 10px rgba(0, 255, 255, 0.3);
}

.game-close-btn {
  background: rgba(255, 0, 76, 0.2);
  border: 1px solid rgba(255, 0, 76, 0.4);
  color: #ff4b4b;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 1rem;
}

.game-close-btn:hover {
  background: rgba(255, 0, 76, 0.4);
  transform: rotate(90deg);
  box-shadow: 0 0 15px rgba(255, 0, 76, 0.5);
}

.game-modal-body {
  display: flex;
  flex-direction: column;
  overflow: auto;
  max-height: calc(90vh - 80px);
}

@media (min-width: 992px) {
  .game-modal-body {
    flex-direction: row;
  }
}

.game-gallery {
  flex: 1.5;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.game-main-image-wrapper {
  position: relative;
  width: 100%;
  border-radius: 12px;
  overflow: hidden;
}

.game-image-frame {
  position: relative;
  width: 100%;
  padding-top: 56.25%; /* 16:9 Aspect Ratio */
  background: rgba(0, 0, 0, 0.5);
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 12px;
  overflow: hidden;
}

.game-main-image {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: contain;
}

.game-image-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, 
    rgba(0, 255, 255, 0.05) 0%, 
    transparent 50%, 
    rgba(255, 0, 255, 0.05) 100%);
  pointer-events: none;
}

.game-corner {
  position: absolute;
  width: 20px;
  height: 20px;
  border-color: #00ffff;
  z-index: 2;
}

.top-left {
  top: 0;
  left: 0;
  border-top: 2px solid;
  border-left: 2px solid;
}

.top-right {
  top: 0;
  right: 0;
  border-top: 2px solid;
  border-right: 2px solid;
}

.bottom-left {
  bottom: 0;
  left: 0;
  border-bottom: 2px solid;
  border-left: 2px solid;
}

.bottom-right {
  bottom: 0;
  right: 0;
  border-bottom: 2px solid;
  border-right: 2px solid;
}

.game-nav-btn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(0, 0, 0, 0.7);
  border: 1px solid rgba(0, 255, 255, 0.5);
  color: #00ffff;
  width: 50px;
  height: 50px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  z-index: 5;
  font-size: 1.2rem;
}

.prev-btn {
  left: 20px;
}

.next-btn {
  right: 20px;
}

.game-nav-btn:hover {
  background: rgba(0, 255, 255, 0.2);
  transform: translateY(-50%) scale(1.1);
  box-shadow: 0 0 20px rgba(0, 255, 255, 0.5);
}

.game-image-counter {
  position: absolute;
  bottom: 20px;
  right: 20px;
  background: rgba(0, 0, 0, 0.7);
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 30px;
  padding: 8px 15px;
  color: #ffffff;
  font-size: 0.9rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
  z-index: 5;
}

.current-index {
  color: #00ffff;
}

.separator {
  color: rgba(255, 255, 255, 0.5);
}

.game-thumbnails {
  display: flex;
  gap: 10px;
  overflow-x: auto;
  padding: 5px 0;
  scrollbar-width: thin;
  scrollbar-color: #00ffff #0a0e17;
}

.game-thumbnails::-webkit-scrollbar {
  height: 5px;
}

.game-thumbnails::-webkit-scrollbar-thumb {
  background: #00ffff;
  border-radius: 5px;
}

.game-thumbnails::-webkit-scrollbar-track {
  background: rgba(10, 14, 23, 0.5);
  border-radius: 5px;
}

.game-thumbnail {
  flex: 0 0 100px;
  height: 70px;
  cursor: pointer;
  transition: all 0.3s ease;
  opacity: 0.6;
  border-radius: 8px;
  overflow: hidden;
}

.game-thumbnail.active {
  opacity: 1;
  transform: scale(1.05);
  box-shadow: 0 0 15px rgba(0, 255, 255, 0.5);
}

.game-thumb-frame {
  position: relative;
  width: 100%;
  height: 100%;
  border: 1px solid rgba(0, 255, 255, 0.3);
  border-radius: 8px;
  overflow: hidden;
}

.game-thumbnail.active .game-thumb-frame {
  border-color: #00ffff;
}

.game-thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.game-thumb-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(to bottom, 
    rgba(0, 0, 0, 0.1), 
    rgba(0, 0, 0, 0.3));
  pointer-events: none;
}

.game-account-info {
  flex: 1;
  padding: 20px;
  background: rgba(0, 0, 0, 0.3);
  border-left: 1px solid rgba(0, 255, 255, 0.2);
  display: flex;
  flex-direction: column;
  gap: 20px;
}

@media (max-width: 991px) {
  .game-account-info {
    border-left: none;
    border-top: 1px solid rgba(0, 255, 255, 0.2);
  }
}

.game-info-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 10px;
}

.game-seller-badge {
  background: rgba(255, 0, 255, 0.1);
  color: #ff00ff;
  padding: 8px 15px;
  border-radius: 30px;
  font-size: 0.9rem;
  font-weight: 600;
  border: 1px solid rgba(255, 0, 255, 0.3);
  display: flex;
  align-items: center;
  gap: 8px;
}

.game-status-badge {
  background: rgba(0, 255, 127, 0.1);
  color: #00ff7f;
  padding: 8px 15px;
  border-radius: 30px;
  font-size: 0.9rem;
  font-weight: 600;
  border: 1px solid rgba(0, 255, 127, 0.3);
}

.game-status-badge.sold-status {
  background: rgba(255, 75, 75, 0.1);
  color: #ff4b4b;
  border-color: rgba(255, 75, 75, 0.3);
}

.game-price-section {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.game-price-box {
  flex: 1;
  min-width: 200px;
  background: rgba(0, 255, 255, 0.1);
  padding: 15px;
  border-radius: 12px;
  border: 1px solid rgba(0, 255, 255, 0.2);
  transition: all 0.3s ease;
}

.game-price-box:hover {
  background: rgba(0, 255, 255, 0.15);
  transform: translateY(-3px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
  border-color: rgba(0, 255, 255, 0.4);
}

.game-price-box.min-price {
  background: rgba(255, 0, 255, 0.1);
  border-color: rgba(255, 0, 255, 0.2);
}

.game-price-box.min-price:hover {
  background: rgba(255, 0, 255, 0.15);
  border-color: rgba(255, 0, 255, 0.4);
}

.game-price-label {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 5px;
}

.game-price-value {
  font-size: 1.3rem;
  font-weight: 700;
  color: #00ffff;
}

.game-price-box.min-price .game-price-value {
  color: #ff00ff;
}

.game-price-value span {
  font-size: 0.9rem;
  opacity: 0.8;
}

.game-fields-container {
  background: rgba(0, 0, 0, 0.3);
  border-radius: 12px;
  padding: 20px;
  border: 1px solid rgba(0, 255, 255, 0.2);
}

.game-fields-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid rgba(0, 255, 255, 0.2);
  color: #00ffff;
  font-size: 1.1rem;
  font-weight: 600;
}

.game-fields-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 15px;
}

.game-field-item {
  background: rgba(0, 255, 255, 0.05);
  padding: 12px;
  border-radius: 8px;
  transition: all 0.3s ease;
  border: 1px solid rgba(0, 255, 255, 0.1);
}

.game-field-item:hover {
  background: rgba(0, 255, 255, 0.1);
  transform: translateY(-3px);
  border-color: rgba(0, 255, 255, 0.3);
}

.field-name {
  font-size: 0.8rem;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 5px;
}

.field-value {
  font-size: 1rem;
  color: #ffffff;
  word-break: break-word;
}

.game-action-buttons {
  display: flex;
  gap: 15px;
  margin-top: auto;
}

.game-action-btn {
  flex: 1;
  position: relative;
  padding: 12px;
  border: none;
  border-radius: 8px;
  font-family: 'Rajdhani', sans-serif;
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  overflow: hidden;
  transition: all 0.3s ease;
}

.game-action-btn:hover {
  transform: translateY(-3px);
}

.bid-btn {
  background: rgba(0, 255, 255, 0.1);
  color: #00ffff;
  border: 1px solid rgba(0, 255, 255, 0.4);
}

.bid-btn:hover {
  background: rgba(0, 255, 255, 0.2);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3), 0 0 15px rgba(0, 255, 255, 0.3);
}

.buy-btn {
  background: rgba(255, 0, 255, 0.1);
  color: #ff00ff;
  border: 1px solid rgba(255, 0, 255, 0.4);
}

.buy-btn:hover {
  background: rgba(255, 0, 255, 0.2);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3), 0 0 15px rgba(255, 0, 255, 0.3);
}

.btn-glow {
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle at center, rgba(255, 255, 255, 0.3) 0%, transparent 70%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.game-action-btn:hover .btn-glow {
  opacity: 0.5;
  animation: rotate 10s linear infinite;
}

.purchase-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(5, 10, 21, 0.95);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
  backdrop-filter: blur(5px);
}

.modal-content {
  position: relative;
  max-width: 500px;
  width: 90%;
  background: rgba(26, 34, 52, 0.9);
  padding: 30px;
  border-radius: 15px;
  border: 1px solid rgba(122, 78, 254, 0.3);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5);
}

.animated {
  animation: fadeInScale 0.3s ease;
}

@keyframes fadeInScale {
  from {
    opacity: 0;
    transform: scale(0.9);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

@media (max-width: 1200px) {
  .banner-title {
    font-size: 4rem;
  }
  
  .floating-card {
    width: 180px;
    height: 250px;
  }
  
  .card-2 {
    left: 100px;
  }
  
  .holographic-sphere {
    width: 250px;
    height: 250px;
  }
  
  .card-grid {
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  }
  
  .game-fields-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 992px) {
  .top-banner {
    height: auto;
    padding: 80px 0;
  }
  
  .banner-content-wrapper {
    flex-direction: column;
    gap: 60px;
  }
  
  .banner-left {
    max-width: 100%;
    text-align: center;
  }
  
  .banner-title {
    font-size: 3.5rem;
    align-items: center;
  }
  
  .banner-stats {
    justify-content: center;
  }
  
  .banner-buttons {
    justify-content: center;
  }
  
  .banner-right {
    height: 400px;
    width: 100%;
  }
  
  .floating-cards-container {
    position: relative;
    height: 350px;
  }
  
  .card-1 {
    left: 50%;
    transform: translateX(-150px) rotate(-15deg);
  }
  
  .card-2 {
    left: 50%;
    transform: translateX(-50px) rotate(5deg);
  }
  
  .card-3 {
    left: 50%;
    transform: translateX(50px) rotate(-5deg);
  }
  
  .holographic-sphere {
    right: 50%;
    transform: translate(50%, -50%);
    width: 200px;
    height: 200px;
    opacity: 0.7;
  }
  
  @keyframes floatCard1 {
    0% {
      transform: translateX(-150px) rotate(-15deg) translateY(0);
    }
    100% {
      transform: translateX(-150px) rotate(-12deg) translateY(-20px);
    }
  }
  
  @keyframes floatCard2 {
    0% {
      transform: translateX(-50px) rotate(5deg) translateY(0);
    }
    100% {
      transform: translateX(-50px) rotate(8deg) translateY(-15px);
    }
  }
  
  @keyframes floatCard3 {
    0% {
      transform: translateX(50px) rotate(-5deg) translateY(0);
    }
    100% {
      transform: translateX(50px) rotate(-8deg) translateY(-25px);
    }
  }
  
  @keyframes rotateSphere {
    0% {
      transform: translate(50%, -50%) rotate(0deg);
    }
    100% {
      transform: translate(50%, -50%) rotate(360deg);
    }
  }
  
  .game-action-buttons {
    flex-direction: column;
  }
}

@media (max-width: 768px) {
  .top-banner {
    padding: 60px 0;
  }
  
  .banner-title {
    font-size: 3rem;
  }
  
  .banner-subtitle {
    font-size: 1.2rem;
  }
  
  .banner-stats {
    flex-wrap: wrap;
    justify-content: center;
  }
  
  .banner-buttons {
    flex-direction: column;
    gap: 15px;
    align-items: center;
  }
  
  .banner-btn {
    width: 100%;
    max-width: 300px;
    justify-content: center;
  }
  
  .floating-card {
    width: 150px;
    height: 220px;
    padding: 15px;
  }
  
  .card-1 {
    transform: translateX(-120px) rotate(-15deg);
  }
  
  .card-3 {
    transform: translateX(20px) rotate(-5deg);
  }
  
  .card-rank {
    font-size: 1.5rem;
  }
  
  .card-price {
    font-size: 1rem;
  }
  
  @keyframes floatCard1 {
    0% {
      transform: translateX(-120px) rotate(-15deg) translateY(0);
    }
    100% {
      transform: translateX(-120px) rotate(-12deg) translateY(-20px);
    }
  }
  
  @keyframes floatCard3 {
    0% {
      transform: translateX(20px) rotate(-5deg) translateY(0);
    }
    100% {
      transform: translateX(20px) rotate(-8deg) translateY(-25px);
    }
  }
  
  .card-grid {
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  }
  
  .section-header h2 {
    font-size: 1.7rem;
  }
  
  .section-desc {
    font-size: 1rem;
  }
  
  .promotion-banner .banner-btn {
    padding: 12px 30px;
    font-size: 1rem;
  }
  
  .game-image-frame {
    padding-top: 75%; /* 4:3 Aspect Ratio for mobile */
  }
  
  .game-thumbnail {
    flex: 0 0 80px;
    height: 60px;
  }
  
  .game-nav-btn {
    width: 40px;
    height: 40px;
    font-size: 1rem;
  }
  
  .game-price-section {
    flex-direction: column;
  }
  
  .game-action-buttons {
    flex-direction: column;
  }
}

@media (max-width: 576px) {
  .banner-title {
    font-size: 2.5rem;
  }
  
  .banner-subtitle {
    font-size: 1rem;
  }
  
  .stat-item {
    min-width: 80px;
    padding: 10px;
  }
  
  .stat-value {
    font-size: 1.5rem;
  }
  
  .stat-label {
    font-size: 0.8rem;
  }
  
  .floating-card {
    width: 120px;
    height: 180px;
    padding: 10px;
  }
  
  .card-game {
    font-size: 0.7rem;
  }
  
  .card-rank {
    font-size: 1.2rem;
  }
  
  .card-price {
    font-size: 0.9rem;
  }
  
  .card-1 {
    transform: translateX(-90px) rotate(-15deg);
  }
  
  .card-2 {
    transform: translateX(-30px) rotate(5deg);
  }
  
  .card-3 {
    transform: translateX(30px) rotate(-5deg);
  }
  
  .holographic-sphere {
    width: 150px;
    height: 150px;
  }
  
  @keyframes floatCard1 {
    0% {
      transform: translateX(-90px) rotate(-15deg) translateY(0);
    }
    100% {
      transform: translateX(-90px) rotate(-12deg) translateY(-20px);
    }
  }
  
  @keyframes floatCard2 {
    0% {
      transform: translateX(-30px) rotate(5deg) translateY(0);
    }
    100% {
      transform: translateX(-30px) rotate(8deg) translateY(-15px);
    }
  }
  
  @keyframes floatCard3 {
    0% {
      transform: translateX(30px) rotate(-5deg) translateY(0);
    }
    100% {
      transform: translateX(30px) rotate(-8deg) translateY(-25px);
    }
  }
  
  .game-modal-title h3 {
    font-size: 1.2rem;
  }
  
  .game-badge-modal {
    font-size: 0.8rem;
    padding: 4px 10px;
  }
  
  .game-nav-btn {
    width: 36px;
    height: 36px;
    font-size: 0.9rem;
  }
  
  .game-image-counter {
    font-size: 0.8rem;
    padding: 5px 10px;
  }
  
  .game-fields-grid {
    grid-template-columns: 1fr;
  }
}

@keyframes rotate {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
</style>


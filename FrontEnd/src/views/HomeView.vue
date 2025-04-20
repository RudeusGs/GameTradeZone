<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import gameApi from '@/api/gameinfor.api';
import gameAccountApi from '@/api/gameaccount.api';
import gamefieldApi from '@/api/gamefield.api';
import serviceApi from '@/api/service.api';
import GamingLoader from '@/components/LoadingPage.vue';
import { userStore } from '@/stores/auth';

// Định nghĩa các interface cho dữ liệu
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
  createdDate: string;
}

interface Service {
  id: number;
  gameInforID?: number;
  serviceName?: string;
  createrID?: number;
  decription?: string;
  serviceLevel?: number;
  servicePrice: number;
  serviceTime?: string;
  rentedC?: number;
  feedback?: string;
  image?: string;
  isDelete?: boolean;
  createdDate?: string;
}

interface BuyAccountGameModel {
  Id: number;
}

// Khởi tạo các biến và state
const router = useRouter();
const authStore = userStore();
const user = computed(() => authStore.user);

// Trạng thái cho tab và phân trang
const activeTab = ref<'accounts' | 'services'>('accounts');
const itemsPerPage = 8;
const accountsPage = ref(1);
const servicesPage = ref(1);
const totalAccountsPages = ref(1);

// Bộ lọc cho từng tab
const accountsFilters = ref({
  game: 'All',
  minPrice: null as number | null,
  maxPrice: null as number | null,
  search: '',
  timeFilter: 'all',
});

const servicesFilters = ref({
  game: 'All',
  minPrice: null as number | null,
  maxPrice: null as number | null,
  search: '',
  timeFilter: 'all',
});

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

// Trạng thái cho modal thuê dịch vụ
const showConfirmModal = ref(false);
const showDescriptionModal = ref(false);
const selectedServiceId = ref<number | null>(null);
const descriptionInput = ref('');

// Hàm lấy danh sách game
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

// Hàm lấy danh sách tài khoản game với phân trang
const fetchAccounts = async () => {
  console.log('Fetching accounts for page:', accountsPage.value);
  try {
    isLoading.value = true;
    const response = await gameAccountApi.getAllPaged(accountsPage.value, itemsPerPage);
    console.log('API response:', response.data);
    if (response.data?.result?.isSuccess && response.data.result.data) {
      const pagedResult = response.data.result.data;
      accounts.value = await Promise.all(pagedResult.items.map(async (account: any) => {
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
          createdDate: account.createdDate || 'Không xác định',
        };
      }));
      console.log('Updated accounts:', accounts.value);
      totalAccountsPages.value = Math.ceil(pagedResult.totalItems / itemsPerPage);
      console.log('Total pages:', totalAccountsPages.value);
    } else {
      errorMessage.value = 'Không thể lấy danh sách tài khoản';
    }
  } catch (error) {
    errorMessage.value = 'Lỗi khi gọi API tài khoản';
    console.error('Error fetching accounts:', error);
  } finally {
    isLoading.value = false;
  }
};

// Hàm lấy danh sách dịch vụ
const fetchServices = async () => {
  try {
    isLoading.value = true;
    const response = await serviceApi.getAll();
    if (response.data?.result?.isSuccess && response.data.result.data) {
      services.value = response.data.result.data.map((service: any) => ({
        id: service.id,
        gameInforID: service.gameInforID,
        serviceName: service.serviceName,
        createrID: service.createrID,
        decription: service.decription,
        serviceLevel: service.serviceLevel,
        servicePrice: service.servicePrice,
        serviceTime: service.serviceTime,
        rentedC: service.rentedC,
        feedback: service.feedback,
        image: service.image,
        isDelete: service.isDelete,
      }));
    } else {
      errorMessage.value = 'Không thể lấy danh sách dịch vụ';
    }
  } catch (error) {
    errorMessage.value = 'Lỗi khi gọi API dịch vụ';
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

// Gọi các hàm khi component được mounted
onMounted(() => {
  fetchGames().then(() => fetchAccounts());
  fetchServices();
});

// Hàm xử lý URL hình ảnh
const getFullImageUrls = (imageString: string | null | undefined): string[] => {
  if (!imageString || imageString.trim() === '') {
    return ['https://via.placeholder.com/400x250'];
  }
  const baseUrl = 'https://localhost:7232/';
  const images = imageString.split(';').filter(img => img.trim() !== '');
  return images.map(img => `${baseUrl}${img}`);
};

// Hàm định dạng thời gian tương đối
const formatRelativeTime = (dateStr: string): string => {
  if (!dateStr || dateStr === 'Không xác định') {
    return 'Thời gian không xác định';
  }

  const created = new Date(dateStr);
  if (isNaN(created.getTime())) {
    return 'Thời gian không hợp lệ';
  }

  const now = new Date();
  const diff = now.getTime() - created.getTime();
  const seconds = Math.floor(diff / 1000);
  const minutes = Math.floor(seconds / 60);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);
  const months = Math.floor(days / 30);
  const years = Math.floor(days / 365);
  if (seconds < 60) {
    return 'Vừa đăng';
  } else if (minutes < 60) {
    return `${minutes} phút trước`;
  } else if (hours < 24) {
    return `${hours} giờ trước`;
  } else if (days < 30) {
    return `${days} ngày trước`;
  } else if (months < 12) {
    return `${months} tháng trước`;
  } else {
    return `${years} năm trước`;
  }
};

// Hàm kiểm tra thời gian theo bộ lọc
const isWithinTimeFilter = (dateStr: string, filter: string): boolean => {
  if (!dateStr || dateStr === 'Không xác định') {
    return filter === 'all';
  }

  const created = new Date(dateStr);
  if (isNaN(created.getTime())) {
    return filter === 'all';
  }

  const now = new Date();
  const diff = now.getTime() - created.getTime();
  switch (filter) {
    case 'just_now':
      return diff < 60000;
    case 'minutes':
      return diff >= 60000 && diff < 3600000;
    case 'hours':
      return diff >= 3600000 && diff < 86400000;
    case 'days':
      return diff >= 86400000 && diff < 604800000;
    case 'months':
      return diff >= 604800000 && diff < 2592000000;
    case 'years':
      return diff >= 2592000000;
    default:
      return true;
  }
};

// Lọc danh sách tài khoản
const filteredAccounts = computed(() => {
  let result = accounts.value;
  if (accountsFilters.value.game !== 'All') {
    result = result.filter(account => account.game === accountsFilters.value.game);
  }
  if (accountsFilters.value.minPrice !== null) {
    result = result.filter(account => account.price >= accountsFilters.value.minPrice!);
  }
  if (accountsFilters.value.maxPrice !== null) {
    result = result.filter(account => account.price <= accountsFilters.value.maxPrice!);
  }
  if (accountsFilters.value.search) {
    const searchLower = accountsFilters.value.search.toLowerCase();
    result = result.filter(account =>
      account.game.toLowerCase().includes(searchLower) ||
      account.seller.toLowerCase().includes(searchLower) ||
      account.fields.some(field => field.fieldValue.toLowerCase().includes(searchLower))
    );
  }
  if (accountsFilters.value.timeFilter !== 'all') {
    result = result.filter(account => isWithinTimeFilter(account.createdDate, accountsFilters.value.timeFilter));
  }
  result.sort((a, b) => {
    if (a.status === 'Còn hàng' && b.status !== 'Còn hàng') return -1;
    if (a.status !== 'Còn hàng' && b.status === 'Còn hàng') return 1;
    return 0;
  });
  return result;
});

// Hiển thị danh sách tài khoản đã lọc
const displayedAccounts = computed(() => {
  console.log('Displayed accounts:', filteredAccounts.value);
  return filteredAccounts.value;
});

const accountsTotalPages = computed(() => totalAccountsPages.value);

// Lọc danh sách dịch vụ
const filteredServices = computed(() => {
  let result = services.value;
  if (servicesFilters.value.game !== 'All') {
    const gameId = games.value.find(g => g.name === servicesFilters.value.game)?.id;
    if (gameId) {
      result = result.filter(service => service.gameInforID === gameId);
    }
  }
  if (servicesFilters.value.minPrice !== null) {
    result = result.filter(service => service.servicePrice >= servicesFilters.value.minPrice!);
  }
  if (servicesFilters.value.maxPrice !== null) {
    result = result.filter(service => service.servicePrice <= servicesFilters.value.maxPrice!);
  }
  if (servicesFilters.value.search) {
    const searchLower = servicesFilters.value.search.toLowerCase();
    result = result.filter(service =>
      service.serviceName?.toLowerCase().includes(searchLower) ||
      service.decription?.toLowerCase().includes(searchLower)
    );
  }
  result = result.filter(service => {
    if (service.createdDate && servicesFilters.value.timeFilter !== 'all') {
      return isWithinTimeFilter(service.createdDate, servicesFilters.value.timeFilter);
    }
    return true;
  });
  return result;
});

// Hiển thị danh sách dịch vụ với phân trang
const displayedServices = computed(() => {
  const start = (servicesPage.value - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  return filteredServices.value.slice(start, end);
});

const servicesTotalPages = computed(() => Math.ceil(filteredServices.value.length / itemsPerPage));

// Theo dõi thay đổi bộ lọc để reset trang và tải lại dữ liệu
watch(accountsFilters, () => {
  accountsPage.value = 1;
  fetchAccounts();
}, { deep: true });

watch(servicesFilters, () => {
  servicesPage.value = 1;
}, { deep: true });

// Các hàm hỗ trợ
const isOwnAccount = (account: Account) => {
  return user.value && account.sellerId !== null && Number(account.sellerId) === user.value.id;
};

const isOwnService = (service: Service) => {
  return user.value && service.createrID === user.value.id;
};

// Hàm mua tài khoản
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

// Hàm xử lý thuê dịch vụ
const openConfirmModal = (id: number) => {
  selectedServiceId.value = id;
  showConfirmModal.value = true;
};

const closeConfirmModal = () => {
  showConfirmModal.value = false;
};

const proceedToDescription = () => {
  showConfirmModal.value = false;
  showDescriptionModal.value = true;
};

const closeDescriptionModal = () => {
  showDescriptionModal.value = false;
  descriptionInput.value = '';
};

const rentService = async () => {
  if (!selectedServiceId.value) return;

  try {
    isLoading.value = true;
    const model = {
      Id: selectedServiceId.value,
      decription: descriptionInput.value,
    };

    const response = await serviceApi.rentService(model);
    if (response.data?.result?.isSuccess) {
      purchaseStatus.value = 'success';
      purchaseMessage.value = 'Thuê dịch vụ thành công!';
      await fetchServices();
    } else {
      purchaseStatus.value = 'error';
      purchaseMessage.value = response.data?.result?.message || 'Lỗi không xác định';
    }
  } catch (error) {
    purchaseStatus.value = 'error';
    purchaseMessage.value = 'Lỗi khi gọi API thuê dịch vụ';
    console.error('Lỗi thuê dịch vụ:', error);
  } finally {
    isLoading.value = false;
    showDescriptionModal.value = false;
    showPurchaseModal.value = true;
    descriptionInput.value = '';
  }
};
</script>

<template>
  <div class="cosmo-trade-zone">
    <!-- Hiệu ứng nền không gian -->
    <div class="stars-container">
      <div class="stars stars-small"></div>
      <div class="stars stars-medium"></div>
      <div class="stars stars-large"></div>
    </div>
    <div class="nebula-bg"></div>
    <div class="planet planet-1"></div>
    <div class="planet planet-2"></div>

    <!-- Banner trên cùng -->
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

    <!-- Tab điều hướng -->
    <div class="tab-navigation">
      <button :class="{ active: activeTab === 'accounts' }" @click="activeTab = 'accounts'">Tài khoản Game</button>
      <button :class="{ active: activeTab === 'services' }" @click="activeTab = 'services'">Dịch vụ Game</button>
    </div>

    <!-- Nội dung chính -->
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

      <div v-if="!isLoading && !errorMessage">
        <!-- Tab Tài khoản Game -->
        <div v-if="activeTab === 'accounts'">
          <div class="filter-bar">
            <select v-model="accountsFilters.game">
              <option value="All">Tất cả game</option>
              <option v-for="game in games" :key="game.id" :value="game.name">{{ game.name }}</option>
            </select>
            <input type="number" v-model="accountsFilters.minPrice" placeholder="Giá tối thiểu">
            <input type="number" v-model="accountsFilters.maxPrice" placeholder="Giá tối đa">
            <input type="text" v-model="accountsFilters.search" placeholder="Tìm kiếm...">
            <select v-model="accountsFilters.timeFilter">
              <option value="all">Tất cả</option>
              <option value="just_now">Vừa đăng</option>
              <option value="minutes">Vài phút trước</option>
              <option value="hours">Vài tiếng trước</option>
              <option value="days">Vài ngày trước</option>
              <option value="months">Vài tháng trước</option>
              <option value="years">Vài năm trước</option>
            </select>
          </div>
          <div class="card-grid">
            <div v-for="account in displayedAccounts" :key="account.id" class="account-card" :class="{ 'sold': account.status === 'Đã bán' }">
              <div class="card-banner">
                <img :src="account.images[0]" :alt="account.game" class="card-img">
                <div class="game-badge">{{ account.game }}</div>
                <div v-if="account.status === 'Đã bán'" class="sold-tag">Đã bán</div>
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
                  <div class="timer-layout">
                    <i class="fas fa-clock time"></i>
                    <span style="margin-left: 5px">{{ formatRelativeTime(account.createdDate) }}</span>
                  </div>
                </div>
                <div class="account-details">
                  <div v-for="field in showMoreDetails.includes(account.id) ? account.fields : account.fields.slice(0, 3)" :key="field.fieldName" class="detail-item">
                    <span class="detail-label">{{ field.fieldName }}:</span>
                    <span class="detail-value">{{ field.fieldValue }}</span>
                  </div>
                  <div v-if="account.fields.length > 3" class="more-details">
                    <button class="toggle-details-btn" @click="toggleMoreDetails(account.id)">
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
                <div class="card-actions" v-if="!isOwnAccount(account) && account.status !== 'Đã bán'">
                  <button class="bid-btn">
                    <i class="fas fa-gavel"></i> Trả giá
                  </button>
                  <button class="buy-btn" @click="buyAccount(account.id)">
                    <i class="fas fa-shopping-cart"></i> Mua ngay
                  </button>
                </div>
                <div v-else-if="account.status === 'Đã bán'" class="sold-message">
                  <i class="fas fa-times-circle"></i> Đã bán
                </div>
                <div v-else class="own-account-message">
                  <i class="fas fa-info-circle"></i> Tài khoản của bạn
                </div>
              </div>
            </div>
          </div>
          <div class="pagination">
            <button v-for="page in accountsTotalPages" :key="page" @click="accountsPage = page; fetchAccounts()" :class="{ active: accountsPage === page }">{{ page }}</button>
          </div>
        </div>

        <!-- Tab Dịch vụ Game -->
        <div v-else>
          <div class="filter-bar">
            <select v-model="servicesFilters.game">
              <option value="All">Tất cả game</option>
              <option v-for="game in games" :key="game.id" :value="game.name">{{ game.name }}</option>
            </select>
            <input type="number" v-model="servicesFilters.minPrice" placeholder="Giá tối thiểu">
            <input type="number" v-model="servicesFilters.maxPrice" placeholder="Giá tối đa">
            <input type="text" v-model="servicesFilters.search" placeholder="Tìm kiếm...">
            <select v-model="servicesFilters.timeFilter">
              <option value="all">Tất cả</option>
              <option value="just_now">Vừa đăng</option>
              <option value="minutes">Vài phút trước</option>
              <option value="hours">Vài tiếng trước</option>
              <option value="days">Vài ngày trước</option>
              <option value="months">Vài tháng trước</option>
              <option value="years">Vài năm trước</option>
            </select>
          </div>
          <div class="card-grid">
            <div v-for="service in displayedServices" :key="service.id" class="service-card">
              <div class="service-header">
                <h3 class="service-title">{{ service.serviceName }}</h3>
                <div class="service-game">{{ games.find(g => g.id === service.gameInforID)?.name }}</div>
              </div>
              <div class="service-content">
                <div class="creator-info">
                  <i class="fas fa-user-astronaut"></i>
                  <span>{{ service.createrID }}</span>
                </div>
                <p class="service-desc">{{ service.decription }}</p>
                <div class="service-price">
                  <span class="price-label">Giá dịch vụ:</span>
                  <span class="price-value">{{ service.servicePrice.toLocaleString() }} VNĐ</span>
                </div>
                <button v-if="!isOwnService(service)" class="hire-btn" @click="openConfirmModal(service.id)">
                  <i class="fas fa-handshake"></i> Thuê ngay
                </button>
                <div v-else class="own-service-message">
                  <i class="fas fa-info-circle"></i> Dịch vụ của bạn
                </div>
              </div>
            </div>
          </div>
          <div class="pagination">
            <button v-for="page in servicesTotalPages" :key="page" @click="servicesPage = page" :class="{ active: servicesPage === page }">{{ page }}</button>
          </div>
        </div>
      </div>
    </main>

    <!-- Banner khuyến mãi -->
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

    <!-- Modal xác nhận thuê dịch vụ -->
    <teleport to="body">
      <div v-if="showConfirmModal" class="confirm-modal" @click="closeConfirmModal">
        <div class="modal-content animated" @click.stop>
          <h3>Xác nhận thuê dịch vụ</h3>
          <p>Bạn có chắc chắn muốn thuê dịch vụ này không?</p>
          <div class="modal-actions">
            <button class="modal-btn cancel-btn" @click="closeConfirmModal">Không</button>
            <button class="modal-btn confirm-btn" @click="proceedToDescription">Có</button>
          </div>
        </div>
      </div>
    </teleport>

    <!-- Modal nhập mô tả cho dịch vụ -->
    <teleport to="body">
      <div v-if="showDescriptionModal" class="description-modal" @click="closeDescriptionModal">
        <div class="modal-content animated" @click.stop>
          <h3>Nhập thông tin tài khoản game</h3>
          <textarea v-model="descriptionInput" placeholder="Nhập mô tả của bạn (ví dụ: thông tin tài khoản, yêu cầu cụ thể,...)"></textarea>
          <div class="modal-actions">
            <button class="modal-btn cancel-btn" @click="closeDescriptionModal">Đóng</button>
            <button class="modal-btn confirm-btn" @click="rentService">Gửi</button>
          </div>
        </div>
      </div>
    </teleport>

    <!-- Modal thông báo mua hàng -->
    <teleport to="body">
      <div v-if="showPurchaseModal" class="purchase-modal" @click="closePurchaseModal">
        <div class="modal-content animated" @click.stop>
          <button class="close-btn" @click="closePurchaseModal">×</button>
          <div class="modal-body">
            <i :class="purchaseStatus === 'success' ? 'fas fa-check-circle' : 'fas fa-exclamation-triangle'" class="modal-icon"></i>
            <h3>{{ purchaseStatus === 'success' ? 'Thành công!' : 'Thất bại!' }}</h3>
            <p>{{ purchaseMessage }}</p>
            <div class="modal-actions">
              <button class="modal-btn confirm-btn" @click="closePurchaseModal">Đóng</button>
            </div>
          </div>
        </div>
      </div>
    </teleport>

    <!-- Modal xem hình ảnh -->
    <teleport to="body">
      <div v-if="showImageModal && selectedAccount" class="game-image-modal" @click="closeImageModal">
        <div class="game-modal-container" @click.stop>
          <div class="game-modal-header">
            <div class="game-modal-title">
              <div class="game-badge-modal">{{ selectedAccount.game }}</div>
            </div>
            <button class="game-close-btn" @click="closeImageModal">
              <i class="fas fa-times"></i>
            </button>
          </div>
          <div class="game-modal-body">
            <div class="game-gallery">
              <div class="game-main-image-wrapper">
                <div class="game-image-frame">
                  <img :src="selectedAccount.images[currentImageIndex]" :alt="'Hình ' + (currentImageIndex + 1)" class="game-main-image">
                  <div class="game-image-overlay"></div>
                  <div class="game-corner top-left"></div>
                  <div class="game-corner top-right"></div>
                  <div class="game-corner bottom-left"></div>
                  <div class="game-corner bottom-right"></div>
                </div>
                <button v-if="selectedAccount.images.length > 1 && currentImageIndex > 0" class="game-nav-btn prev-btn" @click.stop="prevImage">
                  <i class="fas fa-chevron-left"></i>
                </button>
                <button v-if="selectedAccount.images.length > 1 && currentImageIndex < selectedAccount.images.length - 1" class="game-nav-btn next-btn" @click.stop="nextImage">
                  <i class="fas fa-chevron-right"></i>
                </button>
                <div class="game-image-counter">
                  <span class="current-index">{{ currentImageIndex + 1 }}</span>
                  <span class="separator">/</span>
                  <span class="total-images">{{ selectedAccount.images.length }}</span>
                </div>
              </div>
              <div v-if="selectedAccount.images.length > 1" class="game-thumbnails">
                <div v-for="(image, index) in selectedAccount.images" :key="index" class="game-thumbnail" :class="{ active: index === currentImageIndex }" @click.stop="currentImageIndex = index">
                  <div class="game-thumb-frame">
                    <img :src="image" :alt="'Thumbnail ' + (index + 1)">
                    <div class="game-thumb-overlay"></div>
                  </div>
                </div>
              </div>
            </div>
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
              <div class="game-action-buttons" v-if="!isOwnAccount(selectedAccount) && selectedAccount.status !== 'Đã bán'">
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
              <div v-else-if="selectedAccount.status === 'Đã bán'" class="sold-message">
                <i class="fas fa-times-circle"></i> Đã bán
              </div>
              <div v-else class="own-account-message">
                <i class="fas fa-info-circle"></i> Tài khoản của bạn
              </div>
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

/* Hiệu ứng nền không gian */
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
  from { transform: translateY(0); }
  to { transform: translateY(-200px); }
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
  0%, 100% { opacity: 0.7; }
  50% { opacity: 0.4; }
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

/* Tab điều hướng */
.tab-navigation {
  display: flex;
  justify-content: center;
  gap: 20px;
  margin: 40px 0 20px;
  position: relative;
  z-index: 2;
}

.tab-navigation button {
  padding: 12px 25px;
  background: rgba(26, 34, 52, 0.6);
  color: #e1e7ef;
  border: 1px solid rgba(122, 78, 254, 0.2);
  border-radius: 8px;
  cursor: pointer;
  font-size: 1.1rem;
  font-weight: 600;
  transition: all 0.3s ease;
}

.tab-navigation button.active {
  background: linear-gradient(to right, #7A4EFE, #00E0AA);
  color: white;
  border-color: #7A4EFE;
  box-shadow: 0 0 15px rgba(122, 78, 254, 0.5);
}

.tab-navigation button:hover {
  background: rgba(122, 78, 254, 0.2);
  border-color: #7A4EFE;
}

/* Filter Bar */
.filter-bar {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 24px;
  background: rgba(26, 34, 52, 0.6);
  border-radius: 12px;
  padding: 16px;
  border: 1px solid rgba(122, 78, 254, 0.2);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.filter-bar select,
.filter-bar input {
  background: rgba(10, 14, 23, 0.8);
  border: 1px solid rgba(122, 78, 254, 0.3);
  border-radius: 8px;
  color: #e1e7ef;
  padding: 10px 16px;
  font-size: 14px;
  outline: none;
  flex: 1;
  min-width: 140px;
  transition: all 0.3s ease;
}

.filter-bar select {
  cursor: pointer;
  appearance: none;
  background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23e1e7ef' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpath d='M6 9l6 6 6-6'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 12px center;
  padding-right: 36px;
}

.filter-bar select:focus,
.filter-bar input:focus {
  border-color: #7A4EFE;
  box-shadow: 0 0 8px rgba(122, 78, 254, 0.4);
}

/* Phân trang */
.pagination {
  display: flex;
  justify-content: center;
  gap: 8px;
  margin-top: 32px;
  margin-bottom: 32px;
}

.pagination button {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  background: rgba(26, 34, 52, 0.6);
  border: 1px solid rgba(122, 78, 254, 0.2);
  color: #e1e7ef;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.pagination button:hover {
  background: rgba(122, 78, 254, 0.2);
  border-color: #7A4EFE;
}

.pagination button.active {
  background: #7A4EFE;
  border-color: #7A4EFE;
  color: white;
  box-shadow: 0 0 10px rgba(122, 78, 254, 0.5);
}

/* Banner trên cùng */
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
  0% { background-position: 0% 0%, 0% 0%, 0% 0%, 0% 0%; }
  100% { background-position: 10% 20%, -10% 10%, 5% -5%, -5% 5%; }
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
  0% { background-position: 0 0; }
  100% { background-position: 0 100%; }
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
  from { opacity: 0; transform: translateX(-50px); }
  to { opacity: 1; transform: translateX(0); }
}

@keyframes fadeInRight {
  from { opacity: 0; transform: translateX(50px); }
  to { opacity: 1; transform: translateX(0); }
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
  0% { transform: rotate(-15deg) translateY(0); }
  100% { transform: rotate(-12deg) translateY(-20px); }
}

@keyframes floatCard2 {
  0% { transform: rotate(5deg) translateY(0); }
  100% { transform: rotate(8deg) translateY(-15px); }
}

@keyframes floatCard3 {
  0% { transform: rotate(-5deg) translateY(0); }
  100% { transform: rotate(-8deg) translateY(-25px); }
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
  padding: 16px;
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
  0% { transform: translateY(-50%) rotate(0deg); }
  100% { transform: translateY(-50%) rotate(360deg); }
}

@keyframes rotateSphereGradient {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(-360deg); }
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

/* Nội dung chính */
.main-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 20px;
  position: relative;
  z-index: 2;
  min-height: 180vh;
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
  0% { transform: scale(1); opacity: 0.7; }
  100% { transform: scale(1.1); opacity: 1; }
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
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 24px;
  margin-bottom: 32px;
  overflow: hidden; /* Loại bỏ hoàn toàn scrollbar */
}

/* Account Card Styling */
.account-card {
  background: linear-gradient(135deg, #1a1a2e, #16213e);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  position: relative;
  border: 1px solid rgba(122, 78, 254, 0.2);
}

.account-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.3), 0 0 20px rgba(122, 78, 254, 0.4);
}

.account-card.sold {
  opacity: 0.75;
}

.card-banner {
  position: relative;
  height: 180px;
  overflow: hidden;
}

.card-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.account-card:hover .card-img {
  transform: scale(1.05);
}

.game-badge {
  position: absolute;
  top: 12px;
  left: 12px;
  background: linear-gradient(to right, #7A4EFE, #00E0AA);
  color: #fff;
  font-size: 12px;
  font-weight: 600;
  padding: 6px 12px;
  border-radius: 20px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.sold-tag {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  font-size: 24px;
  font-weight: 700;
  text-transform: uppercase;
}

.sold-tag::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: repeating-linear-gradient(
    45deg,
    rgba(255, 0, 85, 0.1),
    rgba(255, 0, 85, 0.1) 10px,
    rgba(0, 0, 0, 0.2) 10px,
    rgba(0, 0, 0, 0.2) 20px
  );
}

.view-more-images {
  position: absolute;
  bottom: 12px;
  right: 12px;
  background: rgba(0, 0, 0, 0.7);
  color: #fff;
  border: none;
  border-radius: 8px;
  padding: 8px 16px;
  font-size: 13px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: background 0.3s ease;
  overflow: hidden;
}

.view-more-images:hover {
  background: rgba(122, 78, 254, 0.7);
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

.view-more-images:hover .btn-glow {
  opacity: 0.3;
  animation: rotate 10s linear infinite;
}

.card-content {
  padding: 16px;
}

.seller-info {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
  font-size: 14px;
  color: #b0b5c3;
}

.seller-info i {
  font-size: 16px;
  color: #7A4EFE;
}

.account-details {
  margin-bottom: 16px;
}

.detail-item {
  display: flex;
  align-items: flex-start;
  margin-bottom: 8px;
  font-size: 14px;
}

.detail-label {
  flex: 0 0 40%;
  color: #b0b5c3;
  font-weight: 500;
}

.detail-value {
  flex: 0 0 60%;
  color: #e1e7ef;
  font-weight: 500;
  word-break: break-word;
}

.more-details {
  margin-top: 12px;
}

.toggle-details-btn {
  background: none;
  border: none;
  color: #7A4EFE;
  font-size: 13px;
  cursor: pointer;
  padding: 0;
  display: inline-flex;
  align-items: center;
  text-decoration: underline;
  transition: color 0.3s ease;
}

.toggle-details-btn:hover {
  color: #00E0AA;
}

.price-container {
  background: rgba(0, 0, 0, 0.2);
  border-radius: 8px;
  padding: 12px;
  margin-bottom: 16px;
  border: 1px solid #7A4EFE;
}

.price,
.min-price {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.price {
  margin-bottom: 8px;
}

.price-label {
  font-size: 14px;
  color: #b0b5c3;
}

.price .price-value {
  font-size: 18px;
  font-weight: 700;
  color: #00E0AA;
}

.min-price .price-value {
  font-size: 14px;
  font-weight: 600;
  color: #e1e7ef;
}

.card-actions {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}

.bid-btn,
.buy-btn {
  padding: 10px 8px;
  border-radius: 8px;
  border: none;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  transition: background 0.3s ease, transform 0.3s ease;
}

.bid-btn {
  background: #7A4EFE;
  color: #fff;
}

.bid-btn:hover {
  background: #5a3ece;
  transform: translateY(-2px);
}

.buy-btn {
  background: #00E0AA;
  color: #fff;
}

.buy-btn:hover {
  background: #00b38f;
  transform: translateY(-2px);
}

.sold-message,
.own-account-message {
  text-align: center;
  color: #ff4b4b;
  font-weight: 600;
  margin-top: 16px;
}

.own-account-message {
  color: #00E0AA;
}

.timer-layout {
  margin-left: auto;
}

/* Service Card */
.service-card {
  background: linear-gradient(135deg, #1a1a2e, #16213e);
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  position: relative;
  border: 1px solid rgba(0, 224, 170, 0.2);
}

.service-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.3), 0 0 20px rgba(0, 224, 170, 0.4);
}

.service-header {
  padding: 15px;
  background: linear-gradient(to right, #7A4EFE, #00E0AA);
  border-bottom: 1px solid #00E0AA;
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
  color: #fff;
  font-size: 0.8rem;
  border-radius: 5px;
  margin-top: 5px;
}

.service-content {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 15px;
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
  background: linear-gradient(to right, #00E0AA, #00b38f);
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

.own-service-message {
  text-align: center;
  color: #00E0AA;
  font-weight: 600;
  margin-top: 16px;
}

/* Banner khuyến mãi */
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
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 20px;
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

/* Modal Styling */
.confirm-modal,
.description-modal,
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
  background: #0a0e17;
  padding: 30px;
  border-radius: 15px;
  border: 1px solid #7A4EFE;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.5), 0 0 20px rgba(122, 78, 254, 0.3);
}

.modal-content h3 {
  font-size: 1.5rem;
  margin-bottom: 15px;
  color: #fff;
  text-shadow: 0 0 5px rgba(122, 78, 254, 0.5);
}

.modal-content p {
  margin-bottom: 20px;
  color: #e1e7ef;
}

.modal-content textarea {
  width: 100%;
  height: 100px;
  padding: 10px;
  border: 1px solid #7A4EFE;
  background: rgba(26, 34, 52, 0.6);
  color: #e1e7ef;
  border-radius: 8px;
  resize: none;
  transition: border-color 0.3s ease;
}

.modal-content textarea:focus {
  border-color: #00E0AA;
  outline: none;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

.modal-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
}

.cancel-btn {
  background: #2c3e50;
  color: #e1e7ef;
}

.cancel-btn:hover {
  background: #34495e;
  transform: translateY(-2px);
}

.confirm-btn {
  background: #7A4EFE;
  color: white;
}

.confirm-btn:hover {
  background: #5a3ece;
  transform: translateY(-2px);
}

.close-btn {
  position: absolute;
  top: 10px;
  right: 10px;
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #e1e7ef;
  cursor: pointer;
  transition: all 0.3s ease;
}

.close-btn:hover {
  color: #ff4b4b;
  transform: rotate(90deg);
}

.modal-icon {
  font-size: 3rem;
  margin-bottom: 15px;
}

.modal-icon.fa-check-circle {
  color: #00E0AA;
}

.modal-icon.fa-exclamation-triangle {
  color: #ff4b4b;
}

.animated {
  animation: fadeInScale 0.3s ease;
}

@keyframes fadeInScale {
  from { opacity: 0; transform: scale(0.9); }
  to { opacity: 1; transform: scale(1); }
}

/* Modal xem hình ảnh */
.game-image-modal {
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
  padding: 20px;
}

.game-modal-container {
  width: 90%;
  max-width: 1200px;
  max-height: 90vh;
  background: #0a0e17;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 0 40px rgba(122, 78, 254, 0.3);
  border: 1px solid #7A4EFE;
  display: flex;
  flex-direction: column;
}

.game-modal-header {
  background: linear-gradient(to right, #000000, #000000);
  padding: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #00E0AA;
}

.game-modal-title {
  display: flex;
  align-items: center;
  gap: 15px;
}

.game-modal-title h3 {
  color: #fff;
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0;
}

.game-badge-modal {
  background: #00E0AA;
  color: #fff;
  padding: 5px 15px;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 600;
  border: 1px solid #00E0AA;
  box-shadow: 0 0 10px rgba(0, 224, 170, 0.3);
}

.game-close-btn {
  background: #ff4b4b;
  color: #fff;
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
  background: #ff1a1a;
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
  padding-top: 56.25%;
  background: #000;
  border: 1px solid #7A4EFE;
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
  background: linear-gradient(135deg, rgba(122, 78, 254, 0.05) 0%, transparent 50%, rgba(0, 224, 170, 0.05) 100%);
  pointer-events: none;
}

.game-corner {
  position: absolute;
  width: 20px;
  height: 20px;
  border-color: #00E0AA;
  z-index: 2;
}

.top-left { top: 0; left: 0; border-top: 2px solid; border-left: 2px solid; }
.top-right { top: 0; right: 0; border-top: 2px solid; border-right: 2px solid; }
.bottom-left { bottom: 0; left: 0; border-bottom: 2px solid; border-left: 2px solid; }
.bottom-right { bottom: 0; right: 0; border-bottom: 2px solid; border-right: 2px solid; }

.game-nav-btn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(0, 0, 0, 0.7);
  border: 1px solid #7A4EFE;
  color: #fff;
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

.prev-btn { left: 20px; }
.next-btn { right: 20px; }

.game-nav-btn:hover {
  background: rgba(122, 78, 254, 0.7);
  transform: translateY(-50%) scale(1.1);
  box-shadow: 0 0 20px rgba(122, 78, 254, 0.5);
}

.game-image-counter {
  position: absolute;
  bottom: 20px;
  right: 20px;
  background: rgba(0, 0, 0, 0.7);
  border: 1px solid #7A4EFE;
  border-radius: 30px;
  padding: 8px 15px;
  color: #fff;
  font-size: 0.9rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
  z-index: 5;
}

.current-index { color: #00E0AA; }
.separator { color: rgba(255, 255, 255, 0.5); }

.game-thumbnails {
  display: flex;
  gap: 10px;
  overflow-x: auto;
  padding: 5px 0;
  scrollbar-width: thin;
  scrollbar-color: #00E0AA #0a0e17;
}

.game-thumbnails::-webkit-scrollbar {
  height: 5px;
}

.game-thumbnails::-webkit-scrollbar-thumb {
  background: #00E0AA;
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
  box-shadow: 0 0 15px rgba(0, 224, 170, 0.5);
}

.game-thumb-frame {
  position: relative;
  width: 100%;
  height: 100%;
  border: 1px solid #7A4EFE;
  border-radius: 8px;
  overflow: hidden;
}

.game-thumbnail.active .game-thumb-frame {
  border-color: #00E0AA;
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
  background: linear-gradient(to bottom, rgba(0, 0, 0, 0.1), rgba(0, 0, 0, 0.3));
  pointer-events: none;
}

.game-account-info {
  flex: 1;
  padding: 20px;
  background: #0a0e17;
  border-left: 1px solid #7A4EFE;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

@media (max-width: 991px) {
  .game-account-info {
    border-left: none;
    border-top: 1px solid #7A4EFE;
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
  background: #7A4EFE;
  color: #fff;
  padding: 8px 15px;
  border-radius: 30px;
  font-size: 0.9rem;
  font-weight: 600;
  border: 1px solid #7A4EFE;
  display: flex;
  align-items: center;
  gap: 8px;
}

.game-status-badge {
  background: #00E0AA;
  color: #fff;
  padding: 8px 15px;
  border-radius: 30px;
  font-size: 0.9rem;
  font-weight: 600;
  border: 1px solid #00E0AA;
}

.game-status-badge.sold-status {
  background: #ff4b4b;
  border-color: #ff4b4b;
}

.game-price-section {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.game-price-box {
  flex: 1;
  min-width: 200px;
  background: #1a1a2e;
  padding: 15px;
  border-radius: 12px;
  border: 1px solid #7A4EFE;
  transition: all 0.3s ease;
}

.game-price-box:hover {
  background: #16213e;
  transform: translateY(-3px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
  border-color: #00E0AA;
}

.game-price-box.min-price {
  background: #16213e;
  border-color: #00E0AA;
}

.game-price-box.min-price:hover {
  background: #1a1a2e;
  border-color: #7A4EFE;
}

.game-price-label {
  font-size: 0.8rem;
  color: #b0b5c3;
  margin-bottom: 5px;
}

.game-price-value {
  font-size: 1.3rem;
  font-weight: 700;
  color: #00E0AA;
}

.game-price-box.min-price .game-price-value {
  color: #7A4EFE;
}

.game-price-value span {
  font-size: 0.9rem;
  opacity: 0.8;
}

.game-fields-container {
  background: #16213e;
  border-radius: 12px;
  padding: 20px;
  border: 1px solid #7A4EFE;
}

.game-fields-header {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid #7A4EFE;
  color: #00E0AA;
  font-size: 1.1rem;
  font-weight: 600;
}

.game-fields-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 15px;
}

.game-field-item {
  background: #1a1a2e;
  padding: 12px;
  border-radius: 8px;
  transition: all 0.3s ease;
  border: 1px solid #7A4EFE;
}

.game-field-item:hover {
  background: #16213e;
  transform: translateY(-3px);
  border-color: #00E0AA;
}

.field-name {
  font-size: 0.8rem;
  color: #b0b5c3;
  margin-bottom: 5px;
}

.field-value {
  font-size: 1rem;
  color: #e1e7ef;
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
  background: #7A4EFE;
  color: #fff;
}

.bid-btn:hover {
  background: #5a3ece;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3), 0 0 15px rgba(122, 78, 254, 0.3);
}

.buy-btn {
  background: #00E0AA;
  color: #fff;
}

.buy-btn:hover {
  background: #00b38f;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.3), 0 0 15px rgba(0, 224, 170, 0.3);
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

/* Responsive */
@media (max-width: 1200px) {
  .banner-title { font-size: 4rem; }
  .floating-card { width: 180px; height: 250px; }
  .card-2 { left: 100px; }
  .holographic-sphere { width: 250px; height: 250px; }
  .card-grid { grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); }
}

@media (max-width: 992px) {
  .top-banner { height: auto; padding: 80px 0; }
  .banner-content-wrapper { flex-direction: column; gap: 60px; }
  .banner-left { max-width: 100%; text-align: center; }
  .banner-title { font-size: 3.5rem; align-items: center; }
  .banner-stats { justify-content: center; }
  .banner-buttons { justify-content: center; }
  .banner-right { height: 400px; width: 100%; }
  .floating-cards-container { position: relative; height: 350px; }
  .card-1 { left: 50%; transform: translateX(-150px) rotate(-15deg); }
  .card-2 { left: 50%; transform: translateX(-50px) rotate(5deg); }
  .card-3 { left: 50%; transform: translateX(50px) rotate(-5deg); }
  .holographic-sphere { right: 50%; transform: translate(50%, -50%); width: 200px; height: 200px; opacity: 0.7; }
}

@media (max-width: 768px) {
  .top-banner { padding: 60px 0; }
  .banner-title { font-size: 3rem; }
  .banner-subtitle { font-size: 1.2rem; }
  .banner-stats { flex-wrap: wrap; justify-content: center; }
  .banner-buttons { flex-direction: column; gap: 15px; align-items: center; }
  .banner-btn { width: 100%; max-width: 300px; justify-content: center; }
  .floating-card { width: 150px; height: 220px; padding: 15px; }
  .card-1 { transform: translateX(-120px) rotate(-15deg); }
  .card-3 { transform: translateX(20px) rotate(-5deg); }
  .card-grid { grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); }
  .filter-bar { flex-direction: column; align-items: stretch; }
  .filter-bar select, .filter-bar input { min-width: unset; width: 100%; }
}

@media (max-width: 576px) {
  .banner-title { font-size: 2.5rem; }
  .banner-subtitle { font-size: 1rem; }
  .stat-item { min-width: 80px; padding: 10px; }
  .stat-value { font-size: 1.5rem; }
  .stat-label { font-size: 0.8rem; }
  .floating-card { width: 120px; height: 180px; padding: 10px; }
  .card-1 { transform: translateX(-90px) rotate(-15deg); }
  .card-2 { transform: translateX(-30px) rotate(5deg); }
  .card-3 { transform: translateX(30px) rotate(-5deg); }
  .holographic-sphere { width: 150px; height: 150px; }
}

@keyframes rotate {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: #050A15;
}

::-webkit-scrollbar-thumb {
  background: #7A4EFE;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: #00E0AA;
}
</style>
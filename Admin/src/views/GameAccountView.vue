<template>
    <div class="container">
      <h2>Danh sách tài khoản game</h2>
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th>STT</th>
              <th>Tên game</th>
              <th>Account Name</th>
              <th>Password</th>
              <th>Hình ảnh</th>
              <th>Trạng thái</th>
              <th>Ngày tạo</th>
              <th>Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(account, index) in gameAccounts" :key="account.id">
              <td>{{ index + 1 }}</td>
              <td>
                <div class="game-info">
                  <div class="game-badge">{{ account.gameName?.charAt(0) || '?' }}</div>
                  <span>{{ account.gameName }}</span>
                </div>
              </td>
              <td>{{ account.accountName }}</td>
              <td>
                <div class="password-field">
                  <span>{{ account.password }}</span>
                </div>
              </td>
              <td>
                <button
                  class="view-images-btn"
                  @click="openModal(account.images)"
                  :disabled="account.images.length === 0"
                  :class="{ 'no-images': account.images.length === 0 }"
                >
                  <i class="fas fa-images"></i>
                  {{ account.images.length > 0 ? `${account.images.length} hình` : 'Không có hình' }}
                </button>
              </td>
              <td>
                <span class="status-badge" :class="{ 'confirmed': account.isCheck }">
                  {{ account.isCheck ? 'Đã xác nhận' : 'Chưa xác nhận' }}
                </span>
              </td>
              <td>{{ account.createdDate }}</td>
              <td>
                <button
                  v-if="account.id"
                  @click="confirmCheckAccount(account.id)"
                  class="action-btn"
                  :disabled="account.isCheck ?? false"
                >
                  Kiểm tra tài khoản
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
  
      <!-- Modal for images -->
      <transition name="modal">
        <div v-if="isModalOpen" class="modal-backdrop" @click="closeModal">
          <div class="modal-content" @click.stop>
            <div class="modal-header">
              <h3>Xem hình ảnh</h3>
              <button class="close-btn" @click="closeModal">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <div class="main-image-container">
                <img :src="currentImages[currentImageIndex]" alt="Game Image" class="main-image" />
              </div>
              <div class="navigation-bar">
                <button class="nav-btn prev-btn" @click="prevImage" :disabled="currentImages.length <= 1">
                  <i class="fas fa-chevron-left"></i>
                </button>
                <div class="image-counter">
                  {{ currentImageIndex + 1 }} / {{ currentImages.length }}
                </div>
                <button class="nav-btn next-btn" @click="nextImage" :disabled="currentImages.length <= 1">
                  <i class="fas fa-chevron-right"></i>
                </button>
              </div>
              <div class="thumbnails-container" v-if="currentImages.length > 1">
                <div
                  v-for="(image, index) in currentImages"
                  :key="index"
                  class="thumbnail"
                  :class="{ 'active': index === currentImageIndex }"
                  @click="selectImage(index)"
                >
                  <img :src="image" :alt="`Thumbnail ${index + 1}`" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </transition>
  
      <!-- Modal for check account result -->
      <transition name="modal">
        <div v-if="isMessageModalOpen" class="modal-backdrop" @click="closeMessageModal">
          <div class="modal-content message-modal" @click.stop>
            <div class="modal-header" :class="{ 'error-header': isError, 'success-header': !isError }">
              <h3>{{ isError ? 'Lỗi' : 'Thành công' }}</h3>
              <button class="close-btn" @click="closeMessageModal">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <p>{{ messageContent }}</p>
            </div>
          </div>
        </div>
      </transition>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref, onMounted } from 'vue';
  import accountGameApi from '@/api/gameaccount.api';
  import gameInforApi from '@/api/gameinfor.api';
  import type { GameAccount } from '@/models/gameaccount.model';
  
  // Adjust the type to explicitly define createdDate as string
  const gameAccounts = ref<(Omit<GameAccount, 'createdDate'> & { gameName?: string; images: string[]; createdDate: string })[]>([]);
  const isModalOpen = ref(false);
  const currentImages = ref<string[]>([]);
  const currentImageIndex = ref(0);
  const isMessageModalOpen = ref(false);
  const messageContent = ref<string>('');
  const isError = ref<boolean>(false);
  
  const getFullImageUrl = (imageString: string | null | undefined): string[] => {
    if (!imageString || imageString.trim() === '') return [];
    const baseUrl = 'https://localhost:7232/';
    const images = imageString.split(';').filter(img => img.trim() !== '');
    return images.map(img => `${baseUrl}${img}`);
  };
  
  const openModal = (images: string[]) => {
    if (images.length === 0) return;
    currentImages.value = images;
    currentImageIndex.value = 0;
    isModalOpen.value = true;
    document.body.style.overflow = 'hidden';
  };
  
  const closeModal = () => {
    isModalOpen.value = false;
    document.body.style.overflow = '';
  };
  
  const prevImage = () => {
    if (currentImages.value.length <= 1) return;
    currentImageIndex.value =
      currentImageIndex.value > 0 ? currentImageIndex.value - 1 : currentImages.value.length - 1;
  };
  
  const nextImage = () => {
    if (currentImages.value.length <= 1) return;
    currentImageIndex.value =
      currentImageIndex.value < currentImages.value.length - 1 ? currentImageIndex.value + 1 : 0;
  };
  
  const selectImage = (index: number) => {
    currentImageIndex.value = index;
  };
  
  // Function to format date as "dd/mm/yyyy hh:mm:ss"
  const formatDate = (date: Date | string | null): string => {
    if (!date) return '';
    const d = typeof date === 'string' ? new Date(date) : date;
    if (isNaN(d.getTime())) return ''; // Handle invalid date
    return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()} ${d.getHours().toString().padStart(2, '0')}:${d.getMinutes().toString().padStart(2, '0')}:${d.getSeconds().toString().padStart(2, '0')}`;
  };
  
  const fetchGameAccounts = async () => {
    try {
      const response = await accountGameApi.getAll();
      let accounts = (response.data.result.data ?? []).map((account: GameAccount) => ({
        ...account,
        images: getFullImageUrl(account.image),
        createdDate: formatDate(account.createdDate ?? null),
      }));
  
      if (!accounts) {
        console.error('Dữ liệu tài khoản không tồn tại');
        return;
      }
  
      for (const account of accounts) {
        if (account.gameInforID) {
          const gameResponse = await gameInforApi.getById(account.gameInforID);
          const gameData = gameResponse.data.result.data;
          account.gameName = gameData?.gameName || 'Không xác định';
        }
        account.images = getFullImageUrl(account.image);
      }
  
      // Sort accounts by createdDate in descending order
      accounts.sort((a, b) => {
        const dateA = new Date(a.createdDate.split(' ')[0].split('/').reverse().join('-') + ' ' + a.createdDate.split(' ')[1]);
        const dateB = new Date(b.createdDate.split(' ')[0].split('/').reverse().join('-') + ' ' + b.createdDate.split(' ')[1]);
        return dateB.getTime() - dateA.getTime();
      });
  
      gameAccounts.value = accounts;
    } catch (error) {
      console.error('Lỗi khi lấy danh sách tài khoản game:', error);
    }
  };
  
  const confirmCheckAccount = (id: number) => {
    if (confirm('Bạn có chắc rằng tài khoản này đã hợp lệ?')) {
      checkAccount(id);
    }
  };
  
  const checkAccount = async (id: number) => {
    try {
      const response = await accountGameApi.checkAccount(id);
      if (response.data.result.isSuccess) {
        messageContent.value = 'Kiểm tra tài khoản thành công.';
        isError.value = false;
        await fetchGameAccounts();
      } else {
        messageContent.value = response.data.result.message || 'Không thể kiểm tra tài khoản.';
        isError.value = true;
      }
    } catch (error: any) {
      messageContent.value = 'Đã xảy ra lỗi khi kiểm tra tài khoản.';
      isError.value = true;
    }
    isMessageModalOpen.value = true;
  };
  
  const closeMessageModal = () => {
    isMessageModalOpen.value = false;
  };
  
  const handleKeyDown = (event: KeyboardEvent) => {
    if (isModalOpen.value) {
      if (event.key === 'ArrowLeft') prevImage();
      else if (event.key === 'ArrowRight') nextImage();
      else if (event.key === 'Escape') closeModal();
    } else if (isMessageModalOpen.value && event.key === 'Escape') {
      closeMessageModal();
    }
  };
  
  onMounted(() => {
    fetchGameAccounts();
    window.addEventListener('keydown', handleKeyDown);
  });
  </script>
  
  <style lang="css" scoped>
  .container {
    margin-top: 100px;
    padding-top: 30px;
    padding-left: 30px;
    padding-right: 30px;
    padding-bottom: 30px;
    background-color: #f8fafc;
    min-height: 80vh;
  }
  
  h2 {
    font-size: 28px;
    color: #2c3e50;
    margin-bottom: 25px;
    font-weight: 700;
    text-align: center;
    position: relative;
    padding-bottom: 12px;
  }
  
  h2::after {
    content: '';
    position: absolute;
    bottom: 0;
    left: 50%;
    transform: translateX(-50%);
    width: 100px;
    height: 4px;
    background: linear-gradient(90deg, #3498db, #1abc9c);
    border-radius: 2px;
  }
  
  .table-container {
    border-radius: 15px;
    overflow: hidden;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
  }
  
  table {
    width: 100%;
    border-collapse: separate;
    border-spacing: 0;
    background-color: #fff;
    overflow: hidden;
  }
  
  th, td {
    padding: 15px;
    text-align: left;
    border-bottom: 1px solid #e0e0e0;
  }
  
  th {
    background: linear-gradient(135deg, #3498db, #2980b9);
    color: #fff;
    font-weight: 600;
    text-transform: uppercase;
    font-size: 14px;
    letter-spacing: 0.5px;
  }
  
  td {
    color: #34495e;
    font-size: 14px;
    vertical-align: middle;
  }
  
  tr {
    transition: all 0.3s ease;
  }
  
  tr:hover {
    background-color: #f1f9ff;
  }
  
  .game-info {
    display: flex;
    align-items: center;
    gap: 10px;
  }
  
  .game-badge {
    width: 30px;
    height: 30px;
    border-radius: 50%;
    background: linear-gradient(135deg, #3498db, #2980b9);
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: bold;
  }
  
  .password-field {
    background-color: #f1f5f9;
    padding: 8px 12px;
    border-radius: 4px;
    font-family: monospace;
    max-width: 150px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
  
  .status-badge {
    display: inline-block;
    padding: 6px 12px;
    border-radius: 15px;
    font-size: 12px;
    font-weight: 600;
    background-color: #ffeaa7;
    color: #d35400;
  }
  
  .status-badge.confirmed {
    background-color: #d4f5e9;
    color: #16a085;
  }
  
  .view-images-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
    padding: 8px 14px;
    background: linear-gradient(135deg, #1abc9c, #16a085);
    color: #fff;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 13px;
    font-weight: 500;
    transition: all 0.3s ease;
    box-shadow: 0 3px 6px rgba(22, 160, 133, 0.2);
  }
  
  .view-images-btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 5px 15px rgba(22, 160, 133, 0.3);
  }
  
  .view-images-btn.no-images {
    background: linear-gradient(135deg, #bdc3c7, #95a5a6);
    cursor: not-allowed;
  }
  
  .view-images-btn:disabled {
    opacity: 0.7;
    transform: none;
    box-shadow: none;
  }
  
  .action-btn {
    padding: 8px 16px;
    background: linear-gradient(135deg, #3498db, #2980b9);
    color: #fff;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 13px;
    font-weight: 500;
    transition: all 0.3s ease;
    box-shadow: 0 3px 6px rgba(41, 128, 185, 0.2);
  }
  
  .action-btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 5px 15px rgba(41, 128, 185, 0.3);
  }
  
  .action-btn:disabled {
    background: linear-gradient(135deg, #bdc3c7, #95a5a6);
    cursor: not-allowed;
    transform: none;
    box-shadow: none;
  }
  
  .modal-backdrop {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background: rgba(0, 0, 0, 0.85);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 2000;
    backdrop-filter: blur(5px);
  }
  
  .modal-content {
    position: relative;
    width: 90%;
    max-width: 900px;
    height: auto;
    max-height: 90vh;
    background: #fff;
    border-radius: 15px;
    box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
    display: flex;
    flex-direction: column;
    overflow: hidden;
  }
  
  .message-modal {
    max-width: 400px;
  }
  
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 15px 20px;
    color: white;
  }
  
  .error-header {
    background: linear-gradient(135deg, #dc2626, #b91c1c);
  }
  
  .success-header {
    background: linear-gradient(135deg, #059669, #047857);
  }
  
  .modal-header h3 {
    margin: 0;
    font-size: 18px;
    font-weight: 600;
  }
  
  .close-btn {
    background: rgba(255, 255, 255, 0.15);
    border: none;
    border-radius: 50%;
    width: 36px;
    height: 36px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 18px;
    color: #fff;
    cursor: pointer;
    transition: all 0.3s ease;
  }
  
  .close-btn:hover {
    background: rgba(255, 255, 255, 0.3);
    transform: rotate(90deg);
  }
  
  .modal-body {
    padding: 20px;
    display: flex;
    flex-direction: column;
    gap: 15px;
    overflow-y: auto;
  }
  
  .modal-body p {
    margin: 0;
    font-size: 16px;
    color: #34495e;
    text-align: center;
  }
  
  .main-image-container {
    width: 100%;
    height: 400px;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #f1f5f9;
    border-radius: 10px;
    overflow: hidden;
  }
  
  .main-image {
    max-width: 100%;
    max-height: 100%;
    object-fit: contain;
    transition: transform 0.3s ease;
  }
  
  .navigation-bar {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 20px;
    margin-top: 10px;
  }
  
  .nav-btn {
    background: linear-gradient(135deg, #3498db, #2980b9);
    border: none;
    border-radius: 50%;
    width: 40px;
    height: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-size: 16px;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 3px 10px rgba(41, 128, 185, 0.3);
  }
  
  .nav-btn:hover {
    transform: scale(1.1);
    box-shadow: 0 5px 15px rgba(41, 128, 185, 0.4);
  }
  
  .nav-btn:disabled {
    background: #bdc3c7;
    cursor: not-allowed;
    transform: none;
    box-shadow: none;
  }
  
  .image-counter {
    background: #f1f5f9;
    color: #2c3e50;
    padding: 8px 15px;
    border-radius: 20px;
    font-size: 14px;
    font-weight: 500;
    min-width: 60px;
    text-align: center;
  }
  
  .thumbnails-container {
    display: flex;
    overflow-x: auto;
    gap: 10px;
    padding: 10px 0;
    scrollbar-width: thin;
    scrollbar-color: #3498db #e0e0e0;
  }
  
  .thumbnails-container::-webkit-scrollbar {
    height: 6px;
  }
  
  .thumbnails-container::-webkit-scrollbar-track {
    background: #e0e0e0;
    border-radius: 10px;
  }
  
  .thumbnails-container::-webkit-scrollbar-thumb {
    background: #3498db;
    border-radius: 10px;
  }
  
  .thumbnail {
    width: 80px;
    height: 60px;
    border-radius: 5px;
    overflow: hidden;
    cursor: pointer;
    border: 2px solid transparent;
    transition: all 0.3s ease;
    flex-shrink: 0;
  }
  
  .thumbnail.active {
    border-color: #3498db;
    transform: scale(1.05);
  }
  
  .thumbnail img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
  
  .modal-enter-active,
  .modal-leave-active {
    transition: all 0.3s ease;
  }
  
  .modal-enter-from,
  .modal-leave-to {
    opacity: 0;
    transform: scale(0.9);
  }
  
  @media (max-width: 768px) {
    .container {
      padding: 20px 15px;
    }
  
    .modal-content {
      width: 95%;
      max-height: 80vh;
    }
  
    .main-image-container {
      height: 300px;
    }
  
    .thumbnail {
      width: 60px;
      height: 45px;
    }
  
    table {
      font-size: 12px;
    }
  
    th, td {
      padding: 10px;
    }
  }
  </style>
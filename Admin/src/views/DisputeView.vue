<template>
    <div class="container">
      <h2>Danh sách tranh chấp</h2>
      <div class="button-container">
        <button class="action-btn" @click="openEmailLogModal">
          Xem danh sách email đã gửi
        </button>
      </div>
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th>STT</th>
              <th>ID</th>
              <th>Tài khoản mua</th>
              <th>Lý do</th>
              <th>Hình ảnh chứng cứ</th>
              <th>Trạng thái</th>
              <th>Ngày tạo</th>
              <th>Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(dispute, index) in disputes" :key="dispute.id">
              <td>{{ index + 1 }}</td>
              <td>{{ dispute.id }}</td>
              <td>{{ dispute.purchasedAccountID || 'N/A' }}</td>
              <td>{{ dispute.reason || 'Không có' }}</td>
              <td>
                <button
                  class="view-images-btn"
                  @click="openModal(dispute.images)"
                  :disabled="dispute.images.length === 0"
                  :class="{ 'no-images': dispute.images.length === 0 }"
                >
                  <i class="fas fa-flag"></i>
                  {{ dispute.images.length > 0 ? `${dispute.images.length} hình` : 'Không có hình' }}
                </button>
              </td>
              <td>
                <span class="status-badge" :class="{ 'confirmed': dispute.status === 'Resolved' }">
                  {{ dispute.status || 'Chưa xử lý' }}
                </span>
              </td>
              <td>{{ dispute.createdDate }}</td>
              <td>
                <button
                  v-if="dispute.status === 'Đang chờ duyệt'"
                  class="action-btn"
                  @click="openEmailModal(dispute)"
                >
                  Giải quyết
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
              <h3>Xem hình ảnh chứng cứ</h3>
              <button class="close-btn" @click="closeModal">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <div class="main-image-container">
                <img :src="currentImages[currentImageIndex]" alt="Proof Image" class="main-image" />
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
  
      <!-- Modal for sending email -->
      <transition name="modal">
        <div v-if="isEmailModalOpen" class="modal-backdrop" @click="closeEmailModal">
          <div class="modal-content email-modal" @click.stop>
            <div class="modal-header">
              <h3>Gửi Email Giải Quyết Tranh Chấp</h3>
              <button class="close-btn" @click="closeEmailModal">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <form @submit.prevent="sendEmail">
                <div class="form-group">
                  <label>User ID:</label>
                  <input v-model.number="emailForm.userId" type="number" disabled />
                </div>
                <div class="form-group">
                  <label>Tiêu đề:</label>
                  <input v-model="emailForm.subject" type="text" required />
                </div>
                <div class="form-group">
                  <label>Nội dung:</label>
                  <textarea v-model="emailForm.messageBody" required></textarea>
                </div>
                <button type="submit" class="submit-btn">Gửi Email</button>
              </form>
            </div>
          </div>
        </div>
      </transition>
  
      <!-- Modal for email logs -->
      <transition name="modal">
        <div v-if="isEmailLogModalOpen" class="modal-backdrop" @click="closeEmailLogModal">
          <div class="modal-content email-log-modal" @click.stop>
            <div class="modal-header">
              <h3>Danh sách Email Đã Gửi</h3>
              <button class="close-btn" @click="closeEmailLogModal">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <div class="table-container">
                <table>
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Tiêu đề</th>
                      <th>Nội dung</th>
                      <th>Email người gửi</th>
                      <th>Email người nhận</th>
                      <th>Ngày gửi</th>
                      <th>Thành công</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="log in emailLogs" :key="log.id">
                      <td>{{ log.id }}</td>
                      <td>{{ log.subject || 'N/A' }}</td>
                      <td class="message-body">
                        {{ truncateMessage(log.messageBody) }}
                        <button
                          v-if="log.messageBody && log.messageBody.length > 50"
                          class="view-full-btn"
                          @click="openMessageModal(log.messageBody)"
                        >
                          Xem đầy đủ
                        </button>
                      </td>
                      <td>{{ log.senderEmail || 'N/A' }}</td>
                      <td>{{ log.receiverEmail || 'N/A' }}</td>
                      <td>{{ formatDate(log.sentDate) }}</td>
                      <td>
                        <span class="status-badge" :class="{ 'confirmed': log.isSuccess }">
                          {{ log.isSuccess ? 'Thành công' : 'Thất bại' }}
                        </span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </transition>
  
      <!-- Modal for full message content -->
      <transition name="modal">
        <div v-if="isMessageContentModalOpen" class="modal-backdrop" @click="closeMessageContentModal">
          <div class="modal-content message-content-modal" @click.stop>
            <div class="modal-header">
              <h3>Nội dung Email</h3>
              <button class="close-btn" @click="closeMessageContentModal">
                <i class="fas fa-times"></i>
              </button>
            </div>
            <div class="modal-body">
              <p class="full-message">{{ selectedMessage }}</p>
            </div>
          </div>
        </div>
      </transition>
  
      <!-- Modal for messages -->
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
  import disputeApi from '@/api/dispute.api';
  import authenticateApi from '@/api/authenticate.api';
  import emailLogApi from '@/api/emaillog.api';
  import type { Dispute } from '@/models/dispute.model';
  import type { EmailLog } from '@/api/emaillog.api';
  
  const disputes = ref<(Dispute & { images: string[]; createdDate: string })[]>([]);
  const isModalOpen = ref(false);
  const currentImages = ref<string[]>([]);
  const currentImageIndex = ref(0);
  const isMessageModalOpen = ref(false);
  const messageContent = ref<string>('');
  const isError = ref<boolean>(false);
  const isEmailModalOpen = ref(false);
  const isEmailLogModalOpen = ref(false);
  const isMessageContentModalOpen = ref(false);
  const emailLogs = ref<EmailLog[]>([]);
  const selectedMessage = ref<string>('');
  const emailForm = ref<{
    userId: number;
    subject: string;
    messageBody: string;
  }>({
    userId: 0,
    subject: '',
    messageBody: '',
  });
  
  const getFullImageUrl = (imageString: string | null | undefined): string[] => {
    if (!imageString || imageString.trim() === '') return [];
    const baseUrl = 'https://localhost:7232/';
    const images = imageString.split(';').filter(img => img.trim() !== '');
    return images.map(img => `${baseUrl}${img}`);
  };
  
  const formatDate = (date: Date | string | null): string => {
    if (!date) return 'N/A';
    const d = typeof date === 'string' ? new Date(date) : date;
    if (isNaN(d.getTime())) return 'N/A';
    return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()} ${d.getHours().toString().padStart(2, '0')}:${d.getMinutes().toString().padStart(2, '0')}:${d.getSeconds().toString().padStart(2, '0')}`;
  };
  
  const truncateMessage = (message: string | null): string => {
    if (!message) return 'N/A';
    return message.length > 50 ? `${message.substring(0, 50)}...` : message;
  };
  
  const fetchDisputes = async () => {
    try {
      const response = await disputeApi.getAll();
      let disputesData = (response.data.result.data ?? []).map((dispute: Dispute) => ({
        ...dispute,
        images: getFullImageUrl(dispute.proof),
        createdDate: formatDate(dispute.createdDate ?? null),
      }));
  
      if (!disputesData) {
        console.error('Dữ liệu tranh chấp không tồn tại');
        return;
      }
  
      disputesData.sort((a, b) => {
        const dateA = new Date(a.createdDate.split(' ')[0].split('/').reverse().join('-') + ' ' + a.createdDate.split(' ')[1]);
        const dateB = new Date(b.createdDate.split(' ')[0].split('/').reverse().join('-') + ' ' + b.createdDate.split(' ')[1]);
        return dateB.getTime() - dateA.getTime();
      });
  
      disputes.value = disputesData;
    } catch (error) {
      console.error('Lỗi khi lấy danh sách tranh chấp:', error);
      messageContent.value = 'Đã xảy ra lỗi khi lấy danh sách tranh chấp.';
      isError.value = true;
      isMessageModalOpen.value = true;
    }
  };
  
  const fetchEmailLogs = async () => {
    try {
      const response = await emailLogApi.getAll();
      emailLogs.value = response.data.result.data || [];
    } catch (error: any) {
      console.error('Lỗi khi lấy danh sách email logs:', error);
      messageContent.value = error.message || 'Đã xảy ra lỗi khi lấy danh sách email đã gửi.';
      isError.value = true;
      isMessageModalOpen.value = true;
    }
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
  
  const openEmailModal = (dispute: Dispute & { images: string[]; createdDate: string }) => {
    emailForm.value = {
      userId: dispute.userID ?? 0,
      subject: `Giải quyết tranh chấp #${dispute.id}`,
      messageBody: '',
    };
    isEmailModalOpen.value = true;
    document.body.style.overflow = 'hidden';
  };
  
  const closeEmailModal = () => {
    isEmailModalOpen.value = false;
    document.body.style.overflow = '';
  };
  
  const openEmailLogModal = async () => {
    await fetchEmailLogs();
    isEmailLogModalOpen.value = true;
    document.body.style.overflow = 'hidden';
  };
  
  const closeEmailLogModal = () => {
    isEmailLogModalOpen.value = false;
    document.body.style.overflow = '';
  };
  
  const openMessageContentModal = (message: string) => {
      selectedMessage.value = message || 'N/A';
      isMessageContentModalOpen.value = true;
      document.body.style.overflow = 'hidden';
  };
  
  const openMessageModal = (message: string) => {
      messageContent.value = message || 'N/A';
      isMessageModalOpen.value = true;
      document.body.style.overflow = 'hidden';
  };
  
  const closeMessageContentModal = () => {
    isMessageContentModalOpen.value = false;
    document.body.style.overflow = '';
  };
  
  const sendEmail = async () => {
    try {
      const response = await authenticateApi.sendCustomEmail(
        emailForm.value.userId.toString(),
        emailForm.value.subject,
        emailForm.value.messageBody
      );
      messageContent.value = 'Gửi email thành công.';
      isError.value = false;
      isMessageModalOpen.value = true;
      closeEmailModal();
    } catch (error: any) {
      messageContent.value = error.message || 'Gửi email thất bại.';
      isError.value = true;
      isMessageModalOpen.value = true;
      console.error('Lỗi:', error);
    }
  };
  
  const closeMessageModal = () => {
    isMessageModalOpen.value = false;
  };
  
  const handleKeyDown = (event: KeyboardEvent) => {
    if (isModalOpen.value) {
      if (event.key === 'ArrowLeft') prevImage();
      else if (event.key === 'ArrowRight') nextImage();
      else if (event.key === 'Escape') closeModal();
    } else if (isEmailModalOpen.value && event.key === 'Escape') {
      closeEmailModal();
    } else if (isEmailLogModalOpen.value && event.key === 'Escape') {
      closeEmailLogModal();
    } else if (isMessageContentModalOpen.value && event.key === 'Escape') {
      closeMessageContentModal();
    } else if (isMessageModalOpen.value && event.key === 'Escape') {
      closeMessageModal();
    }
  };
  
  onMounted(() => {
    fetchDisputes();
    window.addEventListener('keydown', handleKeyDown);
  });
  </script>
  
  <style lang="css" scoped>
  .container {
    margin-top: 100px;
    padding: 30px;
    background-color: #f8fafc;
    min-height: 80vh;
  }
  
  .button-container {
    margin-bottom: 20px;
    display: flex;
    justify-content: flex-end;
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
  
  th,
  td {
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
  
  td.message-body {
    max-width: 200px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
  
  tr {
    transition: all 0.3s ease;
  }
  
  tr:hover {
    background-color: #f1f9ff;
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
  
  .view-images-btn,
  .action-btn,
  .submit-btn,
  .view-full-btn {
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
  
  .view-full-btn {
    margin-left: 10px;
    padding: 6px 12px;
    font-size: 12px;
  }
  
  .view-images-btn:hover,
  .action-btn:hover,
  .submit-btn:hover,
  .view-full-btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 5px 15px rgba(22, 160, 133, 0.3);
  }
  
  .view-images-btn.no-images {
    background: linear-gradient(135deg, #bdc3c7, #95a5a6);
    cursor: not-allowed;
  }
  
  .view-images-btn:disabled,
  .action-btn:disabled {
    opacity: 0.7;
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
  
  .message-modal,
  .email-modal,
  .message-content-modal {
    max-width: 400px;
  }
  
  .email-log-modal {
    max-width: 1200px;
  }
  
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 15px 20px;
    color: white;
    background: linear-gradient(135deg, #3498db, #2980b9);
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
  
  .full-message {
    white-space: pre-wrap;
    text-align: left;
    max-height: 400px;
    overflow-y: auto;
    padding: 10px;
    border: 1px solid #e0e0e0;
    border-radius: 4px;
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
  
  .form-group {
    display: flex;
    flex-direction: column;
    gap: 5px;
  }
  
  .form-group label {
    font-weight: 500;
    color: #34495e;
  }
  
  .form-group input,
  .form-group textarea {
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    font-size: 14px;
  }
  
  .form-group input:disabled {
    background-color: #e0e0e0;
    cursor: not-allowed;
  }
  
  .form-group textarea {
    resize: vertical;
    min-height: 100px;
  }
  
  @media (max-width: 768px) {
    .container {
      padding: 20px 15px;
    }
  
    .modal-content {
      width: 95%;
      max-height: 80vh;
    }
  
    .email-log-modal {
      max-width: 95%;
    }
  
    .message-content-modal {
      max-width: 95%;
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
  
    th,
    td {
      padding: 10px;
    }
  
    td.message-body {
      max-width: 150px;
    }
  }
  </style>
<template>
    <div>
      <h2>Danh sách tài khoản game</h2>
      <table>
        <thead>
          <tr>
            <th>STT</th>
            <th>Tên game</th>
            <th>Account Name</th>
            <th>Password</th>
            <th>Danh sách hình ảnh</th>
            <th>Trạng thái</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(account, index) in gameAccounts" :key="account.id">
            <td>{{ index + 1 }}</td>
            <td>{{ account.gameName }}</td>
            <td>{{ account.accountName }}</td>
            <td>{{ account.password }}</td>
            <td>
              <img
                v-for="(img, imgIndex) in account.image"
                :key="imgIndex"
                :src="img"
                alt="Game Image"
                style="width: 50px; height: 50px; margin-right: 5px;"
              />
            </td>
            <td>{{ account.isCheck ? 'Đã xác nhận' : 'Chưa xác nhận' }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref, onMounted } from 'vue';
  import accountGameApi from '@/api/gameaccount.api';
  import type { GameAccount } from '@/models/gameaccount.model';
  
  // Khai báo biến reactive để lưu danh sách tài khoản
  const gameAccounts = ref<GameAccount[]>([]);
  
  // Hàm xử lý chuỗi hình ảnh để trả về danh sách URL
  const getFullImageUrl = (imageString: string | null | undefined): string[] => {
    if (!imageString || imageString.trim() === "") return [];
    const baseUrl = "https://localhost:7232/";
    const images = imageString.split(";").filter(img => img.trim() !== "");
    return images.map(img => `${baseUrl}${img}`);
  };
  
  // Hàm lấy toàn bộ danh sách tài khoản và tên game
  const fetchGameAccounts = async () => {
    try {
      // Gọi API GetAll để lấy danh sách tài khoản
      const response = await accountGameApi.getAll();
      const accounts = response.data.result.data; // Truy cập dữ liệu từ result.data
  
      // Kiểm tra nếu accounts tồn tại
      if (!accounts) {
        console.error('Dữ liệu tài khoản không tồn tại');
        return;
      }
  
      // Lặp qua từng tài khoản để lấy tên game và xử lý hình ảnh
      for (const account of accounts) {
        // Gọi API getById để lấy tên game dựa trên gameInforID
        const gameResponse = await accountGameApi.getByIdForGame(account.gameInforID, account.id);
        const gameData = gameResponse.data.result.data; // Truy cập dữ liệu từ result.data
        account.gameName = gameData ? gameData.name : 'Không xác định'; // Giả sử API trả về trường name
        account.image = getFullImageUrl(account.image); // Xử lý chuỗi hình ảnh
      }
  
      // Gán dữ liệu vào biến reactive
      gameAccounts.value = accounts;
    } catch (error) {
      console.error('Lỗi khi lấy danh sách tài khoản game:', error);
    }
  };
  
  // Gọi hàm fetchGameAccounts khi component được mount
  onMounted(() => {
    fetchGameAccounts();
  });
  </script>
  
  <style lang="css" scoped>
  table {
    width: 100%;
    border-collapse: collapse;
  }
  th,
  td {
    border: 1px solid #ddd;
    padding: 8px;
    text-align: left;
  }
  th {
    background-color: #f2f2f2;
  }
  </style>
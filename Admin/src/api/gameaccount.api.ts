import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';
import type { GameAccount } from '@/models/gameaccount.model';
import type { GameAccountField } from '@/models/gameaccountfield.model';
// Định nghĩa interface cho dữ liệu gửi lên API
export interface AddAccountGameModel {
  gameInforId: number | null;
  accountName: string;
  password: string;
  price: number;
  priceMin: number;
  File: File[];
}

export interface UpdateAccountGameModel extends AddAccountGameModel {
  id: number;
}

export interface BuyAccountGameModel {
  Id: number;
}

export default {
  // Lấy tất cả tài khoản game (không yêu cầu token)
  getAll: async (): Promise<AxiosResponse<ApiResult<GameAccount[]>>> => {
    return await baseApi.get('AccountGame/Get-All');
  },

  // Lấy danh sách tài khoản theo userId (không yêu cầu token)
  getAllByUserID: async (userId: number): Promise<AxiosResponse<ApiResult<GameAccount[]>>> => {
    return await baseApi.get('AccountGame/Get-All-By-UserID', { userId });
  },

  // Thêm tài khoản game mới (yêu cầu token, gửi FormData)
  add: async (formData: FormData): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.postForm('AccountGame/Add', formData);
  },

  // Cập nhật tài khoản game (yêu cầu token, gửi JSON)
  update: async (accountGame: UpdateAccountGameModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('AccountGame/Update', accountGame, { params: { id: accountGame.id } });
  },

  // Xóa tài khoản game (yêu cầu token)
  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`AccountGame/Delete?id=${id}`, null, { params: { id } });
  },

  // Mua tài khoản game (yêu cầu token, gửi JSON)
  buy: async (model: BuyAccountGameModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('AccountGame/Buy', model, { params: { id: model.Id } });
  },

  // Lấy thông tin người dùng (yêu cầu token)
  getInforUser: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get(`AccountGame/Get-Infor-User?id=${id}`);
},

  // Thêm trường cho tài khoản game (yêu cầu token, gửi JSON)
  addFieldForGame: async (model: GameAccountField): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('GameAccountField/Add-Field-For-Game', model, { params: { id: 0 } });
  },

  // Lấy thông tin trường theo id (không yêu cầu token)
  getByIdForGame: async (id: number, id2: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('GameAccountField/Get-By-Id-For-Game', { id, id2 });
  },
  // Kiểm tra tài khoản game (yêu cầu token)
    checkAccount: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`AccountGame/AccountCheck?id=${id}`, null, { params: { id } });
  },
};
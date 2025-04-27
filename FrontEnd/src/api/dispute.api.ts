
import type { AddDisputeModel, Dispute, UpdateDisputeModel } from '@/models/dispute.model';
import baseApi from './base.api';
import type { ApiResult } from '@/models/api-result.model';
import type { AxiosResponse } from 'axios';

export default {
  // Thêm tranh chấp mới (yêu cầu token, gửi FormData)
  add: async (disputeData: FormData): Promise<AxiosResponse<ApiResult<Dispute>>> => {
    return await baseApi.postForm('Dispute/Add', disputeData);
  },

  // Xóa tranh chấp theo ID (yêu cầu token)
  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.delete(`Dispute/Delete/${id}`);
  },

  // Lấy tất cả tranh chấp (yêu cầu token)
  getAll: async (): Promise<AxiosResponse<ApiResult<Dispute[]>>> => {
    return await baseApi.get('Dispute/GetAll');
  },

  // Lấy tất cả tranh chấp theo SellerID (yêu cầu token)
  getAllBySellerId: async (sellerId: number): Promise<AxiosResponse<ApiResult<Dispute[]>>> => {
    return await baseApi.get(`Dispute/GetAllBySellerId/${sellerId}`);
  },

  // Lấy tất cả tranh chấp theo UserID (yêu cầu token)
  getAllByUserId: async (userId: number): Promise<AxiosResponse<ApiResult<Dispute[]>>> => {
    return await baseApi.get(`Dispute/GetAllByUserId/${userId}`);
  },

  // Lấy tranh chấp theo ID (yêu cầu token)
  getById: async (id: number): Promise<AxiosResponse<ApiResult<Dispute>>> => {
    return await baseApi.get(`Dispute/GetById/${id}`);
  },

  // Phản hồi tranh chấp (yêu cầu token, gửi JSON)
  reply: async (id: number, reply: string): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.put(`Dispute/Reply/${id}`, reply);
  },

  // Cập nhật tranh chấp (yêu cầu token, gửi FormData)
  update: async (disputeData: FormData): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.postForm('Dispute/Update', disputeData);
  },
};
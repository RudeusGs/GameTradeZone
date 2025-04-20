import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';
import type { UpdateServiceModel, RentServiceModel } from '@/models/service.model';

export default {
  // Lấy tất cả dịch vụ
  getAll: async (): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('Service/Get-All');
  },

  // Lấy dịch vụ theo ID
  getById: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get(`Service/Get-By-Id?id=${id}`);
  },

  // Lấy tất cả dịch vụ theo userId
  getAllByUserId: async (userId: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get(`Service/Get-All-By-UserId?id=${userId}`);
  },

  // Thêm dịch vụ mới
  add: async (formData: FormData): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.postForm('Service/Add', formData);
  },

  // Xóa dịch vụ
  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`Service/Delete?id=${id}`, null);
  },

  // Cập nhật dịch vụ
  update: async (model: UpdateServiceModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('Service/Update', model);
  },

  // Thuê dịch vụ
  rentService: async (model: RentServiceModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('Service/RentService', model);
  },
};
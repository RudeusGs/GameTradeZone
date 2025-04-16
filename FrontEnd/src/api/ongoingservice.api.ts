import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';

// Define interface for ConfirmService payload
export interface ConfirmServiceModel {
  id: number;
}

export default {
  // Get all ongoing services (no token required)
  getAll: async (): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('OnGoingService/Get-All');
  },

  getAllByUserId: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('OnGoingService/Get-All-By-User-Id', { id });
  },

  getAllByServiceId: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('OnGoingService/Get-All-By-Service-Id', { id });
  },

  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`OnGoingService/Delete?id=${id}`, null);
  },

  confirmService: async (model: ConfirmServiceModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('OnGoingService/Confirm-Service', model);
  },
};
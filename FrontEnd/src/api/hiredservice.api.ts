import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';

export interface AcceptServiceModel {
  id: number;
}

export default {
  getAll: async (): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('HiredService/Get-All');
  },

  getAllByUserId: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('HiredService/Get-All-By-User-Id', { id });
  },

  getAllByServiceId: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('HiredService/Get-All-By-Service-Id', { id });
  },
  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`HiredService/Delete?id=${id}`, null);
  },

  confirmService: async (model: AcceptServiceModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('HiredService/Confirm-Service', model);
  },
};
import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';

export default {
  getAllByUserId: async (userId: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('Notification/Get-All-By-User-Id', { userId });
  },

  read: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`Notification/Read?id=${id}`, null);
  },

  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`Notification/Delete?id=${id}`, null);
  },
};
import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';
import type { UpdateServiceModel, RentServiceModel } from '@/models/service.model';

export default {
  getAll: async (): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('Service/Get-All');
  },

  getById: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('Service/Get-By-Id', { id });
  },

  add: async (formData: FormData): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.postForm('Service/Add', formData);
  },

  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`Service/Delete?id=${id}`, null);
  },

  update: async (model: UpdateServiceModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('Service/Update', model);
  },

  rentService: async (model: RentServiceModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('Service/RentService', model);
  },
};
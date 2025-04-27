import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';

export interface ConfirmServiceModel {
  id: number;
}

export default {
  getAll: async (): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('OnGoingService/Get-All');
  },

  getAllByUserId: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get(`OnGoingService/Get-All-By-User-Id?id=${id}`);
  },

  getAllByServiceId: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get(`OnGoingService/Get-All-By-Service-Id?id=${id}`);
  },

  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`OnGoingService/Delete?id=${id}`, null);
  },

  confirmService: async (model: ConfirmServiceModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('OnGoingService/Confirm-Service', model);
  },

  getServiceName: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get(`OnGoingService/Get-ServiceName?id=${id}`);
  },

  confirmExtension: async (id: number, approve: boolean, approvedTime?: string): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`OnGoingService/Confirm-Extension?id=${id}&approve=${approve}&approvedTime=${approvedTime || ''}`, null);
  },

  getRemainingTime: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get(`OnGoingService/Get-Remaining-Time?id=${id}`);
  }
};
// src/api/gameinfor.api.ts
import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { GameInfor } from '@/models/gameinfor.model';
import type { ApiResult } from '@/models/api-result.model';

export default {
  getAll: async (): Promise<AxiosResponse<ApiResult<GameInfor[]>>> => {
    return await baseApi.get('GameInfor/Get-All');
  },

  getById: async (id: number): Promise<AxiosResponse<ApiResult<GameInfor>>> => {
    return await baseApi.get(`GameInfor/Get-By-Id?id=${id}`);
  },

  add: async (gameInfor: GameInfor): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('GameInfor/Add', gameInfor, { params: { id: 0 } });
  },

  addWithFormData: async (formData: FormData): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.postForm('GameInfor/Add', formData);
  },

  update: async (gameInfor: GameInfor): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`GameInfor/Update?id=${gameInfor.id}`, gameInfor, { params: { id: gameInfor.id } });
  },

  updateWithFormData: async (formData: FormData): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.postForm(`GameInfor/Update?id=${formData.get("id")}`, formData);
  },

  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`GameInfor/Delete?id=${id}`, null, { params: { id } });
  },
};
import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';

export interface AddLevelIconModel {
  name: string;
  imageUrl?: string;
  level: number;
}

export interface UpdateLevelIconModel extends AddLevelIconModel {
  id: number;
}

export default {
  getAll: async (): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('LevelIcon/Get-All');
  },

  getById: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.get('LevelIcon/Get-By-Id', { id });
  },

  add: async (model: AddLevelIconModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('LevelIcon/Add', model);
  },

  update: async (model: UpdateLevelIconModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('LevelIcon/Update', model);
  },

  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`LevelIcon/Delete?id=${id}`, null);
  },
};
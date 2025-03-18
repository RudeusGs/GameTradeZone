// src/api/gamefield.api.ts
import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { GameField } from '@/models/gameinfor.model';
import type { ApiResult } from '@/models/api-result.model';

export interface AddGameFieldModel {
  gameInforId: number | null;
  fieldName: string;
}

export interface UpdateGameFieldModel extends AddGameFieldModel {
  id: number;
}

export default {
  getAll: async (gameId: number): Promise<AxiosResponse<ApiResult<GameField[]>>> => {
    return await baseApi.get(`GameField/Get-All?gameInforId=${gameId}`);
  },

  getById: async (id: number): Promise<AxiosResponse<ApiResult<GameField>>> => {
    return await baseApi.get(`GameField/Get-By-Id?id=${id}`);
  },

  add: async (gameField: AddGameFieldModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('GameField/Add', gameField);
  },

  update: async (gameField: UpdateGameFieldModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`GameField/Update?id=${gameField.id}`, gameField);
  },

  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`GameField/Delete?id=${id}`, null);
  },

  getField: async (id: number): Promise<AxiosResponse<ApiResult<GameField[]>>> => {
    return await baseApi.get(`GameInfor/Get-Field?id=${id}`);
  },
};
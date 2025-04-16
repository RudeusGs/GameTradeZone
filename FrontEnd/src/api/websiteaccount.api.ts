
import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { UserInfoModel } from '@/models/user-model';
import type { ApiResult } from '@/models/api-result.model';

export default {
  getAll: async (): Promise<AxiosResponse<ApiResult<UserInfoModel[]>>> => {
    return await baseApi.get('WebsiteAccount/Get-All');
  },

  getById: async (userId: number) => {
    return await baseApi.get(`WebsiteAccount/Get-By-Id?id=${userId}`);
  },

  blockAccount: async (id: number): Promise<AxiosResponse<ApiResult>> => {
    return await baseApi.post(`WebsiteAccount/Block-Account?id=${id}`, null);
  },

  updateRoles: async (id: number, newRoles: string[]): Promise<AxiosResponse<ApiResult>> => {
    return await baseApi.post(`WebsiteAccount/Update-Roles?id=${id}`, newRoles);
  },
};

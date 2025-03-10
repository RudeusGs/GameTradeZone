// src/api/websiteaccount.api.ts

import baseApi from './base.api'; // axios instance
import type { AxiosResponse } from 'axios';
import type { UserInfoModel } from '@/models/user-model';
import type { ApiResult } from '@/models/api-result.model';

export default {
  getAll: async (): Promise<AxiosResponse<ApiResult<UserInfoModel[]>>> => {
    return await baseApi.get('WebsiteAccount/Get-All');
  },

  getById: async (id: number): Promise<AxiosResponse<ApiResult<UserInfoModel>>> => {
    return await baseApi.get('WebsiteAccount/Get-By-Id', { params: { id } });
  },

  blockAccount: async (id: number): Promise<AxiosResponse<ApiResult>> => {
    return await baseApi.post(`WebsiteAccount/Block-Account?id=${id}`, null, { params: { id } });
  },

  updateRoles: async (id: number, newRoles: string[]): Promise<AxiosResponse<ApiResult>> => {
    return await baseApi.post(`WebsiteAccount/Update-Roles?id=${id}`, newRoles, { params: { id } });
  },
};

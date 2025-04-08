import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';
import type { PurchasedAccount } from '@/models/purchased.model';

export interface ConfirmAccountModel {
  id: number;
  status: string;
}

export interface ResponseModel {
  id: number;
  model: string;
}

export default {
      getAll: async (): Promise<AxiosResponse<ApiResult<PurchasedAccount[]>>> => {
    return await baseApi.get('PurchasedAccount/Get-All');
  },

  getAllByUserID: async (id: number): Promise<AxiosResponse<ApiResult<PurchasedAccount[]>>> => {
    return await baseApi.get(`PurchasedAccount/Get-All-By-User-Id?id=${id}`);
},

  delete: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`PurchasedAccount/Delete?id=${id}`, null);
  },

  confirmAccount: async (model: ConfirmAccountModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('PurchasedAccount/Confirm-Account', model);
  },

  emailRequest: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`PurchasedAccount/Email-request?id=${id}`, null);
  },

  emailResponse: async (model: ResponseModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('PurchasedAccount/Email-response', model);
  },

  otpRequest: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`PurchasedAccount/OTP-request?id=${id}`, null);
  },

  otpResponse: async (model: ResponseModel): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post('PurchasedAccount/OTP-response', model);
  },
};
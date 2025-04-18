import baseApi from './base.api';
import type { AxiosResponse } from 'axios';
import type { ApiResult } from '@/models/api-result.model';
import type { PurchasedAccount } from '@/models/purchased.model';

export interface ConfirmAccountModel {
  id: number;
  status: string;
}

// No need for ResponseModel anymore since we're using query parameters
export default {
  getAll: async (): Promise<AxiosResponse<ApiResult<PurchasedAccount[]>>> => {
    return await baseApi.get('PurchasedAccount/Get-All');
  },
  getDontConfirm: async (): Promise<AxiosResponse<ApiResult<PurchasedAccount[]>>> => {
    return await baseApi.get('PurchasedAccount/Get-Dont-Confirm');
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
  emailResponse: async (id: number, model: string): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`PurchasedAccount/Email-response?id=${id}&model=${encodeURIComponent(model)}`, null);
  },
  otpRequest: async (id: number): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`PurchasedAccount/OTP-request?id=${id}`, null);
  },
  otpResponse: async (id: number, model: string): Promise<AxiosResponse<ApiResult<any>>> => {
    return await baseApi.post(`PurchasedAccount/OTP-response?id=${id}&model=${encodeURIComponent(model)}`, null);
  },
};
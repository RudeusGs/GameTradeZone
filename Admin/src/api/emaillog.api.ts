import baseApi from './base.api';
export interface EmailLog {
    id: number;
    userId: number;
    subject: string | null;
    messageBody: string | null;
    senderEmail: string | null;
    receiverEmail: string | null;
    sentDate: string;
    isSuccess: boolean;
    errorMessage: string | null;
    createdDate: string | null;
    updatedDate: string | null;
    deleteDate: string | null;
  }
export default {
  getAll: async (): Promise<any> => {
    return await baseApi.get('EmailLog/Get-All');
         
}
}
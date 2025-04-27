  import baseApi from './base.api';
  export default {
      login: async (model: LoginModel) : Promise<any|undefined> => {
        const response = await baseApi.postAuthenticate('Authenticate/login', model);
      if (response.status === 200) {
        return response.data;
      } else {
        throw new Error('Login failed');
      }
      },
      logout:()=>{
          baseApi.postAuthenticate("Authenticate/Logout",null);
      },
      getRoleById: async (id: number): Promise<any> => {
        const response = await baseApi.get(`Authenticate/roles?userId=${id}`);
        if (response.status === 200) {
          return response.data;
        } else {
          throw new Error(response.data?.message || 'Tài khoản hoặc mật khẩu không chính xác.');
        }
      },
      externalLogin: (provider: string = 'Google'): string => {
        return `https://localhost:7232/api/authenticate/external-login?provider=${provider}`;
      },
      sendOtpForEmailVerification: async (): Promise<any> => {
        const response = await baseApi.post('Authenticate/send-otp-email-verification', null);
        if (response.status === 200) {
          return response.data;
        } else {
          throw new Error(response.data?.message || 'Failed to send OTP for email verification.');
        }
      },
    
      verifyOtpForEmailVerification: async (username: string, otp: string): Promise<any> => {
        const response = await baseApi.post('Authenticate/verify-otp-email-verification', { username, otp });
        if (response.status === 200) {
          return response.data;
        } else {
          throw new Error(response.data?.message || 'Failed to verify OTP.');
        }
      },
  };
  export interface LoginModel{
      userName: string,
      password: string,
  }

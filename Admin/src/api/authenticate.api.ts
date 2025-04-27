  //api/authenticate.api.ts
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
      getRoleById: async (id: number) => {
        return await baseApi.get(`Authenticate/roles?userId=${id}`);
      }, 
      externalLogin: (provider: string = 'Google'): string => {
        return `https://localhost:7232/api/authenticate/external-login?provider=${provider}`;
      },
      sendCustomEmail: async (userId: string, subject: string, messageBody: string): Promise<any|undefined> => {
        const response = await baseApi.post('Authenticate/send-custom-email', {
          userId,
          subject,
          messageBody
        }, { params: { id: 0 } });
        if (response.status === 200) {
          return response.data;
        } else {
          throw new Error('Failed to send custom email');
        }
      }
  };
  export interface LoginModel{
      userName: string,
      password: string,
  }

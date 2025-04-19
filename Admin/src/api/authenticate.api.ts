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
  };
  export interface LoginModel{
      userName: string,
      password: string,
  }

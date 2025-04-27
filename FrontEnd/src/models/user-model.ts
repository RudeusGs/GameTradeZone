export interface LoginModel{
    user: string,
    password: string,
  }
  
// models/user-model.ts
export interface UserInfoModel {
  userName: string;
  id: number;
  email: string;
  fullName?: string;
  balance: number;
  coin: number;
  level: number;
  status: boolean;
  experience?: number | null;
  bankName: string; 
  bankNumber: string; 
  avatar?: string | null;
  isAuthen?: boolean;
  createdDate?: string | null;
}   
  
 export  interface LoginResponseModel{
    userId:number,
    username:string,
    email: string,
    token: string,
    expiration: string,
    roles: string[]
  }
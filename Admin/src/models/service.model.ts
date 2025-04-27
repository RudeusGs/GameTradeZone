import type { IEntityBase } from '@/models/basemodel';

export interface Service extends IEntityBase {
  gameInforID?: number;
  serviceName?: string;
  createrID?: number;
  decription?: string;
  serviceLevel?: number;
  servicePrice: number;
  serviceTime?: string;
  rentedC?: number;
  feedback?: string;
  image?: string;
  isDelete?: boolean;
}

export interface UpdateServiceModel {
  id: number;           
  gameInforID: string; 
  Servicename: string;
  decription?: string;
  Serviceprice: number;
  ServiceTime: string;
}

export interface RentServiceModel {
  Id: number;
}
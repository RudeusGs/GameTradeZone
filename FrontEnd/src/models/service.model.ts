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

export interface UpdateServiceModel extends IEntityBase {
  name: string;
  description?: string;
  price: number;
}

export interface RentServiceModel {
  Id: number;
}
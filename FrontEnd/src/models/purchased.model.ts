import type { IEntityBase } from '@/models/basemodel';

export interface PurchasedAccount extends IEntityBase {
  userID?: number | null;
  sellerID?: number | null;
  email?: string | null;
  otpEmail?: string | null;
  accountGameId?: number | null;
  gameName?: string | null;
  accountName?: string | null;
  password?: string | null;
  price: number;
  statusBuyer?: string | null;
  statusSeller?: string | null;
  reason?: string | null;
  isDelete?: boolean | null;
}
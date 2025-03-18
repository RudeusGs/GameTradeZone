// src/models/accountgame.model.ts
import type { IEntityBase } from '@/models/basemodel';

export interface GameAccount extends IEntityBase {
  gameInforID?: number | null;
  userID?: number | null;
  accountName?: string | null;
  password?: string | null;
  priceMin?: number | null;
  price: number;
  status?: string | null;
  image?: string | null;
  customerFeedback?: string | null;
  isDelete?: boolean | null;
  files?: File[];
}

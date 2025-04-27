import type { IEntityBase } from './basemodel';
export interface Dispute extends IEntityBase {
  purchasedAccountID?: number | null;
  onGoingServiceID?: number | null;
  hiredServiceID?: number | null;
  userID?: number | null; 
  sellertID?: number | null;
  reason?: string | null;
  proof?: string | null;
  status?: string | null;
  reply?: string | null;
  isDelete?: boolean | null;
}

export interface AddDisputeModel {
  id: number;
  purchasedAccountID?: number | null;
  onGoingServiceID?: number | null;
  hiredServiceID?: number | null;
  reason?: string | null;
  files?: File[] | null;
  reply?: string | null;
}

export interface UpdateDisputeModel {
  id: number;
  reason: string;
  files?: File[] | null;
  reply: string;
}
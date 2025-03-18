// src/models/accountgame.model.ts
import type { IEntityBase } from '@/models/basemodel';

export interface GameAccountField extends IEntityBase {
  GameAccountId?: number | null;
  GameFieldId? : number | null;
  FieldValue? : string | null;
}

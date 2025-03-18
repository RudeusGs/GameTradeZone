import type { IEntityBase } from '@/models/basemodel';

export interface GameInfor extends IEntityBase {
  gameName?: string | null;
  image?: string | null;
  genre?: string | null;
  isDelete?: boolean | null;
  files?: File[];
  additionalInfo?: string;
}

export interface GameField extends IEntityBase {
        gameInforId : number| null;
        fieldName : string | null;
        isDelete? : boolean | null;
  }

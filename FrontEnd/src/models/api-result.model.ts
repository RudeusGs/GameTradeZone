// src/models/api-result.model.ts
export interface ApiResult<T = any> {
    data: any;
    result: {
      data?: T;
      message?: string | null;
      isSuccess: boolean;
    };
  }
  
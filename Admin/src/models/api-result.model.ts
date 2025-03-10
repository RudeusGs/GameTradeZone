// src/models/api-result.model.ts
export interface ApiResult<T = any> {
    result: {
      data?: T;
      message?: string | null;
      isSuccess: boolean;
    };
  }
  
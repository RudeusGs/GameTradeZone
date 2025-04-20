export interface PagedResult<T> {
    items: T[];
    totalItems: number;
    pageIndex: number;
    pageSize: number;
  }
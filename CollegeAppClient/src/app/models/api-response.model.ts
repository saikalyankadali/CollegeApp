export interface ApiResponse<T> {
  status: boolean;
  statusCode: number;
  data: T;
  errors: string[];
}
export interface User {
  id: number;
  username: string;
  isActive?: boolean;
  isDeleted?: boolean;
  userTypeId?: number;
  createdDate?: string;
}
export type UserDto = Omit<User, 'id'> & { id?: number; password?: string };
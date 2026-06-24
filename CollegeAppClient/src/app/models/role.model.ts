export interface Role {
  id: number;
  roleName: string;        // confirm against RoleDTO
  description?: string;
  isActive?: boolean;
}
export type RoleDto = Omit<Role, 'id'> & { id?: number };
export interface RolePrivilege {
  id: number;
  roleId: number;
  rolePrivilegeName: string;
  description?: string;
  isActive?: boolean;
}
export type RolePrivilegeDto = Omit<RolePrivilege, 'id'> & { id?: number };
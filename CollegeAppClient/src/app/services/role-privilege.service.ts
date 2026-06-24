import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { RolePrivilege, RolePrivilegeDto } from '../models/role-privilege.model';

@Injectable({ providedIn: 'root' })
export class RolePrivilegeService {
  // NOTE: controller name is "RolePrivilege" exactly as listed
  private base = `${environment.apiUrl}/RolePrivilege`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<RolePrivilege[]> {
    return this.http.get<ApiResponse<RolePrivilege[]>>(`${this.base}/All`).pipe(map(r => r.data ?? []));
  }
  getByRole(roleId: number): Observable<RolePrivilege[]> {
    return this.http.get<ApiResponse<RolePrivilege[]>>(`${this.base}/ByRole/${roleId}`).pipe(map(r => r.data ?? []));
  }
  // "Add" privilege to a role
  add(priv: RolePrivilegeDto): Observable<RolePrivilege> {
    return this.http.post<ApiResponse<RolePrivilege>>(`${this.base}/Create`, priv).pipe(map(r => r.data));
  }
  update(priv: RolePrivilege): Observable<ApiResponse<null>> {
    return this.http.put<ApiResponse<null>>(`${this.base}/updaterecord`, priv);
  }
  // "Remove" privilege by its id
  remove(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.base}/deleterolePrivilege/${id}`);
  }
}
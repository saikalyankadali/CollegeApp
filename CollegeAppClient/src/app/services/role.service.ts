import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Role, RoleDto } from '../models/role.model';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private base = `${environment.apiUrl}/Role`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Role[]> {
    return this.http.get<ApiResponse<Role[]>>(`${this.base}/All`).pipe(map(r => r.data ?? []));
  }
  getById(id: number): Observable<Role> {
    return this.http.get<ApiResponse<Role>>(`${this.base}/${id}`).pipe(map(r => r.data));
  }
  create(role: RoleDto): Observable<Role> {
    return this.http.post<ApiResponse<Role>>(`${this.base}/Create`, role).pipe(map(r => r.data));
  }
  update(role: Role): Observable<ApiResponse<null>> {
    return this.http.put<ApiResponse<null>>(`${this.base}/updaterecord`, role);
  }
  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.base}/deleterole/${id}`);
  }
}
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { User, UserDto } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private base = `${environment.apiUrl}/User`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<User[]> {
    return this.http.get<ApiResponse<User[]>>(`${this.base}/All`).pipe(map(r => r.data ?? []));
  }
  getById(id: number): Observable<User> {
    return this.http.get<ApiResponse<User>>(`${this.base}/${id}`).pipe(map(r => r.data));
  }
  getByUsername(username: string): Observable<User> {
    return this.http.get<ApiResponse<User>>(`${this.base}/${username}`).pipe(map(r => r.data));
  }
  create(user: UserDto): Observable<User> {
    return this.http.post<ApiResponse<User>>(`${this.base}/Create`, user).pipe(map(r => r.data));
  }
  update(user: User): Observable<ApiResponse<null>> {
    return this.http.put<ApiResponse<null>>(`${this.base}/Update`, user);
  }
  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.base}/Delete/${id}`);
  }
}
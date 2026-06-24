import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { UserType } from '../models/user-type.model';

@Injectable({ providedIn: 'root' })
export class UserTypeService {
  private base = `${environment.apiUrl}/UserType`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<UserType[]> {
    return this.http
      .get<ApiResponse<UserType[]>>(`${this.base}/All`)
      .pipe(map(r => r.data ?? []));
  }
}
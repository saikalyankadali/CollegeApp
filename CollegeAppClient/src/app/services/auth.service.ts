import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginResponse } from '../models/login.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'jwt_token';
  private userKey = 'user_name';
  isLoggedIn = signal<boolean>(!!this.getToken());
  userName = signal<string | null>(localStorage.getItem(this.userKey));

  constructor(private http: HttpClient) {}

  login(userName: string, password: string): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${environment.apiUrl}/Login`, { userName, password })
      .pipe(
        tap((res) => {
          // Only store credentials on logical success — bad creds come back as HTTP 200 + status:false
          if (res.status && res.data?.token) {
            localStorage.setItem(this.tokenKey, res.data.token);
            localStorage.setItem(this.userKey, res.data.userName);
            this.isLoggedIn.set(true);
            this.userName.set(res.data.userName);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.isLoggedIn.set(false);
    this.userName.set(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
}
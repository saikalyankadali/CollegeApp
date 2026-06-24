import { Injectable, signal, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginResponse } from '../models/login.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'jwt_token';
  private userKey = 'user_name';

  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);

  isLoggedIn = signal<boolean>(this.isBrowser && !!this.getToken());
  userName = signal<string | null>(this.isBrowser ? localStorage.getItem(this.userKey) : null);

  constructor(private http: HttpClient) {}

  login(userName: string, password: string): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${environment.apiUrl}/Login`, { userName, password })
      .pipe(
        tap((res) => {
          if (res.status && res.data?.token && this.isBrowser) {
            localStorage.setItem(this.tokenKey, res.data.token);
            localStorage.setItem(this.userKey, res.data.userName);
            this.isLoggedIn.set(true);
            this.userName.set(res.data.userName);
          }
        })
      );
  }

  logout(): void {
    if (this.isBrowser) {
      localStorage.removeItem(this.tokenKey);
      localStorage.removeItem(this.userKey);
    }
    this.isLoggedIn.set(false);
    this.userName.set(null);
  }

  getToken(): string | null {
    return this.isBrowser ? localStorage.getItem(this.tokenKey) : null;
  }
}
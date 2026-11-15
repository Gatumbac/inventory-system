import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from './auth.models';

const TOKEN_KEY = 'token';
const ROLE_KEY = 'role';
const EMAIL_KEY = 'email';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  login(request: LoginRequest) {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap((response) => {
        localStorage.setItem(TOKEN_KEY, response.token);
        localStorage.setItem(ROLE_KEY, response.role);
        localStorage.setItem(EMAIL_KEY, request.email);
      }),
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(ROLE_KEY);
    localStorage.removeItem(EMAIL_KEY);
  }

  token(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  role(): string | null {
    return localStorage.getItem(ROLE_KEY);
  }

  email(): string | null {
    return localStorage.getItem(EMAIL_KEY);
  }

  isAdmin(): boolean {
    return this.role() === 'Admin';
  }

  isLoggedIn(): boolean {
    return this.token() !== null;
  }
}

import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { LoginRequest, LoginResponse } from '../models/api.models';

const TOKEN_KEY = 'rte.token';
const LOGIN_KEY = 'rte.login';

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly loginAtual = signal<string | null>(sessionStorage.getItem(LOGIN_KEY));

  constructor(private http: HttpClient, private router: Router) {}

  get token(): string | null {
    return sessionStorage.getItem(TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    return !!this.token;
  }

  login(payload: LoginRequest) {
    return this.http.post<LoginResponse>('/api/auth/login', payload).pipe(
      tap((response) => {
        sessionStorage.setItem(TOKEN_KEY, response.token);
        sessionStorage.setItem(LOGIN_KEY, response.login);
        this.loginAtual.set(response.login);
      })
    );
  }

  logout(): void {
    sessionStorage.removeItem(TOKEN_KEY);
    sessionStorage.removeItem(LOGIN_KEY);
    this.loginAtual.set(null);
    void this.router.navigate(['/login']);
  }
}

import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  RefreshTokenRequest,
} from '../models/api.models';
import { AuthUser } from '../models/marketplace.models';

const TOKEN_KEY = 'll_access_token';
const REFRESH_KEY = 'll_refresh_token';
const USER_KEY = 'll_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly base = `${environment.apiBaseUrl}/auth`;

  private readonly _user = signal<AuthUser | null>(this.loadUser());
  readonly user = this._user.asReadonly();
  readonly isLoggedIn = computed(() => this._user() !== null);

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.base}/login`, request).pipe(
      tap((res) => this.persistSession(res))
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.base}/register`, request).pipe(
      tap((res) => this.persistSession(res))
    );
  }

  logout(): Observable<{ message: string }> {
    const refreshToken = this.getRefreshToken();
    this.clearSession();
    if (!refreshToken) {
      this.router.navigate(['/']);
      return new Observable((sub) => {
        sub.next({ message: 'Logged out' });
        sub.complete();
      });
    }
    return this.http
      .post<{ message: string }>(`${this.base}/logout`, { refreshToken })
      .pipe(
        tap(() => this.router.navigate(['/'])),
        catchError(() => {
          this.router.navigate(['/']);
          return throwError(() => new Error('Logout failed'));
        })
      );
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return throwError(() => new Error('No refresh token'));
    }
    const body: RefreshTokenRequest = { refreshToken };
    return this.http
      .post<AuthResponse>(`${this.base}/refresh-token`, body)
      .pipe(tap((res) => this.persistSession(res)));
  }

  getAccessToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_KEY);
  }

  updateUserName(firstName: string, lastName: string): void {
    const current = this._user();
    if (current) {
      const updated = { ...current, firstName, lastName };
      this._user.set(updated);
      localStorage.setItem(USER_KEY, JSON.stringify(updated));
    }
  }

  private persistSession(res: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, res.accessToken);
    localStorage.setItem(REFRESH_KEY, res.refreshToken);
    const user: AuthUser = { userId: res.userId, email: res.email };
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this._user.set(user);
  }

  private clearSession(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_KEY);
    localStorage.removeItem(USER_KEY);
    this._user.set(null);
  }

  private loadUser(): AuthUser | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as AuthUser;
    } catch {
      return null;
    }
  }
}

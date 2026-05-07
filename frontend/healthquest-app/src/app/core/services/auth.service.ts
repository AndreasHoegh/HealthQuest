import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/api';
  private tokenKey = 'healthquest_token';
  
  isAuthenticated = signal(false);
  currentUser = signal<any>(null);

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Check if token exists on init
    const token = this.getToken();
    if (token) {
      this.isAuthenticated.set(true);
      // Decode token to get user info (simple decode, no validation)
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        this.currentUser.set({
          email: payload.email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
          role: payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
        });
      } catch (e) {
        console.error('Error parsing token:', e);
      }
    }
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/Auth/login`, credentials)
      .pipe(
        tap(response => {
          this.setToken(response.token);
          this.isAuthenticated.set(true);
          this.currentUser.set({
            email: response.email,
            role: response.role
          });
        })
      );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/Auth/register`, data)
      .pipe(
        tap(response => {
          this.setToken(response.token);
          this.isAuthenticated.set(true);
          this.currentUser.set({
            email: response.email,
            role: response.role
          });
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }
}

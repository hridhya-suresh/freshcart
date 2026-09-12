import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7136/api/Auth';

  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }

  login(loginData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, loginData);
  }

  storeLogin(response: any): void {
    const token = response?.token ?? response?.Token;

    if (!token) {
      console.error('Login response did not include a token');
      return;
    }

    localStorage.setItem('token', token);

    localStorage.setItem(
      'currentUser',
      JSON.stringify({
        userId: response.userId ?? response.UserId,
        firstName: response.firstName ?? response.FirstName,
        lastName: response.lastName ?? response.LastName,
        email: response.email ?? response.Email
      })
    );
  }

  getToken(): string | null {
    const token = localStorage.getItem('token');

    if (!token || token === 'undefined' || token === 'null') {
      return null;
    }

    return token;
  }

  getCurrentUser() {
    const user = localStorage.getItem('currentUser');

    return user ? JSON.parse(user) : null;
  }

  isLoggedIn(): boolean {

    return !!this.getToken();

  }

  logout(): void {

    localStorage.removeItem('token');

    localStorage.removeItem('currentUser');

  }

}
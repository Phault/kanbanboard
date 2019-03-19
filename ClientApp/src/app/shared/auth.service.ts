import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable()
export class AuthService {

  public isAuthenticatedChange: Observable<boolean>;
  private statusSource: BehaviorSubject<boolean>;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.statusSource = new BehaviorSubject<boolean>(this.isAuthenticated);
    this.isAuthenticatedChange = this.statusSource.asObservable();
  }

  public get isAuthenticated(): boolean {
    const expiresAt = JSON.parse(localStorage.getItem('expires_at') || '0');
    return new Date().getTime() < expiresAt;
  }

  public get token(): string {
    return this.isAuthenticated ? localStorage.getItem('access_token') : null;
  }

  public login(username: string, password: string): Observable<AccessToken> {
    const body = { username, password };

    return this.http.post<AccessToken>(`${this.baseUrl}api/auth/login`, body)
      .pipe(
        tap(result => this.setSession(result))
        );
  }

  public register(username: string, password: string, email: string): Observable<AccessToken> {
    const body = { username, password, email };

    return this.http.post<AccessToken>(`${this.baseUrl}api/auth/register`, body)
      .pipe(
        tap(result => this.setSession(result))
        );
  }

  public logout(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('expires_at');
    this.statusSource.next(false);
  }

  private setSession(authResult: AccessToken): void {
    localStorage.setItem('access_token', authResult.accessToken);
    localStorage.setItem('expires_at', JSON.stringify(authResult.expiresAt * 1000));
    this.statusSource.next(true);
  }
}

import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class AuthHeaderInterceptor implements HttpInterceptor {
  constructor(private auth: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<any> {
    if (!this.auth.isAuthenticated) {
      return next.handle(req);
    }

    const updatedRequest = req.clone({
      setHeaders: {
        'Authorization': `Bearer ${this.auth.token}`
      }
    });

    return next.handle(updatedRequest);
  }
}

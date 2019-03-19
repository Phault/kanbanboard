import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class ContentTypeHeaderInterceptor implements HttpInterceptor {
  constructor() { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<any> {
    const updatedRequest = req.clone({
      setHeaders: {
        'Content-Type': 'application/json'
      }
    });

    return next.handle(updatedRequest);
  }
}

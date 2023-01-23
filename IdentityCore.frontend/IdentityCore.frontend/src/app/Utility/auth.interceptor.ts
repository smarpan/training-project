import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../Service/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private auth:AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (!this.auth.isAuthenticatedGetter) {
      return next.handle(request);
    }
    const req1 = request.clone({
      headers: request.headers.set('Authorization', `Bearer ${this.auth.tokenGetter}`),
    });
    return next.handle(req1);
  }
}

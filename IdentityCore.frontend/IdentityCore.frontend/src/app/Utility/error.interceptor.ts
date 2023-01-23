import { Injectable } from '@angular/core';
import { catchError } from "rxjs/operators";
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { AuthService } from '../Service/auth.service';
import { NotificationService } from '../Service/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private authService:AuthService,private notifyService:NotificationService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError((err) => {
      if (err.status === 401) {
        this.authService.Logout();       
      }
      if (err.name ="HttpErrorResponse") {
        this.notifyService.Notify("Connection refused");
        //console.log("error triggered in error interceptor");
        //console.log(err);
      }
      const error = err.error.message || err.statusText;
      return throwError(error);
    }));
  }
}

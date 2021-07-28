import { User } from './../_models/User';
import { AccountService } from './../_services/account.service';
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { JwtHelper } from 'angular2-jwt';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentUser: User
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => currentUser = user)
    if (currentUser) {
      // let jwtHelper = new JwtHelper();
      // let token = currentUser.token;
      // let isExpired = jwtHelper.isTokenExpired(token);
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`
        }
      });
    }
    return next.handle(request);
  }
}

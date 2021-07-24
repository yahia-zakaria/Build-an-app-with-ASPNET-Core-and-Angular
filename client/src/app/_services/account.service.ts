import { PresenceService } from './presence.service';
import { environment } from './../../environments/environment';
import { User } from './../_models/User';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators'
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient, private presenceService: PresenceService) { }
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user)
          this.presenceService.createHubConnection(user);
        }
      })
    )
  }

  setCurrentUser(user: User){
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();
  }

  register(model:any){
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
          this.presenceService.createHubConnection(user);
        }
        return user;
      })
    )
  }

  getDecodedToken(token){
    return JSON.parse(atob(token.split(".")[1]));
  }
}

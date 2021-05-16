import { map } from 'rxjs/operators';
import { Member } from './../_models/member';
import { Observable, of } from 'rxjs';
import { HttpClient, JsonpClientBackend } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})

export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {
    if (this.members.length > 0)
      return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members=>{
        this.members = members;
        return members;
      })
    );
  }

  getMember(username: string) {
    const member = this.members.find(a => a.userName === username);
    if (member !== undefined)
      return of(member)
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.post<Member>(this.baseUrl + 'users', member).pipe(
      map(member=>{
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

}

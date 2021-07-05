import { AccountService } from './account.service';
import { User } from './../_models/User';
import { UserParams } from './../_models/userParams';
import { PaginatedResult } from './../_models/pagination';
import { Photo } from './../_models/photo';
import { map, take } from 'rxjs/operators';
import { Member } from './../_models/member';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpParams, JsonpClientBackend } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})

export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user : User;
  userParams : UserParams;


  constructor(private http: HttpClient, private accountService:AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
   }

   getUserParams(){
     return this.userParams;
   }

   setUserParams(params : UserParams){
    this.userParams = params;
  }
  resetUserParams(){
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  getMembers(userParams: UserParams) {
    let response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response){
      return of(response);
    }

    let params = this.getPaginationHeader(userParams.pageNumber, userParams.pageSize);

    params = params.append("minAge", userParams.minAge.toString())
    params = params.append("maxAge", userParams.maxAge.toString())
    params = params.append("gender", userParams.gender)
    params = params.append("orderBy", userParams.orderBy)

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)
    .pipe(map(response=>{
      this.memberCache.set(Object.values(userParams).join('-'), response);
      return response;
    }));
  }


  getMember(username: string) {
    const member = [...this.memberCache.values()]
    .reduce((arr, elem)=>arr.concat(elem.result), [])
    .find((member:Member)=>member.userName === username);

    if(member){
      return of(member);
    }

    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.post<Member>(this.baseUrl + 'users', member).pipe(
      map(member => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.post(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.post(this.baseUrl + 'users/delete-photo/' + photoId, {});
  }

  private getPaginatedResult<T>(url, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body;
        console.log(response.headers.get('Pagination'));
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      }));
  }

  private getPaginationHeader(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append("pageNumber", pageNumber.toString())
    params = params.append("pageSize", pageSize.toString())

    return params;
  }

  addLike(username:string){
    return this.http.post(this.baseUrl + "Likes/" + username, {});
  }

  getLikes(predicate:string, pageNumber:number, pageSize:number){
    let params = this.getPaginationHeader(pageNumber, pageSize);
    params = params.append('predicate', predicate)

    return this.getPaginatedResult<Partial<Member[]>>(this.baseUrl + "likes", params);
  }

}

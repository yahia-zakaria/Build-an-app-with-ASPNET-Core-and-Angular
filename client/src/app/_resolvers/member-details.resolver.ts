import { MembersService } from './../_services/members.service';
import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MemberDetailsResolver implements Resolve<Member> {

  constructor(private memberService: MembersService) { }

  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    console.log(route.paramMap.get('username'))
    return this.memberService.getMember(route.paramMap.get('username'));
  }
}

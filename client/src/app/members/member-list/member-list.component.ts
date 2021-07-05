import { User } from './../../_models/User';
import { take } from 'rxjs/operators';
import { AccountService } from './../../_services/account.service';
import { UserParams } from './../../_models/userParams';
import { Pagination } from './../../_models/pagination';
import { Observable } from 'rxjs';
import { MembersService } from './../../_services/members.service';
import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  genderList:any = [{value : 'male', display : 'Males'}, {value : 'female', display : 'Females'}]


  constructor(private membersService: MembersService) {
    this.userParams = this.membersService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.membersService.setUserParams(this.userParams)
    this.membersService.getMembers(this.userParams).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChange(event: any) {
    this.membersService.setUserParams(this.userParams)
    this.userParams.pageNumber = event.page;
    this.loadMembers();
  }

  resetFilters(){
    this.userParams = this.membersService.resetUserParams();
    this.loadMembers();
  }

}

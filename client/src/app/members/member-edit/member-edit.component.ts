import { User } from './../../_models/User';
import { MembersService } from './../../_services/members.service';
import { Member } from 'src/app/_models/member';
import { AccountService } from './../../_services/account.service';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { take } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  member: Member;
  user: User;
  @ViewChild("editForm") editForm: NgForm;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event:any){
    if(this.editForm.dirty){
      $event.returnValue = true;
    }
  }
  constructor(private accountService: AccountService,
    private memberService: MembersService,
    private toastr: ToastrService) {

    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    console.log('user:', this.user)
    this.getCurrentUser();
  }
  getCurrentUser() {
    this.memberService.getMember(this.user.username).subscribe(member => this.member = member);
  }

  editMember() {
    this.memberService.updateMember(this.member).subscribe(()=>{
      this.toastr.success("Profile updated successfully");
      this.editForm.reset(this.member);
    });
  }
}

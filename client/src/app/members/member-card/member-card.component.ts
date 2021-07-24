import { PresenceService } from './../../_services/presence.service';
import { User } from './../../_models/User';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from './../../_services/members.service';
import { Member } from 'src/app/_models/member';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() member: Member;

  constructor(private memberService: MembersService, private toastr: ToastrService,
    public presenceService: PresenceService) {

  }

  ngOnInit(): void {
  }
  addLike(username: string) {
    this.memberService.addLike(username).subscribe(() => {
      this.toastr.success("you have liked " + username);
    })
  }
}

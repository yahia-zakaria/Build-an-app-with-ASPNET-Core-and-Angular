import { AccountService } from './../../_services/account.service';
import { PresenceService } from './../../_services/presence.service';
import { take } from 'rxjs/operators';
import { Message } from './../../_models/message';
import { Member } from './../../_models/member';
import { MembersService } from './../../_services/members.service';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from 'src/app/_services/message.service';
import { User } from 'src/app/_models/User';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  activeTab: TabDirective;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  member: Member;
  messages: Message[] = []
  user: User;

  constructor(private memberService: MembersService, private router: ActivatedRoute,
    private messageService: MessageService, public presenceService: PresenceService,
    private accountService: AccountService, private rtr: Router) {
    this.rtr.routeReuseStrategy.shouldReuseRoute = () => false;
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    })
  }


  ngOnInit(): void {

    this.router.data.subscribe(data => {
      this.member = data.member;
    })

    this.router.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];

    this.galleryImages = this.getImages();
  }


  getImages(): NgxGalleryImage[] {
    var images = [];
    if (this.member) {
      for (const image of this.member.photos) {
        images.push({
          small: image.url,
          medium: image.url,
          big: image.url
        });
      }
    }
    return images;
  }

  loadMessages() {
    this.messageService.getMessageThread(this.member.userName).subscribe(rseponse => {
      this.messages = rseponse;
    })
  }

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data
    if (this.activeTab.heading === "Messages") {
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}

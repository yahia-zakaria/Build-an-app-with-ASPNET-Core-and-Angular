import { take } from 'rxjs/operators';
import { Message } from './../../_models/message';
import { Member } from './../../_models/member';
import { MembersService } from './../../_services/members.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  activeTab: TabDirective;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  member: Member;
  messages: Message[] = []

  constructor(private memberService: MembersService, private router: ActivatedRoute,
    private messageService: MessageService) { }

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
      this.loadMessages();
    }
  }
}

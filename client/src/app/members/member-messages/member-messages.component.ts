import { NgForm } from '@angular/forms';
import { Message } from './../../_models/message';
import { MessageService } from './../../_services/message.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() username: string;
  @ViewChild("messageForm") messageForm: NgForm;
  messageContent: string;

  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
    console.log(this.username);

  }
  addMessage() {
    this.messageService.addMessage(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
    })
  }

}

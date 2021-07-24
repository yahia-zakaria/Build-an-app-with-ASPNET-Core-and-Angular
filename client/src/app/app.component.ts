import { PresenceService } from './_services/presence.service';
import { AccountService } from './_services/account.service';
import { User } from './_models/User';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(private accountService: AccountService, private presenceService: PresenceService) { }
  ngOnInit() {

    this.setCurrentUser();
  }
  title = 'The Dating App';

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'))
    if(user){
      this.accountService.setCurrentUser(user);
      this.presenceService.createHubConnection(user);
    }
  }
}

import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  RegisterMode = false;

  constructor() { }

  ngOnInit(): void {
  }

  RegisterToggle() {
    this.RegisterMode = !this.RegisterMode;
  }

  cancelRegisteration(event:boolean){
    this.RegisterMode = event
  }
}

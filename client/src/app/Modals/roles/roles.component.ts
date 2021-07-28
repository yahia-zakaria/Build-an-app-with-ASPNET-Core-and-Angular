import { User } from './../../_models/User';
import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.css']
})
export class RolesComponent implements OnInit {
  @Output() updateSelectedRoles = new EventEmitter();
  user: User;
  roles: any;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  updateRoles() {
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}

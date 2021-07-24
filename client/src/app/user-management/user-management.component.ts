import { RolesComponent } from './../Modals/roles/roles.component';
import { AdminService } from './../_services/admin.service';
import { AdminGuard } from './../_guards/admin.guard';
import { Component, OnInit } from '@angular/core';
import { User } from '../_models/User';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  users: Partial<User[]>
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe(users => {
      this.users = users;
    })
  }

  openRolesModal(user: User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    };
    this.bsModalRef = this.modalService.show(RolesComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe(values=>{
      var rolesToUpdate = {
        roles : [...values.filter(el=>el.checked === true).map(el=>el.name)]
      }
      if(rolesToUpdate){
        console.log(rolesToUpdate.roles)
        this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(()=>{
          user.roles = rolesToUpdate.roles;
        })
      }
    });
  }

  getRolesArray(user) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      { name: 'Admin', value: 'Admin' },
      { name: 'Member', value: 'Member' },
      { name: 'Moderator', value: 'Moderator' }
    ];

    availableRoles.forEach(role => {
      let isMatch = false;
      for (const userRole of userRoles) {
        if(role.name === userRole){
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break;
        }
      }
      if(!isMatch){
        role.checked = false;
        roles.push(role);
      }
    })

    return roles;
  }

}

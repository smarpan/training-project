import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CreateRoleModel } from '../../../Model/CreateRoleModel';
import { AuthService } from '../../../Service/auth.service';
import { NotificationService } from '../../../Service/notification.service';
import { RoleService } from '../../../Service/role.service';

@Component({
  selector: 'app-create-role',
  templateUrl: './create-role.component.html',
  styleUrls: ['./create-role.component.css']
})
export class CreateRoleComponent {
  roleobj: CreateRoleModel = {
    roleName: ''
  };
  hasAccessToCreateRole: boolean = false;

  createForm!: FormGroup;

  message: string = '';

  private createsubs!: Subscription;

  constructor(
    private authService: AuthService,
    private roleService: RoleService,
    private notifyService: NotificationService,
    private fb: FormBuilder) {

  }
  ngOnInit() {
    this.hasAccessToCreateRole = this.authService.hasAccessToCreateRoleGetter;
    this.createForm = this.fb.group({
      roleName: ['', [Validators.required]]
    });
  
  } 

  CreateRole(roleName: string) {
    
    this.roleobj.roleName = roleName;
    this.createsubs = this.roleService.CreateRole(this.roleobj).subscribe({
      next: (result) => {
        if (result.status) {
         // console.log(result);
          this.message = '';
          this.notifyService.Notify(result.message);
          this.roleService.RoleAddedNotify();
        } else {
          this.message = result.message;
          this.notifyService.Notify(result.message);
          //console.log(result);
        }
      },
      error: (er) => {       
        console.log(er);
      }
    });
  }
  ngOnDestroy() {
    this.createsubs?.unsubscribe();
  }
}

import { Component, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { AssignRoleModel } from '../../../Model/AssignRoleModel';
import { AuthService } from '../../../Service/auth.service';
import { NotificationService } from '../../../Service/notification.service';
import { RoleService } from '../../../Service/role.service';
import { UserService } from '../../../Service/user.service';

@Component({
  selector: 'app-assign-role-to-user',
  templateUrl: './assign-role-to-user.component.html',
  styleUrls: ['./assign-role-to-user.component.css']
})
export class AssignRoleToUserComponent {
  @Input() roleName!: string;
  message: string = '';
  notifySuccessAssignMessage: string = "Role Assigned";

  submitDisabled: boolean = false;
  hasAccessToAssignRole: boolean = false;
  statusChange: boolean = false;
  availableEmail: boolean = false;

  assignRole: AssignRoleModel = {
    userEmail: '',
    roleName: ''
  };

  assignRoleForm!: FormGroup;

  private assignsubs!: Subscription;
  private availablesubs!: Subscription;

  constructor(private roleService: RoleService,
    private authService: AuthService,
    private fb: FormBuilder,
    private userService: UserService,
    private notifyService: NotificationService) { }

  ngOnInit() {
    this.hasAccessToAssignRole = this.authService.hasAccessToAssignRoleGetter;

    this.assignRoleForm = this.fb.group({
      userEmail: [this.assignRole.userEmail, [Validators.required, Validators.email]]

    });
  }

  resetChecker() {
    this.statusChange = true;
    this.submitDisabled = true;
  }

  isAvailable(email: string) {
    this.statusChange = false;
    this.submitDisabled = false;
    console.log(this.assignRoleForm);
    this.availablesubs = this.userService.checkAvailableEmail(email).subscribe({
      next: (result) => {
        this.availableEmail = result.status;
        this.submitDisabled = result.status;      
        //console.log(result);
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  AssignRole() {
    Object.assign(this.assignRole, this.assignRoleForm.value, this.roleName);
    console.log(this.assignRole);
    this.assignsubs = this.roleService.AssignRole(this.assignRole).subscribe({
      next: (result) => {       
          this.notifyService.Notify(this.notifySuccessAssignMessage);       
      },
      error: (er) => {

        console.log(er);
      }
    });
  }
  ngonDestroy() {

    this.availablesubs?.unsubscribe();
    this.assignsubs?.unsubscribe();
  }
}

import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { AssignRoleModel } from '../../../Model/AssignRoleModel';
import { ClaimUserModel } from '../../../Model/claimUserModel';
import { UserModel } from '../../../Model/UserModel';
import { AuthService } from '../../../Service/auth.service';
import { NotificationService } from '../../../Service/notification.service';
import { RoleService } from '../../../Service/role.service';
import { UserService } from '../../../Service/user.service';

@Component({
  selector: 'app-user-claim-info-dialog',
  templateUrl: './user-claim-info-dialog.component.html',
  styleUrls: ['./user-claim-info-dialog.component.css']
})
export class UserClaimInfoDialogComponent {

  hasAccessToRevokeRole: boolean = false;
  hasAccessToRemoveClaimFromUser: boolean = false;

  userClaimSub!: Subscription;
  revokeClaimSub!: Subscription;
  revokeRoleSub!: Subscription;

  user: UserModel = {
    id: '',
    userName: '',
    email: '',
    phoneNumber: '',
    gender: '',
    role: '',
    profileImage: '',
    claim: []
  };

  claimUser: ClaimUserModel = {
    claimName: '',
    userEmail: ''
  };

  roleUser: AssignRoleModel = {
    roleName: '',
    userEmail: ''
  };

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private roleService: RoleService,
    private dialogRef: MatDialogRef<UserClaimInfoDialogComponent>,
    private notifyService: NotificationService,
    @Inject(MAT_DIALOG_DATA) public data: { userEmail: string, id: string }) { }

  ngOnInit() {
    this.userClaimSub = this.userService.GetUser(this.data.userEmail).subscribe({
      next: (result) => {

        if (result.status) {
          this.user = result.data;
          this.hasAccessToRevokeRole = this.authService.hasAccessToRevokeRoleGetter;
          this.hasAccessToRemoveClaimFromUser = this.authService.hasAccessToRemoveClaimFromUserGetter;
        } else {
          this.notifyService.Notify(result.message);
        }
      },
      error: (er) => {
        this.notifyService.Notify(er.name);
      }
    });
  }

  revokeAccessToRole(roleName: string, user: UserModel) {
    this.roleUser.roleName = roleName;
    this.roleUser.userEmail = user.email;

    this.revokeRoleSub = this.roleService.RevokeRole(this.roleUser).subscribe({
      next: (result) => {

        this.notifyService.Notify(result.message);
        if (result.status) {

          this.ngOnInit();
        } else {

          console.log(result);
        }
      },
      error: (er) => {

        console.log(er);
      }
    });
  }

  revokeAccessToClaim(claimName: string, user: UserModel) {

    this.claimUser.claimName = claimName;
    this.claimUser.userEmail = user.email;
    this.revokeClaimSub = this.userService.RevokeAccessFromUser(this.claimUser).subscribe({
      next: (result) => {

        this.notifyService.Notify(result.message);
        if (result.status) {

          console.log(result);
          this.ngOnInit();
        } else {

          console.log(result);

        }
      },
      error: (er) => {
        console.log(er);
      }
    });
  }

  ngOnDestroy() {
    this.revokeClaimSub?.unsubscribe();
    this.revokeRoleSub?.unsubscribe();
  }
}

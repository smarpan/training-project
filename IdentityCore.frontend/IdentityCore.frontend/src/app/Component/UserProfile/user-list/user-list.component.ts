import { Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../Service/auth.service';
import { NotificationService } from '../../../Service/notification.service';
import { UserService } from '../../../Service/user.service';
import { RoleService } from '../../../Service/role.service';
import { MiniUserModel } from '../../../Model/MiniUserModel';
import { MatDialog } from '@angular/material/dialog';
import { UserClaimInfoDialogComponent } from '../../UserProfile/user-claim-info-dialog/user-claim-info-dialog.component';
import { AddClaimToUserComponent } from '../add-claim-to-user/add-claim-to-user.component';
import { FullUserInfoComponent } from '../../UserProfile/full-user-info/full-user-info.component';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent {
  userList: MiniUserModel[] = [];

  userListSubscription!: Subscription;
  filterString: string = '';

  role: string[] = [];
  // isAdmin: boolean = false;
  hasAccessToViewUserList: boolean = false;
  hasAccessToUpdateUserData: boolean = false;
  hasAccessToViewUserData: boolean = false;
  hasAccessToViewUserClaimsAndRoles: boolean = false;
  hasAccessToAddClaimToUser: boolean = false;
  loader: boolean = true;

  constructor(private userService: UserService,
    private authService: AuthService,
    private roleService: RoleService,
    private notifyService: NotificationService,
    private matDialog: MatDialog) { }

  ngOnInit() {

    this.userListSubscription = this.userService.GetUserList().subscribe({
      next: (result) => {
        this.loader = false;
        if (result.status) {
          this.userList = result.data;
          this.hasAccessToViewUserList = this.authService.hasAccessToViewUserListGetter;
          this.hasAccessToUpdateUserData = this.authService.hasAccessToUpdateUserDataGetter;
          this.hasAccessToViewUserClaimsAndRoles = this.authService.hasAccessToViewUserClaimsAndRolesGetter;
          this.hasAccessToAddClaimToUser = this.authService.hasAccessToAddClaimToUserGetter;
          this.hasAccessToViewUserData = this.authService.hasAccessToViewRoleGetter;
        }
        console.log(result.message);
      },
      error: (error) => {
        console.log(error);
      }

    });
  }
 
  //applyFilter(event: Event) {
  //  const filterValue = (event.target as HTMLInputElement).value;
  //  console.log(this.userList.filter(x => x.email.toLowerCase().includes(filterValue.trim().toLowerCase()))); 
  //}

  openDialogForAssigningClaims(userEmail: string) {
    this.matDialog.open(AddClaimToUserComponent, {
      data: {userEmail},
      hasBackdrop: true,
     
    });
  } 

  openDialogForClaimsAndRoles(userEmail:string, id:string) {
    this.matDialog.open(UserClaimInfoDialogComponent, {
      data: { userEmail: userEmail,id:id },
      hasBackdrop:true
    });
  }

  openDialogForPersonalInfo(userEmail: string, id: string) {
    this.matDialog.open(FullUserInfoComponent, {
      data: { userEmail: userEmail, id: id },
      hasBackdrop: true,
      minWidth: 300,
      maxHeight:'90vh'
    });
  }

  ngOnDestroy() {

    this.userListSubscription?.unsubscribe();

  }


}

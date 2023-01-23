import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { AssignRoleModel } from '../../../Model/AssignRoleModel';
import { ClaimUserModel } from '../../../Model/claimUserModel';
import { UserModel } from '../../../Model/UserModel';
import { AuthService } from '../../../Service/auth.service';
import { NotificationService } from '../../../Service/notification.service';
import { RoleService } from '../../../Service/role.service';
import { UserService } from '../../../Service/user.service';
import { ImageCropperComponent } from '../../segments/image-cropper/image-cropper.component';

@Component({
  selector: 'app-full-user-info',
  templateUrl: './full-user-info.component.html',
  styleUrls: ['./full-user-info.component.css']
})
export class FullUserInfoComponent {
  hasAccessToViewUserData: boolean = false;
  hasAccessToUpdateUserData: boolean = false;
  hasAccessToViewUserClaimsAndRoles: boolean = false;
  edit: boolean = false;
  loader: boolean = true;

  user: UserModel = {
    id:'',
    userName: '',
    email: '',
    phoneNumber: '',
    gender: '',
    role: '',
    profileImage: '',
    claim: []
  };

  roleUser: AssignRoleModel = {
    roleName: '',
    userEmail: ''
  };

  claimUser: ClaimUserModel = {
    claimName: '',
    userEmail: ''
  };

  userClaimSub!: Subscription;
  revokeClaimSub!: Subscription;
  revokeRoleSub!: Subscription;
  subscriptionHandler!: Subscription;

  imagePreview: string = '';
  imageTemp: string = '';
  rawImage: any;

  @ViewChild("fileInput", { static: false })
  inputVar!: ElementRef;

  updateForm!: FormGroup;

  constructor(
    private matDialog: MatDialog,
    private fb: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private roleService: RoleService,
    private dialogRef: MatDialogRef<FullUserInfoComponent>,
    private notifyService: NotificationService,
    @Inject(MAT_DIALOG_DATA) public data: { userEmail: string, id: string }) { }

  ngOnInit() {
    this.userClaimSub = this.userService.GetUser(this.data.userEmail).subscribe({
      next: (result) => {
        this.loader = false;
        if (result.status) {          
          this.user = result.data;
          this.imagePreview ="data:image/png;base64,"+ this.user.profileImage;
          this.hasAccessToViewUserData = this.authService.hasAccessToViewUserDataGetter;
          this.hasAccessToUpdateUserData = this.authService.hasAccessToUpdateUserDataGetter;
          this.hasAccessToViewUserClaimsAndRoles = this.authService.hasAccessToViewUserClaimsAndRolesGetter;
          this.updateForm = this.fb.group({
            phoneNumber: [this.user.phoneNumber],
            userName: [this.user.userName],
            gender: [this.user.gender],
            profileImage: [this.user.profileImage]
          });
                   
        } else {
          this.notifyService.Notify(result.message);
        }
      },
      error: (er) => {
        this.notifyService.Notify(er.name);
      }
    });
    
  }

  Edit() {
    this.edit = !this.edit;
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

  onSubmit() {
    Object.assign(this.user, this.updateForm.value);
    this.user.profileImage = this.imageTemp;
    this.user.claim = [];
    console.log(this.user);
    this.userService.UpdateUserInfo(this.user).subscribe({
      next: (result) => {
        if (result.status) {
          this.notifyService.Notify(result.message);
          
          this.dialogRef.close();
        } else {
          this.notifyService.Notify(result.message);
        }
      },
      error: (er) => {
        console.log(er);
        this.notifyService.Notify(er.name);
      }
    });
  }

  fileRawSave(event: any) {
    console.log("processor is called");
    this.rawImage = event;
    this.subscriptionHandler = this.matDialog.open(ImageCropperComponent, {
      data: { rawImage: this.rawImage,aspectRatio:1/1 },
      hasBackdrop: false
    }).afterClosed().subscribe({
      next: (result) => {
        this.user.profileImage = result;
        this.imageTemp = this.user.profileImage;
        this.inputVar.nativeElement.value = "";
        this.imagePreview = "data:image/png;base64," + result;
      },
      error: (er) => {
        console.log(er);
      }
    });
  }

  ngOnDestroy() {
    this.revokeClaimSub?.unsubscribe();
    this.revokeRoleSub?.unsubscribe();
    this.userClaimSub?.unsubscribe();
  }
}

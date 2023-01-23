import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder,FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ClaimUserModel } from '../../../Model/claimUserModel';
import { AuthService } from '../../../Service/auth.service';
import { NotificationService } from '../../../Service/notification.service';
import { UserService } from '../../../Service/user.service';

@Component({
  selector: 'app-add-claim-to-user',
  templateUrl: './add-claim-to-user.component.html',
  styleUrls: ['./add-claim-to-user.component.css']
})

export class AddClaimToUserComponent {
  claimUser: ClaimUserModel = {
      claimName: '',
      userEmail: ''
  };

  AssignClaimToUserForm!: FormGroup;

  submitDisabled: boolean = false;
  availableEmail: boolean = false;
  statusChange: boolean = false;

  hasAccessToAddClaimToUser: boolean = false;

  private availablesubs!: Subscription;
  private claimsubs!: Subscription;

  constructor(private fb: FormBuilder,
    private authService: AuthService,
    private notifyService: NotificationService,
    private userService: UserService,
    @Inject(MAT_DIALOG_DATA) public data: { userEmail: string },
    private matDialogRef: MatDialogRef<AddClaimToUserComponent>  ) {

  }
  ngOnInit() {
    this.hasAccessToAddClaimToUser = this.authService.hasAccessToAddClaimToUserGetter;

    this.AssignClaimToUserForm = this.fb.group({
      userEmail: [this.data.userEmail, [Validators.required, Validators.email]],
      claimName: [this.claimUser.claimName, [Validators.required]]
      });
  }
  resetChecker() {
    this.statusChange = true;
    this.submitDisabled = true;
  }

  isAvailable(email: string) {
    this.statusChange = false;
    this.availablesubs = this.userService.checkAvailableEmail(email).subscribe({
      next: (result) => {
        this.availableEmail = result.status;
        this.submitDisabled = result.status;
        console.log(result);
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  Submit() {

    Object.assign(this.claimUser, this.AssignClaimToUserForm.value);
    this.claimsubs = this.userService.AddClaimToUser(this.claimUser).subscribe({
      next: (result) => {
        if (result.status) {
          this.notifyService.Notify("Claim is added");
          this.matDialogRef.close();
        } else {
          this.notifyService.Notify(result.message);
        }

        console.log(result);
      },
      error: (er) => {
        console.log(er);
      }
      });
    
   
  }
  ngOnDestroy() {
    this.availablesubs?.unsubscribe();
    this.claimsubs?.unsubscribe();
  }
}

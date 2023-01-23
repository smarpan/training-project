import { Component, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { UserModel } from '../../../Model/UserModel';
import { AuthService } from '../../../Service/auth.service';
import { NotificationService } from '../../../Service/notification.service';
import { UserService } from '../../../Service/user.service';
import { ChatService } from '../../../Service/chat.service';
@Component({
  selector: 'app-user-data',
  templateUrl: './user-data.component.html',
  styleUrls: ['./user-data.component.css']
})
export class UserDataComponent {
  @Input() id!:string;
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
  alreadyAddedToMessenger:boolean=false;
  subscriptionHandler!: Subscription;

  constructor(
    private matDialog: MatDialog,
    private authService: AuthService,
    private chatService:ChatService,
    private userService: UserService,
    private notifyService:NotificationService
  ) { }

  ngOnInit() {
    

    this.subscriptionHandler = this.userService.GetUserById(this.id).subscribe({
      next: (result) => {
        //this.notifyService.Notify(result.message);
        console.log(result.message);
        if (result.status) {
          this.user = result.data;
          this.alreadyAddedToMessenger = this.chatService.CheckIfContactExists(result.data.id);
        }
      },
      error: (er) => {
        console.log(er.name);
      }
    });
  }

  addToMessenger() {
    this.chatService.contactListMiddleware(this.user.id, this.user.userName);
    this.alreadyAddedToMessenger=true;
  }

  ngOnDestroy() {
    this.subscriptionHandler?.unsubscribe();
  }
}

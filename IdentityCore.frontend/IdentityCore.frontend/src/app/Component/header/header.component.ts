import { Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserModel } from '../../Model/UserModel';
import { AuthService } from '../../Service/auth.service';
import { ChatService } from '../../Service/chat.service';
import { LoaderService } from '../../Service/loader.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
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

  hoverUsername: boolean = false;
  hasAccessToViewRole: boolean = false;
  hasAccessToViewUserList: boolean = false;
  isAuthenticated: boolean = false;

  imagePreview: string = '';
  myId: String = '';

  private loginsubscription!: Subscription;

  constructor(
    public loader:LoaderService,
    private authService: AuthService) { }

  ngOnInit() {
    this.myId = this.authService.user.id;

    this.loginsubscription = this.authService.loginData.subscribe({
      next: (user) => {
        this.user = user;
        console.log("this is header:");
        console.log(this.user);
        this.imagePreview = "data:image/png;base64,"+ this.user.profileImage;
        this.hasAccessToViewRole = this.authService.hasAccessToViewRoleGetter;
        this.hasAccessToViewUserList = this.authService.hasAccessToViewUserListGetter;
        this.isAuthenticated = this.authService.isAuthenticatedGetter;
      },
      error: (er) => {
        console.log(er.name);
      }
    });

  }

  ngOnDestroy() {
    this.loginsubscription?.unsubscribe();

  }

  Logout() {
    this.authService.Logout();
  }
}

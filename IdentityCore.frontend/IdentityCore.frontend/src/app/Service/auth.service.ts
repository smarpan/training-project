import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, Subject, Subscription } from 'rxjs';
import { ClaimModel } from '../Model/claimModel';
import { LoginModel } from '../Model/loginModel';
import { ResponseApi } from '../Model/ResponseApi';
import { UserModel } from '../Model/UserModel';
import { ChatService } from './chat.service';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})

export class AuthService {

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
  loginFeedData: ResponseApi = {
    status: false,
    message: '',
    data: undefined,
    token: '',
    role: undefined,
    errors: undefined
  };
  private checkIfSessionExist: ResponseApi = {
    status: false,
    message: '',
    data: undefined,
    token: '',
    role: undefined,
    errors: undefined
  };
  //role permissions.

  private hasAccessToCreateRole: boolean = false;
  private hasAccessToAssignRole: boolean = false;
  private hasAccessToRevokeRole: boolean = false;
  private hasAccessToViewRole: boolean = false;
  private hasAccessToDeleteRole: boolean = false;
  //hasAccessToViewRoleComponent: boolean = false;
  //user data permissions.
  private hasAccessToUpdateUserData: boolean = false;
  private hasAccessToViewUserData: boolean = false;
  private hasAccessToViewUserList: boolean = false;
  private hasAccessToViewUserClaimsAndRoles: boolean = false;
  //hasAccessToViewUserListComponent: boolean = false;
  //permissions for user.
  private hasAccessToAddClaimToUser: boolean = false;
  private hasAccessToRemoveClaimFromUser: boolean = false;

  //post
  private hasAccessToAddPost: boolean = false;
  private hasAccessToViewPostList: boolean = false;

  private isAuthenticated: boolean = false;

  private token: string = '';
  //private role: string[] = [];
  private claim: ClaimModel[] = [];

  loginData = new BehaviorSubject<UserModel>(this.user);
  loginFeed = new Subject<ResponseApi>;

  private loginSub!: Subscription;

  constructor(
    private chatService: ChatService,
    private http: HttpClient,
    private router: Router,
    private notifyService: NotificationService) { }

  get hasAccessToCreateRoleGetter() { return this.hasAccessToCreateRole; }
  get hasAccessToAssignRoleGetter() { return this.hasAccessToAssignRole; }
  get hasAccessToRevokeRoleGetter() { return this.hasAccessToRevokeRole; }
  get hasAccessToViewRoleGetter() { return this.hasAccessToViewRole; }
  get hasAccessToDeleteRoleGetter() { return this.hasAccessToDeleteRole; }
  get hasAccessToUpdateUserDataGetter() { return this.hasAccessToUpdateUserData; }
  get hasAccessToViewUserDataGetter() { return this.hasAccessToViewUserData; }
  get hasAccessToViewUserListGetter() { return this.hasAccessToViewUserList; }
  get hasAccessToViewUserClaimsAndRolesGetter() { return this.hasAccessToViewUserClaimsAndRoles; }
  get hasAccessToAddClaimToUserGetter() { return this.hasAccessToAddClaimToUser; }
  get hasAccessToRemoveClaimFromUserGetter() { return this.hasAccessToRemoveClaimFromUser; }
  get hasAccessToViewPostListGetter() { return this.hasAccessToViewPostList; }
  get hasAccessToAddPostGetter() { return this.hasAccessToAddPost; }
  get isAuthenticatedGetter() { return this.isAuthenticated; }
  get tokenGetter() { return this.token; }

  LoginDataBroadCast() {
    this.loginData.next(this.user);
  }

  Logout() {
    this.InternalLogoutProcess();
  }

  private InternalLogoutProcess() {
    this.isAuthenticated = false;
    this.ResetUserData();
    this.loginData.next(this.user);//broadcast changes.
    localStorage.removeItem("ResponseApi");
    this.chatService.exitConnection();
    this.notifyService.Notify("logged out");
    this.router.navigate(['login']);
  }

  CheckIfSessionExist() {
    this.checkIfSessionExist = JSON.parse(localStorage.getItem("ResponseApi") || '{}');
    if (this.checkIfSessionExist.status) {
      this.InternalLoginProcess(this.checkIfSessionExist);
    }
  }


  LoginEntryPoint(login: LoginModel) {

    this.loginSub = this.Login(login).subscribe({
      next: (middleware) => {
        if (middleware.status) {
          this.InternalLoginProcess(middleware);         
          /*this.loginFeed.next(this.loginFeedData);*/

        } else {
          this.loginFeedData = middleware;
          this.loginFeed.next(this.loginFeedData);
        }
      },
      error: (er) => {
        this.notifyService.Notify(er.name);
        this.loginFeed.next(er.name);
      }
    });

  }

  private Login(login: LoginModel): Observable<ResponseApi> {

    return this.http.post<ResponseApi>("http://localhost:45149/api/Account/login", login);

  }

  private InternalLoginProcess(middleware: ResponseApi) {

    this.token = middleware.token;
    // this.role = middleware.role;
    this.claim = middleware.data.claim;
    this.user = middleware.data;
    this.isAuthenticated = true;
    this.MapClaims();
    this.LoginDataBroadCast();
    this.loginFeedData = {
      status: middleware.status,
      message: middleware.message,
      data: undefined,
      token: '',
      role: undefined,
      errors: middleware.errors
    };
    localStorage.setItem("ResponseApi", JSON.stringify(middleware));
    this.loginFeed.next(this.loginFeedData);
    this.chatService.initiateConnectionProcess(this.token, middleware.data.id, middleware.data.userName);
  }

  ResetUserData() {

    this.token = '';
    this.claim = [];
    //this.role = [];
    this.user = {
      id: '',
      email: '',
      gender: '',
      phoneNumber: '',
      userName: '',
      role: '',
      profileImage: '',
      claim: []
    };
    this.ResetClaims();
  }

  ResetClaims() {

    this.hasAccessToCreateRole = false;
    this.hasAccessToAssignRole = false;
    this.hasAccessToRevokeRole = false;
    this.hasAccessToViewRole = false;
    this.hasAccessToDeleteRole = false;
    //hasAccessToViewRoleComponent: boolean = false;
    //user data permissions.
    this.hasAccessToUpdateUserData = false;
    this.hasAccessToViewUserData = false;
    this.hasAccessToViewUserList = false;
    this.hasAccessToViewUserClaimsAndRoles = false;
    //hasAccessToViewUserListComponent: boolean = false;
    //permissions for user.
    this.hasAccessToAddClaimToUser = false;
    this.hasAccessToRemoveClaimFromUser = false;
    this.hasAccessToViewPostList = false;
    this.hasAccessToAddPost = false;

  }

  MapClaims() {

    this.claim.forEach((i) => {
      //console.log(i.type);
      if (i.type == 'hasAccessToViewUserList') {
        this.hasAccessToViewUserList = true;
      }
      if (i.type == 'hasAccessToUpdateUserData') {
        this.hasAccessToUpdateUserData = true;
      }
      if (i.type == 'hasAccessToViewUserData') {
        this.hasAccessToViewUserData = true;
      }
      if (i.type == 'hasAccessToViewUserClaimsAndRoles') {
        this.hasAccessToViewUserClaimsAndRoles = true;
      }
      if (i.type == 'hasAccessToAddClaimToUser') {
        this.hasAccessToAddClaimToUser = true;
      }
      if (i.type == 'hasAccessToRemoveClaimFromUser') {
        this.hasAccessToRemoveClaimFromUser = true;
      }
      if (i.type == 'hasAccessToCreateRole') {
        this.hasAccessToCreateRole = true;
      }
      if (i.type == 'hasAccessToAssignRole') {
        this.hasAccessToAssignRole = true;
      }
      if (i.type == 'hasAccessToViewRole') {
        this.hasAccessToViewRole = true;
      }
      if (i.type == 'hasAccessToRevokeRole') {
        this.hasAccessToRevokeRole = true;
      }
      if (i.type == 'hasAccessToDeleteRole') {
        this.hasAccessToDeleteRole = true;
      }
      if (i.type == 'hasAccessToAddPost') {
        this.hasAccessToAddPost = true;
      }
      if (i.type == 'hasAccessToViewPostList') {
        this.hasAccessToViewPostList = true;
      }
    });
  }
}

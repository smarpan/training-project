import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '../../Component/Auth/login/login.component';
import { UserListComponent } from '../../Component/UserProfile/user-list/user-list.component';
import { AuthGuard } from '../../Utility/auth.guard';
import { RegistrationComponent } from '../../Component/Auth/registration/registration.component';
import { RoleListComponent } from '../../Component/role-list/role-list.component';
import { AddPostComponent } from '../../Component/UserPost/add-post/add-post.component';
import { PostListComponent } from '../../Component/UserPost/post-list/post-list.component';
import { UserProfilePageComponent } from '../../Component/UserProfile/user-profile-page/user-profile-page.component';
import { ChatBoxComponent } from '../../Component/segments/chat-box/chat-box.component';
import { MessengerComponent } from '../../Component/segments/messenger/messenger.component';
import { LoadComponent } from '../../Component/segments/load/load.component';

const Route: Routes = [
  { path: "registration", component: RegistrationComponent },
  { path: "roles", component: RoleListComponent, canActivate: [AuthGuard] },
  { path: "login", component: LoginComponent },
  { path: "addpost", component: AddPostComponent, canActivate: [AuthGuard] },
  { path: "login/:returnUrl", component: LoginComponent },
  { path: "", component: PostListComponent },
  { path: "load", component: LoadComponent },
  { path: "chat", component: MessengerComponent, canActivate: [AuthGuard] },
  { path: "userlist", component: UserListComponent, canActivate: [AuthGuard] },
  { path: "profile/:userId", component: UserProfilePageComponent, canActivate: [AuthGuard] }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forRoot(Route)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }

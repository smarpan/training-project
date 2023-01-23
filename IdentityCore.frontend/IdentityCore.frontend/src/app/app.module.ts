import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { HeaderComponent } from './Component/header/header.component';
import { AppRoutingModule } from './Module/routing/app-routing.module';
import { LoginComponent } from './Component/Auth/login/login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserListComponent } from './Component/UserProfile/user-list/user-list.component';
import { AuthInterceptor } from './Utility/auth.interceptor';
import { RegistrationComponent } from './Component/Auth/registration/registration.component';
import { RoleListComponent } from './Component/role-list/role-list.component';
import { AddClaimToUserComponent } from './Component/UserProfile/add-claim-to-user/add-claim-to-user.component';
import { AssignRoleToUserComponent } from './Component/UserProfile/assign-role-to-user/assign-role-to-user.component';
import { CreateRoleComponent } from './Component/Auth/create-role/create-role.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './Module/material/material.module';
import { UserClaimInfoDialogComponent } from './Component/UserProfile/user-claim-info-dialog/user-claim-info-dialog.component';
import { LoaderComponent } from './Component/segments/loader/loader.component';
import { Ng2SearchPipeModule } from 'ng2-search-filter';
import { FullUserInfoComponent } from './Component/UserProfile/full-user-info/full-user-info.component';
import { AddPostComponent } from './Component/UserPost/add-post/add-post.component';
import { PostListComponent } from './Component/UserPost/post-list/post-list.component';
import { UserProfilePageComponent } from './Component/UserProfile/user-profile-page/user-profile-page.component';
import { PostContainerComponent } from './Component/UserPost/post-container/post-container.component';
import { LoadComponent } from './Component/segments/load/load.component';
import { UserDataComponent } from './Component/UserProfile/user-data/user-data.component';
import { ImageCropperModule } from 'ngx-image-cropper';
import { ImageCropperComponent } from './Component/segments/image-cropper/image-cropper.component';
import { ChatBoxComponent } from './Component/segments/chat-box/chat-box.component';
import { MessengerComponent } from './Component/segments/messenger/messenger.component';
import { ErrorInterceptor } from './Utility/error.interceptor';
import { LoaderInterceptor } from './Service/loader.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    LoginComponent,
    UserListComponent,
    RegistrationComponent,
    RoleListComponent,
    AddClaimToUserComponent,
    AssignRoleToUserComponent,
    CreateRoleComponent,
    UserClaimInfoDialogComponent,
    LoaderComponent,
    FullUserInfoComponent,
    AddPostComponent,
    PostListComponent,
    UserProfilePageComponent,
    PostContainerComponent,
    LoadComponent,
    UserDataComponent,
    ImageCropperComponent,
    ChatBoxComponent,
    MessengerComponent
    
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MaterialModule,
    Ng2SearchPipeModule,
    ImageCropperModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

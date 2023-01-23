import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ClaimUserModel } from '../Model/claimUserModel';
import { MessageStoreModel } from '../Model/MessageStoreModel';
import { RegistrationModel } from '../Model/RegistrationModel';
import { ResponseApi } from '../Model/ResponseApi';
import { UserModel } from '../Model/UserModel';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }
  GetUserList(): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/User/getusers");
  }
  GetUser(email: string): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/User/getuserbyemail/"+email);
  }
  GetUserById(id: string): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/User/getuserbyid/" + id);
  }
  GetMiniUserbyId(id: string): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/User/getuserminibyid/" + id);
  }
  checkAvailableEmail(email: string): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/User/checkavailableemail/"+email);
  }
  UserRegistration(registerModel: RegistrationModel): Observable<ResponseApi> {
    //console.log(registerModel);
    return this.http.post<ResponseApi>("http://localhost:45149/api/Account/register",registerModel);
  }
  AddClaimToUser(model: ClaimUserModel): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/User/addclaimtouser",model);
  }
  RevokeAccessFromUser(model: ClaimUserModel): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/User/removeclaimfromuser", model); 
  }
  UpdateUserInfo(model: UserModel): Observable<ResponseApi> {
    return this.http.put<ResponseApi>("http://localhost:45149/api/User/updateuserinfo",model);
  }
  GetContacts(userId: string): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/User/getcontacts/"+userId);
  }
  DeleteContact(userId: string, contactUserId: string): Observable<ResponseApi> {
    console.log("delete contact called");
    return this.http.delete<ResponseApi>("http://localhost:45149/api/User/deletecontact/"+userId+"/"+contactUserId);
  }
  AddContact(userId: string, contactUserId: string): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/User/addcontact/" + userId + "/" + contactUserId,"hello");
  }
  GetPendingMessages(userId:string): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/User/getpendingmessages/"+userId);
  }
  AddToMessageStore(userId:string, model: MessageStoreModel): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/User/messagestore/" + userId,model);
  }
  GetMessagesFromMessageStore(userId: string): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/User/getmessages/"+userId);
  }
}

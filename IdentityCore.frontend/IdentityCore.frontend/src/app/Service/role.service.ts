import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { AssignRoleModel } from '../Model/AssignRoleModel';
import { CreateRoleModel } from '../Model/CreateRoleModel';
import { ResponseApi } from '../Model/ResponseApi';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  newRoleAdded = new Subject<boolean>;
  RoleAddedNotify() {
    this.newRoleAdded.next(true);
  }
  constructor(private http: HttpClient) { }
  GetRoles(): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/Role/getroles");
  }
  AssignRole(model: AssignRoleModel): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/Role/assignrole",model);
  }
  CreateRole(roleName: CreateRoleModel): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/Role/createrole",roleName);
  }
  RevokeRole(model: AssignRoleModel): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/Role/removerolefromuser",model);
  }
  DeleteRole(model: CreateRoleModel) {
    return this.http.post<ResponseApi>("http://localhost:45149/api/Role/deleterole",model);
  }
}

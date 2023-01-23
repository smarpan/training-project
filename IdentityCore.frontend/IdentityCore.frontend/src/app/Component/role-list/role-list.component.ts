import { Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { RoleModel } from '../../Model/RoleModel';
import { AuthService } from '../../Service/auth.service';
import { RoleService } from '../../Service/role.service';
import { CreateRoleModel } from '../../Model/CreateRoleModel';
import { NotificationService } from '../../Service/notification.service';

@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.css']
})
export class RoleListComponent {
  roleList: RoleModel[] = [];
  roleModel: CreateRoleModel = { roleName: '' };

  role: string[] = [];
  roleNameToAssign: string = '';

  hasAccess: boolean = false;
  hasAccessToViewRole: boolean = false;
  hasAccessToDeleteRole: boolean = false;

  private roleListSub!: Subscription;
  private subscriptionHandler1!: Subscription;
  private subscriptionHandler2!: Subscription;

  constructor(
    private roleService: RoleService,
    private authService: AuthService,
    private notifyService: NotificationService) { }

  ngOnInit() {
    this.subscriptionHandler1 = this.roleService.GetRoles().subscribe({
      next: (result) => {
        this.roleList = result.data;
        this.hasAccessToViewRole = this.authService.hasAccessToViewRoleGetter;
        this.hasAccessToDeleteRole = this.authService.hasAccessToDeleteRoleGetter;

      },
      error: (er) => {

        console.log(er);
      }
    });
    this.roleListSub = this.roleService.newRoleAdded.subscribe({
      next: (nextItem) => {
        this.NewRoleAdded();
      }
    });
  }

  NewRoleAdded() {
    this.ngOnInit();
  }

  DeleteRole(roleName: string) {
    this.roleModel = { roleName: roleName };
    this.subscriptionHandler2 = this.roleService.DeleteRole(this.roleModel).subscribe({
      next: (result) => {
        console.log(result);
        if (result.status) {
          this.NewRoleAdded();
          this.notifyService.Notify(result.message);
        }
      }
    });
  }
  roleNameChange(role: string) {
    this.roleNameToAssign = role;
  }
  ngOnDestroy() {
    this.roleListSub?.unsubscribe();
    this.subscriptionHandler1?.unsubscribe();
    this.subscriptionHandler2?.unsubscribe();
  }
}


import { Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { NotificationService } from './Service/notification.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from './Service/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  notifySubs!: Subscription;

  shownMessage: string = '';
  notificationType: string = 'info';
  constructor(
    private authService: AuthService,
    private notifyService: NotificationService,
    private snackBar: MatSnackBar) {

    this.authService.CheckIfSessionExist();
  }

  ngOnInit() {
    
    this.notifySubs = this.notifyService.notifySuccessSubject.subscribe({
      next: (result) =>{
        this.shownMessage = result;       
        this.notifyInfo();
      },
      error: (er) => {
        console.log(er);
      }
    });
 
  }

  notifyInfo() {
    this.snackBar.open(this.shownMessage,'Ok',{
      duration: 4000,
      verticalPosition: 'top',
      horizontalPosition:'end'
      });

  }

  ngOnDestroy() {
    this.notifySubs?.unsubscribe();
  }
 
}



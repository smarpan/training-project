import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { LoginModel } from '../../../Model/loginModel';
import { AuthService } from '../../../Service/auth.service';
import { NotificationService } from '../../../Service/notification.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email: string = 'Robert@gmail.com';
  password: string = 'Robert@123';
  private returnUrl: string = '';
  loginButtonText: string = 'Login';
  error: string = '';

  login: LoginModel = {
    email: '',
    password: ''
  };
  alreadyLoggedIn: boolean = false;
  submitDisabled: boolean = false;

  private loginSubscription!: Subscription;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private notifyService: NotificationService) {

    if (this.authService.isAuthenticatedGetter) {
        this.alreadyLoggedIn = true;
        this.router.navigate(['']);     
    }
  }
  ngOnInit() {
    // console.log(this.element);
    // console.log("Lets see the returnUrl"+this.route.snapshot.params['returnUrl'])
    if (this.route.snapshot.params['returnUrl']) {
      this.returnUrl = this.route.snapshot.params['returnUrl'];
      console.log(this.returnUrl);
      //console.log("return url" + this.route.snapshot.params['returnUrl']);
    }

  }


  Login() {
    this.submitDisabled = true;
    this.loginButtonText = 'Processing...';
    this.login = {
      email: this.email,
      password: this.password
    };
    this.authService.LoginEntryPoint(this.login);

    this.loginSubscription = this.authService.loginFeed.subscribe({
      next: (result) => {
        this.submitDisabled = false;      
        if (result.status) {                   
          this.notifyService.Notify(result.message);
          this.router.navigate([this.returnUrl]);        
          
        } else {
         
          this.error = result.message;
          console.log(this.error);
          console.log(result);
        }
        this.loginButtonText = 'Login';

      },
      error: (er) => {
        this.submitDisabled = false;
        this.loginButtonText = 'Login';               
        this.notifyService.Notify(er.name);
        console.log(er);
      }
    });
  }

  ngOnDestroy() {
    this.alreadyLoggedIn = false;
    this.loginSubscription?.unsubscribe();
  }

}

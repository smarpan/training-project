import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ConfirmedValidator } from '../../../Model/mustMatch';
import { RegistrationModel } from '../../../Model/RegistrationModel';
import { UserService } from '../../../Service/user.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent {
  errors: string[] = [];
  register: RegistrationModel = {
    email: '',
    password: '',
    confirmPassword: '',
    userName: '',
    gender: 'Male'
  };
  private availablesubs!: Subscription;
  private subscriptionHandler!: Subscription;

  statusChange: boolean = false;
  availableEmail: boolean = false;
  registrationForm!: FormGroup;

  constructor(private fb: FormBuilder, private userService: UserService, private router: Router) { }

  ngOnInit() {
    this.registrationForm = this.fb.group({
      email: [this.register.email,[Validators.required,Validators.email]],
      password: [this.register.password, [Validators.required]],
      confirmPassword: [this.register.confirmPassword, [Validators.required]],
      userName: [this.register.userName, [Validators.required]],
      gender: [this.register.gender, [Validators.required]]
    },
      {
        validator: ConfirmedValidator('password', 'confirmPassword')
      }
    );
  }

  resetEmailChecker() {
    //this.availableEmail = false;
    this.statusChange = true;
  }

  isAvailable(email: string) {
    this.statusChange = false;
    this.availablesubs = this.userService.checkAvailableEmail(email).subscribe({
      next: (result) => {
        this.availableEmail = result.status;
        //console.log(result);
      },
      error: (error) => {
        //console.log(error);
      }
    });
  }

  Registration() {
    //console.log(this.registrationForm);
   this.subscriptionHandler= this.userService.UserRegistration(Object.assign(this.register, this.registrationForm.value)).subscribe({
      next: (result) => {
        if (result.status) {
          //console.log(result);
          this.router.navigate(['']);
        }
        else {
          this.errors = result.errors;
        }
      }
      });
  }

  ngOnDestroy() {
    this.availablesubs?.unsubscribe();
    this.subscriptionHandler?.unsubscribe();
  }

}

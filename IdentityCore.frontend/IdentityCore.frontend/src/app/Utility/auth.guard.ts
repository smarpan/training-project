import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../Service/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private _router: Router) {

  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

     if (this.authService.isAuthenticatedGetter) {
      //this._router.navigate(['state.url']);
      console.log("authenticated done");
      return true;
    }
  
    console.log("not logged in" + state.url);
    this._router.navigate(['/login/', state.url]);
    // you can save redirect url so after authing we can move them back to the page they requested
    return false;
  }
  
}

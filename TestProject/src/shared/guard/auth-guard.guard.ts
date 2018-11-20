import { Injectable } from '@angular/core';
import { CanActivate,  Router } from '@angular/router';
import { UserService } from '../services/user.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardGuard implements CanActivate {
  constructor(private _user: UserService, private _router: Router) { }

  canActivate() {
    if (!this._user.isLoggedIn()) {
      this._router.navigate(['/login']);

      return false;
    }

    return true;
  }
}
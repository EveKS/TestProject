import { Injectable, Inject } from "@angular/core";
import { BaseService } from "./base.service";
import { Observable, BehaviorSubject } from "rxjs";
import { Headers, RequestOptions, Http } from "@angular/http";
import { UserRegistration } from "../models/user.registration.interface";
import { Router } from "@angular/router";

import { map, catchError } from 'rxjs/operators';

@Injectable()
export class UserService extends BaseService {
  private static _authNavStatusSource = new BehaviorSubject<boolean>(false);
  static authNavStatus$: Observable<boolean>;

  private static loggedIn = false;

  constructor(private _http: Http,
    private _router: Router,
    @Inject('BASE_URL') private _baseUrl: string) {
    super();

    UserService.authNavStatus$ = UserService._authNavStatusSource.asObservable();

    UserService.loggedIn = false;
    if (typeof window !== 'undefined') {
      let token = localStorage.getItem('auth_token');

      UserService.loggedIn = !!token;
    }

    UserService._authNavStatusSource.next(UserService.loggedIn);
  }

  register(value: UserRegistration): Observable<boolean> {
    let body = JSON.stringify(value);
    let headers = new Headers({ 'Content-Type': 'application/json' });
    let options = new RequestOptions({ headers: headers });

    return this._http.post(this._baseUrl + "api/accounts/register", body, options)
      .pipe(map(res => {
        let isOk = res.ok === true && res.statusText === 'OK';

        return isOk
      }))
      .pipe(catchError(this.handleError));
  }

  login(email: string, password: string) {
    let headers = new Headers();
    headers.append('Content-Type', 'application/json');

    return this._http
      .post(
        this._baseUrl + 'api/accounts/login',
        JSON.stringify({ email, password }), { headers }
      )
      .pipe(map(res => {
        localStorage.setItem('auth_token', res.json().auth_token);

        UserService.loggedIn = true;
        UserService._authNavStatusSource.next(true);

        return true;
      }))
      .pipe(catchError(this.handleError));
  }

  logout() {
    localStorage.removeItem('auth_token');
    UserService.loggedIn = false;
    UserService._authNavStatusSource.next(false);

    this._router.navigate(['/login']);
  }

  isLoggedIn() {
    return UserService.loggedIn;
  }
}

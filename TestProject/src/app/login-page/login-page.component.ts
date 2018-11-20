import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { UserService } from '../../shared/services/user.service';
import { Credentials } from '../../shared/models/credentials.interface';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent implements OnInit {
  private subscription: Subscription;
  brandNew: boolean;
  errors: string;
  isRequesting: boolean;
  submitted: boolean = false;
  credentials: Credentials = { email: '', password: '' };
  returnUrl: string;

  constructor(private _userService: UserService, private _router: Router,
    private _activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.subscription = this._activatedRoute.queryParams.subscribe(
      (param: any) => {
        this.brandNew = param['brandNew'];
        this.credentials.email = param['email'];
        this.returnUrl = param['returnUrl'];
      });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  login({ value, valid }: { value: Credentials, valid: boolean }) {
    this.submitted = true;
    this.isRequesting = true;
    this.errors = '';
    if (valid) {
      this._userService.login(value.email, value.password)
        .pipe(finalize(() => this.isRequesting = false))
        .subscribe(
          result => {
            if (result) {
              if (this.returnUrl !== undefined || this.returnUrl != null) {
                this._router.navigate([this.returnUrl]);
              } else {
                this._router.navigate(['']);
              }
            }

            this.isRequesting = false;
          },
          error => {
            if (error.code === 500) {
              this.errors = 'Отсутствует интернет соединение';
            } else {
              this.errors = error;
            }

            this.isRequesting = false;
          });
    } else {
      this.submitted = false;
      this.isRequesting = false;
    }
  }
}

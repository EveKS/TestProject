import { Component, OnInit } from '@angular/core';
import { UserService } from '../../shared/services/user.service';
import { Router } from '@angular/router';
import { UserRegistration } from '../../shared/models/user.registration.interface';

import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent implements OnInit {
  errors: string;
  isRequesting: boolean;
  submitted: boolean = false;

  constructor(private _userService: UserService, private _router: Router) {
  }

  ngOnInit() {
  }

  registerUser({ value, valid }: { value: UserRegistration, valid: boolean }) {
    this.submitted = true;
    this.isRequesting = true;
    this.errors = '';
    if (valid) {
      console.log('register');

      this._userService.register(value)
        .pipe(finalize(() => this.isRequesting = false))
        .subscribe(
          result => {
            if (result) {
              this._router.navigate(['/login'], {
                queryParams:
                {
                  brandNew: true, email: value.email
                }
              });
            }

            this.isRequesting = false
          },
          errors => {
            if (errors.code === 500) {
              this.errors = 'Отсутствует интернет соединение';
            } else {
              this.errors = errors;
            }

            this.isRequesting = false
          });
    } else {
      this.submitted = false;
      this.isRequesting = false;
    }
  }
}

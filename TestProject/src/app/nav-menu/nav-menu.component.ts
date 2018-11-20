import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserService } from '../../shared/services/user.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit, OnDestroy {
  subscription: Subscription;
  @Input() status: boolean;

  constructor(private _userService: UserService) {
  }

  signOut() {
    this._userService.logout();
  }

  ngOnInit(): void {
    this.subscription = UserService.authNavStatus$.subscribe(status => {
      this.status = status;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}

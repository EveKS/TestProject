import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { FormsModule } from '@angular/forms';
import { HttpModule, XHRBackend } from '@angular/http';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { UserService } from '../shared/services/user.service';
import { AuthenticateXHRBackend } from './authenticate-xhr.backend';
import { AuthGuard } from '../shared/guard/auth-guard.guard';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpModule
  ],
  providers: [
    UserService,
    AuthGuard,
    { provide: XHRBackend, useClass: AuthenticateXHRBackend },
    { provide: 'BASE_URL', useFactory: getBaseUrl }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

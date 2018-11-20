import { Injectable } from "@angular/core";
import { Request, XHRBackend, BrowserXhr, ResponseOptions, XSRFStrategy, XHRConnection } from "@angular/http";
import { Observable } from "rxjs";

import { catchError } from 'rxjs/operators';

@Injectable()
export class AuthenticateXHRBackend extends XHRBackend {

  constructor(_browserXhr: BrowserXhr, _baseResponseOptions: ResponseOptions, _xsrfStrategy: XSRFStrategy) {
    super(_browserXhr, _baseResponseOptions, _xsrfStrategy);
  }

  createConnection(request: Request): XHRConnection {
    let xhrConnection = super.createConnection(request);

    xhrConnection.response = xhrConnection.response.pipe(catchError((error: Response) => {
      if ((error.status === 401 || error.status === 403) && (window.location.href.match(/\?/g) || []).length < 2) {

        console.log('The authentication session expired or the user is not authorized. Force refresh of the current page.');

        localStorage.removeItem('auth_token');
        window.location.href = window.location.href + '?' + new Date().getMilliseconds();
      }
      return Observable.throw(error);
    }));
    return xhrConnection;
  }
}

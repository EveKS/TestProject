import { Observable } from "rxjs";

export abstract class BaseService {

  constructor() { }

  protected handleError(error: any) {
    let applicationError = error.headers.get('Application-Error');

    if (applicationError) {
      return Observable.throw(applicationError);
    }

    let modelStateErrors: string | null;
    let serverError = error.json();

    if (!serverError.type) {
      for (var key in serverError) {
        if (serverError[key])
          modelStateErrors += serverError[key] + '\n';
      }
    }

    modelStateErrors = modelStateErrors === null ? '' : modelStateErrors;
    return Observable.throw(modelStateErrors || 'Server error');
  }
}

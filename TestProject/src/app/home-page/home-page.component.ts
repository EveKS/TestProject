import { Component, OnInit, Inject } from '@angular/core';
import { HttpHeaders, HttpClient, HttpRequest, HttpEventType, HttpResponse} from '@angular/common/http';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent implements OnInit {
  public progress: number;
  public message: string;

  constructor(private _http: HttpClient,
    @Inject('BASE_URL') private _baseUrl: string) { }


  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files) {
        if (file.type.match(/(.png)|(.jpeg)|(.jpg)|(.gif)$/i)) {
          formData.append('files', file);
        }
    }

    const uploadReq = new HttpRequest('POST', `${this._baseUrl}api/files/files-upload`, formData, {
      reportProgress: true,
      headers: this.getHeaders(),
      responseType: 'json'
    });

    console.log(uploadReq, this.getHeaders());

    this._http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round(100 * event.loaded / event.total);
      }
      else if (event.type === HttpEventType.Response) {
        console.log('event', event, event.body);

        this.message = event.body.toString();
      }
    });
  }

  ngOnInit() {

  }

  private getHeaders(): HttpHeaders {
    let authToken = localStorage.getItem('auth_token');

    return new HttpHeaders({
      "Authorization": `Bearer ${authToken}`,
      // начиная с 5 версии, Content-Type выдает ошибку
      // "Content-Type": "application/json"
    });
  }
}

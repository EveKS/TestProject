import { Component, OnInit, Inject, ViewChild, ElementRef } from '@angular/core';
import { HttpHeaders, HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';
import { FilesModel } from '../../shared/models/files-model.interface';
import { UploadFileResponse } from '../../shared/models/upload-file-response.interface';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent implements OnInit {
  @ViewChild('file') fileInput: ElementRef;

  public progress: number;
  public message: string;
  public filesData: FilesModel[];
  public maxPage: number = 0;

  constructor(private _http: HttpClient,
    private _activateRoute: ActivatedRoute,
    @Inject('BASE_URL') private _baseUrl: string) {
  }

  upload(files) {
    if (files.length === 0) {
      this.fileInput.nativeElement.value = null;

      return;
    }

    let page = this._activateRoute.snapshot.params["page"]

    page = page ? parseInt(page, 10) : 0;

    this.uploadFiles(files, page);
  }

  private uploadFiles(files, page: number) {
    const formData = new FormData();

    for (let file of files) {
      if (file.type.match(/(.png)|(.jpeg)|(.jpg)|(.gif)$/i)) {
        formData.append('files', file);
      }
    }

    if (formData.getAll('files').length === 0) {
      this.fileInput.nativeElement.value = null;

      return;
    }

    formData.append('page', page.toString());

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
        let
          response = event.body as UploadFileResponse;

        if (response) {
          this.message = response.message;

          this.filesData = response.filesData;

          this.maxPage = response.maxPage;

          console.log(this.filesData);
        }

        this.fileInput.nativeElement.value = null;
      }
    });
  }

  ngOnInit() {
    this.initFilesData(0);
  }

  initFilesData(page): void {
    console.log('initFilesData', page);

    const uploadReq = new HttpRequest('GET', `${this._baseUrl}api/files/get-files/?page=${page}`, {
      headers: this.getHeaders()
    });

    this._http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.Response) {
        let
          response = event.body as UploadFileResponse;

        if (response) {
          this.filesData = response.filesData;

          this.maxPage = response.maxPage;

          console.log(this.filesData);
        }
      }
    });
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

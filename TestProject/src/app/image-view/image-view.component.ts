import { Component, OnInit, Input, Inject, Output, EventEmitter } from '@angular/core';
import { FilesModel } from '../../shared/models/files-model.interface';
import { BaseService } from '../../shared/services/base.service';
import { Headers, Http } from '@angular/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-image-view',
  templateUrl: './image-view.component.html',
  styleUrls: ['./image-view.component.css']
})
export class ImageViewComponent extends BaseService implements OnInit {
  @Input() filesModel: FilesModel;

  @Output() initFilesData = new EventEmitter<number>();
  constructor(private _http: Http,
    private _activateRoute: ActivatedRoute,
    @Inject('BASE_URL') private _baseUrl: string) {
    super();
  }

  ngOnInit() {
  }

  delete(id: string) {
    console.log(id);
    this._http.delete(`${this._baseUrl}api/files/${id}`, {
      headers: this.getHeaders()
    })
      .subscribe((ok) => {
        console.log(ok);

        let page = this._activateRoute.snapshot.params["page"]

        page = page ? parseInt(page, 10) : 0;

        this.initFilesData.emit(page);
      });
  }

  private getHeaders(): Headers {
    let authToken = localStorage.getItem('auth_token');

    return new Headers({
      "Authorization": `Bearer ${authToken}`,
      // начиная с 5 версии, Content-Type выдает ошибку
      // "Content-Type": "application/json"
    });
  }
}

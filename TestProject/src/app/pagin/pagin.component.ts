import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { range } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-pagin',
  templateUrl: './pagin.component.html',
  styleUrls: ['./pagin.component.css']
})
export class PaginComponent implements OnInit {
  @Input() maxPages: number;

  @Output() initFilesData = new EventEmitter<number>();
  constructor(private _activateRoute: ActivatedRoute,
    private _router: Router) { }

  pageChange(newPage: number) {
    let page: number = 0;

    this._activateRoute.params.subscribe(params => {
      console.log(params);

      let pg = params['page'];

      page = pg ? parseInt(pg, 10) : 0;

      if (newPage !== page) {
        this.initFilesData.emit(page);
      }
    });
  }

  ngOnInit() {

  }

  getPages(): number[] {
    let pages: number[] = [];

    for (var index = 0; index < this.maxPages; index++) {
      pages.push(index);
    }

    return pages;
  }

  pahing(add: number) {
    let page: number = 0;

    this._activateRoute.params.subscribe(params => {
      let pg = params['page'];

      page = pg ? parseInt(pg, 10) : 0;

      if ((add > 0 && this.maxPages > page + add) || (add < 0 && page > 0)) {
        page += add;
      }

      this._router.navigate(['/home', page]);
    });
  }
}

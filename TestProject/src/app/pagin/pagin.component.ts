import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

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
    let page = this._activateRoute.snapshot.params["page"]

    page = page ? parseInt(page, 10) : 0;

    console.log(page);

    if (newPage !== page) {
      this.initFilesData.emit(page);
    }
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
    let page = this._activateRoute.snapshot.params["page"]

    page = page ? parseInt(page, 10) : 0;

    let oldPage = page;

    if ((add > 0 && this.maxPages > page + add) || (add < 0 && page > 0)) {
      page += add;
    }

    if (oldPage != page) {
      this._router.navigate(['/home', page]);
      this.initFilesData.emit(page);
    }
  }

  ngOnDestroy(): void {
    console.log('ngOnDestroy');

    this.initFilesData.unsubscribe();
  }
}

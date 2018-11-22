import { Component, OnInit, Input } from '@angular/core';
import { FilesModel } from '../../shared/models/files-model.interface';

@Component({
  selector: 'app-image-view',
  templateUrl: './image-view.component.html',
  styleUrls: ['./image-view.component.css']
})
export class ImageViewComponent implements OnInit {
  @Input() filesModel: FilesModel;

  constructor() { }

  ngOnInit() {
  }

  delete(id: string) {
    console.log(id);
  }
}

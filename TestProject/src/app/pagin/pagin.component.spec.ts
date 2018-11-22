import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaginComponent } from './pagin.component';

describe('PaginComponent', () => {
  let component: PaginComponent;
  let fixture: ComponentFixture<PaginComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaginComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LensDetailComponent } from './lens-detail.component';

describe('LensDetailComponent', () => {
  let component: LensDetailComponent;
  let fixture: ComponentFixture<LensDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LensDetailComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LensDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

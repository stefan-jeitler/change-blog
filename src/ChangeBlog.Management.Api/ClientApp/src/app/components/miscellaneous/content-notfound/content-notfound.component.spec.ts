import {ComponentFixture, TestBed} from '@angular/core/testing';

import {ContentNotfoundComponent} from './content-notfound.component';

describe('ContentNotfoundComponent', () => {
  let component: ContentNotfoundComponent;
  let fixture: ComponentFixture<ContentNotfoundComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ContentNotfoundComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ContentNotfoundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

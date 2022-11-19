import {ComponentFixture, TestBed} from '@angular/core/testing';

import {AccountProductsComponent} from './account-products.component';

describe('AccountProductsComponent', () => {
  let component: AccountProductsComponent;
  let fixture: ComponentFixture<AccountProductsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AccountProductsComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AccountProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

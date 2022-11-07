import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountMasterDataComponent } from './account-master-data.component';

describe('AccountMasterDataComponent', () => {
  let component: AccountMasterDataComponent;
  let fixture: ComponentFixture<AccountMasterDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AccountMasterDataComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountMasterDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

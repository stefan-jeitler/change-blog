import { ChangeBlogApi } from '../../../clients/ChangeBlogApiClient';
import { Component, OnInit } from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  private readonly emptyUser: ChangeBlogApi.IUserDto = {
    id: '',
    email: '',
    firstName: '',
    lastName: '',
    timeZone: '',
    culture: ''
  };

  constructor(private changeBlogApiClient: ChangeBlogApi.Client) {
    this._currentUser = this.emptyUser;
    this.titleTranslationKey = TranslationKey.userProfile;
  }

  private _currentUser: ChangeBlogApi.IUserDto;
  titleTranslationKey: string;

  get currentUser(): ChangeBlogApi.IUserDto {
    return this._currentUser ?? this.emptyUser;
  }

  get firstAndLastName(): string {
    return `${this.currentUser.firstName} ${this.currentUser.lastName}`;
  }
  set firstAndLastName(value) {}

  ngOnInit(): void {
    this.changeBlogApiClient
      .getUserInfo()
      .subscribe((u) => (this._currentUser = u));
  }
}

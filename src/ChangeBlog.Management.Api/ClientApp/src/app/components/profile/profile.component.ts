import {ChangeBlogApi} from '../../../clients/ChangeBlogApiClient';
import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {tap} from "rxjs/operators";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  translationKey: TranslationKey = TranslationKey;

  titleTranslationKey: string;
  nameTranslationKey: string;
  emailTranslationKey: string;
  timezoneTranslationKey: string;
  cultureTranslationKey: string;
  saveTranslationKey: string;
  usernameTooltipTranslationKey: string;
  emailTooltipTranslationKey: string;
  availableTimezones: (string | undefined)[];
  availableCultures: (string | undefined)[];
  isLoadingFinished: boolean;
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
    this.nameTranslationKey = TranslationKey.$name
    this.emailTranslationKey = TranslationKey.email;
    this.timezoneTranslationKey = TranslationKey.timezone;
    this.cultureTranslationKey = TranslationKey.culture;
    this.saveTranslationKey = TranslationKey.save;
    this.usernameTooltipTranslationKey = TranslationKey.userProfileUsernameInfo;
    this.emailTooltipTranslationKey = TranslationKey.userProfileEmailInfo;

    this.translationKey = TranslationKey;

    this.availableTimezones = [
      'Europe/Berlin',
      'Europe/Vienna',
      'Europe/London',
      'America/New_York'
    ];

    this.availableCultures = [
      'de-AT',
      'de-DE',
      'de-CH',
      'en-US',
      'en-GB'
    ];

    this.isLoadingFinished = false;
  }

  private _currentUser: ChangeBlogApi.IUserDto;

  get currentUser(): ChangeBlogApi.IUserDto {
    return this._currentUser ?? this.emptyUser;
  }

  get firstAndLastName(): string {
    return `${this.currentUser.firstName} ${this.currentUser.lastName}`;
  }

  set firstAndLastName(value) {
  }

  ngOnInit(): void {
    this.changeBlogApiClient
      .getUserInfo()
      .pipe(tap(x => this.isLoadingFinished = true))
      .subscribe((u) => {
          this._currentUser = u;
        },
        error => console.error(error));
  }
}

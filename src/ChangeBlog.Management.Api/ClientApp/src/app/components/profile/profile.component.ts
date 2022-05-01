import {ChangeBlogApi} from '../../../clients/ChangeBlogApiClient';
import {Component, OnInit} from '@angular/core';
import {TranslationKey} from "../../generated/TranslationKey";
import {tap} from "rxjs/operators";
import {ChangeBlogManagementApi as MngmtApiClient} from "../../../clients/ChangeBlogManagementApiClient";
import {zip} from "rxjs";
import ITimezoneDto = MngmtApiClient.ITimezoneDto;


@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  availableTimezones: ITimezoneDto[];
  availableCultures: string[];
  isLoadingFinished: boolean;
  selectedTimeZone: ITimezoneDto;
  private readonly emptyUser: ChangeBlogApi.IUserDto = {
    id: '',
    email: '',
    firstName: '',
    lastName: '',
    timeZone: '',
    culture: 'en-US'
  };
  private readonly utc: ITimezoneDto = {
    windowsId: 'UTC',
    olsonId: 'Etc/UTC',
    offset: '+00:00'
  };

  constructor(public translationKey: TranslationKey, private mngmtApiClient: MngmtApiClient.Client) {
    this.currentUser = this.emptyUser;
    this.selectedTimeZone = this.utc;

    this.availableTimezones = [];
    this.availableCultures = [];

    this.isLoadingFinished = false;
  }

  currentUser: ChangeBlogApi.IUserDto;

  get firstAndLastName(): string {
    return `${this.currentUser.firstName} ${this.currentUser.lastName}`;
  }

  set firstAndLastName(value) {
  }

  ngOnInit(): void {

    const loadTimezones = this.mngmtApiClient
      .getSupportedTimezones();
    loadTimezones.subscribe(t => this.availableTimezones = t);

    const loadCulture = this.mngmtApiClient
      .getSupportedCultures()
    loadCulture.subscribe(x => this.availableCultures = x);

    zip(loadTimezones, loadCulture).subscribe(() => {
      this.mngmtApiClient
        .getUserProfile()
        .pipe(tap(x => this.isLoadingFinished = true))
        .subscribe((u) => {
            this.currentUser = u;
            this.selectedTimeZone = this.availableTimezones.find(x => x.olsonId === u.timeZone) ?? this.utc;
          },
          error => console.error(error));
    });
  }
}

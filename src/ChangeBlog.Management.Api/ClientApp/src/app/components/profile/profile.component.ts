import {ChangeBlogApi} from '../../../clients/ChangeBlogApiClient'
import {Component, OnInit} from '@angular/core';
import {OAuthService} from "angular-oauth2-oidc";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  private readonly emptyUser: ChangeBlogApi.IUserDto = {
    id: "",
    email: "",
    firstName: "",
    lastName: "",
    createdAt: undefined,
    timeZone: ""
  };

  constructor(private changeBlogApiClient: ChangeBlogApi.Client, private authService: OAuthService) {
    this._currentUser = this.emptyUser;
  }

  private _currentUser: ChangeBlogApi.IUserDto;

  get currentUser(): ChangeBlogApi.IUserDto {
    return this._currentUser ?? this.emptyUser;
  }

  ngOnInit(): void {
    this.changeBlogApiClient.getUserInfo()
      .subscribe(u => this._currentUser = u);
  }

}

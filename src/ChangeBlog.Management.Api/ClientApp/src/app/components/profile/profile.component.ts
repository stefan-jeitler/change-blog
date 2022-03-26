import {HttpClient} from '@angular/common/http';
import {Component, OnInit} from '@angular/core';
import {OAuthService} from "angular-oauth2-oidc";

interface User {
  email: string;
  firstName: string;
  lastName: string;
  timeZone: string;
}

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  private readonly emptyUser: User = {email: "", firstName: "", lastName: "", timeZone: ""};

  constructor(private http: HttpClient, private authService: OAuthService) {
    this._currentUser = this.emptyUser;
  }

  private _currentUser: User;

  get currentUser(): User {
    return this._currentUser ?? this.emptyUser;
  }

  ngOnInit(): void {
    const accessToken = this.authService.getAccessToken();
    this.http.get<User>("https://app-change-blog-staging.azurewebsites.net/api/v1/user/info", {
      headers: {'Authorization': `Bearer ${accessToken}`}
    })
      .subscribe(u => this._currentUser = u);
  }

}

import {Component, OnInit} from '@angular/core';
import {OAuthService} from "angular-oauth2-oidc";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  showLoginDialog: boolean;


  constructor(private authService: OAuthService) {
    this.showLoginDialog = true;
  }

  get isLoggedIn(): boolean {
    return this.authService.hasValidIdToken();
  }

  ngOnInit(): void {
  }

  login() {
    this.authService.initCodeFlow();
  }
}

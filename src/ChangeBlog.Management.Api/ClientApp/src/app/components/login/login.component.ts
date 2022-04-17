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

  ngOnInit(): void {
  }

  login() {
    this.authService.initLoginFlow('/app/home');
  }
}

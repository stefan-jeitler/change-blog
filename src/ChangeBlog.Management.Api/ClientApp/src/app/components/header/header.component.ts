import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  constructor(
    private authService: OAuthService
  ) {}

  get isLoggedIn(): boolean {
    return this.authService.hasValidIdToken();
  }

  get userName(): string {
    const claims = this.authService.getIdentityClaims() as any;

    return claims?.preferred_username ?? '';
  }

  ngOnInit() {
    this.authService.loadDiscoveryDocumentAndTryLogin();
  }

  login() {
    this.authService.initLoginFlow();
  }

  logout() {
    this.authService.logOut();
  }
}

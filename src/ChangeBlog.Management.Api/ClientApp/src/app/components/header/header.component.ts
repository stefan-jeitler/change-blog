import {Component, EventEmitter, OnInit, Output} from '@angular/core';
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

  @Output("triggerMobileSideNav") triggerMobileSideNav: EventEmitter<any> = new EventEmitter();

  get isLoggedIn(): boolean {
    return this.authService.hasValidIdToken();
  }

  get userName(): string {
    const claims = this.authService.getIdentityClaims() as any;

    return claims?.preferred_username ?? '';
  }

  ngOnInit() {
  }

  login() {
    this.authService.initCodeFlow();
  }

  logout() {
    this.authService.logOut();
  }

  showMobileSideNav() {
    this.triggerMobileSideNav.emit();
  }
}

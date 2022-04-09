import { Component, OnInit } from '@angular/core';
import { AuthConfig, OAuthService } from 'angular-oauth2-oidc';
import { filter, map, mergeMap } from 'rxjs/operators';
import { ChangeBlogManagementApi } from 'src/clients/ChangeBlogManagementApiClient';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  constructor(
    private authService: OAuthService,
    private authConfig: AuthConfig,
    private changeBlogManagementApiClient: ChangeBlogManagementApi.Client
  ) {}

  get isLoggedIn(): boolean {
    return this.authService.hasValidIdToken();
  }

  get userName(): string {
    const claims = this.authService.getIdentityClaims() as any;

    return claims?.preferred_username ?? '';
  }

  ngOnInit() {
    this.authService.events
      .pipe(
        filter(
          (e) =>
            e.type === 'token_received' &&
            this.authService.hasValidAccessToken()
        ),
        mergeMap((x) =>
          this.changeBlogManagementApiClient.ensureUserIsImported()
        )
      )
      .subscribe(
        (x) => console.debug(x),
        (e) => console.error(e)
      );

    this.authService.configure(this.authConfig);
    this.authService.loadDiscoveryDocumentAndTryLogin();
  }

  login() {
    this.authService.initLoginFlow();
  }

  logout() {
    this.authService.logOut();
  }
}

import { Component, OnInit } from '@angular/core';
import {OAuthService} from "angular-oauth2-oidc";
import {Router} from "@angular/router";

@Component({
  templateUrl: './redirect.component.html'
})
export class RedirectComponent implements OnInit {

  constructor(private authClient: OAuthService,
              private router: Router) {
  }

  isLoggedIn() {
    return this.authClient.hasValidIdToken() && this.authClient.hasValidAccessToken();
  }

  ngOnInit(): void {

    this.authClient.tryLogin()
      .then(x => {
        if(this.isLoggedIn())
          this.router.navigateByUrl('/app/home');
        else
          this.router.navigateByUrl('/home');
      });
  }

}

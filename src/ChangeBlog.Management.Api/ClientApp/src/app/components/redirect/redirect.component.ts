import { Component, OnInit } from '@angular/core';
import {OAuthService} from "angular-oauth2-oidc";
import {Router} from "@angular/router";

@Component({
  selector: 'app-redirect',
  templateUrl: './redirect.component.html',
  styleUrls: ['./redirect.component.scss']
})
export class RedirectComponent implements OnInit {

  constructor(private authClient: OAuthService,
              private router: Router) {
  }

  isLoggedIn() {
    return this.authClient.hasValidIdToken() && this.authClient.hasValidAccessToken();
  }

  ngOnInit(): void {

    if(this.isLoggedIn())
      this.router.navigateByUrl('/app/home');
    else
      this.router.navigateByUrl('/home');
  }

}

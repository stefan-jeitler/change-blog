import { Component, OnInit } from '@angular/core';
import {ActivationEnd, Router} from "@angular/router";
import {OAuthService} from "angular-oauth2-oidc";
import {filter} from "rxjs/operators";

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {
  showMobileSideNav: boolean;
  title = 'change-blog';

  constructor(private router: Router, private authService: OAuthService) {
    this.showMobileSideNav = false;

    this.router.events
      .pipe(filter(event => event instanceof ActivationEnd))
      .subscribe((event) => {
        this.showMobileSideNav = false;
      });
  }

  triggerMobileSideNav() {
    this.showMobileSideNav = true;
  }

  get isLoggedIn(): boolean {
    return this.authService.hasValidIdToken();
  }

  ngOnInit(): void {
  }

}

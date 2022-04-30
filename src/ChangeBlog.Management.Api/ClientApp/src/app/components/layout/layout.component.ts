import { Component, OnInit } from '@angular/core';
import {ActivationEnd, Router} from "@angular/router";
import {OAuthService} from "angular-oauth2-oidc";
import {filter} from "rxjs/operators";
import {Constants} from "../../../constants";

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit {
  showMobileSideNav: boolean;

  constructor(private router: Router) {
    this.showMobileSideNav = false;

    this.router.events
      .pipe(filter(event => event instanceof ActivationEnd))
      .subscribe((_) => {
        this.showMobileSideNav = false;
      });
  }

  triggerMobileSideNav() {
    this.showMobileSideNav = true;
  }


  ngOnInit(): void {
  }

  onSwipeRight($event: any) {
    if(window.innerWidth < Constants.MobileBreakpoint.value)
      this.triggerMobileSideNav();
  }
}

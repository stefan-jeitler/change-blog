import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {OAuthService} from 'angular-oauth2-oidc';
import {MenuItem} from "primeng/api";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  @Output("triggerMobileSideNav") triggerMobileSideNav: EventEmitter<any> = new EventEmitter();
  menuItems: MenuItem[];

  constructor(
    private authService: OAuthService
  ) {
    this.menuItems = [];
  }

  get userName(): string {
    const claims = this.authService.getIdentityClaims() as any;

    return claims?.preferred_username ?? '';
  }

  ngOnInit() {
    this.menuItems = [
      {
        label: this.userName,
        style: {
          'pointer-events': 'none'
        },
      },
      {
        separator: true
      },
      {
        label: 'Profile',
        routerLink: '/profile',
        icon: 'pi pi-fw pi-user'
      },
      {
        label: 'Logout',
        icon: 'pi pi-fw pi-sign-out',
        command: () => {
          this.logout();
        }
      }
    ];
  }


  logout() {
    this.authService.logOut();
  }

  showMobileSideNav() {
    this.triggerMobileSideNav.emit();
  }
}

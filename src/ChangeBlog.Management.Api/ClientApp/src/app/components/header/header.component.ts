import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {MenuItem, MessageService, Message} from "primeng/api";
import {OAuthService} from "angular-oauth2-oidc";
import {translate, TranslocoService} from "@ngneat/transloco";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  @Output("triggerMobileSideNav") triggerMobileSideNav: EventEmitter<any> = new EventEmitter();
  menuItems: MenuItem[];
  langItems: MenuItem[];

  constructor(
    private authService: OAuthService,
    private translationService: TranslocoService,
    private messageService: MessageService
  ) {
    this.menuItems = [];
    this.langItems = [];
  }

  get userName(): string {
    const claims = this.authService.getIdentityClaims() as any;

    return claims?.preferred_username ?? '';
  }

  get currentLang() {
    return this.translationService.getActiveLang().toUpperCase();
  }

  ngOnInit() {
    this.populateMenuItems();
    this.populateLangItems();
  }


  private populateMenuItems() {
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
        label: translate('profile'),
        routerLink: '/app/profile',
        icon: 'pi pi-fw pi-user'
      },
      {
        label: translate('logout'),
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

  private populateLangItems() {
    const createLangItem: (x:string) => MenuItem = x => {
      return {
        label: x.toUpperCase(),
        command: () => this.changeLanguage(x)
      };
    }

    this.langItems = (<string[]>this.translationService.getAvailableLangs())
      .map(createLangItem);
  }

  private changeLanguage(targetLang: string) {
    if(targetLang === this.currentLang)
      return;

    this.translationService.setActiveLang(targetLang);
    this.translationService
      .load(targetLang)
      .subscribe(x => {
        this.populateMenuItems();
        localStorage.setItem('language', targetLang);
      });

  }
}

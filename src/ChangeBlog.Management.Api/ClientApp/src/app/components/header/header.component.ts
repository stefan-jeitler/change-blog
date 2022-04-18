import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {MenuItem, MessageService} from "primeng/api";
import {OAuthService} from "angular-oauth2-oidc";
import {translate, TranslocoService} from "@ngneat/transloco";
import {filter} from "rxjs/operators";

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

    this.translationService.events$
      .pipe(filter(e => e.type === 'translationLoadSuccess' && e.wasFailure))
      .subscribe((x) => {
          this.messageService.add({
            severity: 'error',
            summary: translate('genericErrorMessageShort'),
            detail: translate('genericErrorMessage')
          });
        }
      );
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

  logout() {
    this.authService.logOut();
  }

  showMobileSideNav() {
    this.triggerMobileSideNav.emit();
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

  private populateLangItems() {
    const createLangItem: (x: string) => MenuItem = x => {
      return {
        label: x.toUpperCase(),
        command: () => this.changeLanguage(x)
      };
    }

    this.langItems = (<string[]>this.translationService.getAvailableLangs())
      .map(createLangItem);
  }

  private changeLanguage(targetLang: string) {
    if (targetLang.toUpperCase() === this.currentLang.toUpperCase())
      return;

    this.translationService.setActiveLang(targetLang);

    this.translationService
      .load(targetLang)
      .subscribe(
        x => {
          this.populateMenuItems();
          localStorage.setItem('language', targetLang);
          this.messageService.add({
            severity: 'success',
            summary: translate('languageChangedShort'),
            detail: translate('languageChanged', {langCode: targetLang.toUpperCase()})
          });
        });

  }
}

import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {MenuItem, MessageService} from "primeng/api";
import {OAuthService} from "angular-oauth2-oidc";
import {translate, TranslocoService} from "@ngneat/transloco";
import {filter} from "rxjs/operators";
import {LanguageInfo} from "../../transloco-root.module";

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

    return claims.emails.length > 0
      ? claims.emails[0]
      : `${claims.given_name} ${claims.family_name}`;
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
    const createLangItem: (x: LanguageInfo) => MenuItem = x => {
      return {
        label: x.id.toUpperCase(),
        command: () => this.changeLanguage(x)
      };
    }

    this.langItems = (<LanguageInfo[]>this.translationService.getAvailableLangs())
      .map(createLangItem);
  }

  private changeLanguage(targetLang: LanguageInfo) {
    if (targetLang.id.toUpperCase() === this.currentLang.toUpperCase())
      return;

    this.translationService.setActiveLang(targetLang.id);

    this.translationService
      .load(targetLang.id)
      .subscribe(
        x => {
          this.populateMenuItems();
          localStorage.setItem('language', targetLang.id);
          this.messageService.add({
            severity: 'success',
            summary: translate('languageChangedShort'),
            detail: translate('languageChanged', {lang: targetLang.label})
          });
        });

  }
}

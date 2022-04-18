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
    const createLangItem: (x: {id: string, label: string}) => MenuItem = x => {
      return {
        label: x.id.toUpperCase(),
        command: () => this.changeLanguage(x.id, x.label)
      };
    }

    this.langItems = (<{id: string, label: string}[]>this.translationService.getAvailableLangs())
      .map(createLangItem);
  }

  private changeLanguage(targetLangId: string, targetLang: string) {
    if (targetLangId.toUpperCase() === this.currentLang.toUpperCase())
      return;

    this.translationService.setActiveLang(targetLangId);

    this.translationService
      .load(targetLangId)
      .subscribe(
        x => {
          this.populateMenuItems();
          localStorage.setItem('language', targetLangId);
          this.messageService.add({
            severity: 'success',
            summary: translate('languageChangedShort'),
            detail: translate('languageChanged', {langCode: targetLang})
          });
        });

  }
}

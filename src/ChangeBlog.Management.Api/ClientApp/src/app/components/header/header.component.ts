import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {MenuItem, MessageService} from "primeng/api";
import {OAuthService} from "angular-oauth2-oidc";
import {translate, TranslocoService} from "@ngneat/transloco";
import {filter} from "rxjs/operators";
import {LanguageInfo} from "../../transloco-root.module";
import {IdentityUser} from "../../models/identityUser.interface";
import {TranslationKey} from "../../generated/TranslationKey";

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
    public translationKey: TranslationKey,
    private translationService: TranslocoService,
    private authService: OAuthService,
    private messageService: MessageService
  ) {
    this.menuItems = [];
    this.langItems = [];

    this.translationService.events$
      .pipe(filter(e => e.type === 'translationLoadSuccess' && e.wasFailure))
      .subscribe((x) => {
          const genericerrorMessage = translate(this.translationKey.genericErrorMessageShort);
          const isTranslationAvailableAtAll = !!genericerrorMessage && genericerrorMessage !== this.translationKey.genericErrorMessageShort;

          // show error message only if at least one translation is available
          if (isTranslationAvailableAtAll) {
            this.messageService.add({
              severity: 'error',
              summary: genericerrorMessage,
              detail: translate(this.translationKey.genericErrorMessage)
            });
          }
        }
      );
  }

  get userName(): string {
    const claims = this.authService.getIdentityClaims() as IdentityUser;

    return claims.emails?.length ?? 0 > 0
      ? claims.emails[0]
      : `${claims.given_name} ${claims.family_name}`;
  }

  ngOnInit() {
    this.populateMenuItems();
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
        }
      },
      {
        separator: true
      },
      {
        label: translate(this.translationKey.userProfile),
        routerLink: '/app/profile',
        icon: 'pi pi-fw pi-user'
      },
      {
        label: translate(this.translationKey.logout),
        icon: 'pi pi-fw pi-sign-out',
        command: () => {
          this.logout();
        }
      }
    ];
  }
}

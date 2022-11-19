import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {MenuItem, MessageService} from "primeng/api";
import {OAuthService} from "angular-oauth2-oidc";
import {translate, TranslocoService} from "@ngneat/transloco";
import {filter} from "rxjs/operators";
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

  constructor(
    public translationKey: TranslationKey,
    private translationService: TranslocoService,
    private authService: OAuthService,
    private messageService: MessageService
  ) {
    this.menuItems = [];

    this.translationService.events$
      .pipe(filter(e => e.type === 'translationLoadSuccess' && e.wasFailure))
      .subscribe((x) => {
          const genericErrorMessage = translate(this.translationKey.genericErrorMessageShort);
          const isTranslationAvailableAtAll = !!genericErrorMessage && genericErrorMessage !== this.translationKey.genericErrorMessageShort;

          // show error message only if at least one translation is available
          if (isTranslationAvailableAtAll) {
            this.messageService.add({
              severity: 'error',
              summary: genericErrorMessage,
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

    this.translationService.selectTranslate(this.translationKey.userProfile).subscribe({
      next: v => {
        const userProfileMenuItem = this.menuItems.find(x => x.id == this.translationKey.userProfile);

        if (userProfileMenuItem)
          userProfileMenuItem.label = v;
      }
    });

    this.translationService.selectTranslate(this.translationKey.logout).subscribe({
      next: v => {
        const userProfileMenuItem = this.menuItems.find(x => x.id == this.translationKey.logout);

        if (userProfileMenuItem)
          userProfileMenuItem.label = v;
      }
    });
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
        id: this.translationKey.userProfile,
        label: translate(this.translationKey.userProfile),
        routerLink: '/app/profile',
        icon: 'pi pi-fw pi-user'
      },
      {
        id: this.translationKey.logout,
        label: translate(this.translationKey.logout),
        icon: 'pi pi-fw pi-sign-out',
        command: () => {
          this.logout();
        }
      }
    ];
  }
}

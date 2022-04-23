import {APP_INITIALIZER, NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {HttpClientModule} from '@angular/common/http';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HeaderComponent} from './components/header/header.component';

import {OAuthModule, OAuthService, OAuthStorage} from 'angular-oauth2-oidc';
import {HomeComponent} from './components/home/home.component';
import {ProfileComponent} from './components/profile/profile.component';
import {ApikeyComponent} from './components/apikey/apikey.component';

import {ChangeBlogApi} from 'src/clients/ChangeBlogApiClient';
import {ChangeBlogManagementApi} from 'src/clients/ChangeBlogManagementApiClient';
import {filter, mergeMap} from 'rxjs/operators';
import {APP_CONFIG, AppConfig} from 'app.config';
import {SidebarModule} from "primeng/sidebar";
import {SideNavigationComponent} from './components/side-navigation/side-navigation.component';
import {ButtonModule} from "primeng/button";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {PanelMenuModule} from "primeng/panelmenu";
import {MenuModule} from "primeng/menu";
import {MenubarModule} from "primeng/menubar";
import {OverlayPanelModule} from "primeng/overlaypanel";
import {Router} from "@angular/router";
import {LoginComponent} from './components/login/login.component';
import {LandingComponent} from './components/landing/landing.component';
import {DialogModule} from "primeng/dialog";
import {FormsModule} from "@angular/forms";
import {TooltipModule} from "primeng/tooltip";
import {InputTextModule} from "primeng/inputtext";
import {RippleModule} from "primeng/ripple";
import {LayoutComponent} from './components/layout/layout.component';
import {RedirectComponent} from './components/redirect/redirect.component';
import {CheckboxModule} from "primeng/checkbox";
import {RadioButtonModule} from "primeng/radiobutton";
import {LanguageInfo, TranslocoRootModule} from './transloco-root.module';
import {getBrowserLang, TranslocoService} from "@ngneat/transloco";
import {MessageModule} from "primeng/message";
import {MessagesModule} from "primeng/messages";
import {MessageService} from "primeng/api";
import {ToastModule} from "primeng/toast";
import {DEBUG} from "@angular/compiler-cli/src/ngtsc/logging/src/console_logger";
import {TranslocoLocaleModule} from "@ngneat/transloco-locale";
import { ContentHeaderComponent } from './components/content-header/content-header.component';

export function initializeApp(
  router: Router,
  oAuthService: OAuthService,
  appConfig: AppConfig,
  apiClient: ChangeBlogManagementApi.Client,
  translationService: TranslocoService
): () => Promise<void> {
  return () => {
    // TODO: get out of here
    return new Promise<void>((resolve, reject) => {
      // Router
      router.onSameUrlNavigation = 'reload';

      // Auth
      oAuthService.configure(appConfig.authConfig);
      oAuthService.setupAutomaticSilentRefresh();

      oAuthService.events
        .pipe(
          filter(
            (e) => e.type === 'token_received' && oAuthService.hasValidAccessToken()
          ),
          mergeMap((x) => apiClient.ensureUserIsImported())
        )
        .subscribe(
          (x) => console.debug(x),
          (e) => console.error(e)
        );

      const authSetup = oAuthService.loadDiscoveryDocument(appConfig.discoveryDocument);

      // i18n
      const getValueOrDefault = (v: string | undefined | null) => v ? v : undefined;

      const storedLang = getValueOrDefault(localStorage.getItem('language'));
      const browserLang = getValueOrDefault(getBrowserLang());
      const defaultLang = getValueOrDefault(translationService.getDefaultLang());

      const chosenLang = storedLang ?? browserLang ?? defaultLang ?? 'en-US';
      const finalLang = (<LanguageInfo[]>translationService.getAvailableLangs()).find(x => x.id === chosenLang)?.id ?? defaultLang;

      translationService.setActiveLang(finalLang!);
      const i18nSetup = translationService.load(finalLang!).toPromise();

      // wait till setup is finished
      Promise.all([authSetup, i18nSetup])
        .then(() => resolve());
    });

  };
}

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    HomeComponent,
    ProfileComponent,
    ApikeyComponent,
    SideNavigationComponent,
    LoginComponent,
    LandingComponent,
    LayoutComponent,
    RedirectComponent,
    ContentHeaderComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    OAuthModule.forRoot({
      resourceServer: {
        sendAccessToken: true,
        // TODO: get urls from app config
        allowedUrls: [
          'https://app-change-blog-staging.azurewebsites.net',
          'https://app-change-blog.azurewebsites.net',
          'http://localhost:6430',
          '/',
        ],
      },
    }),
    TranslocoRootModule,
    FormsModule,
    InputTextModule,
    RippleModule,
    TooltipModule,
    SidebarModule,
    ButtonModule,
    PanelMenuModule,
    MenuModule,
    MenubarModule,
    OverlayPanelModule,
    DialogModule,
    CheckboxModule,
    RadioButtonModule,
    MessageModule,
    MessagesModule,
    ToastModule,
    TranslocoLocaleModule
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [Router, OAuthService, APP_CONFIG, ChangeBlogManagementApi.Client, TranslocoService],
      multi: true,
    },
    {
      provide: ChangeBlogApi.API_BASE_URL,
      useFactory: (appConfig: AppConfig) => appConfig.changeBlogApiBaseUrl,
      deps: [APP_CONFIG],
      multi: true
    },
    { provide: OAuthStorage, useValue: localStorage },
    ChangeBlogApi.Client,
    ChangeBlogManagementApi.Client,
    MessageService
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
}

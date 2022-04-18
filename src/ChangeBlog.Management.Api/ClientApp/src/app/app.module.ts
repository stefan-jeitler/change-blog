import {APP_INITIALIZER, NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {HttpClientModule} from '@angular/common/http';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HeaderComponent} from './components/header/header.component';

import {OAuthModule, OAuthService} from 'angular-oauth2-oidc';
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
import { RedirectComponent } from './components/redirect/redirect.component';
import {CheckboxModule} from "primeng/checkbox";
import {RadioButtonModule} from "primeng/radiobutton";

export function initializeApp(
  router: Router,
  oAuthService: OAuthService,
  appConfig: AppConfig,
  apiClient: ChangeBlogManagementApi.Client
): () => Promise<void> {
  return () => {
    return new Promise((resolve, reject) => {
      // Router
      router.onSameUrlNavigation = 'reload';

      // Auth
      oAuthService.configure(appConfig.authConfig!);
      oAuthService.setupAutomaticSilentRefresh();

      oAuthService.events
        .pipe(
          filter(
            (e) => e.type === 'token_received' && oAuthService.hasValidIdToken()
          ),
          mergeMap((x) => apiClient.ensureUserIsImported())
        )
        .subscribe(
          (x) => console.debug(x),
          (e) => console.error(e)
        );

      oAuthService.loadDiscoveryDocument()
        .then(x => resolve());
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
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    OAuthModule.forRoot({
      resourceServer: {
        sendAccessToken: true,
        allowedUrls: [
          'https://app-change-blog-staging.azurewebsites.net',
          'http://localhost:6430',
          '/',
        ],
      },
    }),
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
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [Router, OAuthService, APP_CONFIG, ChangeBlogManagementApi.Client],
      multi: true,
    },
    {
      provide: ChangeBlogApi.API_BASE_URL,
      useFactory: (appConfig: AppConfig) => appConfig.changeBlogApiBaseUrl,
      deps: [APP_CONFIG],
      multi: true
    },
    ChangeBlogApi.Client,
    ChangeBlogManagementApi.Client,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
}

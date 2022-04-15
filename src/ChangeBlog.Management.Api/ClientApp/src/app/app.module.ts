import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';

import { OAuthModule, OAuthService } from 'angular-oauth2-oidc';
import { HomeComponent } from './components/home/home.component';
import { ProfileComponent } from './components/profile/profile.component';
import { ApikeyComponent } from './components/apikey/apikey.component';

import { ChangeBlogApi } from 'src/clients/ChangeBlogApiClient';
import { ChangeBlogManagementApi } from 'src/clients/ChangeBlogManagementApiClient';
import { filter, mergeMap } from 'rxjs/operators';
import { AppConfig, APP_CONFIG } from 'app.config';
import {SidebarModule} from "primeng/sidebar";
import { SideNavigationComponent } from './components/side-navigation/side-navigation.component';
import {ButtonModule} from "primeng/button";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {PanelMenuModule} from "primeng/panelmenu";
import {MenuModule} from "primeng/menu";
import {MenubarModule} from "primeng/menubar";
import {OverlayPanelModule} from "primeng/overlaypanel";

export function initializeAuthentication(
  authService: OAuthService,
  appConfig: AppConfig,
  apiClient: ChangeBlogManagementApi.Client
): () => Promise<void> {
  return () => {
    return new Promise((resolve, reject) => {
      authService.configure(appConfig.authConfig!);

      authService.events
        .pipe(
          filter(
            (e) => e.type === 'token_received' && authService.hasValidIdToken()
          ),
          mergeMap((x) => apiClient.ensureUserIsImported())
        )
        .subscribe(
          (x) => console.debug(x),
          (e) => console.error(e)
        );

      authService.loadDiscoveryDocumentAndTryLogin();

      resolve();
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
    SidebarModule,
    ButtonModule,
    PanelMenuModule,
    MenuModule,
    MenubarModule,
    OverlayPanelModule,
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initializeAuthentication,
      deps: [OAuthService, APP_CONFIG, ChangeBlogManagementApi.Client],
      multi: true,
    },
    {
      provide: ChangeBlogApi.API_BASE_URL,
      useFactory: (appConfig: AppConfig) => appConfig.changeBlogApiBaseUrl,
      deps: [APP_CONFIG],
    },
    ChangeBlogApi.Client,
    ChangeBlogManagementApi.Client,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}

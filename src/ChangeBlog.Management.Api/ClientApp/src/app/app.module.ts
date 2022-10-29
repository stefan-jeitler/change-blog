import {APP_INITIALIZER, NgModule} from '@angular/core';
import {BrowserModule, HAMMER_GESTURE_CONFIG, HammerModule} from '@angular/platform-browser';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HeaderComponent} from './components/header/header.component';

import {OAuthModule, OAuthService, OAuthStorage} from 'angular-oauth2-oidc';
import {HomeComponent} from './components/home/home.component';
import {ProfileComponent} from './components/profile/profile.component';
import {ApikeyComponent} from './components/apikey/apikey.component';

import {ChangeBlogApi} from 'src/clients/ChangeBlogApiClient';
import {ChangeBlogManagementApi} from 'src/clients/ChangeBlogManagementApiClient';
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
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {TooltipModule} from "primeng/tooltip";
import {InputTextModule} from "primeng/inputtext";
import {RippleModule} from "primeng/ripple";
import {LayoutComponent} from './components/layout/layout.component';
import {RedirectComponent} from './components/redirect/redirect.component';
import {CheckboxModule} from "primeng/checkbox";
import {RadioButtonModule} from "primeng/radiobutton";
import {TranslocoRootModule} from './transloco-root.module';
import {TranslocoService} from "@ngneat/transloco";
import {MessageModule} from "primeng/message";
import {MessagesModule} from "primeng/messages";
import {ConfirmationService, MessageService} from "primeng/api";
import {ToastModule} from "primeng/toast";
import {DropdownModule} from 'primeng/dropdown';
import {ProgressSpinnerModule} from 'primeng/progressspinner';
import {TranslocoLocaleModule} from "@ngneat/transloco-locale";
import {ContentHeaderComponent} from './components/content-header/content-header.component';
import {initializeApp} from "./app-init";
import {LoadingSpinnerComponent} from './components/loading-spinner/loading-spinner.component';
import {TranslationKey} from "./generated/TranslationKey";
import {SwipeConfig} from "./configuration/swipe-config.service";
import {DefaultRequestHeadersInterceptor} from "./interceptors/default-request-headers.interceptor";
import {AppUserService} from "./services/app-user.service";
import {TableModule} from "primeng/table";
import {ConfirmDialogModule} from "primeng/confirmdialog";
import {ToolbarModule} from "primeng/toolbar";
import {CalendarModule} from "primeng/calendar";
import { AccountsComponent } from './components/accounts/accounts.component';

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
    LoadingSpinnerComponent,
    AccountsComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
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
    TranslocoLocaleModule,
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
    DropdownModule,
    ProgressSpinnerModule,
    HammerModule,
    TableModule,
    ConfirmDialogModule,
    ToolbarModule,
    CalendarModule
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [Router, OAuthService, APP_CONFIG, ChangeBlogManagementApi.Client, TranslocoService, AppUserService],
      multi: true,
    },
    {
      provide: ChangeBlogApi.API_BASE_URL,
      useFactory: (appConfig: AppConfig) => appConfig.changeBlogApiBaseUrl,
      deps: [APP_CONFIG],
      multi: true
    },
    {provide: OAuthStorage, useValue: localStorage},
    {provide: HAMMER_GESTURE_CONFIG, useClass: SwipeConfig},
    {provide: HTTP_INTERCEPTORS, useClass: DefaultRequestHeadersInterceptor, multi: true},
    ChangeBlogApi.Client,
    ChangeBlogManagementApi.Client,
    MessageService,
    ConfirmationService,
    TranslationKey
  ],
  bootstrap: [AppComponent],
})
export class AppModule {
}

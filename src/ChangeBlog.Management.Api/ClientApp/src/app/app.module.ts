import {NgModule} from "@angular/core";
import {BrowserModule} from "@angular/platform-browser";
import {HttpClientModule} from "@angular/common/http";

import {AppRoutingModule} from "./app-routing.module";
import {AppComponent} from "./app.component";
import {HeaderComponent} from "./components/header/header.component";

import {AuthConfig, OAuthModule} from "angular-oauth2-oidc";
import {environment} from "src/environments/environment";
import {HomeComponent} from "./components/home/home.component";
import {ProfileComponent} from "./components/profile/profile.component";

let host = `${window.location.origin}`;

@NgModule({
    declarations: [
        AppComponent,
        HeaderComponent,
        HomeComponent,
        ProfileComponent
    ],
    imports: [
        BrowserModule,
        HttpClientModule,
        AppRoutingModule,
        OAuthModule.forRoot({
            resourceServer: {
                sendAccessToken: true,
                allowedUrls: [host]
            }
        })
    ],
    providers: [
        {provide: AuthConfig, useValue: environment.authConfig}
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}

import {InjectionToken} from "@angular/core"
import {AuthConfig} from "angular-oauth2-oidc"

export interface AppConfig {
  changeBlogApiBaseUrl: string;
  discoveryDocument: string;
  authConfig: AuthConfig;
  appVersion: string;
}

export let APP_CONFIG = new InjectionToken<AppConfig>('APP_CONFIG')

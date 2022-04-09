import { InjectionToken } from "@angular/core"
import { AuthConfig } from "angular-oauth2-oidc"

export class AppConfig {
  authConfig?: AuthConfig;
  changeBlogApiBaseUrl?: string;
  appVersion?: string;
}

export let APP_CONFIG = new InjectionToken<AppConfig>('APP_CONFIG')

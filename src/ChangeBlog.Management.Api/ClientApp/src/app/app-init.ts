import {Router} from "@angular/router";
import {OAuthService} from "angular-oauth2-oidc";
import {AppConfig} from "../../app.config";
import {ChangeBlogManagementApi as MngmtApiClient} from "../clients/ChangeBlogManagementApiClient";
import {getBrowserLang, TranslocoService} from "@ngneat/transloco";
import {filter, mergeMap} from "rxjs/operators";
import {firstValueFrom} from "rxjs";
import {AppUserService} from "./services/app-user.service";

async function setBrowserLanguageOrDefault(oAuthService: OAuthService,
                                           translationService: TranslocoService) {
  const language = getBrowserLang() ?? translationService.getDefaultLang();

  translationService.setActiveLang(language);
  await firstValueFrom(translationService.load(language))
}

function subscribeTokenReceived(authService: OAuthService, apiClient: MngmtApiClient.Client) {
  authService.events
    .pipe(
      filter((e) => e.type === 'token_received' && authService.hasValidAccessToken()),
      mergeMap((x) => apiClient.ensureUserIsImported())
    )
    .subscribe((x) => console.debug(x));
}

export function initializeApp(
  router: Router,
  oAuthService: OAuthService,
  appConfig: AppConfig,
  apiClient: MngmtApiClient.Client,
  translationService: TranslocoService,
  appCulture: AppUserService
): () => Promise<void> {
  return async () => {
    router.onSameUrlNavigation = 'reload';
    oAuthService.configure(appConfig.authConfig);
    oAuthService.setupAutomaticSilentRefresh();

    await setBrowserLanguageOrDefault(oAuthService, translationService);
    subscribeTokenReceived(oAuthService, apiClient);
    await oAuthService.loadDiscoveryDocument(appConfig.discoveryDocument);
    await oAuthService.tryLoginCodeFlow();

    const isLoggedIn = oAuthService.hasValidIdToken() && oAuthService.hasValidAccessToken();
    if (isLoggedIn) {
      await appCulture.applyUserSettings();
    }
  };
}

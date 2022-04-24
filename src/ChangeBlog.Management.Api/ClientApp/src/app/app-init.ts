import {Router} from "@angular/router";
import {OAuthService} from "angular-oauth2-oidc";
import {AppConfig} from "../../app.config";
import {ChangeBlogManagementApi} from "../clients/ChangeBlogManagementApiClient";
import {getBrowserLang, TranslocoService} from "@ngneat/transloco";
import {filter, mergeMap} from "rxjs/operators";
import {LanguageInfo} from "./transloco-root.module";

function initializeAuthentication(oAuthService: OAuthService, appConfig: AppConfig, apiClient: ChangeBlogManagementApi.Client) {
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

  return oAuthService.loadDiscoveryDocument(appConfig.discoveryDocument);
}

function initializeI18n(translationService: TranslocoService) {
  const getValueOrDefault = (v: string | undefined | null) => v ? v : undefined;

  const storedLang = getValueOrDefault(localStorage.getItem('language'));
  const browserLang = getValueOrDefault(getBrowserLang());
  const defaultLang = getValueOrDefault(translationService.getDefaultLang());

  const chosenLang = storedLang ?? browserLang ?? defaultLang ?? 'en-US';
  const finalLang = (<LanguageInfo[]>translationService.getAvailableLangs()).find(x => x.id === chosenLang)?.id ?? defaultLang;

  translationService.setActiveLang(finalLang!);
  return translationService.load(finalLang!).toPromise();
}

export function initializeApp(
  router: Router,
  oAuthService: OAuthService,
  appConfig: AppConfig,
  apiClient: ChangeBlogManagementApi.Client,
  translationService: TranslocoService
): () => Promise<void> {
  return () => {
    return new Promise<void>((resolve, reject) => {
      router.onSameUrlNavigation = 'reload';

      const authSetup = initializeAuthentication(oAuthService, appConfig, apiClient);
      const i18nSetup = initializeI18n(translationService);

      // wait till setup is finished
      Promise.all([authSetup, i18nSetup])
        .then(() => resolve());
    });

  };
}

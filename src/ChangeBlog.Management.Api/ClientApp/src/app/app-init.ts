import {Router} from "@angular/router";
import {OAuthService} from "angular-oauth2-oidc";
import {AppConfig} from "../../app.config";
import {ChangeBlogManagementApi as MngmtApiClient} from "../clients/ChangeBlogManagementApiClient";
import {getBrowserLang, TranslocoService} from "@ngneat/transloco";
import {filter, mergeMap} from "rxjs/operators";
import {TranslocoLocaleService} from "@ngneat/transloco-locale";

function initializeAuthentication(oAuthService: OAuthService, appConfig: AppConfig, apiClient: MngmtApiClient.Client) {
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

function initializeI18n(translationService: TranslocoService, translocoLocaleService: TranslocoLocaleService, apiClient: MngmtApiClient.Client) {
  return new Promise<void>((resolve, reject) => {
    apiClient.getUserProfile()
      .subscribe(x => {
        debugger;

        const lang = x.culture?.split("-")[0];
        translocoLocaleService.setLocale(x.culture ?? 'en-US');

        const language = lang ?? getBrowserLang() ?? translationService.getDefaultLang();

        translationService.setActiveLang(language);
        translationService.load(language)
          .toPromise()
          .then(x => {
            resolve();
          });
      });
  });
}

export function initializeApp(
  router: Router,
  oAuthService: OAuthService,
  appConfig: AppConfig,
  apiClient: MngmtApiClient.Client,
  translationService: TranslocoService,
  translocoLocaleService: TranslocoLocaleService
): () => Promise<void> {
  return () => {
    return new Promise<void>((resolve, reject) => {
      router.onSameUrlNavigation = 'reload';

      const authSetup = initializeAuthentication(oAuthService, appConfig, apiClient);
      const i18nSetup = initializeI18n(translationService, translocoLocaleService, apiClient);

      // wait till setup is finished
      Promise.all([authSetup, i18nSetup])
        .then(() => resolve());
    });

  };
}

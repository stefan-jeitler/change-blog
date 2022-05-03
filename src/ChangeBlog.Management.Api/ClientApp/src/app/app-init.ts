import {Router} from "@angular/router";
import {OAuthService} from "angular-oauth2-oidc";
import {AppConfig} from "../../app.config";
import {ChangeBlogManagementApi as MngmtApiClient} from "../clients/ChangeBlogManagementApiClient";
import {getBrowserLang, TranslocoService} from "@ngneat/transloco";
import {TranslocoLocaleService} from "@ngneat/transloco-locale";

async function setBrowserLanguageOrDefault(oAuthService: OAuthService,
                                           translationService: TranslocoService) {
  const language = getBrowserLang() ?? translationService.getDefaultLang();

  translationService.setActiveLang(language);
  await translationService.load(language).toPromise();
}

async function setUserCulture(userCulture: MngmtApiClient.ICultureDto, translationService: TranslocoService, translocoLocaleService: TranslocoLocaleService) {
  const activeCulture = translocoLocaleService.getLocale();

  if (activeCulture === userCulture.culture)
    return;

  const language = userCulture.language ?? getBrowserLang() ?? translationService.getDefaultLang();

  translocoLocaleService.setLocale(userCulture.culture ?? 'en-US');
  translationService.setActiveLang(language);
  await translationService.load(language).toPromise();
}

export function initializeApp(
  router: Router,
  oAuthService: OAuthService,
  appConfig: AppConfig,
  apiClient: MngmtApiClient.Client,
  translationService: TranslocoService,
  translocoLocaleService: TranslocoLocaleService
): () => Promise<void> {
  return async () => {
    router.onSameUrlNavigation = 'reload';
    oAuthService.configure(appConfig.authConfig);
    oAuthService.setupAutomaticSilentRefresh();

    await setBrowserLanguageOrDefault(oAuthService, translationService);
    await oAuthService.loadDiscoveryDocument(appConfig.discoveryDocument);
    await oAuthService.tryLoginCodeFlow();

    if (oAuthService.hasValidIdToken()) {
      const userCulture = await apiClient
        .getUserCulture()
        .toPromise();

      await setUserCulture(userCulture, translationService, translocoLocaleService);
    }
  };
}

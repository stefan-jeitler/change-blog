import {getBrowserLang, TranslocoService} from "@ngneat/transloco";
import {TranslocoLocaleService} from "@ngneat/transloco-locale";
import {firstValueFrom} from "rxjs";
import {ChangeBlogManagementApi} from "../../clients/ChangeBlogManagementApiClient";
import {PrimeNGConfig} from "primeng/api";
import {Injectable} from "@angular/core";
import {TranslationKey} from "../generated/TranslationKey";

@Injectable({
  providedIn: 'root',
})
export class AppCultureService {
  constructor(private translationService: TranslocoService,
              private localeService: TranslocoLocaleService,
              private mngmApiClient: ChangeBlogManagementApi.Client,
              private translationKey: TranslationKey,
              private primeNgConfig: PrimeNGConfig) {
  }

  async applyUserCulture() {
    const activeCulture = this.localeService.getLocale();
    const userCulture = await firstValueFrom(this.mngmApiClient.getUserCulture());

    if (activeCulture === userCulture.culture)
      return;

    const language = userCulture.language ?? getBrowserLang() ?? this.translationService.getDefaultLang();

    this.localeService.setLocale(userCulture.culture ?? 'en-US');
    this.translationService.setActiveLang(language);
    await firstValueFrom(this.translationService.load(language));

    const primeNgConfig = this.translationService.translateObject(this.translationKey.primeng.$key)

    primeNgConfig['firstDayOfWeek'] = userCulture.firstDayOfWeek;
    primeNgConfig['dateFormat'] = userCulture.shortDateFormat;

    this.primeNgConfig.setTranslation(primeNgConfig)
  }
}

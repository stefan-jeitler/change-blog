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
export class AppUserService {
  /*
    d - day of month (no leading zero)
    dd - day of month (two digit)
    o - day of the year (no leading zeros)
    oo - day of the year (three digit)
    D - day name short
    DD - day name long
    m - month of year (no leading zero)
    mm - month of year (two digit)
    M - month name short
    MM - month name long
    y - year (two digit)
    yy - year (four digit)
    @ - Unix timestamp (ms since 01/01/1970)
    ! - Windows ticks (100ns since 01/01/0001)
    '...' - literal text
    '' - single quote
    anything else - literal text
  */
  private primeNgDateFormatAdjustments: { [key: string]: string } = {
    'M/d/yyyy': 'm/d/yy', // en-US
    'dd/MM/yyyy': 'dd/mm/yy', // en-GB
    'dd.MM.yyyy': 'dd.mm.yy' // de-*
  };

  constructor(private translationService: TranslocoService,
              private localeService: TranslocoLocaleService,
              private mngmApiClient: ChangeBlogManagementApi.Client,
              private translationKey: TranslationKey,
              private primeNgConfig: PrimeNGConfig) {
  }

  async applyUserSettings() {
    const activeCulture = this.localeService.getLocale();
    const userCulture = await firstValueFrom(this.mngmApiClient.getUserCulture());

    if (activeCulture === userCulture.culture)
      return;

    const language = userCulture.language ?? getBrowserLang() ?? this.translationService.getDefaultLang();

    this.localeService.setLocale(userCulture.culture!);
    this.translationService.setActiveLang(language);
    await firstValueFrom(this.translationService.load(language));

    const primeNgConfig = this.translationService.translateObject(this.translationKey.primeng.$key)

    primeNgConfig['firstDayOfWeek'] = userCulture.firstDayOfWeek;
    primeNgConfig['dateFormat'] = this.applyDateFormatAdjustments(userCulture.shortDateFormat!);

    this.primeNgConfig.setTranslation(primeNgConfig)
  }

  private applyDateFormatAdjustments(dateFormat: string): string {
    if (this.primeNgDateFormatAdjustments.hasOwnProperty(dateFormat))
      return this.primeNgDateFormatAdjustments[dateFormat];

    return dateFormat;
  }
}

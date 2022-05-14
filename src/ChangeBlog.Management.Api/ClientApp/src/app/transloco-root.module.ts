import {TRANSLOCO_CONFIG, TRANSLOCO_LOADER, translocoConfig, TranslocoLoader, TranslocoModule} from '@ngneat/transloco';
import {Injectable, NgModule} from '@angular/core';
import {environment} from '../environments/environment';
import {TranslocoLocaleModule} from "@ngneat/transloco-locale";


export interface LanguageInfo {
  id: string;
  label: string;
}

@Injectable({providedIn: 'root'})
export class TranslocoHttpLoader implements TranslocoLoader {
  constructor() {
  }

  getTranslation(lang: string) {
    return import(`../assets/i18n/${lang}.json`).then(res => res.default);
  }
}

@NgModule({
  imports: [TranslocoLocaleModule.forRoot({})],
  exports: [TranslocoModule],
  providers: [
    {
      provide: TRANSLOCO_CONFIG,
      useValue: translocoConfig({
        availableLangs: [
          {id: 'en', label: 'English'},
          {id: 'de', label: 'Deutsch'}
        ],
        defaultLang: 'en',
        fallbackLang: 'en',
        missingHandler: {
          useFallbackTranslation: true,
          logMissingKey: true
        },
        reRenderOnLangChange: true,
        prodMode: environment.production
      })
    },
    {provide: TRANSLOCO_LOADER, useClass: TranslocoHttpLoader}
  ]
})
export class TranslocoRootModule {
}

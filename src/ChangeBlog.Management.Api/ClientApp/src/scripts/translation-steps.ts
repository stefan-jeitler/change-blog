// @ts-ignore
import defaultTranslations from "../assets/i18n/en.json";
import * as fs from 'fs';

type TranslationObject = { [key: string]: any };
const translations = defaultTranslations as TranslationObject;

export function generateTranslationKeys(): void {
  const targetFile = 'src/app/generated/TranslationKey.ts';

  console.log('Use source file: ../assets/i18n/en.json');
  console.log('Use target file: ' + targetFile)

  function findInTranslations(translations: TranslationObject, key: string): object | string | undefined {
    if(translations.hasOwnProperty(key))
      return translations[key];

    for(const t in translations){
      if(translations.hasOwnProperty(t) && typeof translations[t] === 'object')
        return findInTranslations(translations[t], key);
    }

    return undefined;
  }

  function generateEntry(key: string): string {
    if(!key)
      return '';

    const currentItem = findInTranslations(translations, key);

    if(!currentItem)
      return '';

    if(typeof currentItem === 'string')
      return `public static ${key} = '${key}';`;

    if(typeof currentItem !== 'object')
      return '';

    const nested = Object.keys(currentItem)
      .filter(x => currentItem.hasOwnProperty(x))
      .map(x => generateEntry(x))
      .join('');

    return `public static ${key} = class { public static $key = '${key}';${nested}};`;
  }

  const fields = Object.keys(translations)
    .map(x => generateEntry(x))
    .join('\n\t');

  const translationKeys =
    `export class TranslationKey {\n\t${fields}\n}`;

  console.log('Write translation keys to file');
  fs.writeFileSync(targetFile, translationKeys);
}

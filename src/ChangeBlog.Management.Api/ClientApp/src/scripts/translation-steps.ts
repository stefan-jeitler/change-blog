// @ts-ignore
import * as defaultTranslations from "../assets/i18n/en.json";
import * as fs from 'fs';

export function generateTranslationKeys(): void {
    const targetFile = 'src/app/generated/TranslationKey.ts';

    console.log('Use source file: ../assets/i18n/en.json');
    console.log('Use target file: ' + targetFile)

    const fields = Object.keys(defaultTranslations)
      .filter(x => x !== 'default')
      .map(x => `public static ${x} = '${x}';`)
      .join('\n\t');

    const translationKeys =
      `export class TranslationKey {
  ${fields}
}`;

    console.log('Write translation keys to file');
    fs.writeFileSync(targetFile, translationKeys);
}

export function verifyConsistentTranslationFiles() : void {
}

// @ts-ignore
import defaultTranslations from "../assets/i18n/en.json";
import * as fs from 'fs';

type TranslationObject = { [key: string]: any };
const translations = defaultTranslations as TranslationObject;

function normalizeKey(key: string) {
  if (!key)
    throw `key '${key}' is falsy`;

  return key.replace('-', '_');
}

function findInTranslations(transl: TranslationObject, key: string): object | string | undefined {
  if (typeof transl !== 'object')
    return undefined;

  if (transl.hasOwnProperty(key))
    return transl[key];

  for (const t in transl) {
    if (transl.hasOwnProperty(t) && typeof transl[t] === 'object')
      return findInTranslations(transl[t], key);
  }

  return undefined;
}

function generateEntry(transl: TranslationObject, key: string): string {
  if (!key)
    return '';

  const currentItem = findInTranslations(transl, key);

  if (!currentItem) {
    return '';
  }

  if (typeof currentItem === 'string')
    return `public ${normalizeKey(key)} = '${key}';`;

  if (typeof currentItem !== 'object')
    return '';

  const nested = Object.keys(currentItem)
    .filter(x => currentItem.hasOwnProperty(x))
    .map(x => generateEntry(currentItem, x))
    .join('');

  const nestedClassDeclaration = `public _${normalizeKey(key)} = class { public $key = '${normalizeKey(key)}';${nested}};`;
  return `${nestedClassDeclaration} public ${normalizeKey(key)} = new this._${normalizeKey(key)}();`;
}

export function generateTranslationKeys(): void {
  const targetFile = 'src/app/generated/TranslationKey.ts';

  console.log('Use source file: ../assets/i18n/en.json');
  console.log('Use target file: ' + targetFile)

  console.log('Generating translation keys ...');
  const fields = Object.keys(translations)
    .map(x => generateEntry(translations, x))
    .join('\n\t');

  const translationKeys =
    `export class TranslationKey {\n\t${fields}\n}`;

  console.log('Write translation keys to file');
  fs.writeFileSync(targetFile, translationKeys);
}

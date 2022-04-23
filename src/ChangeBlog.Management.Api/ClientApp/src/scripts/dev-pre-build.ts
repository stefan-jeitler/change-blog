import {generateTranslationKeys, verifyConsistentTranslationFiles} from './translation-steps';

const PreBuildSteps: (() => void)[] = [
  verifyConsistentTranslationFiles,
  generateTranslationKeys
];

console.log("Run Dev-Pre-Build Steps ...");

for(const step of PreBuildSteps) {
  console.log('Run step: ' + step.name)
  step();
}

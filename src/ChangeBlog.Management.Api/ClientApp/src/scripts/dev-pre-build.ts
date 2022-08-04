import {generateTranslationKeys} from './translation-steps';

const preBuildSteps: (() => void)[] = [
  generateTranslationKeys
];

console.log("Run Dev-Pre-Build Steps ...");

for (const step of preBuildSteps) {
  console.log('Run step: ' + step.name)
  step();
}

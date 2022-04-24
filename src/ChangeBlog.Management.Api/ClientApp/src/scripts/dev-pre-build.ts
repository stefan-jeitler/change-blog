import {generateTranslationKeys} from './translation-steps';
import {generateRouterLinks} from "./router-steps";

const PreBuildSteps: (() => void)[] = [
  generateTranslationKeys,
  generateRouterLinks
];

console.log("Run Dev-Pre-Build Steps ...");

for (const step of PreBuildSteps) {
  console.log('Run step: ' + step.name)
  step();
}

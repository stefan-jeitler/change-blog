import {generateTranslationKeys} from './translation-steps';

const preBuildSteps: (() => Promise<void>)[] = [
  generateTranslationKeys
];

const runPreBuildStepsAsync = async () => {
  for (const step of preBuildSteps) {
    console.log('Run step: ' + step.name);
    await step();
  }
};

console.log("Run Dev-Pre-Build Steps ...");
runPreBuildStepsAsync()
  .then(_ => console.log('Pre-Build Steps finished'));

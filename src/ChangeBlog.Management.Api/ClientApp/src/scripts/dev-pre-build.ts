import {generateTranslationKeys} from './translation-steps';

const preBuildSteps: (() => Promise<void>)[] = [
  generateTranslationKeys
];

console.log("Run Dev-Pre-Build Steps ...");
const asyncLoop = async () => {
  for (const step of preBuildSteps) {
    console.log('Run step: ' + step.name)
    await step();
  }
};

asyncLoop()
  .then(r => console.log('Pre-Build Steps finished'));

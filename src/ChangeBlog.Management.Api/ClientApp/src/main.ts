import './app/extensions/form.extensions';
import {enableProdMode} from '@angular/core';
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';

import {APP_CONFIG} from 'app.config';
import {AppModule} from './app/app.module';

import {environment} from './environments/environment';

fetch("/api/appsettings")
  .then((res) => res.json())
  .then((config) => {
    config.authConfig.redirectUri = window.location.origin + '/';
    config.appVersion = require("package.json").version;

    if (environment.production) {
      enableProdMode();
    }

    platformBrowserDynamic([
      {provide: APP_CONFIG, useValue: config}])
      .bootstrapModule(AppModule)
      .catch((err) => console.error(err));
  });

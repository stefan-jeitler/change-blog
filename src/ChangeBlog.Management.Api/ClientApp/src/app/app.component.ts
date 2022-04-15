import { Component, Inject } from '@angular/core';
import { AppConfig, APP_CONFIG } from 'app.config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  constructor(@Inject(APP_CONFIG) private appConfig: AppConfig) {
    this.showMobileSideNav = false;
  }

  showMobileSideNav: boolean;

  title = 'change-blog';
  get appVersion(): string {
    return this.appConfig.appVersion!;
  }

  triggerMobileSideNav() {
    this.showMobileSideNav = true;
  }
}

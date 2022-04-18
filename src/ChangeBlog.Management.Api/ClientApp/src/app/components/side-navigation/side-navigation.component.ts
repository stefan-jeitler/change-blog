import {Component, Inject, Input, OnInit} from '@angular/core';
import {MenuItem} from "primeng/api";
import {APP_CONFIG, AppConfig} from "app.config";
import {translate, TranslocoService} from "@ngneat/transloco";

@Component({
  selector: 'app-side-navigation',
  templateUrl: './side-navigation.component.html',
  styleUrls: ['./side-navigation.component.scss']
})
export class SideNavigationComponent implements OnInit {

  menuItems: MenuItem[] = [];
  currentYear: number;

  constructor(@Inject(APP_CONFIG)
              private appConfig: AppConfig,
              private translationService: TranslocoService) {
    this.populateMenuItems();

    this.currentYear = new Date().getUTCFullYear();
    this.showTitle = false;

    // listen to profile only is enough and update all other labels
    this.translationService
      .selectTranslate('profile')
      .subscribe(x => this.populateMenuItems());
  }

  private populateMenuItems() {
    this.menuItems = [
      {
        label: translate('profile'),
        icon: 'pi pi-fw pi-user',
        routerLink: '/app/profile'
      },
      {
        label: translate('apikey'),
        icon: 'pi pi-fw pi-key',
        routerLink: "/app/apikey"
      }
    ];
  }

  ngOnInit(): void {

  }

  @Input() showTitle: boolean;

  get appVersion(): string {
    return this.appConfig.appVersion!;
  }
}

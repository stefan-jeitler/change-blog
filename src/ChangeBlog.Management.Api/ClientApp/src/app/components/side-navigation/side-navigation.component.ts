import {Component, Inject, OnInit} from '@angular/core';
import {MenuItem} from "primeng/api";
import {APP_CONFIG, AppConfig} from "app.config";

@Component({
  selector: 'app-side-navigation',
  templateUrl: './side-navigation.component.html',
  styleUrls: ['./side-navigation.component.scss']
})
export class SideNavigationComponent implements OnInit {

  menuItems: MenuItem[];

  constructor(@Inject(APP_CONFIG)
              private appConfig: AppConfig,) {
    this.menuItems = [
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user',
        routerLink: '/profile'
      },
      {
        label: 'Api Key',
        icon: 'pi pi-fw pi-key',
        routerLink: "/apikey"
      }
    ];
  }

  ngOnInit(): void {

  }

  get appVersion(): string {
    return this.appConfig.appVersion!;
  }
}

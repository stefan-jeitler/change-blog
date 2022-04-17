import {Component, Inject, Input, OnInit} from '@angular/core';
import {MenuItem} from "primeng/api";
import {APP_CONFIG, AppConfig} from "app.config";

@Component({
  selector: 'app-side-navigation',
  templateUrl: './side-navigation.component.html',
  styleUrls: ['./side-navigation.component.scss']
})
export class SideNavigationComponent implements OnInit {

  menuItems: MenuItem[];
  currentYear: number;

  constructor(@Inject(APP_CONFIG)
              private appConfig: AppConfig) {
    this.menuItems = [
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user',
        routerLink: '/app/profile'
      },
      {
        label: 'Api Key',
        icon: 'pi pi-fw pi-key',
        routerLink: "/app/apikey"
      }
    ];

    this.currentYear = new Date().getUTCFullYear();
    this.showTitle = false;
  }

  ngOnInit(): void {

  }

  @Input() showTitle: boolean;

  get appVersion(): string {
    return this.appConfig.appVersion!;
  }
}

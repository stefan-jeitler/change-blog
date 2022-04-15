import { Component, OnInit } from '@angular/core';
import {MenuItem} from "primeng/api";

@Component({
  selector: 'app-side-navigation',
  templateUrl: './side-navigation.component.html',
  styleUrls: ['./side-navigation.component.scss']
})
export class SideNavigationComponent implements OnInit {

  constructor() {
    this.items = [
      {label: 'Accounts', icon: 'pi pi-fw pi-plus'},
      {label: 'Users', icon: 'pi pi-fw pi-download'},
      {label: 'Products', icon: 'pi pi-fw pi-refresh'},
      {label: 'Versions', icon: 'pi pi-fw pi-refresh'},
      {label: 'Change Logs', icon: 'pi pi-fw pi-refresh'}
    ];
  }

  items: MenuItem[];

  ngOnInit(): void {

  }

}

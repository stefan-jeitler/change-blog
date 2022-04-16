import {Component} from '@angular/core';
import {ActivationEnd, Router} from "@angular/router";
import {filter} from "rxjs/operators";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  showMobileSideNav: boolean;
  title = 'change-blog';

  constructor(private router: Router) {
    this.showMobileSideNav = false;

    this.router.events
      .pipe(filter(event => event instanceof ActivationEnd))
      .subscribe((event) => {
        this.showMobileSideNav = false;
      });
  }

  triggerMobileSideNav() {
    this.showMobileSideNav = true;
  }
}

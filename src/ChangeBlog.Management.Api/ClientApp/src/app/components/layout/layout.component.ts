import {Component} from '@angular/core';
import {ActivationEnd, Router} from "@angular/router";
import {filter} from "rxjs/operators";
import {Constants} from "../../../constants";

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {
  showMobileSideNav: boolean;

  constructor(private router: Router) {
    this.showMobileSideNav = false;

    this.router.events
      .pipe(filter(event => event instanceof ActivationEnd))
      .subscribe((_) => {
        this.showMobileSideNav = false;
      });
  }

  openMobileSideNav() {
    this.showSideMobileNavAndAddCloseSwipeHandler();
  }

  onSwipeRight($event: any) {
    if (window.innerWidth < Constants.MobileBreakpoint.value) {
      this.showSideMobileNavAndAddCloseSwipeHandler();
    }
  }

  onSwipeLeft() {
    this.showMobileSideNav = false;
  }

  private showSideMobileNavAndAddCloseSwipeHandler() {
    this.showMobileSideNav = true;

    setTimeout(() => {
      let overlaySelector = '.p-component-overlay, .p-sidebar-mask, .p-component-overlay-enter';
      const overlay: HTMLElement = document.querySelector(overlaySelector) as HTMLElement;

      if (!!overlay) {
        const manager = new Hammer(overlay)
        manager.on('swipeleft', e => {
          this.onSwipeLeft();
          manager.off('swipeleft')
        });
      }

    }, 100);
  }
}

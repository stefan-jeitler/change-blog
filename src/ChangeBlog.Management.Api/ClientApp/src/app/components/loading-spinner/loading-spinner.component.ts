import {Component, Input, OnInit} from '@angular/core';
import Timeout = NodeJS.Timeout;

@Component({
  selector: 'app-loading-spinner',
  templateUrl: './loading-spinner.component.html',
  styleUrls: ['./loading-spinner.component.scss']
})
export class LoadingSpinnerComponent implements OnInit {

  showOverlay: boolean;
  showSpinner: boolean;
  runningTimeout: Timeout | undefined;
  @Input() spinnerDebounceTime: number;

  constructor() {
    this.spinnerDebounceTime = 500;

    this.showOverlay = false;
    this.showSpinner = false;
  }

  @Input() set show(value: boolean) {
    if (value)
      this.start();
    else
      this.stop();
  }

  stop() {
    if(!!this.runningTimeout)
      clearTimeout(this.runningTimeout);

    this.showOverlay = false;
    this.showSpinner = false;
  }

  start() {
    this.showOverlay = true;
    this.runningTimeout = setTimeout(() => this.showSpinner = true, this.spinnerDebounceTime);
  }

  ngOnInit(): void {
  }

}

import { Component, HostBinding } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  @HostBinding('attr.app-version') appVersionAttr = environment.appVersion;
  title = 'change-blog';
  currentApplicationVersion = environment.appVersion;
}

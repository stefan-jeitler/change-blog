import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable} from 'rxjs';
import {OAuthService} from "angular-oauth2-oidc";
import {filter} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private authService: OAuthService) {
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    const isLoggedIn = () => this.authService.hasValidIdToken() && this.authService.hasValidAccessToken();
    const gotoLandingPage = () => this.router.navigateByUrl('/home');

    const canActivate = new Promise<boolean>((resolve, reject) => {
      if (isLoggedIn())
        resolve(true);
      else {

        this.authService.events
          .pipe(filter(x => x.type === 'token_received'))
          .subscribe(
            x => {
              if (isLoggedIn())
                resolve(true);
              else {
                gotoLandingPage();
                resolve(false);
              }
            },
            e => {
              gotoLandingPage();
              reject(e);
            });
      }
    });

    const timeout = new Promise<boolean>((resolve, reject) => {
      setTimeout(() => {
        const loggedIn = isLoggedIn();

        if(loggedIn)
          resolve(true);
        else {
          resolve(false);
          gotoLandingPage();
        }
      }, 500);
    });

    return Promise.race([canActivate, timeout]);
  }

}

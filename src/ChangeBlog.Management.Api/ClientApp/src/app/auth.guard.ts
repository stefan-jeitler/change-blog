import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable} from 'rxjs';
import {OAuthService} from "angular-oauth2-oidc";

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

    return new Promise<boolean>((resolve, reject) => {
      if (isLoggedIn())
        resolve(true);
      else {
        this.authService.tryLogin()
          .then(x => {
            if (isLoggedIn())
              resolve(true);
            else {
              gotoLandingPage();
              resolve(false);
            }
          })
          .catch(e => {
            gotoLandingPage();
            reject(e);
          })
      }

    });
  }

}

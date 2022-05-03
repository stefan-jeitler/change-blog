import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {OAuthService} from "angular-oauth2-oidc";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private authService: OAuthService) {
  }

  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean | UrlTree> {

    const isLoggedIn = () => this.authService.hasValidIdToken() && this.authService.hasValidAccessToken();
    const gotoLandingPage = () => this.router.navigateByUrl('/home');

    if(isLoggedIn())
      return true;

    await this.authService.tryLoginCodeFlow();

    if(isLoggedIn())
      return true;

    await gotoLandingPage();
    return false;
  }

}

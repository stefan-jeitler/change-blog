import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {LandingComponent} from "./components/landing/landing.component";
import {HomeComponent} from "./components/home/home.component";
import {LayoutComponent} from "./components/layout/layout.component";
import {ProfileComponent} from "./components/profile/profile.component";
import {ApikeyComponent} from "./components/apikey/apikey.component";
import {AuthGuard} from "./guards/auth.guard";
import {RedirectComponent} from "./components/miscellaneous/redirect/redirect.component";
import {AccountsComponent} from "./components/account/accounts/accounts.component";
import {AccountComponent} from "./components/account/account/account.component";

export const routes: Routes = [
  {
    path: 'app',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'home',
        component: HomeComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'accounts',
        component: AccountsComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'accounts/:id',
        component: AccountComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'profile',
        component: ProfileComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'apikey',
        component: ApikeyComponent,
        canActivate: [AuthGuard]
      },
      {
        path: '**',
        redirectTo: 'home',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: 'home',
    component: LandingComponent
  },
  {
    path: '**',
    component: RedirectComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {
}

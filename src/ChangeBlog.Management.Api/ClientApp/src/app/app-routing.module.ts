import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LandingComponent} from "./components/landing/landing.component";
import {HomeComponent} from "./components/home/home.component";
import {LayoutComponent} from "./components/layout/layout.component";
import {ProfileComponent} from "./components/profile/profile.component";
import {ApikeyComponent} from "./components/apikey/apikey.component";
import {AuthGuard} from "./auth.guard";
import {RedirectComponent} from "./components/redirect/redirect.component";

const routes: Routes = [
  {
    path: 'app',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'home',
        component: HomeComponent,
      },
      {
        path: 'profile',
        component: ProfileComponent,
      },
      {
        path: 'apikey',
        component: ApikeyComponent,
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
    component: RedirectComponent,

  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}

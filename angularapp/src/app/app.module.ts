import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {RouterModule} from '@angular/router';
import {JwtModule} from "@auth0/angular-jwt";
import {Routes} from '@angular/router';
import { AppComponent } from './app.component';
import { SignUpPageComponent } from './components/sign-up-page/sign-up-page.component';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { HomeComponent } from './components/home/home.component';
import {ReactiveFormsModule} from "@angular/forms";
import {AuthGuard} from "./auth.guard";
import {TokenInterceptorService} from "./interceptors/token-interceptor.service";
import { DriverListComponent } from './components/drivers/driver-list/driver-list.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatIconModule} from "@angular/material/icon";
import { ViewDriverComponent } from './components/drivers/view-driver/view-driver.component';
import { ViewTripComponent } from './components/trips/view-trip/view-trip.component';

const routes: Routes = [
  {path:"", redirectTo:"app", pathMatch:"full"},
  {path:"login", component:LoginPageComponent},
  {path:"signup", component:SignUpPageComponent},
  {path:"home", component:HomeComponent, canActivate: [AuthGuard]},
  {path:"drivers", component:DriverListComponent, canActivate: [AuthGuard]},
  {path:"viewDriver/:id", component:ViewDriverComponent, canActivate: [AuthGuard]},
  {path:"viewTrip/:driverId/:id", component:ViewTripComponent, canActivate: [AuthGuard]}
  ];

export function tokenGetter() {
  return localStorage.getItem("access_token");
}

@NgModule({
  declarations: [
    AppComponent,
    SignUpPageComponent,
    LoginPageComponent,
    HomeComponent,
    DriverListComponent,
    ViewDriverComponent,
    ViewTripComponent,
  ],
  imports: [
    BrowserModule, HttpClientModule, RouterModule.forRoot(routes), ReactiveFormsModule, JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter
      }
    }), BrowserAnimationsModule, MatIconModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptorService,
      multi: true,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

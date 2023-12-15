import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {RouterModule} from '@angular/router';
import {Routes} from '@angular/router';

import { AppComponent } from './app.component';
import { SignUpPageComponent } from './components/sign-up-page/sign-up-page.component';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { HomeComponent } from './components/home/home.component';
import {ReactiveFormsModule} from "@angular/forms";

const routes: Routes = [
  {path:"", redirectTo:"login", pathMatch:"full"},
  {path:"login", component:LoginPageComponent},
  {path:"signup", component:SignUpPageComponent},
  {path:"home", component:HomeComponent}
  ];

@NgModule({
  declarations: [
    AppComponent,
    SignUpPageComponent,
    LoginPageComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule, HttpClientModule, RouterModule.forRoot(routes), ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Driver } from 'src/shared/models/driver';
import {Router} from "@angular/router";
import {AuthService} from "./auth.service";
import {UserService} from "./services/user.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(http: HttpClient, private router: Router, private authService:AuthService, public userService: UserService) {}

  title = 'angularapp';

  home() {
    this.router.navigate(['home']);
  }
  login() {
    this.router.navigate(['login']);
  }
  register() {
    this.router.navigate(['signup']);
  }
  logout() {
    this.authService.logout()
    this.router.navigate(['home'])
    location.reload()
  }
  getRole(){
    return this.userService.user.role
  }
}

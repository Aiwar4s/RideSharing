import { HttpClient } from '@angular/common/http';
import {Component, OnInit} from '@angular/core';
import { Driver } from 'src/shared/models/driver';
import {Router} from "@angular/router";
import {AuthService} from "./auth.service";
import {UserService} from "./services/user.service";
import {DriverService} from "./services/driver.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  constructor(http: HttpClient, private router: Router, private authService:AuthService, public userService: UserService, private driverService:DriverService) {}
  ngOnInit(): void {
    if(this.userService.user.role===2){
      this.driverService.getDrivers().subscribe(drivers=>{
        drivers.forEach(driver=>{
          if(driver.userId===this.userService.user.id){
            this.userService.user.driverId=driver.id
          }
        })
      })
    }
  }

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
  createDriver() {
    this.router.navigate(['createDriver']);
  }
  logout() {
    this.authService.logout()
    this.router.navigate(['home'])
    location.reload()
  }
  getRole(){
    return this.userService.user.role
  }
  getDriver(){
    return this.userService.user.driverId
  }
  addTrip(){
    this.router.navigate(['addTrip', this.userService.user.driverId])
  }
}

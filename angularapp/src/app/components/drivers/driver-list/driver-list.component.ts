import { Component, OnInit } from '@angular/core';
import {Driver} from "../../../../shared/models/driver";
import {DriverService} from "../../../services/driver.service";
import {Router} from "@angular/router";
import {UserService} from "../../../services/user.service";
import {AuthService} from "../../../auth.service";

@Component({
  selector: 'app-driver-list',
  templateUrl: './driver-list.component.html',
  styleUrls: ['./driver-list.component.css']
})
export class DriverListComponent implements OnInit {
  drivers?: Driver[];
  constructor(private service:DriverService, private auth:AuthService, public userService:UserService, private router:Router) { }
  ngOnInit(): void {
    this.service.getDrivers().subscribe(response=>this.drivers=response)
  }
  viewDriver(id:number){
    this.router.navigate(['viewDriver',id])
  }
  deleteDriver(id:number){
    this.service.deleteDriver(id).subscribe()
    this.service.getDrivers().subscribe(response=>this.drivers=response)
    if(this.userService.user.role===2){
      this.userService.user.role=1
    }
  }
  updateDriver(id:number){
    this.router.navigate(['updateDriver',id])
  }
  isDriver(driver:Driver){
    return driver.userId===this.userService.user.id
  }
}

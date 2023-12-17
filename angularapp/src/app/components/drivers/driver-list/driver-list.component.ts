import { Component, OnInit } from '@angular/core';
import {Driver} from "../../../../shared/models/driver";
import {DriverService} from "../../../services/driver.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-driver-list',
  templateUrl: './driver-list.component.html',
  styleUrls: ['./driver-list.component.css']
})
export class DriverListComponent implements OnInit {
  drivers?: Driver[];
  constructor(private service:DriverService, private router:Router) { }
  ngOnInit(): void {
    this.service.getDrivers().subscribe(response=>this.drivers=response)
  }
  viewDriver(id:number){
    this.router.navigate(['viewDriver',id])
  }

}

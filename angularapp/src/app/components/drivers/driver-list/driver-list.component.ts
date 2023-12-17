import { Component, OnInit } from '@angular/core';
import {Driver} from "../../../shared/models/driver";
import {DriverService} from "../../services/driver.service";

@Component({
  selector: 'app-drivers',
  templateUrl: './drivers.component.html',
  styleUrls: ['./drivers.component.css']
})
export class DriversComponent implements OnInit {
  drivers?: Driver[];
  constructor(private service:DriverService) { }
  ngOnInit(): void {
    this.service.getDrivers().subscribe(response=>this.drivers=response)
  }

}

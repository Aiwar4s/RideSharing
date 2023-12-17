import { Component, OnInit } from '@angular/core';
import {Driver} from "../../../../shared/models/driver";
import {DriverService} from "../../../services/driver.service";
import {ActivatedRoute, Router} from '@angular/router';
import {Subscriber, Subscription} from "rxjs";
import {Trip} from "../../../../shared/models/trip";
import {TripService} from "../../../services/trip.service";

@Component({
  selector: 'app-view-driver',
  templateUrl: './view-driver.component.html',
  styleUrls: ['./view-driver.component.css']
})
export class ViewDriverComponent implements OnInit {
  private sub?:Subscription;
  driver?: Driver;
  trips?: Trip[];
  id?: number;
  constructor(private driverService:DriverService, private tripService: TripService, private router:Router, private route:ActivatedRoute) { }
  ngOnInit(): void {
    this.sub=this.route.params.subscribe(params=>{
      this.id=params['id']
    });
    this.getDriver(this.id as number);
    this.getTrips(this.id as number);
  }
  ngOnDestroy(){
    (this.sub as Subscription).unsubscribe()
  }
  getDriver(id:number){
    this.driverService.getDriver(this.id as number).subscribe(response=>this.driver=response)
  }
  getTrips(id:number){
    this.tripService.getTrips(this.id as number).subscribe(response=>this.trips=response)
  }
  viewTrip(id:number){
    this.router.navigate(['viewTrip',this.driver?.id, id])
  }

}

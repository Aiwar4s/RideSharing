import {Component, OnInit} from '@angular/core';
import {TripService} from "../../../services/trip.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Subscription} from "rxjs";
import {Trip} from "../../../../shared/models/trip";
import {Review} from "../../../../shared/models/review";
import {ReviewService} from "../../../services/review.service";
import {DriverService} from "../../../services/driver.service";
import {Driver} from "../../../../shared/models/driver";

@Component({
  selector: 'app-view-trip',
  templateUrl: './view-trip.component.html',
  styleUrls: ['./view-trip.component.css']
})
export class ViewTripComponent implements OnInit{
  private sub?:Subscription;
  trip?: Trip;
  driver?: Driver;
  reviews?: Review[];
  driverId?: number;
  id?: number;
  constructor(private driverService:DriverService, private tripService:TripService, private reviewService:ReviewService, private router:Router, private route:ActivatedRoute) { }
  ngOnInit(): void {
    this.sub=this.route.params.subscribe(params=>{
      this.driverId=params['driverId'];
      this.id=params['id'];
    });
    this.tripService.getTrip(this.driverId as number, this.id as number).subscribe(response=>this.trip=response);
    this.reviewService.getReviews(this.driverId as number, this.id as number).subscribe(response=>this.reviews=response);
    this.driverService.getDriver(this.driverId as number).subscribe(response=>this.driver=response);
  }
  viewDriver(){
    this.router.navigate(['viewDriver',this.driverId])
  }
  getDate(){
    return this.trip?.time.toString().substring(0,10);
  }
  getTime(){
    return this.trip?.time.toString().substring(11,16);
  }
}
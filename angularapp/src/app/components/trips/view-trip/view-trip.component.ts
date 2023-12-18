import {Component, OnInit} from '@angular/core';
import {TripService} from "../../../services/trip.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Subscription} from "rxjs";
import {Trip} from "../../../../shared/models/trip";
import {Review} from "../../../../shared/models/review";
import {ReviewService} from "../../../services/review.service";
import {DriverService} from "../../../services/driver.service";
import {Driver} from "../../../../shared/models/driver";
import {UserService} from "../../../services/user.service";

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
  constructor(public userService:UserService, private driverService:DriverService, private tripService:TripService, private reviewService:ReviewService, private router:Router, private route:ActivatedRoute) { }
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
  createReview(){
    this.router.navigate(['createReview',this.driverId,this.id])
  }
  editReview(){
    this.router.navigate(['editReview',this.driverId,this.id,this.getReview()?.id])
  }
  deleteReview(){
    this.reviewService.deleteReview(this.driverId as number, this.id as number, this.getReview()?.id as number).subscribe()
  }
  deleteReviewById(id:number){
    this.reviewService.deleteReview(this.driverId as number, this.id as number, id).subscribe()
  }
  hasReview(){
    if(this.userService.user.id===this.trip?.userId){
      return true;
    }
    return this.reviews?.some(review=>review.reviewer===this.userService.user.username)
  }
  isOwner(){
    return this.userService.user.id===this.trip?.userId
  }
  getReview(){
    return this.reviews?.find(review=>review.reviewer===this.userService.user.username)
  }
  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}

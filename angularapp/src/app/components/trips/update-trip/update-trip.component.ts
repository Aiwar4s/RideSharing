import {Component, OnInit} from '@angular/core';
import {Subscription} from "rxjs";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../../../auth.service";
import {TripService} from "../../../services/trip.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Trip} from "../../../../shared/models/trip";

@Component({
  selector: 'app-update-trip',
  templateUrl: './update-trip.component.html',
  styleUrls: ['./update-trip.component.css']
})
export class UpdateTripComponent implements OnInit{
  private sub?:Subscription;
  driverId?: number;
  tripId?: number;
  public tripForm!:FormGroup
  trip?: Trip
  constructor(private formBuilder:FormBuilder, private auth:AuthService, private tripService:TripService, private router:Router, private route:ActivatedRoute) { }
  ngOnInit(): void {
    this.sub=this.route.params.subscribe(params=>{
      this.driverId=params['driverId'];
      this.tripId=params['tripId'];
    });
    this.tripService.getTrip(this.driverId as number, this.tripId as number).subscribe(response=>this.trip=response)
    this.tripForm=this.formBuilder.group({
      time:['', Validators.required],
      seats: [0, Validators.required],
      description: ['', Validators.required]
    });
  }
  onSubmit() {
    if(this.tripForm.value.time===''){
      this.tripForm.patchValue({time:this.trip?.time})
    }
    if(this.tripForm.value.seats===0){
      this.tripForm.patchValue({seats:this.trip?.seats})
    }
    if(this.tripForm.value.description===null){
      this.tripForm.patchValue({description:this.trip?.description})
    }
    this.tripService.updateTrip(this.driverId as number, this.tripId as number, this.tripForm).subscribe()
    this.router.navigate(['viewDriver',this.driverId])
  }
}

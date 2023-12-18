import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../../../auth.service";
import {UserService} from "../../../services/user.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Subscription} from "rxjs";
import {TripService} from "../../../services/trip.service";

@Component({
  selector: 'app-add-trip',
  templateUrl: './add-trip.component.html',
  styleUrls: ['./add-trip.component.css']
})
export class AddTripComponent implements OnInit{
  private sub?:Subscription;
  driverId?: number;
  public tripForm!:FormGroup
  constructor(private formBuilder:FormBuilder, private auth:AuthService, private tripService:TripService, private router:Router, private route:ActivatedRoute) { }
  ngOnInit(): void {
    this.sub=this.route.params.subscribe(params=>{
      this.driverId=params['driverId'];
    });
    this.tripForm=this.formBuilder.group({
      departure: ['', Validators.required],
      destination: ['', Validators.required],
      time:['', Validators.required],
      seats: [0, Validators.required],
      description: ['', Validators.required]
    });
  }
  onSubmit() {
    this.tripService.createTrip(this.driverId as number, this.tripForm).subscribe()
    this.router.navigate(['viewDriver',this.driverId])
  }
}

import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Driver} from "../../../../shared/models/driver";
import {ReviewService} from "../../../services/review.service";
import {ActivatedRoute, Router} from "@angular/router";
import {DriverService} from "../../../services/driver.service";
import {Subscription} from "rxjs";
import {Trip} from "../../../../shared/models/trip";
import {DatePipe} from "@angular/common";

@Component({
  selector: 'app-update-driver',
  templateUrl: './update-driver.component.html',
  styleUrls: ['./update-driver.component.css']
})
export class UpdateDriverComponent implements OnInit{
  private sub?:Subscription;
  driverId?: number;
  driver?: Driver;
  public driverForm!: FormGroup;
  constructor(private formBuilder:FormBuilder, private driverService:DriverService, private router:Router, private route:ActivatedRoute, private date:DatePipe) { }
  ngOnInit(): void {
    this.sub=this.route.params.subscribe(params=>{
      this.driverId=params['id'];
    })
    this.driverService.getDriver(this.driverId as number)
      .subscribe(response=>this.driver=response);
    this.driverForm=this.formBuilder.group({
      email: [this.driver?.email, Validators.required],
      phoneNumber: [this.driver?.phoneNumber, Validators.required],
    })
  }
  onSubmit(){
    if(this.driverForm.value.email===null){
      this.driverForm.patchValue({email:this.driver?.email})
    }
    if(this.driverForm.value.phoneNumber===null){
      this.driverForm.patchValue({phoneNumber:this.driver?.phoneNumber})
    }
    this.driverService.updateDriver(this.driverId as number, {
      email: this.driverForm.value.email,
      phoneNumber: this.driverForm.value.phoneNumber,
      isVerified: true
    }).subscribe()
    this.router.navigate(['drivers'])
  }
}

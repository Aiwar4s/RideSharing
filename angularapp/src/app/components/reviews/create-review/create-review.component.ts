import {Component, OnInit} from '@angular/core';
import {ReviewService} from "../../../services/review.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Subscription} from "rxjs";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-create-review',
  templateUrl: './create-review.component.html',
  styleUrls: ['./create-review.component.css']
})
export class CreateReviewComponent implements OnInit{
  private sub?:Subscription;
  driverId?: number;
  tripId?: number;
  public reviewForm!: FormGroup;
  constructor(private formBuilder:FormBuilder, private reviewService:ReviewService, private router:Router, private route:ActivatedRoute) { }
  ngOnInit(): void {
    this.sub=this.route.params.subscribe(params=>{
      this.driverId=params['driverId'];
      this.tripId=params['tripId'];
    });
    this.reviewForm=this.formBuilder.group({
      rating: [0, Validators.required],
      description: ['', Validators.required]
    });
  }
  createReview(){
    this.reviewService.createReview(this.driverId as number, this.tripId as number, this.reviewForm).subscribe()
    this.router.navigate(['viewTrip',this.driverId,this.tripId])
  }
}

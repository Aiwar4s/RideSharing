import {Component, OnInit} from '@angular/core';
import {Subscription} from "rxjs";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ReviewService} from "../../../services/review.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Review} from "../../../../shared/models/review";

@Component({
  selector: 'app-edit-review',
  templateUrl: './edit-review.component.html',
  styleUrls: ['./edit-review.component.css']
})
export class EditReviewComponent implements OnInit{
  private sub?:Subscription;
  driverId?: number;
  tripId?: number;
  reviewId?: number;
  review?: Review;
  public reviewForm!: FormGroup;
  ratingChanged:boolean=false;
  descriptionChanged:boolean=false;
  constructor(private formBuilder:FormBuilder, private reviewService:ReviewService, private router:Router, private route:ActivatedRoute) { }
  ngOnInit(): void {
    this.sub=this.route.params.subscribe(params=>{
      this.driverId=params['driverId'];
      this.tripId=params['tripId'];
      this.reviewId=params['reviewId'];
    });
    this.reviewService.getReview(this.driverId as number, this.tripId as number, this.reviewId as number)
      .subscribe(response=>this.review=response);
    this.reviewForm=this.formBuilder.group({
      rating: [this.review?.rating, Validators.required],
      description: [this.review?.description]
    });
    this.reviewForm.setValue(this.reviewForm.value)
  }
  updateReview(){
    if(!this.ratingChanged){
      this.reviewForm.setValue({rating:this.review?.rating, description:this.reviewForm.value.description})
    }
    if(!this.descriptionChanged){
      this.reviewForm.setValue({rating:this.reviewForm.value.rating, description:this.review?.description})
    }
    this.reviewService.editReview(this.driverId as number, this.tripId as number, this.reviewId as number, this.reviewForm).subscribe()
    this.router.navigate(['viewTrip',this.driverId,this.tripId])
  }
  ratingChange(){
    this.ratingChanged=true;
  }
  descriptionChange(){
    this.descriptionChanged=true;
  }
}

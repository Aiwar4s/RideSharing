import { Injectable } from '@angular/core';
import {GlobalComponent} from "../../shared/global-component";
import {HttpClient} from "@angular/common/http";
import {Review} from "../../shared/models/review";
import {FormGroup} from "@angular/forms";

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private api= GlobalComponent.apiURL;
  constructor(private httpClient:HttpClient) { }
  getReviews(driverId:number, tripId:number){
    return this.httpClient.get<Review[]>(`${this.api}drivers/${driverId}/trips/${tripId}/reviews`)
  }
  getReview(driverId:number, tripId:number, reviewId:number){
    return this.httpClient.get<Review>(`${this.api}drivers/${driverId}/trips/${tripId}/reviews/${reviewId}`)
  }
  createReview(driverId:number, tripId:number, review:FormGroup){
    return this.httpClient.post<Review>(`${this.api}drivers/${driverId}/trips/${tripId}/reviews`, review.value)
  }
  editReview(driverId:number, tripId:number, reviewId:number, review:FormGroup){
    return this.httpClient.put<Review>(`${this.api}drivers/${driverId}/trips/${tripId}/reviews/${reviewId}`, review.value)
  }
  deleteReview(driverId:number, tripId:number, reviewId:number){
    return this.httpClient.delete<Review>(`${this.api}drivers/${driverId}/trips/${tripId}/reviews/${reviewId}`)
  }
}

import { Injectable } from '@angular/core';
import {GlobalComponent} from "../../shared/global-component";
import {HttpClient} from "@angular/common/http";
import {Review} from "../../shared/models/review";

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private api= GlobalComponent.apiURL;
  constructor(private httpClient:HttpClient) { }
  getReviews(driverId:number, tripId:number){
    return this.httpClient.get<Review[]>(`${this.api}drivers/${driverId}/trips/${tripId}/reviews`)
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GlobalComponent} from "../../shared/global-component";
import {Trip} from "../../shared/models/trip";

@Injectable({
  providedIn: 'root'
})
export class TripService {
  private api= GlobalComponent.apiURL;
  constructor(private httpClient:HttpClient) { }
  getTrips(driverId:number){
    return this.httpClient.get<any>(`${this.api}drivers/${driverId}/trips`)
  }
  getTrip(driverId:number,tripId:number){
    return this.httpClient.get<Trip>(`${this.api}drivers/${driverId}/trips/${tripId}`)
  }
}

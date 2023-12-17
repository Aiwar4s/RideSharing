import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GlobalComponent} from "../../shared/global-component";
import {Driver} from "../../shared/models/driver";

@Injectable({
  providedIn: 'root'
})
export class DriverService {
  private api= `${GlobalComponent.apiURL}drivers`;
  constructor(private httpClient:HttpClient) { }
  getDrivers(){
    return this.httpClient.get<Driver[]>(this.api)
  }
  getDriver(id:number){
    return this.httpClient.get<Driver>(`${this.api}/${id}`)
  }
}

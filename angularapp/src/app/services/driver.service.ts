import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GlobalComponent} from "../../shared/global-component";
import {Driver} from "../../shared/models/driver";
import {Observable} from "rxjs";

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
  createDriver(driver:any){
    let result: Observable<Driver>
    result=this.httpClient.post<Driver>(this.api, driver)
    return result
  }
  updateDriver(id:number, driver:any){
    return this.httpClient.put(`${this.api}/${id}`, driver)
  }
  deleteDriver(id:number){
    return this.httpClient.delete(`${this.api}/${id}`)
  }
}

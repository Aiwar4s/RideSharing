import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Driver } from 'src/shared/models/driver';
import {Router} from "@angular/router";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  drivers:Driver[]=[
    // new Driver(1, 'James Man', 'email@email.com', '+37060000000', 'abc'),
    // new Driver(2, 'James Man', 'email1@email.com', '+37060000001', 'abc')
  ]

  public forecasts?: WeatherForecast[];

  constructor(http: HttpClient, private router: Router) {}

  title = 'angularapp';

  login() {
    this.router.navigate(['login']);
  }
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

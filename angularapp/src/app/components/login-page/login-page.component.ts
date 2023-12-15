import { Component } from '@angular/core';
import {FormGroup, FormBuilder, Validators} from "@angular/forms";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {GlobalComponent} from "../../../shared/global-component";

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent {
  public loginForm!: FormGroup;
  constructor(private formBuilder: FormBuilder, private router: Router, private http: HttpClient) {}
  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      userName: ['', Validators.required],
      password: ['', Validators.required]
    });
  }
  login(){
    this.http.post<any>(GlobalComponent.apiURL.concat('login'), this.loginForm.value)
      .subscribe(res => {
        alert('You have successfully logged in!');
        this.loginForm.reset();
        this.router.navigate(['home']);
      }, err => {
        alert('An error has occurred while logging in')
      });
  }
}

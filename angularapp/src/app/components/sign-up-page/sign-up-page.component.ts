import { Component } from '@angular/core';
import {FormGroup, FormBuilder} from "@angular/forms";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {GlobalComponent} from "../../../shared/global-component";

@Component({
  selector: 'app-sign-up-page',
  templateUrl: './sign-up-page.component.html',
  styleUrls: ['./sign-up-page.component.css']
})
export class SignUpPageComponent {
  public signUpForm!: FormGroup;
  constructor(private formBuilder: FormBuilder, private router: Router, private http: HttpClient) {}
  ngOnInit(): void {
    this.signUpForm = this.formBuilder.group({
      email: [''],
      userName: [''],
      password: ['']
    });
  }
  signUp(){
    this.http.post<any>(GlobalComponent.apiURL.concat('register'), this.signUpForm.value)
      .subscribe(res => {
      alert('You have successfully signed up!');
      this.signUpForm.reset();
      this.router.navigate(['login']);
    }, err => {
      alert('An error has occurred while signing up')
    });
  }
}

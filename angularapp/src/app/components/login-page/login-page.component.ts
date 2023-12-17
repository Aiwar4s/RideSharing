import { Component } from '@angular/core';
import {FormGroup, FormBuilder, Validators} from "@angular/forms";
import {HttpClient} from "@angular/common/http";
import {Router} from "@angular/router";
import {AuthService} from "../../auth.service";

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent {
  public loginForm!: FormGroup;
  constructor(private readonly authService: AuthService, private formBuilder: FormBuilder, private router: Router, private http: HttpClient,
              ) {}
  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }
  onSubmit(){
    this.authService.login(this.loginForm).subscribe(
      ()=>{
        if(this.authService.isAuthenticated()){
          this.router.navigate(['home'])
        }
      },
      (error)=>{
        console.log(error)
      }
    )
  }
}

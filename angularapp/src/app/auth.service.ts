import { Injectable} from '@angular/core';
import {JwtHelperService} from "@auth0/angular-jwt";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {tap} from "rxjs/operators";
import {FormGroup} from "@angular/forms";
import {GlobalComponent} from "../shared/global-component";
import {jwtDecode} from "jwt-decode";
import {UserService} from "./services/user.service";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = GlobalComponent.apiURL;
  constructor(
    private readonly jwtHelper: JwtHelperService,
    private readonly router: Router,
    private readonly http: HttpClient,
    private readonly userService: UserService
  ) { }
  login(loginForm:FormGroup) {
    return this.http.post<LoginResponse>(`${this.apiUrl}login`, loginForm.value)
      .pipe(
        tap(res => {
          localStorage.setItem('access_token', res.accessToken);
          localStorage.setItem('refresh_token', res.refreshToken);
          this.userService.setUser()
        })
      );
  }
  logout(){
    localStorage.removeItem('access_token')
    localStorage.removeItem('refresh_token')
    this.userService.deleteUser()
    this.router.navigate(['home'])
  }
  refresh(token: string){
    if(localStorage.getItem('refresh_token')!==null){
      return this.http.post<LoginResponse>(`${this.apiUrl}accesstoken`, {'refreshToken': token})
        .subscribe(res=>{
          localStorage.setItem('access_token', res.accessToken);
          localStorage.setItem('refresh_token', res.refreshToken);
        })
    }
    return null
  }
  isAuthenticated(){
    const token=localStorage.getItem('refresh_token')
    if (this.jwtHelper.isTokenExpired(localStorage.getItem('access_token'))) {
      this.refresh(localStorage.getItem('refresh_token') as string)
    }
    return !this.jwtHelper.isTokenExpired(token)
  }
  updateUser(){
    this.refresh(localStorage.getItem('refresh_token') as string)
    this.userService.setUser()
}

}
interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  userId: string;
}

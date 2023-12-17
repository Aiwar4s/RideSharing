import {Injectable} from "@angular/core";
import {CanActivate, Router} from "@angular/router";
import {AuthService} from "./auth.service";
import {inject} from "@angular/core/testing";

@Injectable({
  providedIn: 'root'
})

export class AuthGuard implements CanActivate{
  constructor(private readonly authService: AuthService, private readonly router: Router) {
  }
  canActivate(): boolean{
    if(!this.authService.isAuthenticated()){
      this.router.navigate(['login'])
      return false
    }
    return true
  }
}

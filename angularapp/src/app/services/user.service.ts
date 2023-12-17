import { Injectable} from '@angular/core';
import {User} from "../../shared/models/user";
import {AuthService} from "../auth.service";

@Injectable({
  providedIn: 'root'
})
export class UserService{
  public user: User = new User()
  constructor() {
    try{
      this.setUser()
    }
    catch (e){
      this.deleteUser()
    }
  }
  setUser(){
    console.log('setUser')
    let data = JSON.parse(window.atob((localStorage.getItem('access_token') as string).split('.')[1]))
    let role = data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    this.user.id=data['sub']
    this.user.username=data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
    console.log(role)
    if((role as string).includes('Admin')){
      this.user.role = 3
    }
    else if((role as string).includes('Driver')){
      this.user.role = 2
    }
    else{
      this.user.role = 1
    }
  }
  deleteUser(){
    this.user=new User()
  }
  getUserInfo(){
    const token=localStorage.getItem('access_token')
    return JSON.parse(window.atob((token as string).split('.')[1])) //jwtDecode(token as string)
  }
}

import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../../../services/user.service";
import {DriverService} from "../../../services/driver.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-create-driver',
  templateUrl: './create-driver.component.html',
  styleUrls: ['./create-driver.component.css']
})
export class CreateDriverComponent implements OnInit {
  public driverForm!: FormGroup;
  constructor(private formBuilder:FormBuilder, private userService:UserService, private driverService:DriverService, private router:Router) { }
  ngOnInit(): void {
    this.driverForm=this.formBuilder.group({
      firstName:['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', Validators.required],
      phoneNumber: ['', Validators.required],
    });
    }
  onSubmit() {
    this.driverService.createDriver({
      name: `${this.driverForm.value.firstName} ${this.driverForm.value.lastName}`,
      email: this.driverForm.value.email,
      phoneNumber: this.driverForm.value.phoneNumber,
      userId: this.userService.user.id
    }).subscribe()
    this.router.navigate(['home'])
  }
}


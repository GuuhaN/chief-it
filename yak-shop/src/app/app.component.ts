import { Component } from '@angular/core';
import {YakshopService} from './services/yakshop.service';
import { Jsonresult } from './models/JsonResult';
import { Herd } from './models/herd';
import { Stock } from './models/stock';
import { Yak } from './models/Yak';
import { OrderItem } from './models/OrderItem';
import { CustomerOrder } from './models/CustomerOrder';
import {FormBuilder, FormGroup, Validators, FormControl} from '@angular/forms';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent {

  title = 'Yak Shop';
  day: number = 0;
  currentHerd: Yak[];
  currentMilk: number;
  currentSkins: number;

  newOrderMilk: string;
  newOrderSkins: string;
  constructor(private yakshopservice: YakshopService, private _formBuilder: FormBuilder) { }
  customerOrderForm: FormGroup;

  ngOnInit() {
    this.customerOrderForm = this._formBuilder.group({
      customer: new FormControl("Cigel Nheung", Validators.required),
      milk: new FormControl(100, Validators.required),
      skins: new FormControl(1, Validators.required)
    });

    this.updateDay();
  }

  updateDay() {
    this.day = parseInt(localStorage.getItem("currentDay"));
    this.day++;
    localStorage.setItem("currentDay", this.day.toString());
    this.getStock();
    this.getHerdDays();
  }

  getHerd() {
    var yaks: Yak[] = [{
      id: 0,
      name: "Yak-1",
      age: 4,
      sex: "FEMALE",
    ageLastShaved: 0}, {
      id: 0,
      name: "Yak-2",
      age: 8,
      sex: "MALE",
      ageLastShaved: 0},
      {
        id: 0,
        name: "Yak-3",
        age: 9.5,
        sex: "FEMALE",
        ageLastShaved: 0}
    ];
    var herd = 
      {
        id: 0,
        yaks: yaks
      };

    this.yakshopservice.getHerd(herd).subscribe((data: Herd) => {
      localStorage.setItem("currentDay", "0");
    })
  }

  postOrder(){
    var customerOrder: OrderItem = {
      id: 0,
      milk: this.customerOrderForm.controls['milk'].value,
      skins: this.customerOrderForm.controls['skins'].value
    }
    var order = {
      id: 0,
      customer: this.customerOrderForm.controls['customer'].value,
      order: customerOrder  
    };
    this.yakshopservice.postOrder(this.day, order).subscribe((data: CustomerOrder) => {
      if(data.order.milk === undefined)
        this.newOrderMilk = "OUT OF STOCK";
      else 
        this.newOrderMilk = data.order.milk.toString();
      if(data.order.skins === undefined)
        this.newOrderSkins = "OUT OF STOCK";
      else
        this.newOrderSkins = data.order.skins.toString();
    }, error => {
      this.newOrderMilk = "OUT OF STOCK";
      this.newOrderSkins = "OUT OF STOCK";
      console.log("error here", error.error);
    });
  }

  getStock() {
    this.yakshopservice.getStock(this.day).subscribe((data: Stock) => {
      var currentStock = data;
      this.currentMilk = currentStock.milk;
      this.currentSkins = currentStock.skins;
    })
  }

  getHerdDays() {
    this.yakshopservice.getHerdDays(this.day).subscribe((data: Herd) => {
      this.currentHerd = data[0].yaks;
      console.log(data[0].yaks);
    })
  }
}

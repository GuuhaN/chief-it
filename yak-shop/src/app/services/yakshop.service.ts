import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Jsonresult } from '../models/JsonResult';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Herd } from '../models/herd';
import { Stock } from '../models/stock';
import { CustomerOrder } from '../models/CustomerOrder';

@Injectable({
  providedIn: 'root'
})
export class YakshopService {

  constructor(private httpClient: HttpClient) { 

  }

  getHerd(herd: Herd): Observable<Herd> {
    return this.httpClient.post("https://localhost:44317/yak-shop/load", herd).pipe(map((response: Herd) => response));
  }

  postOrder(day:number, customerOrder: CustomerOrder): Observable<CustomerOrder> {
    return this.httpClient.post("https://localhost:44317/yak-shop/order/" + day, customerOrder).pipe(map((response: CustomerOrder) => response));
  }

  getStock(day: number): Observable<Stock> {
    return this.httpClient.get("https://localhost:44317/yak-shop/stock/" + day).pipe(map((response: Stock) => response));
  }

  getHerdDays(day: number) : Observable<Herd> {
    return this.httpClient.get("https://localhost:44317/yak-shop/herd/" + day).pipe(map((response: Herd) => response));
  }
}

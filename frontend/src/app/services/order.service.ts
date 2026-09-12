import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { CreateOrder, Order } from '../models/order';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7136/api/Order';

  createOrder(order: CreateOrder) {
    return this.http.post<Order>(this.apiUrl, order);
  }

  getMyOrders() {
    return this.http.get<Order[]>(this.apiUrl);
  }

  getOrder(orderId: number) {
    return this.http.get<Order>(`${this.apiUrl}/${orderId}`);
  }
}

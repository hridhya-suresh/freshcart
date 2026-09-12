import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';
import { Cart } from '../models/cart';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  private http = inject(HttpClient);
  private readonly cartChanged = new Subject<void>();

  readonly cartChanged$ = this.cartChanged.asObservable();

  private apiUrl = 'https://localhost:7136/api/Cart';

  refreshCart(): void {
    this.cartChanged.next();
  }

  addToCart(productId: number, quantity: number = 1) {
    return this.http.post(this.apiUrl, {
      productId,
      quantity
    });
  }

  getCart() {
    return this.http.get<Cart[]>(this.apiUrl);
  }

  updateQuantity(cartItemId: number, quantity: number) {
    return this.http.put(
      `${this.apiUrl}/${cartItemId}`,
      { quantity }
    );
  }

  removeItem(cartItemId: number) {
    return this.http.delete(
      `${this.apiUrl}/${cartItemId}`
    );
  }

}

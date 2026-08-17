import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CartService } from '../../services/cart.service';
import { Cart } from '../../models/cart';
import { ToastrService } from 'ngx-toastr';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.html',
  styleUrl: './cart.scss',
})
export class CartComponent implements OnInit {
  cartItems: Cart[] = [];
  showRemoveDialog = false;
  selectedCartItemId!: number;

  constructor(
    private cartService: CartService,
    private toastr: ToastrService,
  ) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart() {
    this.toastr.info('Loading cart items...');
    this.cartService.getCart().subscribe({
      next: (data) => {
        this.cartItems = data;
      },
      error: (err) => {
        if (err.status === 401) {
          this.cartItems = [];
        }
      },
    });
  }

  confirmRemove(cartItemId: number) {
    this.selectedCartItemId = cartItemId;
    this.showRemoveDialog = true;
  }

  removeCartItem() {
    this.cartService.removeItem(this.selectedCartItemId).subscribe({
      next: () => {
        this.showRemoveDialog = false;
        this.loadCart();
      },
    });
  }
}

import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter, Subscription } from 'rxjs';

import { AuthService } from '../../services/auth.service';
import { Cart } from '../../models/cart';
import { CartService } from '../../services/cart.service';
import { ConfirmDialog } from '../../shared/components/confirm-dialog/confirm-dialog';
import { EmptyState } from '../../shared/components/empty-state/empty-state';
@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, ConfirmDialog, EmptyState],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent implements OnInit, OnDestroy {
  cartItems: Cart[] = [];
  showRemoveDialog = false;
  selectedCartItemId!: number;
  private subscriptions = new Subscription();

  constructor(
    public authService: AuthService,
    private router: Router,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.loadCart();

    this.subscriptions.add(
      this.router.events
        .pipe(filter(event => event instanceof NavigationEnd))
        .subscribe(() => this.loadCart())
    );

    this.subscriptions.add(
      this.cartService.cartChanged$.subscribe(() => this.loadCart())
    );
  }

  
loadCart(): void {

  if (!this.authService.isLoggedIn()) {
    this.cartItems = [];
    return;
  }

  this.cartService.getCart().subscribe({
    next: (data) => {
      this.cartItems = data;
      console.log('Cart items loaded:', this.cartItems);
    },
    error: (err) => {
      console.error('Error loading cart:', err);

      if (err.status === 401) {
        this.cartItems = [];
        this.authService.logout();
      }
    }
  });
}
  
  logout() {
    this.authService.logout();
    this.cartItems = [];
    this.router.navigate(['/']);
  }
get grandTotal(): number {

  return this.cartItems.reduce(
    (sum, item) => sum + item.subtotal,
    
    0
  );
  
}

increase(item: Cart): void {

  this.cartService
  .updateQuantity(item.cartItemId, item.quantity + 1)
      .subscribe({

        next: () => {

          item.quantity++;

          item.subtotal = item.price * item.quantity;

        },
        
        error: err => console.log(err)

      });

}

decrease(item: Cart): void {

  // If quantity is 1, remove the item
  if (item.quantity === 1) {

    this.remove();

    return;
    
  }
  
  this.cartService
  .updateQuantity(item.cartItemId, item.quantity - 1)
  .subscribe({

        next: () => {

          item.quantity--;

          item.subtotal = item.price * item.quantity;

        },
        
        error: err => console.log(err)
        
      });
      
}
confirmRemove(cartItemId: number) {
  this.selectedCartItemId = cartItemId;
  this.showRemoveDialog = true;
}

remove(): void {
  
  this.cartService
      .removeItem(this.selectedCartItemId)
      .subscribe({

        next: () => {

          this.cartItems = this.cartItems.filter(x => x.cartItemId !== this.selectedCartItemId);
          this.showRemoveDialog = false;
          this.loadCart();

        },
        
        error: err => console.log(err)
        
      });

}
ngOnDestroy(): void {
  this.subscriptions.unsubscribe();
}
}
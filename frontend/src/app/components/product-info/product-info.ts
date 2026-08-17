import { Component } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { Router } from '@angular/router';
import { Input } from '@angular/core';
import { Product } from '../../models/product';
import { QuantitySelector } from '../quantity-selector/quantity-selector';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-product-info',
  standalone: true,
  imports: [
    QuantitySelector
  ],
  templateUrl: './product-info.html',
  styleUrl: './product-info.scss',
})

export class ProductInfo {
@Input() product!: Product;
constructor(
    private cartService: CartService,
    private router: Router,
    private toastr: ToastrService
) {}
quantity = 1;

addToCart() {

 this.cartService.addToCart(
  this.product.productId,
  this.quantity
).subscribe({

next: () => {

  this.cartService.refreshCart();

  this.toastr.success(
    'Product added to cart',
    'Success'
  );

}

});

}
}

import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';

@Component({
  selector: 'app-related-products',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './related-products.html',
  styleUrl: './related-products.scss'
})
export class RelatedProducts implements OnChanges {

  @Input() productId!: number;

  products: Product[] = [];

  constructor(private productService: ProductService) {}

  ngOnChanges(changes: SimpleChanges): void {

    if (changes['productId'] && this.productId) {

      this.productService
        .getRelatedProducts(this.productId)
        .subscribe(res => {

          this.products = res;

        });

    }

  }

}
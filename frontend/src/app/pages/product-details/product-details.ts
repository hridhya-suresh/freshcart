import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';
import { CommonModule } from '@angular/common';
import { ProductGallery } from '../../components/product-gallery/product-gallery';
import { ProductInfo } from '../../components/product-info/product-info';
import { RelatedProducts } from '../../components/related-products/related-products';
  

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [
    CommonModule,
    ProductGallery,
    ProductInfo,
    RelatedProducts   
  ],
  templateUrl: './product-details.html',
  styleUrl: './product-details.scss',
})
export class ProductDetails implements OnInit {

  product!: Product;

  constructor(
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

ngOnInit(): void {

  this.route.paramMap.subscribe(params => {

    const id = Number(params.get('id'));

    if (id) {
      this.loadProduct(id);
    }

  });

}

  loadProduct(id: number): void {
    this.productService.getProduct(id).subscribe({
      next: (res) => {
        this.product = res;
      },
      error: (err) => {
        console.error('Error loading product:', err);
      }
    });
  }
}
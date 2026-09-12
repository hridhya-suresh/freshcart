import { CommonModule, isPlatformBrowser } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  OnDestroy,
  OnInit,
  PLATFORM_ID
} from '@angular/core';
import { RouterLink ,Router} from '@angular/router';

import {
  destroyFreshcartSliders,
  initFreshcartSliders,
} from '../../shared/freshcart-sliders';

import { CategoryService } from '../../services/category.service';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-home1',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home1.html',
  styleUrl: './home1.scss',
})
export class Home1 implements OnInit, AfterViewInit, OnDestroy {

  private readonly platformId = inject(PLATFORM_ID);
  private readonly elementRef = inject(ElementRef<HTMLElement>);

  categories: any[] = [];
  products: any[] = [];
  filteredProducts: any[] = [];
  selectedCategoryId = 0;
  constructor(
    private categoryService: CategoryService,
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService

  ) {}

  ngOnInit(): void {

    this.categoryService.getCategories()
      .subscribe({
        next: (data) => {

          this.categories = data;

          console.log('Categories:', this.categories);

          if (isPlatformBrowser(this.platformId)) {

            setTimeout(() => {

              destroyFreshcartSliders(this.elementRef.nativeElement);

              initFreshcartSliders(this.elementRef.nativeElement);

            }, 200);

          }

        }
      });

    this.productService.getProducts()
    .subscribe(data => {
      this.products = data;
      this.filterProducts(0);

      console.log(this.products);
    });
  }
   filterProducts(categoryId: number): void {
  this.selectedCategoryId = categoryId;

    if (categoryId === 0) {
      this.filteredProducts = this.products;

      return;
    }

    this.filteredProducts = this.products.filter(
      p => p.categoryId === categoryId
    );
  }


 addToCart(productId: number) {

  if (!this.authService.isLoggedIn()) {

    this.router.navigate(['/signin']);

    return;

  }

 this.cartService
      .addToCart(productId)
      .subscribe({

        next: (res) => {

          console.log(res);

          this.toastr.success('Product added to cart');

          this.cartService.refreshCart();

        },

        error: (err) => {

          console.log(err);

          if (err.status === 401) {
            this.authService.logout();
            alert('Session expired. Please sign in again.');
            this.router.navigate(['/signin']);
          }

        }
          });
}


  ngAfterViewInit(): void {

    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    initFreshcartSliders(this.elementRef.nativeElement);

  }

  ngOnDestroy(): void {

    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    destroyFreshcartSliders(this.elementRef.nativeElement);

  }
}
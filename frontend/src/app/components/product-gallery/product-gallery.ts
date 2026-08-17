import { Component, Input } from '@angular/core';
import { Product } from '../../models/product';

@Component({
  selector: 'app-product-gallery',
  standalone: true,
  imports: [],
  templateUrl: './product-gallery.html',
  styleUrl: './product-gallery.scss'
})
export class ProductGallery {
  @Input() product!: Product;
}
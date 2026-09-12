import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private http = inject(HttpClient);

  private apiUrl =
    'https://localhost:7136/api/Products';

  getProducts() {
    return this.http.get<any[]>(this.apiUrl);
  }
  getProduct(id: number) {
  return this.http.get<Product>(
    `${this.apiUrl}/${id}`  
  );
}
getRelatedProducts(id: number) {
  return this.http.get<Product[]>(
    `${this.apiUrl}/${id}/related`
  );
}
}
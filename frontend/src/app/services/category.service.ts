import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private http = inject(HttpClient);

  private apiUrl =
    'https://localhost:7136/api/Categories';

  getCategories() {
    return this.http.get<any[]>(this.apiUrl);
  }
}
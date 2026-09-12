export interface Product {

  productId: number;

  productName: string;

  price: number;

  description?: string;

  imageUrl?: string;

  categoryId: number;

  categoryName: string;

  stock: number;

}
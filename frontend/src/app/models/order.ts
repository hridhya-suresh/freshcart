export interface CreateOrder {
  addressId: number;
  paymentMethod: string;
}

export interface OrderItem {
  productId: number;
  productName: string;
  imageUrl?: string;
  quantity: number;
  price: number;
  subTotal: number;
}
export interface OrderAddress {
  fullName: string;
  phone: string;
  addressLine1: string;
  addressLine2?: string;
  landmark?: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
}

export interface Order {
  orderId: number;
  orderDate: string;
  totalAmount: number;
  orderStatus: string;
  paymentStatus: string;
  paymentMethod: string;
  addressId: number;
  shippingAddress?: OrderAddress;
  items: OrderItem[];
}

import { Routes } from '@angular/router';
import { Home1 } from './pages/home1/home1';
import { Home2 } from './pages/home2/home2';
import { Home3 } from './pages/home3/home3';
import { Home4 } from './pages/home4/home4';
import { Signin } from './pages/signin/signin';
import { Signup } from './pages/signup/signup';
import { authGuard } from './guards/auth-guard';
import { CartComponent } from './pages/cart/cart';
import { ProductDetails } from './pages/product-details/product-details';
import { Checkout } from './pages/checkout/checkout';
import { OrderSuccess } from './pages/order-success/order-success';
import { Orders } from './pages/orders/orders';
import { OrderDetails } from './pages/order-details/order-details';
export const routes: Routes = [
  { path: '', component: Home1 },
  { path: 'home2', component: Home2 },
  { path: 'home3', component: Home3 },
  { path: 'home4', component: Home4 },
  { path: 'signin', component: Signin },
  { path: 'signup', component: Signup },
  { path: 'cart', component: CartComponent, canActivate: [authGuard] },
  { path: 'product/:id', component: ProductDetails },
  { path: 'checkout', component: Checkout, canActivate: [authGuard] },
  { path: 'order-success/:id', component: OrderSuccess },
  { path: 'orders', component: Orders, canActivate: [authGuard] },
  { path: 'order-details/:id', component: OrderDetails, canActivate: [authGuard] },
];

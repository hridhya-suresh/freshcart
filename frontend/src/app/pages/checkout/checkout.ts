import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { AddressService } from '../../services/address.service';
import { CartService } from '../../services/cart.service';

import { Address, CreateAddress } from '../../models/address';
import { Cart } from '../../models/cart';

import { Router } from '@angular/router';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss',
})
export class Checkout implements OnInit {
  private addressService = inject(AddressService);
  private cartService = inject(CartService);
  private orderService = inject(OrderService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  addresses: Address[] = [];

  cartItems: Cart[] = [];

  selectedAddressId: number | null = null;

  showAddressForm = false;

  addressForm!: FormGroup;
  isPlacingOrder = false;
  subtotal = 0;

  deliveryCharge = 0;

  total = 0;

  ngOnInit(): void {
    this.createAddressForm();

    this.loadAddresses();

    this.loadCart();
  }

  loadCart(): void {
    this.cartService.getCart().subscribe({
      next: (items) => {
        this.cartItems = items;

        this.calculateTotal();
      },
    });
  }

  calculateTotal(): void {
    this.subtotal = this.cartItems.reduce((total, item) => total + item.subtotal, 0);

    // Free delivery for now
    this.deliveryCharge = 0;

    this.total = this.subtotal + this.deliveryCharge;
  }

  createAddressForm(): void {
    this.addressForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.maxLength(100)]],

      phone: ['', [Validators.required, Validators.pattern(/^[6-9]\d{9}$/)]],

      addressLine1: ['', Validators.required],

      addressLine2: [''],

      landmark: [''],

      city: ['', Validators.required],

      state: ['', Validators.required],

      postalCode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],

      country: ['India', Validators.required],
    });
  }

  loadAddresses(): void {
    this.addressService.getAddresses().subscribe({
      next: (data) => {
        this.addresses = data;

        const defaultAddress = this.addresses.find((a) => a.isDefault);

        if (defaultAddress) {
          this.selectedAddressId = defaultAddress.addressId;
        }
      },
    });
  }

  selectAddress(addressId: number): void {
    this.selectedAddressId = addressId;
  }

  openAddressForm(): void {
    this.showAddressForm = true;

    this.addressForm.reset({
      country: 'India',
    });
  }

  closeAddressForm(): void {
    this.showAddressForm = false;

    this.addressForm.reset({
      country: 'India',
    });
  }

  saveAddress(): void {
    if (this.addressForm.invalid) {
      this.addressForm.markAllAsTouched();

      return;
    }

    const address: CreateAddress = this.addressForm.value;

    this.addressService.createAddress(address).subscribe({
      next: (newAddress) => {
        this.addresses.push(newAddress);

        this.selectedAddressId = newAddress.addressId;

        this.closeAddressForm();
      },
    });
  }

  placeOrder(): void {
    if (!this.selectedAddressId) {
      return;
    }

    if (this.cartItems.length === 0) {
      return;
    }

    this.isPlacingOrder = true;

    const order = {
      addressId: this.selectedAddressId,
      paymentMethod: 'CashOnDelivery',
    };

    this.orderService.createOrder(order).subscribe({
      next: (response) => {
        this.isPlacingOrder = false;

        this.router.navigate(['/order-success', response.orderId]);
      },

      error: () => {
        this.isPlacingOrder = false;
      },
    });
  }
}

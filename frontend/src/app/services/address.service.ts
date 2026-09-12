import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Address, CreateAddress } from '../models/address';

@Injectable({
  providedIn: 'root'
})
export class AddressService {

  private http = inject(HttpClient);

  private apiUrl = 'https://localhost:7136/api/Address';

  getAddresses() {
    return this.http.get<Address[]>(this.apiUrl);
  }

  getDefaultAddress() {
    return this.http.get<Address>(
      `${this.apiUrl}/default`
    );
  }

  createAddress(address: CreateAddress) {
    return this.http.post<Address>(
      this.apiUrl,
      address
    );
  }

  updateAddress(
    addressId: number,
    address: CreateAddress
  ) {
    return this.http.put<Address>(
      `${this.apiUrl}/${addressId}`,
      address
    );
  }

  deleteAddress(addressId: number) {
    return this.http.delete(
      `${this.apiUrl}/${addressId}`
    );
  }

  setDefaultAddress(addressId: number) {
    return this.http.put(
      `${this.apiUrl}/${addressId}/default`,
      {}
    );
  }
}
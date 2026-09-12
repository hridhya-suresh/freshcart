export interface Address {

    addressId: number;

    fullName: string;

    phone: string;

    addressLine1: string;

    addressLine2?: string;

    landmark?: string;

    city: string;

    state: string;

    postalCode: string;

    country: string;

    isDefault: boolean;
}
export interface CreateAddress {
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
import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-quantity-selector',
  standalone: true,
  imports: [],
  templateUrl: './quantity-selector.html',
  styleUrl: './quantity-selector.scss',
})
export class QuantitySelector {

  @Output()
  quantityChanged = new EventEmitter<number>();

  quantity = 1;

  increase(): void {
    this.quantity++;
    this.quantityChanged.emit(this.quantity);
  }

  decrease(): void {
    if (this.quantity > 1) {
      this.quantity--;
      this.quantityChanged.emit(this.quantity);
    }
  }
}
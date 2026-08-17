import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { OrderService } from '../../services/order.service';
import { Order } from '../../models/order';

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './order-details.html',
  styleUrl: './order-details.scss',
})
export class OrderDetails implements OnInit {
  private orderService = inject(OrderService);
  private route = inject(ActivatedRoute);

  order?: Order;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (id) {
      this.loadOrder(id);
    }
  }

  loadOrder(id: number): void {
    this.orderService.getOrder(id).subscribe({
      next: (data) => {
        this.order = data;
      },
    });
  }
}

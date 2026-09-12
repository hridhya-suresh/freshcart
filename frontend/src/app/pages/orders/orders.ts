import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';

import { OrderService } from '../../services/order.service';
import { OrderHubService } from '../../services/order-hub.service';
import { Order } from '../../models/order';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './orders.html',
  styleUrl: './orders.scss',
})
export class Orders implements OnInit, OnDestroy {
  private orderService = inject(OrderService);
  private orderHub = inject(OrderHubService);

  orders: Order[] = [];
  private statusSub?: Subscription;

  ngOnInit(): void {
    this.loadOrders();
    this.listenForLiveUpdates();
  }

  ngOnDestroy(): void {
    this.statusSub?.unsubscribe();
  }

  loadOrders(): void {
    this.orderService.getMyOrders().subscribe({
      next: (data) => {
        this.orders = data;
      },
    });
  }

  private listenForLiveUpdates(): void {
    // OnConnectedAsync on the hub already adds this user to user-{id}
    void this.orderHub.start();

    this.statusSub = this.orderHub.statusChanged$.subscribe((update) => {
      const order = this.orders.find((o) => o.orderId === update.orderId);

      if (!order) {
        return;
      }

      order.orderStatus = update.orderStatus;
      order.paymentStatus = update.paymentStatus;
    });
  }
}

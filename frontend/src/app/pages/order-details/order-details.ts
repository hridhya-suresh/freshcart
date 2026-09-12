import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { OrderService } from '../../services/order.service';
import { OrderHubService } from '../../services/order-hub.service';
import { Order } from '../../models/order';

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './order-details.html',
  styleUrl: './order-details.scss',
})
export class OrderDetails implements OnInit, OnDestroy {
  private orderService = inject(OrderService);
  private orderHub = inject(OrderHubService);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  order?: Order;
  private orderId = 0;
  private statusSub?: Subscription;

  ngOnInit(): void {
    this.orderId = Number(this.route.snapshot.paramMap.get('id'));

    if (!this.orderId) {
      return;
    }

    this.loadOrder(this.orderId);
    this.listenForLiveUpdates();
  }

  ngOnDestroy(): void {
    this.statusSub?.unsubscribe();

    if (this.orderId) {
      void this.orderHub.leaveOrderGroup(this.orderId);
    }
  }

  loadOrder(id: number): void {
    this.orderService.getOrder(id).subscribe({
      next: (data) => {
        this.order = data;
      },
    });
  }

  private listenForLiveUpdates(): void {
    this.statusSub = this.orderHub.statusChanged$.subscribe((update) => {
      if (!this.order || update.orderId !== this.order.orderId) {
        return;
      }

      this.order.orderStatus = update.orderStatus;
      this.order.paymentStatus = update.paymentStatus;

      this.toastr.info(
        `Payment: ${update.paymentStatus} · Order: ${update.orderStatus}`,
        'Order updated'
      );
    });

    void this.orderHub.joinOrderGroup(this.orderId);
  }
}

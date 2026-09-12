import { Injectable, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

import { AuthService } from './auth.service';

export interface OrderStatusUpdate {
  orderId: number;
  orderStatus: string;
  paymentStatus: string;
}

@Injectable({
  providedIn: 'root',
})
export class OrderHubService {
  private authService = inject(AuthService);

  private hubUrl = 'https://localhost:7136/orderHub';
  private connection?: signalR.HubConnection;
  private startPromise?: Promise<void>;

  private readonly statusChangedSubject = new Subject<OrderStatusUpdate>();
  readonly statusChanged$ = this.statusChangedSubject.asObservable();

  async start(): Promise<void> {
    const token = this.authService.getToken();
    if (!token) {
      return;
    }

    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    if (this.startPromise) {
      return this.startPromise;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => this.authService.getToken() ?? '',
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('OrderStatusChanged', (update: OrderStatusUpdate) => {
      this.statusChangedSubject.next(update);
    });

    this.startPromise = this.connection
      .start()
      .catch((error) => {
        console.error('SignalR connection failed', error);
        this.startPromise = undefined;
        throw error;
      });

    await this.startPromise;
  }

  async joinOrderGroup(orderId: number): Promise<void> {
    await this.start();

    if (this.connection?.state !== signalR.HubConnectionState.Connected) {
      return;
    }

    await this.connection.invoke('JoinOrderGroup', orderId);
  }

  async leaveOrderGroup(orderId: number): Promise<void> {
    if (this.connection?.state !== signalR.HubConnectionState.Connected) {
      return;
    }

    await this.connection.invoke('LeaveOrderGroup', orderId);
  }

  async stop(): Promise<void> {
    if (!this.connection) {
      return;
    }

    await this.connection.stop();
    this.connection = undefined;
    this.startPromise = undefined;
  }
}

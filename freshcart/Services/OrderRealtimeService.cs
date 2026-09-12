using freshcart.Hubs;
using freshcart.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace freshcart.Services
{
    public class OrderRealtimeService : IOrderRealtimeService
    {
        private readonly IHubContext<OrderHub> _orderHub;

        public OrderRealtimeService(IHubContext<OrderHub> orderHub)
        {
            _orderHub = orderHub;
        }

        public async Task NotifyOrderStatusChangedAsync(
            int orderId,
            int userId,
            string orderStatus,
            string paymentStatus)
        {
            var payload = new
            {
                orderId,
                orderStatus,
                paymentStatus
            };

            // Clients on the order-details page join order-{id}
            await _orderHub.Clients
                .Group($"order-{orderId}")
                .SendAsync("OrderStatusChanged", payload);

            // Clients that joined their user group (e.g. orders list) also get updates
            await _orderHub.Clients
                .Group($"user-{userId}")
                .SendAsync("OrderStatusChanged", payload);
        }
    }
}

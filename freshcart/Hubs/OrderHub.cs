using Microsoft.AspNetCore.SignalR;

namespace freshcart.Hubs
{
    public class OrderHub : Hub
    {
        public async Task JoinOrderGroup(int orderId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"order-{orderId}"
            );
        }

        public async Task LeaveOrderGroup(int orderId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"order-{orderId}"
            );
        }
    }
}
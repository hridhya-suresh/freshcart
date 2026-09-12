using System.Security.Claims;
using freshcart.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace freshcart.Hubs
{
    [Authorize]
    public class OrderHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public OrderHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    $"user-{userId.Value}"
                );
            }

            await base.OnConnectedAsync();
        }

        public async Task JoinOrderGroup(int orderId)
        {
            var userId = GetUserId();
            if (userId == null)
                throw new HubException("Unauthorized.");

            var ownsOrder = await _context.Orders
                .AnyAsync(o =>
                    o.OrderId == orderId &&
                    o.UserId == userId.Value);

            if (!ownsOrder)
                throw new HubException("Order not found.");

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

        private int? GetUserId()
        {
            var value = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            return int.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }
}

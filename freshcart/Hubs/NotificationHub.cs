using Microsoft.AspNetCore.SignalR;

namespace freshcart.Hubs
{
    /// <summary>
    /// Simple demo hub for learning SignalR broadcasts.
    /// Prefer OrderHub for real order/payment updates.
    /// </summary>
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}

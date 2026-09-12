
using Microsoft.AspNetCore.SignalR;

namespace freshcart.Hubs
{
    public class NotificationHub : Hub
        {
            public async Task SendMessage(string message)
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
        }
}


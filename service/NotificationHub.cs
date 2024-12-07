namespace service;

using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public async Task SendOrderNotification(string orderDetails)
    {
        // Broadcast message to all connected clients
        await Clients.All.SendAsync("ReceiveOrderNotification", orderDetails);
    }
}

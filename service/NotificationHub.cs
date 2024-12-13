namespace service;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;


public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Sid)?.Value;
        _logger.LogInformation($"User connected: {userId}");
        // Store userId in a dictionary or database if needed
        return base.OnConnectedAsync();
    }


    public async Task SendOrderNotification(string orderDetails)
    {
        // Broadcast message to all connected clients
        _logger.LogInformation($"Sending message to user oder- ");

        await Clients.All.SendAsync("ReceiveOrderNotification", orderDetails);
    }

    public async Task SendMessageToUser(string userId, string message)
    {
        try
        {
            _logger.LogInformation($"Sending message to user {userId}");
            await Clients.All.SendAsync("ReceiveCartNotification", message);
        }
        catch (System.Exception)
        {
            _logger.LogError($"Error sending message to user {userId}");
            throw;
        }
    }
}

using System;
using System.Collections.Generic;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
namespace service;

public class CartService
{
    private readonly CartRepository _cartRepository;
    private readonly NotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<CartService> _logger;

    public CartService(CartRepository cartRepository, NotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext, ILogger<CartService> logger)
    {
        _cartRepository = cartRepository;
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
        _logger = logger;
    }

    public void CreateCart(Guid account_id, Guid product_id, int quantity)
    {
        try
        {
            Guid cartId = _cartRepository.CreateCart(account_id, product_id, quantity);
            NotificationHandler(cartId, account_id).Wait();
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }


    public IEnumerable<CartInQueryResult> GetCartForFeed(Guid account_id)
    {
        try
        {
            return _cartRepository.GetListCart(account_id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public void UpdateCart(Guid cart_id, int quantity)
    {
        try
        {
            _cartRepository.UpdateCart(cart_id, quantity);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    public void DeleteCart(Guid cart_id)
    {
        try
        {
            _cartRepository.DeleteCart(cart_id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private async Task NotificationHandler(Guid cartId, Guid accountId)
    {
        try
        {
            var content = new NotificationContent
            {
                message = $"Cart #{cartId.ToString().Substring(0, 8)} added successfully!.",
                createdAt = DateTime.UtcNow,
                Id = cartId,
            };

            var notification = new Notification
            {
                content = JsonConvert.SerializeObject(content),
                created_at = DateTime.UtcNow,
                type = "CART_NOTIFICATION",
                account_id = accountId
            };

            Guid notificationId = await _notificationRepository.InsertNotification(notification);

            await _hubContext.Clients.User(accountId.ToString()).SendAsync("ReceiveCartNotification", new
            {
                NotificationId = notificationId,
                Message = content.message,
                CreatedAt = content.createdAt
            });

            _logger.LogInformation($"Notification sent to user {accountId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user");
            throw;
        }
    }
}

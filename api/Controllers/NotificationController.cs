using api.TransferModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service;
namespace library.Controllers;

public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/notifications")]
    public async Task<ResponseDto> ListNotification()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = await _notificationService.ListNotification()
        };
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    [Route("/api/notifications/{id}")]
    public async Task<ResponseDto> MarkNotificationAsRead(Guid id)
    {
        var response = await _notificationService.MarkNotificationAsRead(id);
        if (response)
        {
            HttpContext.Response.StatusCode = 200;
            return new ResponseDto()
            {
                MessageToClient = "Successfully marked as read",
                ResponseData = true
            };
        }
        else
        {
            HttpContext.Response.StatusCode = 500;
            return new ResponseDto()
            {
                MessageToClient = "Failed to mark as read",
                ResponseData = false
            };
        }
    }
}
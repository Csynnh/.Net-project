namespace service;
using infrastructure.DataModels;
using infrastructure.Repositories;
using System.Text.Json;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponseModel>> ListNotification();
    Task<bool> MarkNotificationAsRead(Guid id);
}

public class NotificationService : INotificationService
{
    private readonly NotificationRepository _notificationRepository;

    public NotificationService(NotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationResponseModel>> ListNotification()
    {
        var responses = await _notificationRepository.ListNotification();
        IEnumerable<NotificationResponseModel> notificationResponseModels = responses.Select(response => new NotificationResponseModel
        {
            id = response.id,
            content = JsonSerializer.Deserialize<NotificationContent>(response.content),
            created_at = response.created_at,
            is_read = response.is_read
        });
        return notificationResponseModels;
    }

    public async Task<bool> MarkNotificationAsRead(Guid id)
    {
        try
        {
            await _notificationRepository.MarkNotificationAsRead(id);
            return true;
        }
        catch (System.Exception e)
        {
            return false;
        }
    }
}
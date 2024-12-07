using Dapper;
using Npgsql;
using infrastructure.DataModels;

namespace infrastructure.Repositories;


public interface INotificationRepository
{
    Task<Guid> InsertNotification(Notification notification);
    Task<IEnumerable<NotificationQueryResponse>> ListNotification();
    Task MarkNotificationAsRead(Guid id);
}


public class NotificationRepository : INotificationRepository
{

    private readonly NpgsqlDataSource _dataSource;

    public NotificationRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Guid> InsertNotification(Notification notification)
    {
        var sql = $@"
            INSERT INTO DEV.NOTIFICATIONS (content, created_at)
            VALUES (@content::json, @created_at)
            RETURNING id
        ";
        using var conn = _dataSource.OpenConnection();
        var response = await conn.QuerySingleAsync<Guid>(sql, notification);
        return response;
    }

    public async Task<IEnumerable<NotificationQueryResponse>> ListNotification()
    {
        var sql = $@"
            SELECT * FROM DEV.NOTIFICATIONS
            ORDER BY created_at DESC
        ";
        using var conn = _dataSource.OpenConnection();
        var response = await conn.QueryAsync<NotificationQueryResponse>(sql);
        return response;
    }

    public async Task MarkNotificationAsRead(Guid id)
    {
        var sql = $@"
            UPDATE DEV.NOTIFICATIONS
            SET is_read = true
            WHERE id = @id
        ";
        using var conn = _dataSource.OpenConnection();
        var response = await conn.ExecuteAsync(sql, new { id });
    }
}
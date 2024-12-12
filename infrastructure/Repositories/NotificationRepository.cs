using Dapper;
using Npgsql;
using infrastructure.DataModels;

namespace infrastructure.Repositories;


public interface INotificationRepository
{
    Task<Guid> InsertNotification(Notification notification);
    Task<IEnumerable<NotificationQueryResponse>> ListNotification(string? type, Guid? accountId);
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
            INSERT INTO DEV.NOTIFICATIONS (content, created_at, type, account_id)
            VALUES (@content::json, @created_at, @type, @account_id)
            RETURNING id
        ";
        using var conn = _dataSource.OpenConnection();
        var response = await conn.QuerySingleAsync<Guid>(sql, notification);
        return response;
    }

    public async Task<IEnumerable<NotificationQueryResponse>> ListNotification(string? type = "ODER_NOTIFICATION", Guid? accountId = null)
    {
        var sql = @"
            SELECT * FROM DEV.NOTIFICATIONS
            WHERE type = @type
        ";
        if (accountId != Guid.Empty)
        {
            sql += $" AND account_id = '{accountId}'";
        }
        sql += " ORDER BY created_at DESC";

        using var conn = _dataSource.OpenConnection();
        var response = await conn.QueryAsync<NotificationQueryResponse>(sql, new { type });
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
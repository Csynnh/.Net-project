using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;
public class ContactHistoryRepository
{
    private NpgsqlDataSource _dataSource;

    public ContactHistoryRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<ContactHistory> GetAllContactHistories()
    {
        var sql = @"
SELECT id as Id, account_id as AccountId, contact_details as ContactDetails, 
       contact_date as ContactDate
FROM contact_history;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<ContactHistory>(sql);
        }
    }

    public ContactHistory CreateContactHistory(Guid accountId, string contactDetails)
    {
        var sql = @"
INSERT INTO contact_history (account_id, contact_details)
VALUES (@accountId, @contactDetails)
RETURNING id as Id, account_id as AccountId, contact_details as ContactDetails, contact_date as ContactDate;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<ContactHistory>(sql, new { accountId, contactDetails });
        }
    }

    public ContactHistory UpdateContactHistory(Guid contactHistoryId, string contactDetails)
    {
        var sql = @"
UPDATE contact_history
SET contact_details = @contactDetails
WHERE id = @contactHistoryId
RETURNING id as Id, account_id as AccountId, contact_details as ContactDetails, contact_date as ContactDate;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<ContactHistory>(sql, new { contactHistoryId, contactDetails });
        }
    }

    public bool DeleteContactHistory(Guid contactHistoryId)
    {
        var sql = @"DELETE FROM contact_history WHERE id = @contactHistoryId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { contactHistoryId }) == 1;
        }
    }
}
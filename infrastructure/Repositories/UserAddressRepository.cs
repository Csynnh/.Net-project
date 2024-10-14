using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;
public class UserAddressRepository
{
    private NpgsqlDataSource _dataSource;

    public UserAddressRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<UserAddress> GetAllUserAddresses()
    {
        var sql = @"
SELECT id as Id, account_id as AccountId, address as Address
FROM useraddresses;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<UserAddress>(sql);
        }
    }

    public UserAddress CreateUserAddress(Guid accountId, string address)
    {
        var sql = @"
INSERT INTO useraddresses (account_id, address)
VALUES (@accountId, @address)
RETURNING id as Id, account_id as AccountId, address as Address;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<UserAddress>(sql, new { accountId, address });
        }
    }

    public UserAddress UpdateUserAddress(Guid userAddressId, string address)
    {
        var sql = @"
UPDATE useraddresses
SET address = @address
WHERE id = @userAddressId
RETURNING id as Id, account_id as AccountId, address as Address;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<UserAddress>(sql, new { userAddressId, address });
        }
    }

    public bool DeleteUserAddress(Guid userAddressId)
    {
        var sql = @"DELETE FROM useraddresses WHERE id = @userAddressId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { userAddressId }) == 1;
        }
    }
}
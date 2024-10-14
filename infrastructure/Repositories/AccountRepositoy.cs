using Dapper;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class AccountRepository
{
    private NpgsqlDataSource _dataSource;

    public AccountRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<Account> GetAccountForFeed()
    {
        string sql = $@"
SELECT id as {nameof(AccountFeedQuery.id)},
       username as {nameof(AccountFeedQuery.username)},
       password as {nameof(AccountFeedQuery.password)},
       name as {nameof(AccountFeedQuery.name)},
       email as {nameof(AccountFeedQuery.email)},
       phone_number as {nameof(AccountFeedQuery.phone_number)},
       role as {nameof(AccountFeedQuery.role)}
FROM accounts;
";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Account>(sql);
        }
    }

    public Account CreateAccount(string username, string password, string name, string email, string phoneNumber, Role role)
    {
        var sql = $@"
INSERT INTO accounts (username, password, name, email, phone_number, role)
VALUES (@username, @password, @name, @email, @phoneNumber, @role)
RETURNING id as {nameof(Account.id)}, 
          username as {nameof(Account.username)}, 
          password as {nameof(Account.password)}, 
          name as {nameof(Account.name)}, 
          email as {nameof(Account.email)}, 
          phone_number as {nameof(Account.phone_number)}, 
          role as {nameof(Account.role)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Account>(sql, new { username, password, name, email, phoneNumber, role });
        }
    }

    public Account UpdateAccount(Guid accountId, string username, string password, string name, string email, string phoneNumber, Role role)
    {
        var sql = $@"
UPDATE accounts
SET username = @username, 
    password = @password, 
    name = @name, 
    email = @email, 
    phone_number = @phoneNumber, 
    role = @role
WHERE id = @accountId
RETURNING id as {nameof(Account.id)}, 
          username as {nameof(Account.username)}, 
          password as {nameof(Account.password)}, 
          name as {nameof(Account.name)}, 
          email as {nameof(Account.email)}, 
          phone_number as {nameof(Account.phone_number)}, 
          role as {nameof(Account.role)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Account>(sql, new { accountId, username, password, name, email, phoneNumber, role });
        }
    }

    public bool DeleteAccount(Guid accountId)
    {
        var sql = @"DELETE FROM accounts WHERE id = @accountId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { accountId }) == 1;
        }
    }

    public bool DoesAccountWithUsernameExist(string username)
    {
        var sql = @"SELECT COUNT(*) FROM accounts WHERE username = @username;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.ExecuteScalar<int>(sql, new { username }) == 1;
        }
    }
}

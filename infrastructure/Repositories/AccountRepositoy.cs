using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class AccountRepository
{
    private NpgsqlDataSource _dataSource;

    public AccountRepository(NpgsqlDataSource datasource)
    {
        _dataSource = datasource;
    }

    public IEnumerable<Account> GetAccounts()
    {
        string sql = $@"
SELECT id as {nameof(Account.id)},
       username  as {nameof(Account.username )},
       password  as {nameof(Account.password )},
       name as {nameof(Account.name)},
       email as {nameof(Account.email)},
       phone_number as {nameof(Account.phone_number)},
       address  as {nameof(Account.address )},
       role  as {nameof(Account.role )}
FROM accounts;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Account>(sql);
        }
    }

    public Account CreateAccount(string username , string password , string name, string email, string phone_number, string address , string role )
    {
        var sql = $@"
INSERT INTO accounts (username , password , name, email, phone_number, address , role ) 
VALUES (@username , @password , @name, @email, @phone_number, @address , @role )
RETURNING id as {nameof(Account.id)},
          username  as {nameof(Account.username )},
          password  as {nameof(Account.password )},
          name as {nameof(Account.name)},
          email as {nameof(Account.email)},
          phone_number as {nameof(Account.phone_number)},
          address  as {nameof(Account.address )},
          role  as {nameof(Account.role )};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Account>(sql, new { username , password , name, email, phone_number, address , role  });
        }
    }

    public Account UpdateAccount(int id, string username , string password , string name, string email, string phone_number, string address , string role )
    {
        var sql = $@"
UPDATE accounts 
SET username  = @username , password  = @password , name = @name, email = @email, phone_number = @phone_number, address  = @address , role  = @role 
WHERE id = @id
RETURNING id as {nameof(Account.id)},
          username  as {nameof(Account.username )},
          password  as {nameof(Account.password )},
          name as {nameof(Account.name)},
          email as {nameof(Account.email)},
          phone_number as {nameof(Account.phone_number)},
          address  as {nameof(Account.address )},
          role  as {nameof(Account.role )};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Account>(sql, new { id, username , password , name, email, phone_number, address , role  });
        }
    }

    public bool DeleteAccount(int id)
    {
        var sql = @"DELETE FROM accounts WHERE id = @id;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { id }) == 1;
        }
    }

    public bool DoesAccountWithUsernameExist(string username )
    {
        var sql = @"SELECT COUNT(*) FROM accounts WHERE username  = @username ;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.ExecuteScalar<int>(sql, new { username  }) == 1;
        }
    }
}

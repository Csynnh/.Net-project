using infrastructure.DataModels;
using Npgsql;

public interface IUserRepository
{
  Task<User?> GetUserByUsernameAsync(string username);
  Task<User?> GetUserByAccountIdAsync(Guid accountId);
  Task CreateAccountAsync(Account account);
  Task<User?> GetUserByEmailAsync(string email);
  Task UpdatePasswordAsync(User account);
}

namespace infrastructure.Repositories
{
  public class UserRepository : IUserRepository
  {
    private readonly NpgsqlDataSource _dataSource;

    public UserRepository(NpgsqlDataSource dataSource)
    {
      _dataSource = dataSource;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
      await using var conn = await _dataSource.OpenConnectionAsync();
      await using var cmd = new NpgsqlCommand("""
        SELECT id, username, password, role, email, phone_number, name
        FROM DEV.ACCOUNTS
        WHERE username = @Username;
      """, conn);

      cmd.Parameters.AddWithValue("Username", username);
      await using var reader = await cmd.ExecuteReaderAsync();
      if (await reader.ReadAsync())
      {
        return new User
        {
          Id = reader.GetGuid(0),
          Username = reader.GetString(1),
          PasswordHash = reader.GetString(2),
          Role = reader.GetString(3),
          Email = reader.GetString(4),
          PhoneNumber = reader.GetString(5),
          Name = reader.GetString(6)
        };
      }
      return null;
    }

    public async Task<User?> GetUserByAccountIdAsync(Guid accountId)
    {
      try
      {
        await using var conn = await _dataSource.OpenConnectionAsync();
        var cmd = new NpgsqlCommand(@"
                SELECT username, password, role
                FROM DEV.ACCOUNTS
                WHERE id = @AccountId;
            ", conn);

        cmd.Parameters.AddWithValue("AccountId", accountId);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
          return new User
          {
            Username = reader.GetString(0),
            PasswordHash = reader.GetString(1),
            Role = reader.GetString(2)
          };
        }
        return null;
      }
      catch (Exception ex)
      {
        throw new Exception("An error occurred while retrieving user by account ID: ", ex);
      }
    }

    public async Task CreateAccountAsync(Account account)
    {
      await using var conn = await _dataSource.OpenConnectionAsync();
      await using var cmd = new NpgsqlCommand("""
              INSERT INTO DEV.ACCOUNTS (username, password, name, email, phone_number, role)
              VALUES (@Username, @Password, @Name, @Email, @PhoneNumber, @Role);
            """, conn);
      cmd.Parameters.AddWithValue("Name", account.name);
      cmd.Parameters.AddWithValue("Username", account.username);
      cmd.Parameters.AddWithValue("Password", account.password);
      cmd.Parameters.AddWithValue("Email", account.email);
      cmd.Parameters.AddWithValue("PhoneNumber", account.phone_number);
      cmd.Parameters.AddWithValue("Role", account.role);

      await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAccountAsync(Account account)
    {
      await using var conn = await _dataSource.OpenConnectionAsync();
      await using var cmd = new NpgsqlCommand(@"
        UPDATE DEV.ACCOUNTS
        SET
          name = @Name,
          email = @Email,
          phone_number = @PhoneNumber
        WHERE id = @Id;
      ", conn);

      cmd.Parameters.AddWithValue("Id", account.id);
      cmd.Parameters.AddWithValue("Name", account.name);
      cmd.Parameters.AddWithValue("Email", account.email);
      cmd.Parameters.AddWithValue("PhoneNumber", account.phone_number);

      await cmd.ExecuteNonQueryAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
      await using var conn = await _dataSource.OpenConnectionAsync();
      await using var cmd = new NpgsqlCommand("""
        SELECT id, username, password, role, email, phone_number
        FROM DEV.ACCOUNTS
        WHERE email = @Email;
      """, conn);

      cmd.Parameters.AddWithValue("Email", email);
      await using var reader = await cmd.ExecuteReaderAsync();
      if (await reader.ReadAsync())
      {
        return new User
        {
          Id = reader.GetGuid(0),
          Username = reader.GetString(1),
          PasswordHash = reader.GetString(2),
          Role = reader.GetString(3),
          Email = reader.GetString(4),
          PhoneNumber = reader.GetString(5)
        };
      }
      return null;
    }

    public async Task UpdatePasswordAsync(User account)
    {
      await using var conn = await _dataSource.OpenConnectionAsync();
      await using var cmd = new NpgsqlCommand(@"
        UPDATE DEV.ACCOUNTS
        SET
          password = @Password
        WHERE id = @Id;
      ", conn);

      cmd.Parameters.AddWithValue("Id", account.Id);
      cmd.Parameters.AddWithValue("Password", account.PasswordHash);

      await cmd.ExecuteNonQueryAsync();
    }
  }
}

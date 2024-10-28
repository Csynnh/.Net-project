using infrastructure.DataModels;
using Npgsql;

public interface IUserRepository
{
  Task<User?> GetUserByUsernameAsync(string username);
  Task CreateAccountAsync(Account account);
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
        SELECT username, password, role
        FROM NOIRTEST.ACCOUNTS
        WHERE username = @Username;
      """, conn);

      cmd.Parameters.AddWithValue("Username", username);
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

    public async Task CreateAccountAsync(Account account)
    {
      await using var conn = await _dataSource.OpenConnectionAsync();
      await using var cmd = new NpgsqlCommand("""
              INSERT INTO NOIRTEST.ACCOUNTS (username, password, name, email, phone_number, role)
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
  }
}

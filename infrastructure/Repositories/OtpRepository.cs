using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;

public class OtpRepository
{
  private readonly NpgsqlDataSource _dataSource;

  public OtpRepository(NpgsqlDataSource dataSource)
  {
    _dataSource = dataSource;
  }

  public async Task SaveOtpAsync(string email, string otp)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      INSERT INTO DEV.OTP (email, otp)
      VALUES (@email, @otp);
    ";
    await connection.ExecuteAsync(sql, new { email, otp });
  }

  public async Task<OtpRessponse> GetOtpByEmailAsync(string email)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      SELECT otp, created_at FROM DEV.OTP WHERE email = @email ORDER BY created_at DESC;
    ";
    OtpRessponse res = await connection.QueryFirstOrDefaultAsync<OtpRessponse>(sql, new { email }) ?? new OtpRessponse();
    res.Created_At = DateTime.SpecifyKind(res.Created_At, DateTimeKind.Utc);

    return res;
  }
}
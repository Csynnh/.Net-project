using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;


public interface IShippingMethodRepository
{
  Task<PaymentMethod> AddPaymentMethod(PaymentMethod paymentMethod);
  Task<List<PaymentMethod>> GetPaymentMethodByAccountId(Guid accountId);
  Task DeletePaymentMethod(Guid paymentMethodId);
  Task<PaymentMethod> GetPaymentMethodById(Guid paymentMethodId);
  Task<PaymentMethod> GetPaymentMethodByName(string paymentMethodName);
}

public class PaymentMethodRepository : IShippingMethodRepository
{

  private readonly NpgsqlDataSource _dataSource;

  public PaymentMethodRepository(NpgsqlDataSource dataSource)
  {
    _dataSource = dataSource;
  }

  public async Task<PaymentMethod> AddPaymentMethod(PaymentMethod paymentMethod)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      INSERT INTO DEV.PAYMENTMETHODS (account_id, payment_method)
      VALUES (@account_id, @payment_method::json)
      RETURNING id, account_id, payment_method::text;
    ";
    return await connection.QuerySingleAsync<PaymentMethod>(sql, paymentMethod);
  }

  public async Task<List<PaymentMethod>> GetPaymentMethodByAccountId(Guid accountId)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      SELECT id, account_id, payment_method
      FROM DEV.PAYMENTMETHODS
      WHERE account_id = @accountId;
    ";
    var result = await connection.QueryAsync<PaymentMethod>(sql, new { accountId });
    return result.ToList();
  }

  public async Task DeletePaymentMethod(Guid paymentMethodId)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      DELETE FROM DEV.PAYMENTMETHODS
      WHERE id = @paymentMethodId;
    ";
    await connection.ExecuteAsync(sql, new { paymentMethodId });
  }

  public async Task<PaymentMethod> GetPaymentMethodById(Guid paymentMethodId)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      SELECT account_id, payment_method
      FROM DEV.PAYMENTMETHODS
      WHERE id = @paymentMethodId;
    ";
    return await connection.QuerySingleAsync<PaymentMethod>(sql, new { paymentMethodId });
  }

  public async Task<PaymentMethod> GetPaymentMethodByName(string paymentMethodName)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      SELECT id, account_id, payment_method
      FROM DEV.PAYMENTMETHODS
      WHERE payment_method->>'payment_name' = @paymentMethodName
      LIMIT 1;
    ";
    return await connection.QuerySingleOrDefaultAsync<PaymentMethod>(sql, new { paymentMethodName });
  }
}
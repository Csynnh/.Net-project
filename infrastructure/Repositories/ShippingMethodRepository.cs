
using Dapper;
using Npgsql;

public interface IShippingMethodRepository
{
  Task<ShippingMethod> CreateShippingMethod(ShippingMethodRequest shippingMethod);
  Task<List<ShippingMethod>> GetShippingMethods();
  Task DeleteShippingMethod(Guid shippingMethodId);
}

public class ShippingMethodRepository : IShippingMethodRepository
{

  private readonly NpgsqlDataSource _dataSource;

  public ShippingMethodRepository(NpgsqlDataSource dataSource)
  {
    _dataSource = dataSource;
  }

  public async Task<ShippingMethod> CreateShippingMethod(ShippingMethodRequest shippingMethod)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      INSERT INTO DEV.SHIPPINGMETHODS (shipping_name, shipping_cost)
      VALUES (@shipping_name, @shipping_cost)
      RETURNING id, shipping_name, shipping_cost;
    ";
    return await connection.QuerySingleAsync<ShippingMethod>(sql, shippingMethod);
  }

  public async Task<List<ShippingMethod>> GetShippingMethods()
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      SELECT id, shipping_name, shipping_cost
      FROM DEV.SHIPPINGMETHODS;
    ";
    var result = await connection.QueryAsync<ShippingMethod>(sql);
    return result.ToList();
  }

  public async Task<ShippingMethod> GetShippingMethodByName(string shippingName)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      SELECT id, shipping_name, shipping_cost
      FROM DEV.SHIPPINGMETHODS
      WHERE shipping_name = @shippingName;
    ";
    return await connection.QuerySingleOrDefaultAsync<ShippingMethod>(sql, new { shippingName });
  }

  public async Task DeleteShippingMethod(Guid shippingMethodId)
  {
    using var connection = _dataSource.CreateConnection();
    string sql = @"
      DELETE FROM DEV.SHIPPINGMETHODS
      WHERE id = @shippingMethodId;
    ";
    await connection.ExecuteAsync(sql, new { shippingMethodId });
  }
}
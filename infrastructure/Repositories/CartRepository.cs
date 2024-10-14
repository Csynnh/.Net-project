using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;
public class CartRepository
{
    private NpgsqlDataSource _dataSource;

    public CartRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<Cart> GetAllCarts()
    {
        var sql = @"
SELECT id as Id, account_id as AccountId, product_id as ProductId, quantity as Quantity, 
       added_at as AddedAt
FROM carts;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Cart>(sql);
        }
    }

    public Cart CreateCart(Guid accountId, Guid productId, int quantity)
    {
        var sql = @"
INSERT INTO carts (account_id, product_id, quantity)
VALUES (@accountId, @productId, @quantity)
RETURNING id as Id, account_id as AccountId, product_id as ProductId, quantity as Quantity, added_at as AddedAt;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Cart>(sql, new { accountId, productId, quantity });
        }
    }

    public Cart UpdateCart(Guid cartId, int quantity)
    {
        var sql = @"
UPDATE carts
SET quantity = @quantity
WHERE id = @cartId
RETURNING id as Id, account_id as AccountId, product_id as ProductId, quantity as Quantity, added_at as AddedAt;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Cart>(sql, new { cartId, quantity });
        }
    }

    public bool DeleteCart(Guid cartId)
    {
        var sql = @"DELETE FROM carts WHERE id = @cartId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { cartId }) == 1;
        }
    }
}
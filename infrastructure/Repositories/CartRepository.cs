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

    public IEnumerable<Cart> GetCartForFeed()
    {
        var sql = $@"
SELECT id as {nameof(CartFeedQuery.cart_id)}, account_id as {nameof(CartFeedQuery.account_id)}, product_id as {nameof(CartFeedQuery.product_id)}, quantity as {nameof(CartFeedQuery.quantity)}, added_at as {nameof(CartFeedQuery.added_at)}
FROM carts;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Cart>(sql);
        }
    }

    public Cart CreateCart(Guid accountId, Guid productId, int quantity)
    {
        var sql = $@"
INSERT INTO carts (account_id, product_id, quantity)
VALUES (@accountId, @productId, @quantity)
RETURNING id as {nameof(Cart.cart_id)}, account_id as {nameof(Cart.account_id)}, product_id as {nameof(Cart.product_id)}, quantity as {nameof(Cart.quantity)}, added_at as {nameof(Cart.added_at)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Cart>(sql, new { accountId, productId, quantity });
        }
    }

    public Cart UpdateCart(Guid cartId, int quantity)
    {
        var sql = $@"
UPDATE carts
SET quantity = @quantity
RETURNING id as {nameof(Cart.cart_id)}, account_id as {nameof(Cart.account_id)}, product_id as {nameof(Cart.product_id)}, quantity as {nameof(Cart.quantity)}, added_at as {nameof(Cart.added_at)};
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
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

    //Create Cart
    public Guid CreateCart(Guid accountId, Guid variant_productId, int quantity)
    {
        var sql = $@"
        INSERT INTO dev.carts (account_id, product_variant_id, quantity)
        VALUES (@accountId, @variant_productId, @quantity)
        RETURNING id;
        ";

        try
        {
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.QuerySingle<Guid>(sql, new { accountId, variant_productId, quantity });
            }
        }
        catch (Exception ex) // Catch other general exceptions
        {
            // Handle other errors such as general exceptions or unexpected errors
            throw new Exception(ex.Message);
        }
    }

    // Get List Cart
    public IEnumerable<CartInQueryResult> GetListCart(Guid accountId)
    {
        var sql = $@"
            select
                c.id ,
                p.name as product_name,
                p.price as product_price,
                c.quantity as product_quantity,
                c2.color as product_color,
                jsonb_agg(jsonb_build_object(
                    'Id', pv.Id,
                    'Images', pv.Images->>'ImageThumbnail'
                )) AS Variants
            from DEV.carts c
            left join DEV.accounts a  on a.id = c.account_id
            left join dev.productvariants pv on pv.id = c.product_variant_id
            left join dev.products p on p.id = pv.product_id
            left join dev.colors c2 on pv.color_id = c2.id
            where c.account_id = @accountId
            group by c.id, p.name, p.price, c2.color;
        ";

        try
        {
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.Query<CartInQueryResult>(sql, new { accountId });
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    // Update Cart
    public void UpdateCart(Guid cartId, int quantity)
    {
        string sql;
        if (quantity <= 0)
        {
            sql = @"DELETE FROM DEV.carts WHERE id = @cartId;";
        }
        else
        {
            sql = @"
            UPDATE DEV.carts
            SET quantity = @quantity
            WHERE id = @cartId
            ";
        }
        try
        {
            using (var conn = _dataSource.OpenConnection())
            {
                conn.Execute(sql, new { cartId, quantity });
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    public void DeleteCart(Guid cartId)
    {
        var sql = @"DELETE FROM DEV.carts WHERE id = @cartId;";
        try
        {
            using (var conn = _dataSource.OpenConnection())
            {
                conn.Execute(sql, new { cartId });
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
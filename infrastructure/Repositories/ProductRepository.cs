using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;
public class ProductRepository
{
    private NpgsqlDataSource _dataSource;

    public ProductRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<Product> GetAllProducts()
    {
        var sql = @"
SELECT id as Id, prod_name as ProdName, prod_desc as ProdDesc, price as Price, wid as Width, hei as Height, type as Type
FROM products;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Product>(sql);
        }
    }

    public Product CreateProduct(string prodName, string prodDesc, decimal price, decimal width, decimal height, string type)
    {
        var sql = @"
INSERT INTO products (prod_name, prod_desc, price, wid, hei, type)
VALUES (@prodName, @prodDesc, @price, @width, @height, @type)
RETURNING id as Id, prod_name as ProdName, prod_desc as ProdDesc, price as Price, 
          wid as Width, hei as Height, type as Type;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Product>(sql, new { prodName, prodDesc, price, width, height, type });
        }
    }

    public Product UpdateProduct(Guid productId, string prodName, string prodDesc, decimal price, decimal width, decimal height, string type)
    {
        var sql = @"
UPDATE products
SET prod_name = @prodName, prod_desc = @prodDesc, price = @price, wid = @width, hei = @height, type = @type
WHERE id = @productId
RETURNING id as Id, prod_name as ProdName, prod_desc as ProdDesc, price as Price, 
          wid as Width, hei as Height, type as Type;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Product>(sql, new { productId, prodName, prodDesc, price, width, height, type });
        }
    }

    public bool DeleteProduct(Guid productId)
    {
        var sql = @"DELETE FROM products WHERE id = @productId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { productId }) == 1;
        }
    }

    public bool DoesProductExistWithName(string prodName)
    {
        var sql = @"SELECT COUNT(*) FROM products WHERE prod_name = @prodName;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.ExecuteScalar<int>(sql, new { prodName }) > 0;
        }
    }
}
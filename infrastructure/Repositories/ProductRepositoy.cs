using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class ProductRepository
{
    private NpgsqlDataSource _dataSource;

    public ProductRepository(NpgsqlDataSource datasource)
    {
        _dataSource = datasource;
    }

    public IEnumerable<Product> GetProducts()
    {
        string sql = $@"
SELECT id  as {nameof(Product.id )},
       prod_name as {nameof(Product.name)},
       prod_desc as {nameof(Product.desc)},
       price as {nameof(Product.price)},
       inventory as {nameof(Product.inventory)},
       image_url as {nameof(Product.image_url)}
FROM noir.Products;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Product>(sql);
        }
    }

    public Product CreateProduct(string name, string desc, decimal price, int inventory, string image_url)
    {
        var sql = $@"
INSERT INTO products (name, desc, price, inventory, image_url) 
VALUES (@name, @desc, @price, @inventory, @image_url)
RETURNING id  as {nameof(Product.id )},
          name as {nameof(Product.name)},
          desc as {nameof(Product.desc)},
          price as {nameof(Product.price)},
          inventory as {nameof(Product.inventory)},
          image_url as {nameof(Product.image_url)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Product>(sql, new { name, desc, price, inventory, image_url });
        }
    }

    public Product UpdateProduct(int id, string name, string desc, decimal price, int inventory, string image_url)
    {
        var sql = $@"
UPDATE products 
SET name = @name, desc = @desc, price = @price, inventory = @inventory, image_url = @image_url
WHERE id  = @id
RETURNING id  as {nameof(Product.id )},
          name as {nameof(Product.name)},
          desc as {nameof(Product.desc)},
          price as {nameof(Product.price)},
          inventory as {nameof(Product.inventory)},
          image_url as {nameof(Product.image_url)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Product>(sql, new { id, name, desc, price, inventory, image_url });
        }
    }

    public bool DeleteProduct(int id)
    {
        var sql = @"DELETE FROM products WHERE id  = @id;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { id }) == 1;
        }
    }

    public bool DoesProductWithNameExist(string name)
    {
        var sql = @"SELECT COUNT(*) FROM products WHERE name = @name;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.ExecuteScalar<int>(sql, new { name }) == 1;
        }
    }
}
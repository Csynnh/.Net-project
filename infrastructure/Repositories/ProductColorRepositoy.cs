using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;
public class ProductColorRepository
{
    private NpgsqlDataSource _dataSource;

    public ProductColorRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Get all colors for a specific product
    public IEnumerable<ProductColor> GetProductColorForFeed()
    {
        var sql = $@"
SELECT id as {nameof(ProductColorFeedQuery.color_id)}, 
       product_id as {nameof(ProductColorFeedQuery.product_id)}, 
       color_name as {nameof(ProductColorFeedQuery.color_name)}, 
       color_code as {nameof(ProductColorFeedQuery.color_code)}, 
       inventory as {nameof(ProductColorFeedQuery.inventory)}, 
       total as {nameof(ProductColorFeedQuery.total)}, 
       image_url1 as {nameof(ProductColorFeedQuery.image_url1)}, 
       image_url2 as {nameof(ProductColorFeedQuery.image_url2)}, 
       image_url3 as {nameof(ProductColorFeedQuery.image_url3)}, 
       image_url4 as {nameof(ProductColorFeedQuery.image_url4)}, 
       image_url5 as {nameof(ProductColorFeedQuery.image_url5)}
FROM productcolors
WHERE product_id = @productId;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<ProductColor>(sql);
        }
    }

    // Create a new product color
    public ProductColor CreateProductColor(Guid productId, string colorName, string colorCode, int inventory, int total, string image1Url, string image2Url, string image3Url, string image4Url, string image5Url)
    {
        var sql = $@"
INSERT INTO productcolors (product_id, color_name, color_code, inventory, total, image_url1, image_url2, image_url3, image_url4, image_url5)
VALUES (@productId, @colorName, @colorCode, @inventory, @total, @image1Url, @image2Url, @image3Url, @image4Url, @image5Url)
RETURNING id as {nameof(ProductColor.color_id)}, 
          product_id as {nameof(ProductColor.product_id)}, 
          color_name as {nameof(ProductColor.color_name)}, 
          color_code as {nameof(ProductColor.color_code)}, 
          inventory as {nameof(ProductColor.inventory)}, 
          total as {nameof(ProductColor.total)}, 
          image_url1 as {nameof(ProductColor.image_url1)}, 
          image_url2 as {nameof(ProductColor.image_url2)}, 
          image_url3 as {nameof(ProductColor.image_url3)}, 
          image_url4 as {nameof(ProductColor.image_url4)}, 
          image_url5 as {nameof(ProductColor.image_url5)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<ProductColor>(sql, new { productId, colorName, colorCode, inventory, total, image1Url, image2Url, image3Url, image4Url, image5Url });
        }
    }

    // Update an existing product color
    public ProductColor UpdateProductColor(Guid colorId, string colorName, string colorCode, int inventory, int total, string image1Url, string image2Url, string image3Url, string image4Url, string image5Url)
    {
        var sql = $@"
UPDATE productcolors
SET color_name = @colorName, color_code = @colorCode, inventory = @inventory, total = @total, 
    image_url1 = @image1Url, image_url2 = @image2Url, image_url3 = @image3Url, 
    image_url4 = @image4Url, image_url5 = @image5Url
WHERE id = @colorId
RETURNING id as {nameof(ProductColor.color_id)}, 
          product_id as {nameof(ProductColor.product_id)}, 
          color_name as {nameof(ProductColor.color_name)}, 
          color_code as {nameof(ProductColor.color_code)}, 
          inventory as {nameof(ProductColor.inventory)}, 
          total as {nameof(ProductColor.total)}, 
          image_url1 as {nameof(ProductColor.image_url1)}, 
          image_url2 as {nameof(ProductColor.image_url2)}, 
          image_url3 as {nameof(ProductColor.image_url3)}, 
          image_url4 as {nameof(ProductColor.image_url4)}, 
          image_url5 as {nameof(ProductColor.image_url5)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<ProductColor>(sql, new { colorId, colorName, colorCode, inventory, total, image1Url, image2Url, image3Url, image4Url, image5Url });
        }
    }

    // Delete a product color
    public bool DeleteProductColor(Guid colorId)
    {
        var sql = @"DELETE FROM productcolors WHERE id = @colorId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { colorId }) == 1;
        }
    }
}

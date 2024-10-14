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
    public IEnumerable<ProductColor> GetProductColorsByProductId(Guid productId)
    {
        var sql = @"
SELECT id as Id, product_id as ProductId, color_name as ColorName, color_code as ColorCode, 
       inventory as Inventory, total as Total, image1_url as Image1Url, image2_url as Image2Url, 
       image3_url as Image3Url, image4_url as Image4Url, image5_url as Image5Url
FROM productcolors
WHERE product_id = @productId;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<ProductColor>(sql, new { productId });
        }
    }

    // Create a new product color
    public ProductColor CreateProductColor(Guid productId, string colorName, string colorCode, int inventory, int total, string image1Url, string image2Url, string image3Url, string image4Url, string image5Url)
    {
        var sql = @"
INSERT INTO productcolors (product_id, color_name, color_code, inventory, total, image1_url, image2_url, image3_url, image4_url, image5_url)
VALUES (@productId, @colorName, @colorCode, @inventory, @total, @image1Url, @image2Url, @image3Url, @image4Url, @image5Url)
RETURNING id as Id, product_id as ProductId, color_name as ColorName, color_code as ColorCode, 
          inventory as Inventory, total as Total, image1_url as Image1Url, image2_url as Image2Url, 
          image3_url as Image3Url, image4_url as Image4Url, image5_url as Image5Url;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<ProductColor>(sql, new { productId, colorName, colorCode, inventory, total, image1Url, image2Url, image3Url, image4Url, image5Url });
        }
    }

    // Update an existing product color
    public ProductColor UpdateProductColor(int colorId, string colorName, string colorCode, int inventory, int total, string image1Url, string image2Url, string image3Url, string image4Url, string image5Url)
    {
        var sql = @"
UPDATE productcolors
SET color_name = @colorName, color_code = @colorCode, inventory = @inventory, total = @total, 
    image1_url = @image1Url, image2_url = @image2Url, image3_url = @image3Url, image4_url = @image4Url, image5_url = @image5Url
WHERE id = @colorId
RETURNING id as Id, product_id as ProductId, color_name as ColorName, color_code as ColorCode, 
          inventory as Inventory, total as Total, image1_url as Image1Url, image2_url as Image2Url, 
          image3_url as Image3Url, image4_url as Image4Url, image5_url as Image5Url;
";
        using (var conn = _dataSource.OpenConnection())
        {return conn.QueryFirst<ProductColor>(sql, new { colorId, colorName, colorCode, inventory, total, image1Url, image2Url, image3Url, image4Url, image5Url });
        }
    }

    // Delete a product color
    public bool DeleteProductColor(int colorId)
    {
        var sql = @"DELETE FROM productcolors WHERE id = @colorId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { colorId }) == 1;
        }
    }
}
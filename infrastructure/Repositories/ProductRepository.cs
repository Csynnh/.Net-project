using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories
{
    public class ProductRepository
    {
        private NpgsqlDataSource _dataSource;

        public ProductRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public IEnumerable<Product> GetProductForFeed()
        {
            var sql = $@"
SELECT id as {nameof(ProductFeedQuery.id)}, 
       prod_name as {nameof(ProductFeedQuery.prod_name)}, 
       prod_desc as {nameof(ProductFeedQuery.pro_desc)}, 
       price as {nameof(ProductFeedQuery.price)}, 
       wid as {nameof(ProductFeedQuery.width)}, 
       hei as {nameof(ProductFeedQuery.height)}, 
       type as {nameof(ProductFeedQuery.type)}
FROM products;
";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.Query<Product>(sql);
            }
        }

        public IEnumerable<Product> GetProductForHomePage()
        {
             var sql = $@"
            SELECT 
    p.type AS product_type,
    p.prod_name AS product_name,
    p.price AS product_price,
    pc.image1_url,
    pc.image2_url,
    pc.image3_url,
    pc.image4_url,
    pc.image5_url,
    STRING_AGG(DISTINCT pc.color_code, ', ') AS color_codes
FROM 
    Products p
LEFT JOIN 
    ProductColors pc ON p.id = pc.product_id
GROUP BY 
    p.type, p.prod_name, p.price, pc.image1_url, pc.image2_url, pc.image3_url, pc.image4_url, pc.image5_url
ORDER BY 
    p.type, p.prod_name;";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.Query<Product>(sql);
            }
        }

        
        public IEnumerable<Product> GetProductForItemDetailPage(Guid product_id)
        {
             var sql = $@"
             
WITH ProductInfo AS (
    SELECT 
        p.*,
        pc.color_name,
        pc.color_code,
        pc.inventory,
        pc.total,
        pc.image1_url,
        pc.image2_url,
        pc.image3_url,
        pc.image4_url,
        pc.image5_url
    FROM 
        Products p
    JOIN 
        ProductColors pc ON p.id = pc.product_id
    WHERE 
        p.id = {product_id}
),
ReviewInfo AS (
    SELECT 
        cr.*,
        a.username AS reviewer_username
    FROM 
        CustomerReviews cr
    JOIN 
        Accounts a ON cr.account_id = a.id
    WHERE 
        cr.product_id = {product_id}
)
SELECT 
    pi.*,
    ri.id AS review_id,
    ri.reviewer_username,
    ri.content AS review_content,
    ri.vote AS review_vote,
    ri.created_at AS review_date
FROM 
    ProductInfo pi
LEFT JOIN 
    ReviewInfo ri ON pi.id = {product_id}
ORDER BY 
    pi.color_name, ri.created_at DESC;";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.Query<Product>(sql);
            }
        }

        public Product CreateProduct(string prod_name, string pro_desc, decimal price, decimal width, decimal height, string type)
        {
            var sql = $@"
INSERT INTO products (prod_name, prod_desc, price, wid, hei, type)
VALUES (@prod_name, @pro_desc, @price, @width, @height, @type)
RETURNING id as {nameof(Product.id)}, 
          prod_name as {nameof(Product.prod_name)}, 
          prod_desc as {nameof(Product.pro_desc)}, 
          price as {nameof(Product.price)}, 
          wid as {nameof(Product.width)}, 
          hei as {nameof(Product.height)}, 
          type as {nameof(Product.type)};
";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.QueryFirst<Product>(sql, new { prod_name, pro_desc, price, width, height, type });
            }
        }

        public Product UpdateProduct(Guid productId, string prod_name, string pro_desc, decimal price, decimal width, decimal height, string type)
        {
            var sql = $@"
UPDATE products
SET prod_name = @prod_name, 
    prod_desc = @pro_desc, 
    price = @price, 
    wid = @width, 
    hei = @height, 
    type = @type
WHERE id = @productId
RETURNING id as {nameof(Product.id)}, 
          prod_name as {nameof(Product.prod_name)}, 
          prod_desc as {nameof(Product.pro_desc)}, 
          price as {nameof(Product.price)}, 
          wid as {nameof(Product.width)}, 
          hei as {nameof(Product.height)}, 
          type as {nameof(Product.type)};
";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.QueryFirst<Product>(sql, new { productId, prod_name, pro_desc, price, width, height, type });
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

        public bool DoesProductExistWithName(string prod_name)
        {
            var sql = @"SELECT COUNT(*) FROM products WHERE prod_name = @prod_name;";
            using (var conn = _dataSource.OpenConnection())
            {
                return conn.ExecuteScalar<int>(sql, new { prod_name }) > 0;
            }
        }
    }
}

using System.Text.Json;
using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace infrastructure.Repositories;

public class ProductRepository
{
    private NpgsqlDataSource _dataSource;
    private readonly ILogger<ProductRepository> _logger;


    public ProductRepository(NpgsqlDataSource datasource, ILogger<ProductRepository> _logger)
    {
        _dataSource = datasource;
        _logger = _logger;
    }

    public IEnumerable<CollectionDBResponsse> GetProducts()
    {
        string sql = $@"
            SELECT
                id,
                collection,
                img_url->>'main_image' AS image_url,
                name,
                price,
                array_agg(color) AS available_colors
            FROM
                library_app.products
            GROUP BY
                id, collection, img_url->>'main_image', name, price
            ORDER BY
                collection, name;
        ";
        using (var conn = _dataSource.OpenConnection())
        {
            var response = conn.Query<CollectionDBResponsse>(sql);
            return conn.Query<CollectionDBResponsse>(sql);
        }
    }

    public ProductDetailsDBResponse GetProductByName(string name, string collection, string? color = null)
    {
        var sql = $@"
            SELECT
                name,
                collection,
                array_agg(DISTINCT color)::text[] AS available_colors,
                price,
                description,
                COUNT(*) AS total_count,
                jsonb_object_agg(color, jsonb_build_array(
                    img_url->>'main_image',
                    img_url->'additional_images'
                )) AS images_per_color,
                jsonb_agg(ext_fields) as ext_fields
            FROM
                library_app.products p1
            WHERE
                name = @name
                AND collection = @collection
                AND is_sold = FALSE
                AND (@color IS NULL OR color = @color)
            GROUP BY
                name, collection, price, description;
        ";
        using (var conn = _dataSource.OpenConnection())
        {
            var response = conn.QueryFirstOrDefault<ProductDetailsDBResponse>(sql, new { name, collection, color });
            return response;
        }
    }

    public bool CreateProduct(ProductDBRequest product_request)
    {
        var sql = $@"
            INSERT INTO library_app.products (name, collection, is_sold, ext_fields, img_url, color, price, description)
            VALUES (
                @name, @collection, @is_sold, @ext_fields, @img_url::json, @color, @price, @description
            )
            RETURNING id
        ";
        using var conn = _dataSource.OpenConnection();
        var result = conn.QueryFirst(sql, new
        {
            product_request.name,
            product_request.collection,
            product_request.is_sold,
            product_request.ext_fields,
            product_request.img_url,
            product_request.color,
            product_request.price,
            product_request.description
        });
        return result != null;
    }

    public string UpdateProduct(string collection, string name, int count, ProductDBRequest product_request)
    {
        var sql = $@"
            WITH cte AS (
                SELECT *
                FROM library_app.products
                WHERE name = @name
                AND collection = @collection
                AND is_sold = FALSE
                ORDER BY created_at ASC
                LIMIT @count
            )
            UPDATE library_app.products
            SET
                name = @name_updated,
                collection = @collection_updated,
                is_sold = @is_sold,
                ext_fields = @ext_fields,
                img_url = @img_url::json,
                color = @color,
                price = @price,
                description = @description
            FROM cte
            WHERE library_app.products.id = cte.id;

        ";
        using var conn = _dataSource.OpenConnection();
        var result = conn.QueryFirstOrDefault<string>(sql, new
        {
            name,
            collection,
            product_request.is_sold,
            product_request.ext_fields,
            product_request.img_url,
            product_request.color,
            product_request.price,
            product_request.description,
            name_updated = product_request.name,
            collection_updated = product_request.collection,
            count
        });
        return product_request.name;
    }

    public bool DeleteProduct(string collection, string name, string color, int count)
    {
        var sql = $@"
            WITH cte AS (
                SELECT *
                FROM library_app.products
                WHERE name = @name
                AND collection = @collection
                AND color = @color
                AND is_sold = FALSE
                ORDER BY created_at ASC
                LIMIT @count
            )
            DELETE FROM library_app.products
            WHERE id IN (SELECT id FROM cte);
        ";
        using var conn = _dataSource.OpenConnection();
        var result = conn.Execute(sql, new { name, collection, color, count });
        return true;
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
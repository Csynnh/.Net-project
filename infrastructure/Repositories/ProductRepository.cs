using System.Text.Json;
using Dapper;
using infrastructure.QueryModels;
using Npgsql;
public interface IProductRepository
{
    Task AddProductAsync(ProductModel product);
    Task<ProductModelResponse> GetProductByIdAsync(Guid id);
    Task<infrastructure.DataModels.PagedResponse<ProductModelResponse>> ListProductByTypeIdAsync(Guid typeId, int pageNumber, int pageSize, string? size, decimal? minPrice, decimal? maxPrice);
    Task<IEnumerable<ListProductByTypeResponse>> ListProductByTypeAsync();
    Task<IEnumerable<ListProductByOderStatusResponse>> ListProductByOderStatusAsync(Guid accountId, string status);
    Task<bool> IsProductExistAsync(string name, string color, string size);
}


namespace infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public ProductRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<ProductModelResponse> GetProductByIdAsync(Guid id)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            var product = await conn.QueryFirstOrDefaultAsync<ProductModelResponse>(@"
            SELECT p.Name, p.Description, p.Price, p.Inventory, p.Details::text AS Details,
                jsonb_agg(jsonb_build_object(
                    'Images', pv.Images::json,
                    'Inventory', pv.Inventory,
                    'Size', s.Size,
                    'Color', c.Color
                )) AS Variants,
                t.Type
            FROM DEV.Products p
            JOIN DEV.ProductVariants pv ON p.Id = pv.Product_Id
            JOIN DEV.Sizes s ON pv.Size_Id = s.Id
            JOIN DEV.Colors c ON pv.Color_Id = c.Id
            JOIN DEV.Types t ON p.Type_Id = t.Id
            WHERE p.Id = @Id
            GROUP BY p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type", new { Id = id });
            var productVariant = JsonSerializer.Deserialize<List<ProductVariant>>((string)product.Variants);
            var productDetails = JsonSerializer.Deserialize<ProductDetails>((string)product.Details);
            product.Variants = productVariant;
            product.Details = productDetails;
            return product;
        }

        public async Task<DataModels.PagedResponse<ProductModelResponse>> ListProductByTypeIdAsync(Guid typeId, int pageNumber, int pageSize, string? size, decimal? minPrice, decimal? maxPrice)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();

            // Query for the total count of items
            var countQuery = @"
    SELECT COUNT(*)
    FROM DEV.Products p
    JOIN DEV.ProductVariants pv ON p.Id = pv.Product_Id
    JOIN DEV.Sizes s ON pv.Size_Id = s.Id
    JOIN DEV.Colors c ON pv.Color_Id = c.Id
    JOIN DEV.Types t ON p.Type_Id = t.Id
    WHERE t.Id = @TypeId";

            // Build the dynamic count query based on filters
            if (!string.IsNullOrEmpty(size))
            {
                countQuery += " AND s.Size = @Size";
            }

            if (minPrice.HasValue)
            {
                countQuery += " AND p.Price >= @MinPrice";
            }

            if (maxPrice.HasValue)
            {
                countQuery += " AND p.Price <= @MaxPrice";
            }

            var totalItems = await conn.ExecuteScalarAsync<int>(countQuery, new
            {
                TypeId = typeId,
                Size = size,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            });

            // Now query for the paginated items
            var query = @"
    SELECT p.Name, p.Description, p.Price, p.Inventory, p.Details::text AS Details,
        jsonb_agg(jsonb_build_object(
            'Images', pv.Images::json,
            'Inventory', pv.Inventory,
            'Size', s.Size,
            'Color', c.Color
        )) AS Variants,
        t.Type
    FROM DEV.Products p
    JOIN DEV.ProductVariants pv ON p.Id = pv.Product_Id
    JOIN DEV.Sizes s ON pv.Size_Id = s.Id
    JOIN DEV.Colors c ON pv.Color_Id = c.Id
    JOIN DEV.Types t ON p.Type_Id = t.Id
    WHERE t.Id = @TypeId";

            // Build the dynamic query based on filters
            if (!string.IsNullOrEmpty(size))
            {
                query += " AND s.Size = @Size";
            }

            if (minPrice.HasValue)
            {
                query += " AND p.Price >= @MinPrice";
            }

            if (maxPrice.HasValue)
            {
                query += " AND p.Price <= @MaxPrice";
            }

            query += @"
    GROUP BY p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type
    ORDER BY p.Name
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new DynamicParameters();
            parameters.Add("TypeId", typeId);
            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            // Add filter parameters if they are provided
            if (!string.IsNullOrEmpty(size))
            {
                parameters.Add("Size", size);
            }

            if (minPrice.HasValue)
            {
                parameters.Add("MinPrice", minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                parameters.Add("MaxPrice", maxPrice.Value);
            }

            var products = await conn.QueryAsync<ProductModelResponse>(query, parameters);

            foreach (var product in products)
            {
                var productVariant = JsonSerializer.Deserialize<List<ProductVariant>>((string)product.Variants);
                var productDetails = JsonSerializer.Deserialize<ProductDetails>((string)product.Details);
                product.Variants = productVariant;
                product.Details = productDetails;
            }

            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return new DataModels.PagedResponse<ProductModelResponse>
            {
                Items = products,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }




        public async Task<IEnumerable<ListProductByTypeResponse>> ListProductByTypeAsync()
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            var products = await conn.QueryAsync<ProductModelResponse>(@"
            SELECT p.Name, p.Description, p.Price, p.Inventory, p.Details::text AS Details,
                jsonb_agg(jsonb_build_object(
                    'Images', pv.Images::json,
                    'Inventory', pv.Inventory,
                    'Size', s.Size,
                    'Color', c.Color
                )) AS Variants,
                t.Type
            FROM DEV.Products p
            JOIN DEV.ProductVariants pv ON p.Id = pv.Product_Id
            JOIN DEV.Sizes s ON pv.Size_Id = s.Id
            JOIN DEV.Colors c ON pv.Color_Id = c.Id
            JOIN DEV.Types t ON p.Type_Id = t.Id
            GROUP BY p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type");
            foreach (var product in products)
            {
                var productVariant = JsonSerializer.Deserialize<List<ProductVariant>>((string)product.Variants);
                var productDetails = JsonSerializer.Deserialize<ProductDetails>((string)product.Details);
                product.Variants = productVariant;
                product.Details = productDetails;
            }
            var groupedProducts = products.GroupBy(p => new { p.Type })
                                            .Select(g => new ListProductByTypeResponse
                                            {
                                                Type = g.Key.Type,
                                                Products = g.ToList()
                                            });
            return groupedProducts;
        }


        public async Task<IEnumerable<ListProductByOderStatusResponse>> ListProductByOderStatusAsync(Guid accountId, string status)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            var products = await conn.QueryAsync<ProductOrderResponse>(@"
            SELECT
                o.id AS orderId,
                o.status AS orderStatus,
                jsonb_agg(
                    jsonb_build_object(
                        'productName', p.name,
                        'description', p.description,
                        'price', od.price,
                        'quantity', od.quantity,
                        'variant', jsonb_build_object(
                            'size', s.size,
                            'color', c.color,
                            'images', pv.images
                        )
                    )
                ) AS products
            FROM DEV.ORDERS o
            JOIN DEV.ORDERDETAILS od ON o.id = od.order_id
            JOIN DEV.PRODUCTVARIANTS pv ON od.product_variant_id = pv.id
            JOIN DEV.PRODUCTS p ON pv.product_id = p.id
            JOIN DEV.SIZES s ON pv.size_id = s.id
            JOIN DEV.COLORS c ON pv.color_id = c.id
            WHERE o.account_id = @AccountId AND o.status = @Status
            GROUP BY o.id, o.status;", new { AccountId = accountId, Status = status });
            foreach (var product in products)
            {
                var productVariant = JsonSerializer.Deserialize<List<ProductVariant>>((string)product.Variants);
                var productDetails = JsonSerializer.Deserialize<ProductDetails>((string)product.Details);
                product.Variants = productVariant;
                product.Details = productDetails;
            }
            var groupedProducts = products.GroupBy(p => new { p.OrderStatus })
                                            .Select(g => new ListProductByOderStatusResponse
                                            {
                                                OrderStatus = g.Key.OrderStatus,
                                                Products = g.ToList()
                                            });
            Console.WriteLine(groupedProducts);
            return groupedProducts;
        }

        public async Task AddProductAsync(ProductModel product)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("""
            -- Insert the color if it does not exist
            INSERT INTO DEV.Sizes (size)
            VALUES (@Size)
            ON CONFLICT (size)
            WHERE ((size)::text = @Size::text) DO NOTHING;

            -- Insert the color if it does not exist
            INSERT INTO DEV.Colors (color)
            VALUES (@Color)
            ON CONFLICT (color)
            WHERE ((color)::text = @Color::text) DO NOTHING;

            -- Insert the type if it does not exist
            INSERT INTO DEV.Types (type)
            VALUES (@Type)
            ON CONFLICT (type)
            WHERE ((type)::text = @Type::text) DO NOTHING;

            -- Insert the product and check if it already exists based on the name
            INSERT INTO DEV.Products (name, description, price, type_id, inventory, details)
            SELECT @Name, @Description, @Price, t.Id, @Inventory, @Details::json
            FROM DEV.Types t
            WHERE t.type = @Type
            ON CONFLICT (Name)
            WHERE ((Name)::text = @Name::text) DO NOTHING;

            -- Insert the product variant
            INSERT INTO DEV.ProductVariants (product_id, size_id, color_id, images, inventory)
            SELECT p.Id, s.Id, c.Id, @Images::json, @Inventory
            FROM DEV.Products p
            JOIN DEV.Types t ON p.type_id = t.Id
            JOIN DEV.Sizes s ON s.size = @Size
            JOIN DEV.Colors c ON c.color = @Color
            WHERE p.Name = @Name;
            """, conn);

            cmd.Parameters.AddWithValue("Name", product.Name);
            cmd.Parameters.AddWithValue("Description", product.Description);
            cmd.Parameters.AddWithValue("Price", product.Price);
            cmd.Parameters.AddWithValue("Inventory", product.Inventory);
            cmd.Parameters.AddWithValue("Details", product.Details);
            cmd.Parameters.AddWithValue("Size", product.Size);
            cmd.Parameters.AddWithValue("Color", product.Color);
            cmd.Parameters.AddWithValue("Type", product.Type);
            cmd.Parameters.AddWithValue("Images", product.Images);

            await cmd.ExecuteNonQueryAsync();
        }


        /*
        SELECT
            o.id AS order_id,
            o.status AS order_status,
            o.created_at AS order_date,
            od.quantity AS quantity,
            od.price AS unit_price,
            od.quantity * od.price AS total_price,
            p.id AS product_id,
            p.name AS product_name,
            p.description AS product_description,
            p.price AS product_price,
            pv.size_id AS size_id,
            s.size AS size,
            pv.color_id AS color_id,
            c.color AS color,
            pv.images AS images
        FROM
            DEV.ORDERS o
        JOIN
            DEV.ORDERDETAILS od ON o.id = od.order_id
        JOIN
            DEV.PRODUCTVARIANTS pv ON od.product_variant_id = pv.id
        JOIN
            DEV.PRODUCTS p ON pv.product_id = p.id
        LEFT JOIN
            DEV.SIZES s ON pv.size_id = s.id
        LEFT JOIN
            DEV.COLORS c ON pv.color_id = c.id
        WHERE
            o.account_id = :account_id
            AND o.status = :status
        ORDER BY
            o.created_at DESC;
        */
        // public async 

        public async Task<bool> IsProductExistAsync(string name, string color, string size)
        {
            // Define the SQL query to check for existing products with the specified criteria
            const string sql = @"
        SELECT COUNT(*)
        FROM DEV.PRODUCTS p
        JOIN DEV.PRODUCTVARIANTS pv ON p.id = pv.product_id
        JOIN DEV.COLORS c ON pv.color_id = c.id
        JOIN DEV.SIZES s ON pv.size_id = s.id
        WHERE p.name = @Name AND c.color = @Color AND s.size = @Size";

            try
            {
                using (var connection = _dataSource.CreateConnection())
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Color", color.ToUpper());
                        command.Parameters.AddWithValue("@Size", size.ToUpper());

                        // Execute the query and retrieve the count
                        var count = await command.ExecuteScalarAsync();

                        // Return true if at least one product exists, otherwise false
                        if (count != null && count != DBNull.Value)
                        {
                            return Convert.ToInt32(count) > 0;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return null
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

    }
}

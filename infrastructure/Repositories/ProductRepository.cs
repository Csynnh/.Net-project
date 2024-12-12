using System.Text.Json;
using Dapper;
using infrastructure.QueryModels;
using Npgsql;
public interface IProductRepository
{
    Task AddProductAsync(ProductModel product);
    Task<ProductModelResponse> GetProductByIdAsync(Guid id);
    Task<infrastructure.DataModels.PagedResponse<ProductModelResponse>> ListProductByTypeNameAsync(string name, int pageNumber, int pageSize, string? size, decimal? minPrice, decimal? maxPrice);
    Task<IEnumerable<ListProductByTypeResponse>> ListProductByTypeAsync();
    Task<IEnumerable<ListProductByOderStatusResponse>> ListProductByOderStatusAsync(Guid accountId, string status);
    Task<ProductModelResponse?> GetProductByNameColorSizeAsync(string name, string color, string size);
    Task<bool> UpdateProductAsync(Guid id, Guid VariantId, ProductModel product);
    Task DeleteProductAsync(Guid id);
    Task DeleteProductVariantAsync(Guid VariantId);
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

        public async Task<ProductModelResponse> GetProductVariantByIdAsync(Guid id)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            var product = await conn.QueryFirstOrDefaultAsync<ProductModelResponse?>(@"
            SELECT p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text AS Details,
                jsonb_agg(jsonb_build_object(
                    'Id', pv.Id,
                    'Images', pv.Images::json,
                    'Inventory', pv.Inventory,
                    'Size', s.Size,
                    'Color', c.Color
                )) AS Variants,
                t.Type
            FROM DEV.ProductVariants pv
            JOIN DEV.Products p ON p.Id = pv.Product_Id
            JOIN DEV.Sizes s ON pv.Size_Id = s.Id
            JOIN DEV.Colors c ON pv.Color_Id = c.Id
            JOIN DEV.Types t ON p.Type_Id = t.Id
            WHERE pv.Id = @Id
            GROUP BY p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type", new { Id = id });
            if (product == null)
            {
                return null;
            }
            var productVariant = JsonSerializer.Deserialize<List<ProductVariant>>((string)product.Variants);
            var productDetails = JsonSerializer.Deserialize<ProductDetails>((string)product.Details);
            product.Variants = productVariant;
            product.Details = productDetails;
            return product;
        }

        public async Task<ProductModelResponse> GetProductByIdAsync(Guid id)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            var product = await conn.QueryFirstOrDefaultAsync<ProductModelResponse?>(@"
            SELECT p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text AS Details,
                jsonb_agg(jsonb_build_object(
                    'Id', pv.Id,
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
            GROUP BY p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type", new { Id = id });
            if (product == null)
            {
                return null;
            }
            var productVariant = JsonSerializer.Deserialize<List<ProductVariant>>((string)product.Variants);
            var productDetails = JsonSerializer.Deserialize<ProductDetails>((string)product.Details);
            product.Variants = productVariant;
            product.Details = productDetails;
            return product;
        }


        public async Task<Guid> GetIdByName(string name)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();

            var query = @"
                SELECT Id
                FROM DEV.Products 
                WHERE Name=@name
                ";

            var productId = await conn.QueryFirstOrDefaultAsync<dynamic>(query, new { Name = $"{name}" });

            return productId.id;
        }

        public async Task<DataModels.PagedResponse<ProductModelResponse>> ListProductByTypeNameAsync(string name, int pageNumber, int pageSize, string? size, decimal? minPrice, decimal? maxPrice)
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
    WHERE t.type = @Name";

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
            countQuery += " GROUP BY p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.type";
            var countSql = $"SELECT COUNT(*) FROM ({countQuery})";
            var totalItems = await conn.ExecuteScalarAsync<int>(countSql, new
            {
                Name = name.ToUpper(),
                Size = size,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            });

            // Now query for the paginated items
            var query = @"
    SELECT p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text AS Details,
        jsonb_agg(jsonb_build_object(
            'Id', pv.Id,
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
    WHERE t.type = @Name";

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
    GROUP BY p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type
    ORDER BY p.Name
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new DynamicParameters();
            parameters.Add("Name", name.ToUpper());
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
            SELECT p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text AS Details,
                jsonb_agg(jsonb_build_object(
                    'Id', pv.Id,
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
            GROUP BY p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type");
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

        public async Task<bool> UpdateProductAsync(Guid id, Guid VariantId, ProductModel product)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
            -- Update the product
            UPDATE DEV.Products
            SET name = @Name, description = @Description, price = @Price, inventory = @Inventory, details = @Details::json, type_id = (SELECT id FROM DEV.Types WHERE type = @Type)
            WHERE id = @Id::uuid;

            -- Update the product variant
            UPDATE DEV.ProductVariants
            SET images = @Images::json, inventory = @Inventory, size_id = (SELECT id FROM DEV.Sizes WHERE size = @Size), color_id = (SELECT id FROM DEV.Colors WHERE color = @Color)
            WHERE Id = @VariantId::uuid;
            ", conn);

            cmd.Parameters.AddWithValue("Id", id.ToString());
            cmd.Parameters.AddWithValue("VariantId", VariantId.ToString());
            cmd.Parameters.AddWithValue("Name", product.Name);
            cmd.Parameters.AddWithValue("Description", product.Description);
            cmd.Parameters.AddWithValue("Price", product.Price);
            cmd.Parameters.AddWithValue("Inventory", product.Inventory);
            cmd.Parameters.AddWithValue("Details", product.Details);
            cmd.Parameters.AddWithValue("Size", product.Size);
            cmd.Parameters.AddWithValue("Color", product.Color);
            cmd.Parameters.AddWithValue("Images", product.Images);
            cmd.Parameters.AddWithValue("Type", product.Type);

            await cmd.ExecuteNonQueryAsync();

            return true;
        }

        public async Task<ProductModelResponse?> GetProductByNameColorSizeAsync(string name, string color, string size)
        {
            const string sql = @"
            SELECT p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text AS Details,
                        jsonb_agg(jsonb_build_object(
                            'Id', pv.Id,
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
            WHERE p.Name = @Name AND c.Color = @Color AND s.Size = @Size
            GROUP BY p.Id, p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type
            LIMIT 1;";

            try
            {
                await using var conn = await _dataSource.OpenConnectionAsync();
                var product = await conn.QueryFirstOrDefaultAsync<ProductModelResponse>(sql, new { Name = name, Color = color.ToUpper(), Size = size.ToUpper() });
                if (product != null)
                {
                    var productVariant = JsonSerializer.Deserialize<List<ProductVariant>>((string)product.Variants);
                    var productDetails = JsonSerializer.Deserialize<ProductDetails>((string)product.Details);
                    product.Variants = productVariant;
                    product.Details = productDetails;
                }

                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task DeleteProductAsync(Guid Id)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
            DELETE FROM DEV.Products
            WHERE Id = @Id::uuid;
            ", conn);

            cmd.Parameters.AddWithValue("Id", Id.ToString());
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteProductVariantAsync(Guid VariantId)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
            DELETE FROM DEV.ProductVariants
            WHERE Id = @VariantId::uuid;
            ", conn);

            cmd.Parameters.AddWithValue("VariantId", VariantId.ToString());
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> IsLastVariantAsync(Guid Id)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            var count = await conn.ExecuteScalarAsync<int>(@"
            SELECT COUNT(*)
            FROM DEV.ProductVariants pv
            LEFT JOIN DEV.Products p ON p.Id = pv.product_id
            WHERE p.Id = @Id::uuid;
            ", new { Id = Id });

            return count == 1;
        }
    }
}

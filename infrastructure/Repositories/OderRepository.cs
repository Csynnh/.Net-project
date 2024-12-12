using Dapper;
using Npgsql;
using infrastructure.DataModels;
using System.Text.Json;
using infrastructure.QueryModels;

namespace infrastructure.Repositories;

public interface IOderRepository
{
    Task<ListOderResponseModel> GetOrderById(Guid id);
    Task<IEnumerable<ListOderResponseModel>> ListOrderByAccountId(Guid accountId, string status = "");
    Task<OderResponseModel> CreateOrder(Guid accountId, decimal total, Guid paymentMethodId, Guid shippingMethodId, Guid storedInformationId, DateTime created_at);
    Task<IEnumerable<RetrieveChartDataResponse>> RetrieveChartData(DateTime startDate, DateTime endDate);
}
public class OderRepository : IOderRepository
{
    private NpgsqlDataSource _dataSource;

    public OderRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<ListOderResponseModel> GetOrderById(Guid id)
    {
        var sql = $@"
            SELECT
            od.id,
            od.account_id,
            od.created_at,
            od.total,
            od.status,
            pm.payment_method,
            jsonb_build_object(
                'id', sm.id,
                'shipping_name', sm.shipping_name,
                'shipping_cost', sm.shipping_cost
            ) AS shipping_method,
            usi.info,
            (
            SELECT json_agg(row_to_json(lp))
            FROM (
                SELECT
                p.Name,
                p.Price,
                p.Inventory,
                jsonb_agg(jsonb_build_object(
                    'Images', pv.Images::json,
                    'Size', s.Size,
                    'Color', c.Color
                )) AS Variants,
                t.Type
                FROM DEV.Products p
                JOIN DEV.ProductVariants pv ON p.Id = pv.Product_Id
                JOIN DEV.Sizes s ON pv.Size_Id = s.Id
                JOIN DEV.Colors c ON pv.Color_Id = c.Id
                JOIN DEV.Types t ON p.Type_Id = t.Id
                JOIN DEV.ORDERDETAILS odt ON odt.product_variant_id = pv.id
                WHERE odt.order_id = od.id
                GROUP BY p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type
            ) lp
            ) AS list_products
            FROM DEV.ORDERS od
            LEFT JOIN DEV.PAYMENTMETHODS pm ON pm.id = od.payment_method_id
            LEFT JOIN DEV.SHIPPINGMETHODS sm ON sm.id = od.shipping_method_id
            LEFT JOIN DEV.USERSTOREDINFOMATION usi ON usi.id = od.stored_information_id
            WHERE od.id = @id;
        ";
        using var conn = _dataSource.OpenConnection();
        var responses = await conn.QuerySingleAsync<dynamic>(sql, new { id });
        if (responses.id == null)
        {
            throw new Exception("Order not found");
        }
        var orders = new ListOderResponseModel
        {
            id = responses.id,
            account_id = responses.account_id,
            created_at = responses.created_at,
            total = responses.total,
            status = responses.status,
            paymend_method = JsonSerializer.Deserialize<object>(responses.payment_method.ToString()),
            user_info = JsonSerializer.Deserialize<UserInformationRequest>(responses.info.ToString()!),
            shipping_method = JsonSerializer.Deserialize<ShippingMethod>(responses.shipping_method.ToString()!),
            list_products = responses.list_products != null ? JsonSerializer.Deserialize<List<object>>(responses.list_products.ToString()!) : new List<ProductModel>()
        };

        return orders;
    }

    public async Task<IEnumerable<ListOderResponseModel>> ListOrderByAccountId(Guid accountId, string status = "")
    {
        var sql = $@"
            SELECT
            od.id,
            od.account_id,
            od.created_at,
            od.total,
            od.status,
            pm.payment_method,
            jsonb_build_object(
                'id', sm.id,
                'shipping_name', sm.shipping_name,
                'shipping_cost', sm.shipping_cost
            ) AS shipping_method,
            usi.info,
            (
            SELECT json_agg(row_to_json(lp))
            FROM (
                SELECT
                p.Name,
                p.Price,
                p.Inventory,
                t.Type,
                jsonb_agg(jsonb_build_object(
                    'Images', pv.Images->>'ImageThumbnail',
                    'Size', s.Size,
                    'Color', c.Color,
                    'Quantity', odt.Quantity
                )) AS Variants
                FROM DEV.Products p
                JOIN DEV.ProductVariants pv ON p.Id = pv.Product_Id
                JOIN DEV.Sizes s ON pv.Size_Id = s.Id
                JOIN DEV.Colors c ON pv.Color_Id = c.Id
                JOIN DEV.Types t ON p.Type_Id = t.Id
                JOIN DEV.ORDERDETAILS odt ON odt.product_variant_id = pv.id
                WHERE odt.order_id = od.id
                GROUP BY p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type
            ) lp
            ) AS list_products
            FROM DEV.ORDERS od
            LEFT JOIN DEV.PAYMENTMETHODS pm ON pm.id = od.payment_method_id
            LEFT JOIN DEV.SHIPPINGMETHODS sm ON sm.id = od.shipping_method_id
            LEFT JOIN DEV.USERSTOREDINFOMATION usi ON usi.id = od.stored_information_id
            WHERE od.account_id = @accountId" + (string.IsNullOrEmpty(status) ? "" : " AND od.status = @status") + @"
            ORDER BY od.created_at DESC;
        ";
        using var conn = _dataSource.OpenConnection();
        var responses = await conn.QueryAsync<dynamic>(sql, new { accountId, status });
        var orders = responses.Select(x => new ListOderResponseModel
        {
            id = x.id,
            account_id = x.account_id,
            created_at = x.created_at,
            total = x.total,
            status = x.status,
            paymend_method = JsonSerializer.Deserialize<object>(x.payment_method.ToString()),
            user_info = JsonSerializer.Deserialize<UserInformationRequest>(x.info.ToString()!),
            shipping_method = JsonSerializer.Deserialize<ShippingMethod>(x.shipping_method.ToString()!),
            list_products = x.list_products != null ? JsonSerializer.Deserialize<List<object>>(x.list_products.ToString()!) : new List<ProductModel>()
        }).ToList();

        return orders;
    }
    public async Task<IEnumerable<ListOderResponseModel>> ListOrderByStatus(string status, Guid? accountId = null)
    {
        // Xây dựng câu lệnh SQL
        var sql = $@"
        SELECT
        od.id,
        od.account_id,
        od.created_at,
        od.total,
        od.status,
        pm.payment_method,
        jsonb_build_object(
            'id', sm.id,
            'shipping_name', sm.shipping_name,
            'shipping_cost', sm.shipping_cost
        ) AS shipping_method,
        usi.info,
        (
        SELECT json_agg(row_to_json(lp))
        FROM (
            SELECT
            p.Name,
            p.Price,
            p.Inventory,
            t.Type,
            jsonb_agg(jsonb_build_object(
                'Images', pv.Images->>'ImageThumbnail',
                'Size', s.Size,
                'Color', c.Color,
                'Quantity', odt.Quantity
            )) AS Variants
            FROM DEV.Products p
            JOIN DEV.ProductVariants pv ON p.Id = pv.Product_Id
            JOIN DEV.Sizes s ON pv.Size_Id = s.Id
            JOIN DEV.Colors c ON pv.Color_Id = c.Id
            JOIN DEV.Types t ON p.Type_Id = t.Id
            JOIN DEV.ORDERDETAILS odt ON odt.product_variant_id = pv.id
            WHERE odt.order_id = od.id
            GROUP BY p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type
        ) lp
        ) AS list_products
        FROM DEV.ORDERS od
        LEFT JOIN DEV.PAYMENTMETHODS pm ON pm.id = od.payment_method_id
        LEFT JOIN DEV.SHIPPINGMETHODS sm ON sm.id = od.shipping_method_id
        LEFT JOIN DEV.USERSTOREDINFOMATION usi ON usi.id = od.stored_information_id
        WHERE (@status = 'ALL' OR od.status = @status) AND (@accountId IS NULL OR od.account_id = @accountId)
        ORDER BY od.created_at DESC;
    ";

        using var conn = _dataSource.OpenConnection();
        var responses = await conn.QueryAsync<dynamic>(sql, new { status, accountId });

        // Ánh xạ dữ liệu trả về từ query vào các đối tượng của ứng dụng
        var orders = responses.Select(x => new ListOderResponseModel
        {
            id = x.id,
            account_id = x.account_id,
            created_at = x.created_at,
            total = x.total,
            status = x.status,
            paymend_method = JsonSerializer.Deserialize<object>(x.payment_method.ToString()),
            user_info = JsonSerializer.Deserialize<UserInformationRequest>(x.info.ToString()!),
            shipping_method = JsonSerializer.Deserialize<ShippingMethod>(x.shipping_method.ToString()!),
            list_products = x.list_products != null ? JsonSerializer.Deserialize<List<object>>(x.list_products.ToString()!) : new List<ProductModel>()
        }).ToList();

        return orders;
    }

    public async Task<List<OrderStatusSummary>> GetTotalOrdersGroupedByStatus()
    {
        var sql = @"
        SELECT status, COUNT(*) as total
        FROM DEV.ORDERS
        GROUP BY status;
    ";

        using var conn = _dataSource.OpenConnection();
        var results = await conn.QueryAsync<OrderStatusSummary>(sql);

        return results.ToList();
    }

    public async Task<OderResponseModel> CreateOrder(Guid accountId, decimal total, Guid paymentMethodId, Guid shippingMethodId, Guid storedInformationId, DateTime created_at)
    {
        using var conn = _dataSource.OpenConnection();
        var sql = $@"
            INSERT INTO DEV.ORDERS (account_id, total, payment_method_id, shipping_method_id, stored_information_id, status, created_at)
            VALUES (@accountId, @total, @paymentMethodId, @shippingMethodId, @storedInformationId, 'CONFIRMING', @created_at)
            RETURNING id, account_id, total, payment_method_id, shipping_method_id, stored_information_id, status, created_at;
        ";
        return await conn.QueryFirstAsync<OderResponseModel>(sql, new { accountId, total, paymentMethodId, shippingMethodId, storedInformationId, created_at });
    }

    private string GetNextStatus(string currentStatus)
    {
        return currentStatus switch
        {
            "CONFIRMING" => "PREPARING",
            "PREPARING" => "SHIPPING",
            "SHIPPING" => "SUCCESSFULLY",
            _ => throw new InvalidOperationException("Invalid current status or no further status available")
        };
    }

    public async Task<bool> UpdateToNextOrderStatus(Guid orderId, string currentStatus)
    {
        var nextStatus = GetNextStatus(currentStatus);

        const string sql = "UPDATE DEV.ORDERS SET Status = @NextStatus WHERE Id = @OrderId AND Status = @CurrentStatus";
        using var conn = _dataSource.OpenConnection();
        var result = await conn.ExecuteAsync(sql, new { OrderId = orderId, CurrentStatus = currentStatus, NextStatus = nextStatus });
        return result > 0;
    }

    public async Task<IEnumerable<RetrieveChartDataResponse>> RetrieveChartData(DateTime startDate, DateTime endDate)
    {
        var sql = $@"
            SELECT
                od.id,
                10 AS tax_rate,
                odt.quantity AS units_sold,
                p.price AS price,
                p.name AS item_name,
                t.type AS type,
                od.created_at
            FROM DEV.ORDERS od
            LEFT JOIN DEV.ORDERDETAILS odt ON odt.order_id = od.id
            LEFT JOIN DEV.PRODUCTVARIANTS pv ON pv.id = odt.product_variant_id
            LEFT JOIN DEV.PRODUCTS p ON p.id = pv.product_id
            LEFT JOIN DEV.TYPES t ON t.id = p.type_id
            WHERE od.created_at >= @startDate::timestamp
            AND od.created_at < @endDate::timestamp
            GROUP BY created_at, od.id, odt.quantity, p.price, p.name, t.type
            ORDER BY created_at, od.id, odt.quantity, p.price, p.name, t.type;
        ";
        using var conn = _dataSource.OpenConnection();
        return await conn.QueryAsync<RetrieveChartDataResponse>(sql, new { startDate, endDate });
    }
}

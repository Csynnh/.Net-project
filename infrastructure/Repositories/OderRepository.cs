using Dapper;
using Npgsql;
using infrastructure.DataModels;
using System.Text.Json;
using infrastructure.QueryModels;

namespace infrastructure.Repositories;

public interface IOderRepository
{
    Task<ListOderResponseModel> GetOrderById(Guid id);
    Task<IEnumerable<ListOderResponseModel>> ListOrderByAccountId(Guid accountId);
    Task<OderResponseModel> CreateOrder(Guid accountId, decimal total, Guid paymentMethodId, Guid shippingMethodId, Guid storedInformationId, string status);
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
                FROM NOIRTEST.Products p
                JOIN NOIRTEST.ProductVariants pv ON p.Id = pv.Product_Id
                JOIN NOIRTEST.Sizes s ON pv.Size_Id = s.Id
                JOIN NOIRTEST.Colors c ON pv.Color_Id = c.Id
                JOIN NOIRTEST.Types t ON p.Type_Id = t.Id
                JOIN NOIRTEST.ORDERDETAILS odt ON odt.product_variant_id = pv.id
                WHERE odt.order_id = od.id
                GROUP BY p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type
            ) lp
            ) AS list_products
            FROM NOIRTEST.ORDERS od
            LEFT JOIN NOIRTEST.PAYMENTMETHODS pm ON pm.id = od.payment_method_id
            LEFT JOIN NOIRTEST.SHIPPINGMETHODS sm ON sm.id = od.shipping_method_id
            LEFT JOIN NOIRTEST.USERSTOREDINFOMATION usi ON usi.id = od.stored_information_id
            WHERE od.id = @id;
        ";
        using var conn = _dataSource.OpenConnection();
        var response = await conn.QuerySingleAsync<dynamic>(sql, new { id });
        if (response.id == null)
        {
            throw new Exception("Order not found");
        }
        var order = new ListOderResponseModel
        {
            id = response.id,
            account_id = response.account_id,
            created_at = response.created_at,
            total = response.total,
            status = response.status
        };
        return order;
    }

    public async Task<IEnumerable<ListOderResponseModel>> ListOrderByAccountId(Guid accountId)
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
                FROM NOIRTEST.Products p
                JOIN NOIRTEST.ProductVariants pv ON p.Id = pv.Product_Id
                JOIN NOIRTEST.Sizes s ON pv.Size_Id = s.Id
                JOIN NOIRTEST.Colors c ON pv.Color_Id = c.Id
                JOIN NOIRTEST.Types t ON p.Type_Id = t.Id
                JOIN NOIRTEST.ORDERDETAILS odt ON odt.product_variant_id = pv.id
                WHERE odt.order_id = od.id
                GROUP BY p.Name, p.Description, p.Price, p.Inventory, p.Details::text, t.Type
            ) lp
            ) AS list_products
            FROM NOIRTEST.ORDERS od
            LEFT JOIN NOIRTEST.PAYMENTMETHODS pm ON pm.id = od.payment_method_id
            LEFT JOIN NOIRTEST.SHIPPINGMETHODS sm ON sm.id = od.shipping_method_id
            LEFT JOIN NOIRTEST.USERSTOREDINFOMATION usi ON usi.id = od.stored_information_id
            WHERE od.account_id = @accountId;
        ";
        using var conn = _dataSource.OpenConnection();
        var responses = await conn.QueryAsync<dynamic>(sql, new { accountId });
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

    public async Task<OderResponseModel> CreateOrder(Guid accountId, decimal total, Guid paymentMethodId, Guid shippingMethodId, Guid storedInformationId, string status)
    {
        var sql = $@"
            INSERT INTO NOIRTEST.ORDERS (account_id, total, payment_method_id, shipping_method_id, stored_information_id, status)
            VALUES (@accountId, @total, @paymentMethodId, @shippingMethodId, @storedInformationId, @status)
            RETURNING id, account_id, total, payment_method_id, shipping_method_id, stored_information_id, status;
        ";
        using var conn = _dataSource.OpenConnection();
        return await conn.QueryFirstAsync<OderResponseModel>(sql, new { accountId, total, paymentMethodId, shippingMethodId, storedInformationId, status });
    }
}

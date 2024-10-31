using Dapper;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;
public class OderRepository
{
    private NpgsqlDataSource _dataSource;

    public OderRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<OderResponseModel> ListOrderByAccountId(Guid accountId)
    {
        var sql = $@"
            SELECT
            od.id,
            od.account_id,
            od.created_at,
            od.total,
            od.status,
            pm.payment_method,
            sm.shipping_method,
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
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<OderResponseModel>(sql, new { accountId });
        }
    }

    public OderResponseModel CreateOrder(Guid accountId, decimal total, Guid paymentMethodId, Guid shippingMethodId, Guid storedInformationId, string status)
    {
        var sql = $@"
            INSERT INTO NOIRTEST.ORDERS (account_id, total, payment_method_id, shipping_method_id, stored_information_id, status)
            VALUES (@accountId, @total, @paymentMethodId, @shippingMethodId, @storedInformationId, @status)
        ";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<OderResponseModel>(sql, new { accountId, total, paymentMethodId, shippingMethodId, storedInformationId });
        }
    }

//     public Invoice CreateInvoice(Guid accountId, decimal total, Status status, Checkout_method checkout_method, Shipping_method shipping_method)
//     {
//         var sql = $@"
// INSERT INTO invoices (account_id, total, status, checkout_method, shipping_method)
// VALUES (@accountId, @total, @status, @checkoutMethod, @shippingMethod)
// RETURNING id as {nameof(Invoice.id)}, 
//           account_id as {nameof(Invoice.account_id)}, 
//           created_at as {nameof(Invoice.created_date)}, 
//           total as {nameof(Invoice.total)}, 
//           status as {nameof(Invoice.status)}, 
//           checkout_method as {nameof(Invoice.checkout_method)}, 
//           shipping_method as {nameof(Invoice.shipping_method)};
// ";
//         using (var conn = _dataSource.OpenConnection())
//         {
//             return conn.QueryFirst<Invoice>(sql, new { accountId, total, status, checkout_method, shipping_method });
//         }
//     }

//     public Invoice UpdateInvoice(Guid invoiceIde, decimal total, Status status, Checkout_method checkoutMethod, Shipping_method shippingMethod)
//     {
//         var sql = $@"
// UPDATE invoices
// SET total = @total, status = @status, checkout_method = @checkoutMethod, shipping_method = @shippingMethod
// WHERE id = @invoiceId
// RETURNING id as {nameof(Invoice.id)}, 
//           account_id as {nameof(Invoice.account_id)}, 
//           created_at as {nameof(Invoice.created_date)}, 
//           total as {nameof(Invoice.total)}, 
//           status as {nameof(Invoice.status)}, 
//           checkout_method as {nameof(Invoice.checkout_method)}, 
//           shipping_method as {nameof(Invoice.shipping_method)};
// ";
//         using (var conn = _dataSource.OpenConnection())
//         {
//             return conn.QueryFirst<Invoice>(sql, new { total, status, checkoutMethod, shippingMethod });
//         }
//     }

//     public bool DeleteInvoice(Guid invoiceId)
//     {
//         var sql = @"DELETE FROM invoices WHERE id = @invoiceId;";
//         using (var conn = _dataSource.OpenConnection())
//         {
//             return conn.Execute(sql, new { invoiceId }) == 1;
//         }
//     }
}

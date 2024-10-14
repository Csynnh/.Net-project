using Dapper;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
using Npgsql;

namespace infrastructure.Repositories;
public class InvoiceRepository
{
    private NpgsqlDataSource _dataSource;
    
    public InvoiceRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<Invoice> GetAllInvoices()
    {
        var sql = @"
SELECT id as Id, account_id as AccountId, created_at as CreatedAt, total as Total, status as Status, 
       checkout_method as CheckoutMethod, shipping_method as ShippingMethod
FROM invoices;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Invoice>(sql);
        }
    }

    public Invoice CreateInvoice(Guid accountId, decimal total, Status status, Checkout_method checkout_method, Shipping_method shipping_method)
    {
        var sql = @"
INSERT INTO invoices (account_id, total, status, checkout_method, shipping_method)
VALUES (@accountId, @total, @status, @checkoutMethod, @shippingMethod)
RETURNING id as Id, account_id as AccountId, created_at as CreatedAt, total as Total, 
          status as Status, checkout_method as CheckoutMethod, shipping_method as ShippingMethod;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Invoice>(sql, new { accountId, total, status, checkout_method, shipping_method });
        }
    }

    public Invoice UpdateInvoice(Guid invoiceId, decimal total, Status status, Checkout_method checkoutMethod, Shipping_method shippingMethod)
    {
        var sql = @"
UPDATE invoices
SET total = @total, status = @status, checkout_method = @checkoutMethod, shipping_method = @shippingMethod
WHERE id = @invoiceId
RETURNING id as Id, account_id as AccountId, created_at as CreatedAt, total as Total, 
          status as Status, checkout_method as CheckoutMethod, shipping_method as ShippingMethod;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Invoice>(sql, new { invoiceId, total, status, checkoutMethod, shippingMethod });
        }
    }

    public bool DeleteInvoice(Guid invoiceId)
    {
        var sql = @"DELETE FROM invoices WHERE id = @invoiceId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { invoiceId }) == 1;
        }
    }
}
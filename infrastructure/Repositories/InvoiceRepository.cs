using Dapper;
using infrastructure.DataModels;
using infrastructure.EnumVariables;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;
public class InvoiceRepository
{
    private NpgsqlDataSource _dataSource;

    public InvoiceRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<Invoice> GetInvoiceForFeed()
    {
        var sql = $@"
SELECT id as {nameof(InvoiceFeedQuery.id)}, 
       account_id as {nameof(InvoiceFeedQuery.account_id)}, 
       created_at as {nameof(InvoiceFeedQuery.created_date)}, 
       total as {nameof(InvoiceFeedQuery)}, 
       status as {nameof(InvoiceFeedQuery.status)}, 
       checkout_method as {nameof(InvoiceFeedQuery.checkout_method)}, 
       shipping_method as {nameof(InvoiceFeedQuery.shipping_method)}
FROM invoices;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Invoice>(sql);
        }
    }

    public Invoice CreateInvoice(Guid accountId, decimal total, Status status, Checkout_method checkoutMethod, Shipping_method shippingMethod)
    {
        var sql = $@"
INSERT INTO invoices (account_id, total, status, checkout_method, shipping_method)
VALUES (@accountId, @total, @status, @checkoutMethod, @shippingMethod)
RETURNING id as {nameof(Invoice.id)}, 
          account_id as {nameof(Invoice.account_id)}, 
          created_at as {nameof(Invoice.created_date)}, 
          total as {nameof(Invoice.total)}, 
          status as {nameof(Invoice.status)}, 
          checkout_method as {nameof(Invoice.checkout_method)}, 
          shipping_method as {nameof(Invoice.shipping_method)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Invoice>(sql, new { accountId, total, status, checkoutMethod, shippingMethod });
        }
    }

    public Invoice UpdateInvoice(Guid invoiceId, decimal total, Status status, Checkout_method checkoutMethod, Shipping_method shippingMethod)
    {
        var sql = $@"
UPDATE invoices
SET total = @total, status = @status, checkout_method = @checkoutMethod, shipping_method = @shippingMethod
WHERE id = @invoiceId
RETURNING id as {nameof(Invoice.id)}, 
          account_id as {nameof(Invoice.account_id)}, 
          created_at as {nameof(Invoice.created_date)}, 
          total as {nameof(Invoice.total)}, 
          status as {nameof(Invoice.status)}, 
          checkout_method as {nameof(Invoice.checkout_method)}, 
          shipping_method as {nameof(Invoice.shipping_method)};
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

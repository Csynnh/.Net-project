using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;
public class InvoiceDetailRepository
{
    private NpgsqlDataSource _dataSource;

    public InvoiceDetailRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<InvoiceDetail> GetInvoiceDetailForFeed()
    {
        var sql = $@"
SELECT invoice_id as {nameof(InvoiceDetailFeedQuey.invoices_id)}, 
       product_id as {nameof(InvoiceDetailFeedQuey.invoices_id)}, 
       amount as {nameof(InvoiceDetailFeedQuey.invoices_id)}, 
       price as {nameof(InvoiceDetailFeedQuey.price)}
FROM invoicedetails
WHERE invoice_id = @invoiceId;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<InvoiceDetail>(sql);
        }
    }

    public InvoiceDetail CreateInvoiceDetail(Guid invoiceId, Guid productId, int amount, decimal price)
    {
        var sql = $@"
INSERT INTO invoicedetails (invoice_id, product_id, amount, price)
VALUES (@invoiceId, @productId, @amount, @price)
RETURNING invoice_id as {nameof(InvoiceDetail.invoices_id)}, 
          product_id as {nameof(InvoiceDetail.invoices_id)}, 
          amount as {nameof(InvoiceDetail.invoices_id)}, 
          price as {nameof(InvoiceDetail.price)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<InvoiceDetail>(sql, new { invoiceId, productId, amount, price });
        }
    }

    
    public InvoiceDetail UpdateInvoiceDetail(Guid invoiceId, Guid productId, int amount, decimal price)
    {
        var sql = $@"
UPDATE invoicedetails
SET amount = @amount, price = @price
WHERE invoice_id = @invoiceId AND product_id = @productId
RETURNING invoice_id as {nameof(InvoiceDetail.invoices_id)}, 
          product_id as {nameof(InvoiceDetail.product_id)}, 
          amount as {nameof(InvoiceDetail.amount)}, 
          price as {nameof(InvoiceDetail.price)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<InvoiceDetail>(sql, new { invoiceId, productId, amount, price });
        }
    }

    public bool DeleteInvoiceDetail(Guid invoiceId, Guid productId)
    {
        var sql = @"DELETE FROM invoicedetails WHERE invoice_id = @invoiceId AND product_id = @productId;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { invoiceId, productId }) == 1;
        }
    }
}
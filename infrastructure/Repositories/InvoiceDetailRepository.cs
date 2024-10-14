using Dapper;
using infrastructure.DataModels;
using Npgsql;

namespace infrastructure.Repositories;
public class InvoiceDetailRepository
{
    private NpgsqlDataSource _dataSource;

    public InvoiceDetailRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<InvoiceDetail> GetInvoiceDetailsByInvoiceId(Guid invoiceId)
    {
        var sql = @"
SELECT invoice_id as InvoiceId, product_id as ProductId, amount as Amount, price as Price
FROM invoicedetails
WHERE invoice_id = @invoiceId;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<InvoiceDetail>(sql, new { invoiceId });
        }
    }

    public InvoiceDetail CreateInvoiceDetail(Guid invoiceId, Guid productId, int amount, decimal price)
    {
        var sql = @"
INSERT INTO invoicedetails (invoice_id, product_id, amount, price)
VALUES (@invoiceId, @productId, @amount, @price)
RETURNING invoice_id as InvoiceId, product_id as ProductId, amount as Amount, price as Price;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<InvoiceDetail>(sql, new { invoiceId, productId, amount, price });
        }
    }

    public InvoiceDetail UpdateInvoiceDetail(Guid invoiceId, Guid productId, int amount, decimal price)
    {
        var sql = @"
UPDATE invoicedetails
SET amount = @amount, price = @price
WHERE invoice_id = @invoiceId AND product_id = @productId
RETURNING invoice_id as InvoiceId, product_id as ProductId, amount as Amount, price as Price;
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
using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class InvoiceDetailRepository
{
    private NpgsqlDataSource _dataSource;

    public InvoiceDetailRepository(NpgsqlDataSource datasource)
    {
        _dataSource = datasource;
    }

    public IEnumerable<InvoiceDetail> GetInvoiceDetails()
    {
        string sql = $@"
SELECT invoices_id  as {nameof(InvoiceDetail.invoices_id )},
       product_id  as {nameof(InvoiceDetail.product_id )},
       amount as {nameof(InvoiceDetail.amount)},
       price as {nameof(InvoiceDetail.price)}
FROM invoice_details;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<InvoiceDetail>(sql);
        }
    }

    public InvoiceDetail CreateInvoiceDetail(int invoices_id , int product_id, int amount, decimal price)
    {
        var sql = $@"
INSERT INTO invoice_details (invoices_id , product_id , amount, price) 
VALUES (@invoices_id , @product_id, @amount, @price)
RETURNING invoices_id  as {nameof(InvoiceDetail.invoices_id )},
          product_id  as {nameof(InvoiceDetail.product_id )},
          amount as {nameof(InvoiceDetail.amount)},
          price as {nameof(InvoiceDetail.price)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<InvoiceDetail>(sql, new { invoices_id , product_id, amount, price });
        }
    }

    public InvoiceDetail UpdateInvoiceDetail(int invoices_id , int product_id, int amount, decimal price)
    {
        var sql = @"
        UPDATE noir.invoice_details 
        SET amount = @amount, 
            price = @price
        WHERE invoices_id = @invoices_id  AND product_id = @product_id
        RETURNING invoices_id as {nameof(InvoiceDetail.invoices_id )},
                   product_id as {nameof(InvoiceDetail.product_id )},
                   amount as {nameof(InvoiceDetail.amount)},
                   price as {nameof(InvoiceDetail.price)};
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<InvoiceDetail>(sql, new { invoices_id , product_id, amount, price });
        }
    }

    public bool DeleteInvoiceDetail(int invoices_id , int product_id)
    {
        var sql = @"DELETE FROM invoice_details WHERE invoices_id  = @invoices_id  AND product_id  = @product_id;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { invoices_id , product_id }) == 1;
        }
    }
}

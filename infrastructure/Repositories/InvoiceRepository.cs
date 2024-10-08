using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class InvoiceRepository
{
    private NpgsqlDataSource _dataSource;

    public InvoiceRepository(NpgsqlDataSource datasource)
    {
        _dataSource = datasource;
    }

    public IEnumerable<Invoice> GetInvoices()
    {
        string sql = $@"
SELECT id  as {nameof(Invoice.id )},
       account_id  as {nameof(Invoice.account_id )},
       created_date as {nameof(Invoice.created_date)},
       price as {nameof(Invoice.price)},
       status as {nameof(Invoice.status)}
FROM invoices;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Invoice>(sql);
        }
    }

    public Invoice CreateInvoice(int account_id, DateTime created_date, decimal price, string status)
    {
        var sql = $@"
INSERT INTO invoices (account_id , created_date, price, status) 
VALUES (@account_id, @created_date, @price, @status)
RETURNING id  as {nameof(Invoice.id )},
          account_id  as {nameof(Invoice.account_id )},
          created_date as {nameof(Invoice.created_date)},
          price as {nameof(Invoice.price)},
          status as {nameof(Invoice.status)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Invoice>(sql, new { account_id, created_date, price, status });
        }
    }

    public bool DeleteInvoice(int id)
    {
        var sql = @"DELETE FROM invoices WHERE id  = @id;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { id }) == 1;
        }
    }

    public Invoice UpdateInvoice(int id, int account_id , DateTime created_date, decimal price, string status)
    {
        var sql = @"
        UPDATE noir.invoices 
        SET account_id = @id, 
            created_date = @created_date, 
            price = @price, 
            status = @status
        WHERE id = @id
        RETURNING id as {nameof(Invoice.id )},
                   account_id as {nameof(Invoice.account_id )},
                   created_date as {nameof(Invoice.created_date)},
                   price as {nameof(Invoice.price)},
                   status as {nameof(Invoice.status)};
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Invoice>(sql, new { id, account_id, created_date, price, status });
        }
    }
}

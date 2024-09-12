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
SELECT id_hoadon as {nameof(InvoiceDetail.ID_HoaDon)},
       id_hanghoa as {nameof(InvoiceDetail.ID_HangHoa)},
       soluong as {nameof(InvoiceDetail.SoLuong)},
       gia as {nameof(InvoiceDetail.Gia)}
FROM invoice_details;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<InvoiceDetail>(sql);
        }
    }

    public InvoiceDetail CreateInvoiceDetail(int idHoaDon, int idHangHoa, int soLuong, decimal gia)
    {
        var sql = $@"
INSERT INTO invoice_details (id_hoadon, id_hanghoa, soluong, gia) 
VALUES (@idHoaDon, @idHangHoa, @soLuong, @gia)
RETURNING id_hoadon as {nameof(InvoiceDetail.ID_HoaDon)},
          id_hanghoa as {nameof(InvoiceDetail.ID_HangHoa)},
          soluong as {nameof(InvoiceDetail.SoLuong)},
          gia as {nameof(InvoiceDetail.Gia)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<InvoiceDetail>(sql, new { idHoaDon, idHangHoa, soLuong, gia });
        }
    }

    public InvoiceDetail UpdateInvoiceDetail(int idHoaDon, int idHangHoa, int soLuong, decimal gia)
    {
        var sql = @"
        UPDATE library_app.invoice_details 
        SET so_luong = @soLuong, 
            gia = @gia
        WHERE id_hoa_don = @idHoaDon AND id_hang_hoa = @idHangHoa
        RETURNING id_hoa_don as {nameof(InvoiceDetail.ID_HoaDon)},
                   id_hang_hoa as {nameof(InvoiceDetail.ID_HangHoa)},
                   so_luong as {nameof(InvoiceDetail.SoLuong)},
                   gia as {nameof(InvoiceDetail.Gia)};
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<InvoiceDetail>(sql, new { idHoaDon, idHangHoa, soLuong, gia });
        }
    }

    public bool DeleteInvoiceDetail(int idHoaDon, int idHangHoa)
    {
        var sql = @"DELETE FROM invoice_details WHERE id_hoadon = @idHoaDon AND id_hanghoa = @idHangHoa;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { idHoaDon, idHangHoa }) == 1;
        }
    }
}

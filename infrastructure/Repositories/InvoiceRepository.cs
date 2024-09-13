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
SELECT id_hoadon as {nameof(Invoice.ID_HoaDon)},
       id_taikhoan as {nameof(Invoice.ID_TaiKhoan)},
       ngayxuathoadon as {nameof(Invoice.NgayXuatHoaDon)},
       tongtien as {nameof(Invoice.TongTien)},
       trangthai as {nameof(Invoice.TrangThai)}
FROM invoices;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Invoice>(sql);
        }
    }

    public Invoice CreateInvoice(int idTaiKhoan, DateTime ngayXuatHoaDon, decimal tongTien, string trangThai)
    {
        var sql = $@"
INSERT INTO invoices (id_taikhoan, ngayxuathoadon, tongtien, trangthai) 
VALUES (@idTaiKhoan, @ngayXuatHoaDon, @tongTien, @trangThai)
RETURNING id_hoadon as {nameof(Invoice.ID_HoaDon)},
          id_taikhoan as {nameof(Invoice.ID_TaiKhoan)},
          ngayxuathoadon as {nameof(Invoice.NgayXuatHoaDon)},
          tongtien as {nameof(Invoice.TongTien)},
          trangthai as {nameof(Invoice.TrangThai)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Invoice>(sql, new { idTaiKhoan, ngayXuatHoaDon, tongTien, trangThai });
        }
    }

    public bool DeleteInvoice(int idHoaDon)
    {
        var sql = @"DELETE FROM invoices WHERE id_hoadon = @idHoaDon;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { idHoaDon }) == 1;
        }
    }

    public Invoice UpdateInvoice(int idHoaDon, int idTaiKhoan, DateTime ngayXuatHoaDon, decimal tongTien, string trangThai)
    {
        var sql = @"
        UPDATE library_app.invoices 
        SET id_tai_khoan = @idTaiKhoan, 
            ngay_xuat_hoa_don = @ngayXuatHoaDon, 
            tong_tien = @tongTien, 
            trang_thai = @trangThai
        WHERE id_hoa_don = @idHoaDon
        RETURNING id_hoa_don as {nameof(Invoice.ID_HoaDon)},
                   id_tai_khoan as {nameof(Invoice.ID_TaiKhoan)},
                   ngay_xuat_hoa_don as {nameof(Invoice.NgayXuatHoaDon)},
                   tong_tien as {nameof(Invoice.TongTien)},
                   trang_thai as {nameof(Invoice.TrangThai)};
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Invoice>(sql, new { idHoaDon, idTaiKhoan, ngayXuatHoaDon, tongTien, trangThai });
        }
    }
}

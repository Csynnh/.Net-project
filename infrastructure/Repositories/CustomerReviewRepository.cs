using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class CustomerReviewRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public CustomerReviewRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public IEnumerable<CustomerReview> GetCustomerReviews()
    {
        string sql = @"
        SELECT id_nhan_xet as {nameof(CustomerReview.ID_NhanXet)},
               id_taikhoan as {nameof(CustomerReview.ID_TaiKhoan)},
               id_hanghoa as {nameof(CustomerReview.ID_HangHoa)},
               noi_dung as {nameof(CustomerReview.NoiDung)},
               danh_gia as {nameof(CustomerReview.DanhGia)},
               ngay_nhan_xet as {nameof(CustomerReview.NgayNhanXet)}
        FROM library_app.customer_reviews;
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<CustomerReview>(sql);
        }
    }

    public CustomerReview CreateCustomerReview(int idTaiKhoan, int idHangHoa, string noiDung, int danhGia, DateTime ngayNhanXet)
    {
        var sql = @"
        INSERT INTO library_app.customer_reviews (id_taikhoan, id_hanghoa, noi_dung, danh_gia, ngay_nhan_xet)
        VALUES (@idTaiKhoan, @idHangHoa, @noiDung, @danhGia, @ngayNhanXet)
        RETURNING id_nhan_xet as {nameof(CustomerReview.ID_NhanXet)},
                   id_taikhoan as {nameof(CustomerReview.ID_TaiKhoan)},
                   id_hanghoa as {nameof(CustomerReview.ID_HangHoa)},
                   noi_dung as {nameof(CustomerReview.NoiDung)},
                   danh_gia as {nameof(CustomerReview.DanhGia)},
                   ngay_nhan_xet as {nameof(CustomerReview.NgayNhanXet)};
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<CustomerReview>(sql, new { idTaiKhoan, idHangHoa, noiDung, danhGia, ngayNhanXet });
        }
    }

    public CustomerReview UpdateCustomerReview(int idNhanXet, int idTaiKhoan, int idHangHoa, string noiDung, int danhGia, DateTime ngayNhanXet)
    {
        var sql = @"
        UPDATE library_app.customer_reviews 
        SET id_taikhoan = @idTaiKhoan, 
            id_hanghoa = @idHangHoa, 
            noi_dung = @noiDung, 
            danh_gia = @danhGia, 
            ngay_nhan_xet = @ngayNhanXet
        WHERE id_nhan_xet = @idNhanXet
        RETURNING id_nhan_xet as {nameof(CustomerReview.ID_NhanXet)},
                   id_taikhoan as {nameof(CustomerReview.ID_TaiKhoan)},
                   id_hanghoa as {nameof(CustomerReview.ID_HangHoa)},
                   noi_dung as {nameof(CustomerReview.NoiDung)},
                   danh_gia as {nameof(CustomerReview.DanhGia)},
                   ngay_nhan_xet as {nameof(CustomerReview.NgayNhanXet)};
        ";

        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<CustomerReview>(sql, new { idNhanXet, idTaiKhoan, idHangHoa, noiDung, danhGia, ngayNhanXet });
        }
    }

    public bool DeleteCustomerReview(int idNhanXet)
    {
        var sql = @"DELETE FROM library_app.customer_reviews WHERE id_nhan_xet = @idNhanXet;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { idNhanXet }) == 1;
        }
    }
}



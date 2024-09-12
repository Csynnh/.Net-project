using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class ProductRepository
{
    private NpgsqlDataSource _dataSource;

    public ProductRepository(NpgsqlDataSource datasource)
    {
        _dataSource = datasource;
    }

    public IEnumerable<Product> GetProducts()
    {
        string sql = $@"
SELECT id_hanghoa as {nameof(Product.ID_HangHoa)},
       tenhanghoa as {nameof(Product.TenHangHoa)},
       mota as {nameof(Product.MoTa)},
       gia as {nameof(Product.Gia)},
       soluongtonkho as {nameof(Product.SoLuongTonKho)},
       hinhanh as {nameof(Product.HinhAnh)}
FROM products;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Product>(sql);
        }
    }

    public Product CreateProduct(string tenHangHoa, string moTa, decimal gia, int soLuongTonKho, string hinhAnh)
    {
        var sql = $@"
INSERT INTO products (tenhanghoa, mota, gia, soluongtonkho, hinhanh) 
VALUES (@tenHangHoa, @moTa, @gia, @soLuongTonKho, @hinhAnh)
RETURNING id_hanghoa as {nameof(Product.ID_HangHoa)},
          tenhanghoa as {nameof(Product.TenHangHoa)},
          mota as {nameof(Product.MoTa)},
          gia as {nameof(Product.Gia)},
          soluongtonkho as {nameof(Product.SoLuongTonKho)},
          hinhanh as {nameof(Product.HinhAnh)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Product>(sql, new { tenHangHoa, moTa, gia, soLuongTonKho, hinhAnh });
        }
    }

    public Product UpdateProduct(int idHangHoa, string tenHangHoa, string moTa, decimal gia, int soLuongTonKho, string hinhAnh)
    {
        var sql = $@"
UPDATE products 
SET tenhanghoa = @tenHangHoa, mota = @moTa, gia = @gia, soluongtonkho = @soLuongTonKho, hinhanh = @hinhAnh
WHERE id_hanghoa = @idHangHoa
RETURNING id_hanghoa as {nameof(Product.ID_HangHoa)},
          tenhanghoa as {nameof(Product.TenHangHoa)},
          mota as {nameof(Product.MoTa)},
          gia as {nameof(Product.Gia)},
          soluongtonkho as {nameof(Product.SoLuongTonKho)},
          hinhanh as {nameof(Product.HinhAnh)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Product>(sql, new { idHangHoa, tenHangHoa, moTa, gia, soLuongTonKho, hinhAnh });
        }
    }

    public bool DeleteProduct(int idHangHoa)
    {
        var sql = @"DELETE FROM products WHERE id_hanghoa = @idHangHoa;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { idHangHoa }) == 1;
        }
    }

    public bool DoesProductWithNameExist(string tenHangHoa)
    {
        var sql = @"SELECT COUNT(*) FROM products WHERE tenhanghoa = @tenHangHoa;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.ExecuteScalar<int>(sql, new { tenHangHoa }) == 1;
        }
    }
}
using Dapper;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Npgsql;

namespace infrastructure.Repositories;

public class AccountRepository
{
    private NpgsqlDataSource _dataSource;

    public AccountRepository(NpgsqlDataSource datasource)
    {
        _dataSource = datasource;
    }

    public IEnumerable<Account> GetAccounts()
    {
        string sql = $@"
SELECT id_taikhoan as {nameof(Account.ID_TaiKhoan)},
       tendangnhap as {nameof(Account.TenDangNhap)},
       matkhau as {nameof(Account.MatKhau)},
       hoten as {nameof(Account.HoTen)},
       email as {nameof(Account.Email)},
       sodienthoai as {nameof(Account.SoDienThoai)},
       diachi as {nameof(Account.DiaChi)},
       vaitro as {nameof(Account.VaiTro)}
FROM accounts;
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Query<Account>(sql);
        }
    }

    public Account CreateAccount(string tenDangNhap, string matKhau, string hoTen, string email, string soDienThoai, string diaChi, string vaiTro)
    {
        var sql = $@"
INSERT INTO accounts (tendangnhap, matkhau, hoten, email, sodienthoai, diachi, vaitro) 
VALUES (@tenDangNhap, @matKhau, @hoTen, @email, @soDienThoai, @diaChi, @vaiTro)
RETURNING id_taikhoan as {nameof(Account.ID_TaiKhoan)},
          tendangnhap as {nameof(Account.TenDangNhap)},
          matkhau as {nameof(Account.MatKhau)},
          hoten as {nameof(Account.HoTen)},
          email as {nameof(Account.Email)},
          sodienthoai as {nameof(Account.SoDienThoai)},
          diachi as {nameof(Account.DiaChi)},
          vaitro as {nameof(Account.VaiTro)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Account>(sql, new { tenDangNhap, matKhau, hoTen, email, soDienThoai, diaChi, vaiTro });
        }
    }

    public Account UpdateAccount(int idTaiKhoan, string tenDangNhap, string matKhau, string hoTen, string email, string soDienThoai, string diaChi, string vaiTro)
    {
        var sql = $@"
UPDATE accounts 
SET tendangnhap = @tenDangNhap, matkhau = @matKhau, hoten = @hoTen, email = @email, sodienthoai = @soDienThoai, diachi = @diaChi, vaitro = @vaiTro
WHERE id_taikhoan = @idTaiKhoan
RETURNING id_taikhoan as {nameof(Account.ID_TaiKhoan)},
          tendangnhap as {nameof(Account.TenDangNhap)},
          matkhau as {nameof(Account.MatKhau)},
          hoten as {nameof(Account.HoTen)},
          email as {nameof(Account.Email)},
          sodienthoai as {nameof(Account.SoDienThoai)},
          diachi as {nameof(Account.DiaChi)},
          vaitro as {nameof(Account.VaiTro)};
";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.QueryFirst<Account>(sql, new { idTaiKhoan, tenDangNhap, matKhau, hoTen, email, soDienThoai, diaChi, vaiTro });
        }
    }

    public bool DeleteAccount(int idTaiKhoan)
    {
        var sql = @"DELETE FROM accounts WHERE id_taikhoan = @idTaiKhoan;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.Execute(sql, new { idTaiKhoan }) == 1;
        }
    }

    public bool DoesAccountWithUsernameExist(string tenDangNhap)
    {
        var sql = @"SELECT COUNT(*) FROM accounts WHERE tendangnhap = @tenDangNhap;";
        using (var conn = _dataSource.OpenConnection())
        {
            return conn.ExecuteScalar<int>(sql, new { tenDangNhap }) == 1;
        }
    }
}

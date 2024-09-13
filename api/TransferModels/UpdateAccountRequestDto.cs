using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;


// UpdateAccountRequestDto.cs
public class UpdateAccountRequestDto
{
    public int ID_TaiKhoan { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string VaiTro { get; set; } = string.Empty;
}
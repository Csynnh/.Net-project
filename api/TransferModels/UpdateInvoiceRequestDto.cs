using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

// UpdateInvoiceRequestDto.cs
public class UpdateInvoiceRequestDto
{
    public int ID_HoaDon { get; set; }
    public int ID_TaiKhoan { get; set; }
    public DateTime NgayXuatHoaDon { get; set; }
    public decimal TongTien { get; set; }
    public string TrangThai { get; set; } = string.Empty;
}

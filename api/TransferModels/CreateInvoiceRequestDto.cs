using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;

// CreateInvoiceRequestDto.cs
public class CreateInvoiceRequestDto
{
    public int ID_HoaDon { get; set; }  
    public int ID_TaiKhoan { get; set; }  
    public DateTime NgayXuatHoaDon { get; set; }  
    public decimal TongTien { get; set; }  
    public string TrangThai { get; set; } = "Đang xử lý";  
}

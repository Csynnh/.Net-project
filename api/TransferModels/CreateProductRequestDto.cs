using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;


// CreateProductRequestDto.cs
public class CreateProductRequestDto
{
    public string TenHangHoa { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public decimal Gia { get; set; }
    public int SoLuongTonKho { get; set; }
    public string HinhAnh { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

// UpdateProductRequestDto.cs
public class UpdateProductRequestDto
{
    public int ID_HangHoa { get; set; }
    public string TenHangHoa { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public decimal Gia { get; set; }
    public int SoLuongTonKho { get; set; }
    public string HinhAnh { get; set; } = string.Empty;
}
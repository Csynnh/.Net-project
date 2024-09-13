using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

// UpdateInvoiceDetailRequestDto.cs
public class UpdateInvoiceDetailRequestDto
{
    public int ID_HoaDon { get; set; }
    public int ID_HangHoa { get; set; }
    public int SoLuong { get; set; }
    public decimal Gia { get; set; }
}
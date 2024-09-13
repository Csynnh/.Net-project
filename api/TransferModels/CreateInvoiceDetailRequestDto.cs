using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;

public class CreateInvoiceDetailRequestDto
{
    public int ID_HoaDon { get; set; }
    public int ID_HangHoa { get; set; }
    public int SoLuong { get; set; }
    public decimal Gia { get; set; }
}
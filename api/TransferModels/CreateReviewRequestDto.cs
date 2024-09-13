using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;

// CreateReviewRequestDto.cs
public class CreateReviewRequestDto
{
    public int ID_TaiKhoan { get; set; }
    public int ID_HangHoa { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public int DanhGia { get; set; }
    public DateTime NgayNhanXet { get; set; }
}
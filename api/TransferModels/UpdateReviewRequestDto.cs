using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;


// UpdateReviewRequestDto.cs
public class UpdateReviewRequestDto
{
    public int ID_NhanXet { get; set; }
    public int ID_TaiKhoan { get; set; }
    public int ID_HangHoa { get; set; }
    public string NoiDung { get; set; }  = string.Empty;
    public int DanhGia { get; set; }
    public DateTime NgayNhanXet { get; set; }
}
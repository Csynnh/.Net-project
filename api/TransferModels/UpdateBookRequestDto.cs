using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

public class UpdateBookRequestDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string CoverImgUrl { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
}
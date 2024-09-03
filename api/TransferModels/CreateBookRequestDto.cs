using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;

public class CreateBookRequestDto
{
    public string BookTitle { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string CoverImgUrl { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
}
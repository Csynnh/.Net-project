using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;


// CreateProductRequestDto.cs
public class CreateProductRequestDto
{
    public string name { get; set; } = string.Empty;
    public string desc { get; set; } = string.Empty;
    public decimal price { get; set; }
    public int inventory { get; set; }
    public string image_url { get; set; } = string.Empty;
}

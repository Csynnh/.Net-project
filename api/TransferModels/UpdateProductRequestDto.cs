using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

// UpdateProductRequestDto.cs
public class UpdateProductRequestDto
{
    public int id  { get; set; }
    public string name { get; set; } = string.Empty;
    public string desc { get; set; } = string.Empty;
    public decimal price { get; set; }
    public int inventory { get; set; }
    public string image_url { get; set; } = string.Empty;
}
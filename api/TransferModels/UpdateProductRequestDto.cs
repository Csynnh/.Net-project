using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

// UpdateProductRequestDto.cs
public class UpdateProductRequestDto
{
    public int id  { get; set; }
    public string name { get; set; } = string.Empty;
    public string desc { get; set; } = string.Empty;
    public decimal price { get; set; }
    public decimal width { get; set; }
    public decimal height { get; set; }
    public string type { get; set; } = string.Empty;
}
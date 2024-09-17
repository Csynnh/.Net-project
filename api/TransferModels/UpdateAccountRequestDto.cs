using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;


// UpdateAccountRequestDto.cs
public class UpdateAccountRequestDto
{
    public int id { get; set; }
    public string username  { get; set; } = string.Empty;
    public string password  { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string phone_number { get; set; } = string.Empty;
    public string address { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
}
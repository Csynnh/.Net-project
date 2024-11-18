using System.ComponentModel.DataAnnotations;
using infrastructure.EnumVariables;

namespace api.TransferModels;


// UpdateAccountRequestDto.cs
public class UpdateAccountRequestDto
{
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string phone_number { get; set; } = string.Empty;
}
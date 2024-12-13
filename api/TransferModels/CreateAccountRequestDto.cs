using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using infrastructure.EnumVariables;

namespace api.TransferModels;

// CreateAccountRequestDto.cs
public class CreateAccountRequestDto
{
    public string username  { get; set; } = string.Empty;
    public string password  { get; set; } = string.Empty;
    public string name  { get; set; } = string.Empty;
    public string email  { get; set; } = string.Empty;
    public string phone_number  { get; set; } = string.Empty;
    public string role  { get; set; } = "User";
}
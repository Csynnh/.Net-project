using Azure.Communication.Email;
namespace service;

public class EmailService
{
  private readonly string _connectionString;
  private readonly string _senderEmail;

  public EmailService()
  {
    _connectionString = Environment.GetEnvironmentVariable("AZURE_EMAIL_CONNECTION_STRING") ?? "";
    _senderEmail = Environment.GetEnvironmentVariable("AZURE_SENDER_ADDRESS") ?? "";
  }

  public async Task<bool> SendOtpEmailAsync(string recipientEmail, string otpCode)
  {
    var client = new EmailClient(_connectionString);
    string otpEmailTemplatePath = Environment.GetEnvironmentVariable("OTP_EMAIL_TEMPLATE_PATH") ?? "templates/OtpTemplate.html";

    var emailContent = new EmailContent("OTP change password")
    {
      PlainText = $"Your OTP code is: {otpCode}",
      Html = GetOtpTemplate(filePath: otpEmailTemplatePath, otpCode: otpCode)
    };

    var emailMessage = new EmailMessage(_senderEmail, recipientEmail, emailContent);

    try
    {
      var response = await client.SendAsync(wait: Azure.WaitUntil.Completed, emailMessage);
      Console.WriteLine($"Email status: {response.Value.Status}");
      return response.Value.Status == EmailSendStatus.Succeeded;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Failed to send email: {ex.Message}");
      return false;
    }
  }

  //User send email to brand

  public async Task<bool> RecieveEmailSentFromUser(string recipientEmail, string otpCode)
  {
    var client = new EmailClient(_connectionString);
    string otpEmailTemplatePath = Environment.GetEnvironmentVariable("OTP_EMAIL_TEMPLATE_PATH") ?? "templates/OtpTemplate.html";

    var emailContent = new EmailContent("OTP change password")
    {
      PlainText = $"Your OTP code is: {otpCode}",
      Html = GetOtpTemplate(filePath: otpEmailTemplatePath, otpCode: otpCode)
    };

    var emailMessage = new EmailMessage(_senderEmail, recipientEmail, emailContent);

    try
    {
      var response = await client.SendAsync(wait: Azure.WaitUntil.Completed, emailMessage);
      Console.WriteLine($"Email status: {response.Value.Status}");
      return response.Value.Status == EmailSendStatus.Succeeded;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Failed to send email: {ex.Message}");
      return false;
    }
  }


  private string GetOtpTemplate(string filePath, string otpCode)
  {
    // Read the HTML template file
    string template = File.ReadAllText(filePath);

    // Replace the placeholder with the actual OTP code
    return template.Replace("{{otp_code}}", otpCode);
  }


}

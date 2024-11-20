using api.Request;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class RecieveUserEmailController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _senderEmail;
        private readonly string _precieverEmail;

        public RecieveUserEmailController()
        {
            _connectionString = Environment.GetEnvironmentVariable("AZURE_EMAIL_CONNECTION_STRING") ?? "";
            _senderEmail = Environment.GetEnvironmentVariable("AZURE_SENDER_ADDRESS") ?? "";
            _precieverEmail = "6351071040@st.utc2.edu.vn";
        }

        [HttpPost]
        public async Task<bool> SendEmailContact(EmailSentByUserRequest request)
        {
            var client = new EmailClient(_connectionString);
            string emailTemplatePath = Environment.GetEnvironmentVariable("CONTACT_EMAIL_TEMPLATE_PATH") ?? "templates/ContactEmailTamplate.html";

            var emailContent = new EmailContent("Contact From User")
            {
                PlainText = $"You have new contact from user",
                // Chỗ này là những chỗ cần thay bằng biến truyền vào của email
                Html = GetContactTemplate(filePath: emailTemplatePath, request: request)
            };

            var emailMessage = new EmailMessage(_senderEmail, _precieverEmail, emailContent);

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
        private string GetContactTemplate(string filePath, EmailSentByUserRequest request)
        {
            // Read the HTML template file
            string template = System.IO.File.ReadAllText(filePath);

            // Replace the placeholder with the actual OTP code
            template = template.Replace("{{userName}}", request.userName);
            template = template.Replace("{{typeOfProduct}}", request.typeOfProduct);
            template = template.Replace("{{message}}", request.message);
            template = template.Replace("{{userPhone}}", request.userPhone);

            return template;
        }
    }
}

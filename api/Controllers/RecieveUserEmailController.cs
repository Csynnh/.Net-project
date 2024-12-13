using api.Request;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using infrastructure.DataModels;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using service;
using infrastructure.Repositories;



namespace library.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class RecieveUserEmailController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _senderEmail;
        private readonly string _precieverEmail;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly NotificationRepository _notificationRepository;

        public RecieveUserEmailController(IHubContext<NotificationHub> hubContext, NotificationRepository notificationRepository)
        {
            _connectionString = Environment.GetEnvironmentVariable("AZURE_EMAIL_CONNECTION_STRING") ?? "";
            _senderEmail = Environment.GetEnvironmentVariable("AZURE_SENDER_ADDRESS") ?? "";
            _precieverEmail = "6351071040@st.utc2.edu.vn";
            _hubContext=hubContext;
            _notificationRepository = notificationRepository;
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
                NotificationHandler(request);
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

        private async Task NotificationHandler(EmailSentByUserRequest request)
        {
            var contentContact = new NotificationContent
            {
                message = $"New contact received from {request.userName} having phone number as {request.userPhone}.",
                createdAt = DateTime.Now,  // Assuming the contact was created now
                phone = request.userPhone
            };

            Guid notificationId = await _notificationRepository.InsertNotification(new Notification
            {
                content = JsonConvert.SerializeObject(contentContact),
                created_at = DateTime.Now,
                type = "CONTACT_NOTIFICATION"
            });

            await _hubContext.Clients.All.SendAsync("ReceiveContactNotification", new NotificationResponseModel
            {
                id = notificationId,
                content = contentContact,
                created_at = DateTime.Now,
                is_read = false
            });
        }
    }
}

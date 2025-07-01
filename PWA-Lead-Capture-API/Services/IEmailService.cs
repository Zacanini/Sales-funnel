namespace PWA_Lead_Capture_API.Services;

public interface IEmailService
{
    Task<bool> SendWelcomeEmailAsync(string toEmail, string toName);
    Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string textBody = "");
}

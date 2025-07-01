using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace PWA_Lead_Capture_API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string toName)
    {
        var subject = "Bem-vindo ao PWA Master! 🚀";
        
        var htmlBody = GetWelcomeEmailTemplate(toName);
        var textBody = GetWelcomeEmailTextTemplate(toName);
        
        return await SendEmailAsync(toEmail, subject, htmlBody, textBody);
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string textBody = "")
    {
        try
        {
            var email = new MimeMessage();
            
            // Configurações do remetente
            var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@pwamaster.com";
            var fromName = _configuration["EmailSettings:FromName"] ?? "PWA Master";
            
            email.From.Add(new MailboxAddress(fromName, fromEmail));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            
            if (!string.IsNullOrEmpty(textBody))
                bodyBuilder.TextBody = textBody;
                
            if (!string.IsNullOrEmpty(htmlBody))
                bodyBuilder.HtmlBody = htmlBody;

            email.Body = bodyBuilder.ToMessageBody();

            // Configurações SMTP
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];

            using var smtp = new SmtpClient();
            
            await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            
            if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
            {
                await smtp.AuthenticateAsync(smtpUsername, smtpPassword);
            }
            
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email enviado com sucesso para {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {Email}", toEmail);
            return false;
        }
    }

    private static string GetWelcomeEmailTemplate(string name)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Bem-vindo ao PWA Master</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #3b82f6, #8b5cf6); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f8fafc; padding: 30px; border-radius: 0 0 10px 10px; }}
        .cta-button {{ display: inline-block; background: #3b82f6; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #e2e8f0; color: #64748b; }}
        .benefit {{ background: white; padding: 20px; margin: 15px 0; border-radius: 8px; border-left: 4px solid #3b82f6; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🚀 Bem-vindo ao PWA Master, {name}!</h1>
        <p>Sua jornada para dominar Progressive Web Apps começa agora</p>
    </div>
    
    <div class='content'>
        <h2>Parabéns por dar este importante passo!</h2>
        
        <p>Olá <strong>{name}</strong>,</p>
        
        <p>Ficamos muito felizes em tê-lo(a) conosco! Você acabou de se juntar a uma comunidade exclusiva de desenvolvedores que estão transformando suas carreiras com PWAs.</p>
        
        <div class='benefit'>
            <h3>📚 O que você vai receber:</h3>
            <ul>
                <li>Conteúdos exclusivos sobre PWA enviados semanalmente</li>
                <li>Tutoriais práticos e exemplos de código</li>
                <li>Dicas avançadas de otimização e performance</li>
                <li>Acesso antecipado a novos conteúdos</li>
            </ul>
        </div>
        
        <div class='benefit'>
            <h3>🎯 Próximos passos:</h3>
            <ol>
                <li>Acesse sua área de conteúdo exclusivo</li>
                <li>Comece pelos fundamentos de PWA</li>
                <li>Implemente seu primeiro projeto prático</li>
            </ol>
        </div>
        
        <p style='text-align: center;'>
            <a href='https://pwamaster.com/conteudo-engajamento' class='cta-button'>
                Acessar Conteúdo Exclusivo
            </a>
        </p>
        
        <p><strong>Dica especial:</strong> Fique de olho em sua caixa de entrada! Em breve você receberá uma oferta especial do nosso e-book completo ""PWA Master"" com desconto exclusivo para assinantes.</p>
    </div>
    
    <div class='footer'>
        <p>© 2025 PWA Master. Todos os direitos reservados.</p>
        <p>Se você não deseja mais receber estes emails, <a href='#'>clique aqui</a> para cancelar sua inscrição.</p>
    </div>
</body>
</html>";
    }

    private static string GetWelcomeEmailTextTemplate(string name)
    {
        return $@"Bem-vindo ao PWA Master, {name}!

Parabéns por dar este importante passo!

Olá {name},

Ficamos muito felizes em tê-lo(a) conosco! Você acabou de se juntar a uma comunidade exclusiva de desenvolvedores que estão transformando suas carreiras com PWAs.

O que você vai receber:
- Conteúdos exclusivos sobre PWA enviados semanalmente
- Tutoriais práticos e exemplos de código
- Dicas avançadas de otimização e performance
- Acesso antecipado a novos conteúdos

Próximos passos:
1. Acesse sua área de conteúdo exclusivo
2. Comece pelos fundamentos de PWA
3. Implemente seu primeiro projeto prático

Acesse: https://pwamaster.com/conteudo-engajamento

Dica especial: Fique de olho em sua caixa de entrada! Em breve você receberá uma oferta especial do nosso e-book completo ""PWA Master"" com desconto exclusivo para assinantes.

© 2025 PWA Master. Todos os direitos reservados.";
    }
}

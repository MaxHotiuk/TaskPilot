using Application.Abstractions.Messaging;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        await SendEmailAsync(new[] { to }, subject, body, cancellationToken);
    }

    public async Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailOptions.SenderName, _emailOptions.SenderEmail));
            
            foreach (var recipient in recipients)
            {
                message.To.Add(MailboxAddress.Parse(recipient));
            }

            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();

            // Port 465 requires SslOnConnect, port 587 requires StartTls
            var secureSocketOptions = _emailOptions.SmtpPort == 465 
                ? SecureSocketOptions.SslOnConnect 
                : (_emailOptions.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

            await client.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.SmtpPort, 
                secureSocketOptions, 
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(_emailOptions.Username) && !string.IsNullOrWhiteSpace(_emailOptions.Password))
            {
                await client.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", recipients));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", recipients));
            throw;
        }
    }

    public async Task SendSystemEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var systemBody = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>TaskPilot System Notification</h2>
                    <div style='background-color: #f5f5f5; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                        {body}
                    </div>
                    <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;' />
                    <p style='color: #666; font-size: 12px;'>
                        This is an automated message from TaskPilot. Please do not reply to this email.
                    </p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(to, $"[TaskPilot] {subject}", systemBody, cancellationToken);
    }
}

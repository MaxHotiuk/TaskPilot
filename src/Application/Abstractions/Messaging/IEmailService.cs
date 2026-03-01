namespace Application.Abstractions.Messaging;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken cancellationToken = default);
    Task SendSystemEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}

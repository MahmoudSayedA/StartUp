namespace Application.Common.Abstractions.Messaging;
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);

}

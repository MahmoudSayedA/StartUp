using Application.Common.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Messaging;
public class FakeEmailService : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Simulate sending an email by logging to the console
        await Task.Delay(200);
        Console.WriteLine($"Sending email to {to} with subject '{subject}' and body '{body}'");
    }
}

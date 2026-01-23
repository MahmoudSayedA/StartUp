using Application.Common.Abstractions.Messaging;
using Domain.Events.UsersEvents;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Application.Identity.EventHandlers;
public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;
    private readonly IEmailService _emailService;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        // send welcome email and log registration
        _logger.LogInformation("New user registered: {UserId}, Email: {Email}", notification.UserId, notification.Email);

        BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(notification.Email, "Welcome!", "Thank you for registering."));

        return Task.CompletedTask;

    }
}

using Application.Identity.Services;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUser _user;
    private readonly IUserManagementService _userManagementService;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user, IUserManagementService userManagementService)
    {
        _logger = logger;
        _user = user;
        _userManagementService = userManagementService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _userManagementService.GetUserNameAsync(userId);
        }

        _logger.LogInformation("App Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);
    }
}

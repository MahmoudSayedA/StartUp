using Application.Identity.Services;
using System.ComponentModel.DataAnnotations;

namespace Application.Identity.Commands;
public class ConfirmEmailCommand : ICommand 
{

    [EmailAddress]
    public required string Email { get; set; }
    public required string Token { get; set; }
}

public class ConfirmEmailCommandHandler(IIdentityService identityService) : ICommandHandler<ConfirmEmailCommand>
{
    private readonly IIdentityService _identityService = identityService;

    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ConfirmEmailAsync(request.Email, request.Token);
    }
}

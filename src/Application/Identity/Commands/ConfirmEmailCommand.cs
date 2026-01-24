using Application.Identity.Services;
using System.ComponentModel.DataAnnotations;

namespace Application.Identity.Commands;
public class ConfirmEmailCommand : ICommand 
{

    [EmailAddress]
    public required string Email { get; set; }
    public required string Token { get; set; }
}

public class ConfirmEmailCommandHandler(IAuthenticationService authenticationService) : ICommandHandler<ConfirmEmailCommand>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        await _authenticationService.ConfirmEmailAsync(request.Email, request.Token);
    }
}

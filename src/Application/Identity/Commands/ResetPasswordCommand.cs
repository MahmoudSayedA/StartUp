using Application.Identity.Services;
using System.ComponentModel.DataAnnotations;

namespace Application.Identity.Commands;
public class ResetPasswordCommand : ICommand
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}

public class ResetPasswordCommandHandler(IPasswordManagementService passwordManagementService) : ICommandHandler<ResetPasswordCommand>
{
    private readonly IPasswordManagementService _passwordManagementService = passwordManagementService;
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        await _passwordManagementService.ResetPasswordAsync(new Dtos.ResetPasswordDto(request.Email, request.Token, request.NewPassword));
    }
}
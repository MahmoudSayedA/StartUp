using Application.Common.Models;
using Application.Identity.Services;
using System.ComponentModel.DataAnnotations;

namespace Application.Identity.Commands;
public class LoginCommand : ICommand<LoginResponseModel>
{
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Password { get; set; }
}

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponseModel>
{
    private readonly IIdentityService _identityService;
    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<LoginResponseModel> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(new Dtos.LoginDto
        {
            Email = request.Email,
            Password = request.Password,
        });
    }
}
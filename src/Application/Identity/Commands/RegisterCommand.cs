using Application.Common.Exceptions;
using Application.Identity.Dtos;
using Application.Identity.Services;
using FluentValidation.Results;
using System.ComponentModel.DataAnnotations;

namespace Application.Identity.Commands;
public class RegisterCommand : ICommand<string>
{
    [Required]
    
    [StringLength(50, MinimumLength = 3)]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public required string ConfirmPassword { get; set; }
}

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, string>
{
    private readonly IIdentityService _identityService;
    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterAsync(new RegisterDto
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
        });
        if (!result.Succeeded)
        {
            ICollection<ValidationFailure> failures = result.Errors
                .Select(e => new ValidationFailure(string.Empty, e))
                .ToList();

            throw new Common.Exceptions.ValidationException(failures);
        }

        return result.Data.ToString();
    }
}



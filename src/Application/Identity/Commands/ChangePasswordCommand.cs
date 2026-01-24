using Application.Identity.Services;
using Microsoft.AspNetCore.Identity;

namespace Application.Identity.Commands;

public class ChangePasswordCommand : ICommand
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUser _currentUser;
    private readonly IPasswordManagementService _passwordManagementService;

    public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, IUser currentUser, IPasswordManagementService passwordManagementService)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _passwordManagementService = passwordManagementService;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? string.Empty;

        await _passwordManagementService.ChangePasswordAsync(userId,new Dtos.ChangePasswordDto{ CurrentPassword = request.CurrentPassword, NewPassword = request.NewPassword});
    }
}

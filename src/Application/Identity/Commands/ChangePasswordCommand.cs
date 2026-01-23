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
    private readonly IIdentityService _identityService;

    public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, IUser currentUser, IIdentityService identityService)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? string.Empty;

        await _identityService.ChangePasswordAsync(userId,new Dtos.ChangePasswordDto{ CurrentPassword = request.CurrentPassword, NewPassword = request.NewPassword});
    }
}

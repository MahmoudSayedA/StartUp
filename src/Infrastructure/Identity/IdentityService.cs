using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Identity.Dtos;
using Application.Identity.Services;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Identity;

public class IdentityService : IAuthenticationService,
                               IPasswordManagementService,
                               IAccessControlService,
                               IUserManagementService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenGeneratorService _tokenGenerator;
    private readonly ILogger<IdentityService> _logger;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ITokenGeneratorService tokenGenerator,
        ILogger<IdentityService> logger,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<Result<Guid>> RegisterAsync(RegisterDto model)
    {
        ApplicationUser user = new()
        {
            UserName = model.Username,
            Email = model.Email,
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded && _logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("User {Username} registered successfully.", model.Username);

        return result.ToApplicationResult(user.Id);
    }
    public async Task<LoginResponseModel> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || user.IsDeleted)
        {
            throw new UnauthorizedAccessException("Account is not found.");
        }
        if (!await _userManager.CheckPasswordAsync(user, model.Password))
        {
            await _userManager.AccessFailedAsync(user);
            throw new UnauthorizedAccessException("Invalid login attempt.");
        }
        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedAccessException("Email is not confirmed.");
        }
        ICollection<string> roles = await _userManager.GetRolesAsync(user);
        var token = await _tokenGenerator.GenerateJwtToken(user);

        // TODO: Save to Db
        var refreshToken = await _tokenGenerator.GenerateRefreshToken();

        return new LoginResponseModel
        {
            UserId = user.Id.ToString(),
            Roles = roles,
            Token = token,
            TokenExpiryInMinutes = _tokenGenerator.TokenExpiryInMinutes,
            RefreshToken = refreshToken,
        };
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<Result> AddRoleToUserAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure(new[] { "User not found." });
        }
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new ApplicationRole { Name = role });
        }
        var result = await _userManager.AddToRoleAsync(user, role);
        return result.ToApplicationResult();
    }
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<ICollection<string>> GetRolesAsync(IUser? user = null)
    {
        if (user == null) return [];

        var appUser = await _userManager.FindByIdAsync(user?.Id ?? string.Empty);

        if (appUser == null) return [];

        var roles = await _userManager.GetRolesAsync(appUser);
        return roles ?? [];
    }
    public async Task<UserInfoDto> GetUserInfoAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException(nameof(ApplicationUser), userId);

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        return new UserInfoDto()
        {
            Email = user.Email!,
            Username = user.UserName!,
            Roles = roles,
            Claims = claims.ToDictionary(c => c.Type, c => c.Value)
        };
    }

    public Task SaveRefreshTokenAsync(ApplicationUser user, string refreshToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email)
                        ?? throw new NotFoundException(nameof(ApplicationUser), email);

        if (!user.EmailConfirmed)
            throw new UnauthorizedAccessException("Email is not confirmed.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // TODO: Send email with token (not implemented yet)
        return token;
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email)
            ?? throw new NotFoundException(nameof(ApplicationUser), model.Email);

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

        if (!result.Succeeded)
            throw new ValidationException(result.Errors.Select(x => new ValidationFailure(x.Code, x.Description)));

        // await emailService.SendEmailAsync(user.Email!, "Password Reset", "Your password has been reset successfully.", isHtml: true);

        return result.ToApplicationResult();
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto model)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException(nameof(ApplicationUser), userId);

        var result = await _userManager.ChangePasswordAsync(user!, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
            throw new ValidationException(result.Errors.Select(x => new ValidationFailure(x.Code, x.Description)));

        // Send email notification
        // await emailService.SendEmailAsync(user.Email!, "Password Changed", "Your password has been changed successfully.", isHtml: true);

        return result.ToApplicationResult();
    }

    public async Task ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email)
                       ?? throw new NotFoundException(nameof(ApplicationUser), email);

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            throw new ValidationException(result.Errors.Select(x => new ValidationFailure(x.Code, x.Description)));
    }

}

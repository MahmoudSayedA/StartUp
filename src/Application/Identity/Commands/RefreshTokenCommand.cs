using Application.Common.Abstractions.Data;
using Application.Common.Models;
using Application.Identity.Services;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Application.Identity.Commands;
public class RefreshTokenCommand : ICommand<LoginResponseModel>
{
    [MaxLength(512)]
    public required string RefreshToken { get; set; }
}

public class RefreshTokenCommandHandler(
    IApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    ITokenGeneratorService tokenGenerator) : ICommandHandler<RefreshTokenCommand, LoginResponseModel>
{
    public async Task<LoginResponseModel> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Find the refresh token in the database
        var refreshToken = await context
            .Set<RefreshToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        if (!refreshToken.IsActive)
        {
            throw new UnauthorizedAccessException("Refresh token expired");
        }

        // Generate a new access token and a new refresh token
        var user = await context
            .Set<ApplicationUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == refreshToken.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found");

        var roles = await userManager.GetRolesAsync(user);
        var newTokens = tokenGenerator.GenerateJwtToken(user, [.. roles]);

        return new LoginResponseModel
        {
            Token = newTokens,
            TokenExpiryInMinutes = tokenGenerator.TokenExpiryInMinutes,
            RefreshToken = refreshToken.Token, // return the same refresh token
            RefreshTokenExpireAt = refreshToken.ExpiresAt,
            UserId = user.Id.ToString(),
            Roles = roles
        };

    }
}
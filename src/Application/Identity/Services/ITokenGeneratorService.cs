using Domain.Entities.Users;

namespace Application.Identity.Services
{
    public interface ITokenGeneratorService
    {
        string GenerateJwtToken(ApplicationUser user, List<string> userRoles);
        string GenerateRefreshToken();
        int TokenExpiryInMinutes { get; }
        int RefreshTokenExpiryInDays { get; }
    }
}

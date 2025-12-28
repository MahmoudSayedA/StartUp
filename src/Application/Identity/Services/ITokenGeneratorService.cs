namespace Application.Identity.Services
{
    public interface ITokenGeneratorService
    {
        Task<string> GenerateJwtToken(ApplicationUser user);
        Task<string> GenerateRefreshToken();
        int TokenExpiryInMinutes { get; }
        int RefreshTokenExpiryInDays { get; }
    }
}

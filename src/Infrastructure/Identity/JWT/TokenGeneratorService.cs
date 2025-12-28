using Application.Identity.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity.JWT
{
    internal class TokenGeneratorService : ITokenGeneratorService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _options;

        public TokenGeneratorService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> settings)
        {
            _userManager = userManager;
            _options = settings.Value;
        }

        public int TokenExpiryInMinutes => _options.ExpiryInMinutes;

        public int RefreshTokenExpiryInDays => _options.RefreshTokenExpiryInDays;

        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // Fetch roles and add them as claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //// Add role claims to the token
            //foreach(var role in roles)
            //{
            //    var roleObj = await _roleManager.FindByNameAsync(role);
            //    var roleClaims = await _roleManager.GetClaimsAsync(roleObj!);
            //    claims.AddRange(roleClaims);
            //}

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.ValidIssuer,
                audience: _options.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_options.ExpiryInMinutes),
                signingCredentials: creds);

            return await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<string> GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return await Task.Run(() => Convert.ToBase64String(randomNumber));
        }
    }
}

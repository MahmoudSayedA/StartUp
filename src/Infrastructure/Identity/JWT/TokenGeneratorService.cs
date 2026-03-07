using Application.Identity.Services;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        public string GenerateJwtToken(ApplicationUser user, List<string> userRoles)
        {
            // guard
            ArgumentException.ThrowIfNullOrWhiteSpace(_options.Secret, nameof(_options.Secret));
            if (_options.Secret.Length < 32)
                throw new InvalidOperationException("JWT secret must be at least 32 characters.");

            //create claims
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
            };

            if (!string.IsNullOrEmpty(user.Email))
                    claims.Add(new Claim(ClaimTypes.Email, user.Email));

            // add roles as claims
            foreach (var role in userRoles)
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
                expires: DateTime.UtcNow.AddMinutes(_options.ExpiryInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            RandomNumberGenerator.Fill(randomNumber);

            string refresh = Convert.ToBase64String(randomNumber)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');

            return refresh;
        }
    }
}

namespace Infrastructure.Identity.JWT
{
    public class JwtOptions
    {
        public string? ValidIssuer { get; set; }
        public string? ValidAudience { get; set; }
        public string? Secret { get; set; }
        public int ExpiryInMinutes { get; set; }
        public int RefreshTokenExpiryInDays { get; set; }

    }
}

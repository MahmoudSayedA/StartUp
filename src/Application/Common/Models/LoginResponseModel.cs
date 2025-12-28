namespace Application.Common.Models
{
    public class LoginResponseModel
    {
        public string Token { get; set; } = string.Empty;
        public int TokenExpiryInMinutes { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        // public bool IsConfirmed { get; set; }
        public ICollection<string>? Roles { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}

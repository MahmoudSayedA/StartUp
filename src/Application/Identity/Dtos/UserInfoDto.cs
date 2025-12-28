namespace Application.Identity.Dtos
{
    public class UserInfoDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public ICollection<string> Roles { get; set; } = [];
        public Dictionary<string, string> Claims { get; set; } = new();
    }

}

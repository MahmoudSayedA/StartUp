namespace Application.Identity.Dtos
{
    public class ConfigureEmailDto
    {
        public required string UserId { get; set; }
        public required string NewEmail { get; set; }
    }

}

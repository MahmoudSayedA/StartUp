namespace Application.Identity.Dtos
{
    public record ResetPasswordDto(string Email, string Token,  string NewPassword);

}

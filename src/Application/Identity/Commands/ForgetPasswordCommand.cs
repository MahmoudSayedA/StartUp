using Application.Identity.Services;
using System.Net;

namespace Application.Identity.Commands;

public class ForgetPasswordCommand : ICommand<string>
{
    public required string Email { get; set; }
    public required string RedirectUrl { get; set; }
}

public class ForgetPasswordCommandHandler(
    IPasswordManagementService passwordManagementService) : ICommandHandler<ForgetPasswordCommand, string>
{

    public async Task<string> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        var token = await passwordManagementService.ForgotPasswordAsync(request.Email);

        // TODO: Implement email service to send the reset password link
        var emailBody = SendResetPasswordEmail(request.Email, token, request.RedirectUrl);

        // For demonstration purposes, we return the email body instead of sending an email
        return emailBody;
    }

    private static string SendResetPasswordEmail(string email, string resetToken, string redirectUrl)
    {
        var encodedToken = WebUtility.UrlEncode(resetToken);
        var encodedEmail = WebUtility.UrlEncode(email);

        // Construct the password reset link using the frontend's base URL (RedirectUrl)
        var resetLink = $"{redirectUrl}?email={encodedEmail}&token={encodedToken}";

        // Send the email
        var emailBody = $"<p>Please reset your password by clicking the link below:</p><hr>" +
                        $"<a href='{resetLink}'>Reset Password</a>";

        //await emailService.SendEmailAsync(email, "Reset your password", emailBody, isHtml: true);

        return emailBody;
    }

}

//using Microsoft.AspNetCore.Identity;
using Application.Common.Models;
using Application.Identity.Dtos;

namespace Application.Identity.Services
{
    public interface IIdentityService
    {
        Task<Result<Guid>> RegisterAsync(RegisterDto model);
        Task<LoginResponseModel> LoginAsync(LoginDto model);
        Task ConfirmEmailAsync(string email, string token);

        Task<Result> AddRoleToUserAsync(string userId, string role);
        Task<ICollection<string>> GetRolesAsync(IUser? user = null);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<bool> AuthorizeAsync(string userId, string policyName);

        Task<string> ForgotPasswordAsync(string email);
        Task<Result> ResetPasswordAsync(ResetPasswordDto model);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto model);

        Task<UserInfoDto> GetUserInfoAsync(string userId);
        Task<string?> GetUserNameAsync(string userId);
        Task<Result> DeleteUserAsync(string userId);

    }
}

//using Microsoft.AspNetCore.Identity;
using Application.Identity.Dtos;
using Application.Common.Models;

namespace Application.Identity.Services
{
    public interface IIdentityService
    {
        Task<Result<Guid>> RegisterAsync(RegisterDto model);
        Task<LoginResponseModel> LoginAsync(LoginDto model);
        Task<UserInfoDto> GetUserInfoAsync(string userId);
        Task<Result> AddRoleToUserAsync(string userId, string role);
        Task<ICollection<string>> GetRolesAsync(IUser? user = null);
        Task<string?> GetUserNameAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<bool> AuthorizeAsync(string userId, string policyName);
        Task<Result> DeleteUserAsync(string userId);
    }
}

using Application.Common.Models;
using Application.Identity.Dtos;

namespace Application.Identity.Services
{
    public interface IUserManagementService
    {
        Task<UserInfoDto> GetUserInfoAsync(string userId);
        Task<string?> GetUserNameAsync(string userId);
        Task<Result> DeleteUserAsync(string userId);
    }
}
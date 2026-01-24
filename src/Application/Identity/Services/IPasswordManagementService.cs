using Application.Common.Models;
using Application.Identity.Dtos;

namespace Application.Identity.Services
{
    public interface IPasswordManagementService
    {
        Task<string> ForgotPasswordAsync(string email);
        Task<Result> ResetPasswordAsync(ResetPasswordDto model);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto model);
    }
}
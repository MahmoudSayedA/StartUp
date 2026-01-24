using Application.Common.Models;
using Application.Identity.Dtos;

namespace Application.Identity.Services
{
    public interface IAuthenticationService
    {
        Task<Result<Guid>> RegisterAsync(RegisterDto model);
        Task<LoginResponseModel> LoginAsync(LoginDto model);
        Task ConfirmEmailAsync(string email, string token);
    }
}
using Application.Common.Models;

namespace Application.Identity.Services
{
    public interface IAccessControlService
    {
        Task<Result> AddRoleToUserAsync(string userId, string role);
        Task<ICollection<string>> GetRolesAsync(IUser? user = null);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<bool> AuthorizeAsync(string userId, string policyName);
    }
}

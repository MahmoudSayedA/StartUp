using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Users;

public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsDeleted { get; set; }
}
public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() : base()
    {
    }
}
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

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
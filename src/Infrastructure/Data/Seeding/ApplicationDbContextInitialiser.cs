using Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Seeding;

public static class InitialiserExtensions
{
    //public static async Task InitialiseDatabaseAsync(this IApplicationBuilder app)
    //{
    //    using var scope = app.ApplicationServices.CreateScope();

    //    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

    //    await initialiser.InitialiseAsync();
    //    await initialiser.SeedAsync();
    //}
}

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Default roles
        var adminRole = new ApplicationRole() { Name = Roles.Admin };

        if (roleManager.Roles.All(r => r.Name != adminRole.Name))
        {
            await roleManager.CreateAsync(adminRole);
        }
         
        // Default users
        var admin = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };

        if (userManager.Users.All(u => u.UserName != admin.UserName))
        {
            await userManager.CreateAsync(admin, "Admin1!");
            if (!string.IsNullOrWhiteSpace(adminRole.Name))
            {
                await userManager.AddToRolesAsync(admin, new[] { adminRole.Name });
            }
        }
        // defualt data
    }
}

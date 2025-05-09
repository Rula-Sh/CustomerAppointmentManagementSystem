using CAMS.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CAMS.Data.SeedingData;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        string[] roles = { "Admin", "Employee", "Customer" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role { Name = roleName });
            }
        }

        if (await userManager.FindByNameAsync("admin") == null)
        {
            var admin = new User
            {             
                UserName = "admin",
                Email = "admin@example.com",
                FullName = "Admin User",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "P@ssw0rd");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Repeat for Employee and Customer...
    }
}


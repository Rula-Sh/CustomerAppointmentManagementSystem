using CAMS.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace CAMS.Data.SeedingData;

public static class UserSeeding
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        //var hasher = new PasswordHasher<User>();
        //user.PasswordHash = hasher.HashPassword(user, "!Q23wewe");//P@ssw0rd

        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        string[] roles = { "Admin", "Provider", "Customer" };

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
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                FullName = "Admin User",
                EmailConfirmed = true,
                SecurityStamp = "1",
                CreatedAt = DateTime.Today.AddDays(-10),
                LastActivityDate = DateTime.Today.AddDays(-10),
            };

            await userManager.CreateAsync(admin, "!Q23wewe");//P@ssw0rd
            await userManager.AddToRoleAsync(admin, "Admin");
            await userManager.AddToRoleAsync(admin, "Provider");
            await userManager.AddToRoleAsync(admin, "Customer");
        }

        if (await userManager.FindByNameAsync("provider1") == null)
        {

            var provider = new User
            {
                UserName = "provider1",
                NormalizedUserName = "PROVIDER1",
                FullName = "Provider1 User",
                Email = "provider1@gmail.com",
                NormalizedEmail = "PROVIDER1@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = "2",
                CreatedAt = DateTime.Today.AddDays(-10),
                LastActivityDate = DateTime.Today.AddDays(-10),
            };

            await userManager.CreateAsync(provider, "!Q23wewe");//P@ssw0rd
            await userManager.AddToRoleAsync(provider, "Provider");
        }
        if (await userManager.FindByNameAsync("provider2") == null)
        {

            var provider = new User
            {
                UserName = "provider2",
                NormalizedUserName = "PROVIDER2",
                FullName = "Provider2 User",
                Email = "provider2@gmail.com",
                NormalizedEmail = "PROVIDER2@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = "2",
                CreatedAt = DateTime.Today.AddDays(-10),
                LastActivityDate = DateTime.Today.AddDays(-10),
            };

            await userManager.CreateAsync(provider, "!Q23wewe");//P@ssw0rd
            await userManager.AddToRoleAsync(provider, "Provider");
        }

        if (await userManager.FindByNameAsync("customer1") == null)
        {

            var customer = new User
            {
                UserName = "customer1",
                NormalizedUserName = "CUSTOMER1",
                FullName = "Customer1 User",
                Email = "customer1@gmail.com",
                NormalizedEmail = "CUSTOMER1@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = "3",
                CreatedAt = DateTime.Today.AddDays(-10),
                LastActivityDate = DateTime.Today.AddDays(-10),

            };

            await userManager.CreateAsync(customer, "!Q23wewe");//P@ssw0rd
            await userManager.AddToRoleAsync(customer, "Customer");
        }

        if (await userManager.FindByNameAsync("customer2") == null)
        {

            var customer = new User
            {
                UserName = "customer2",
                NormalizedUserName = "CUSTOMER2",
                FullName = "Customer2 User",
                Email = "customer2@gmail.com",
                NormalizedEmail = "CUSTOMER2@GMAIL.COM",
                EmailConfirmed = true,
                SecurityStamp = "3",
                CreatedAt = DateTime.Today.AddDays(-10),
                LastActivityDate = DateTime.Today.AddDays(-10),

            };

            await userManager.CreateAsync(customer, "!Q23wewe");//P@ssw0rd
            await userManager.AddToRoleAsync(customer, "Customer");
        }
    }
}

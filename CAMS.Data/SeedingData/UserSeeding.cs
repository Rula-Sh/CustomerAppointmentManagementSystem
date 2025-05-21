using CAMS.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Data.SeedingData;

public static class UserSeeding
{
    public static void SeedUsers(ModelBuilder builder)
    {
        var hasher = new PasswordHasher<User>();

        var admin = new User
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            FullName = "Admin User",
            EmailConfirmed = true,
            SecurityStamp = "1"
        };
        admin.PasswordHash = hasher.HashPassword(admin, "P@ssw0rd");

        var provider = new User
        {
            Id = 2,
            UserName = "provider",
            NormalizedUserName = "PROVIDER",
            FullName = "Provider User",
            Email = "provider@example.com",
            NormalizedEmail = "PROVIDER@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = "2"
        };
        provider.PasswordHash = hasher.HashPassword(provider, "P@ssw0rd");

        var customer = new User
        {
            Id = 3,
            UserName = "customer",
            NormalizedUserName = "CUSTOMER",
            FullName = "Customer User",
            Email = "customer@example.com",
            NormalizedEmail = "CUSTOMER@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = "3"

        };
        customer.PasswordHash = hasher.HashPassword(customer, "P@ssw0rd");

        builder.Entity<User>().HasData(admin, provider, customer);
    }
}

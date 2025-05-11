using CAMS.Data;
using CAMS.Data.Models;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceSeeding
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Services.Any())
        {
            var servicesToSeed = new List<Service>
            {
                new Service { Name = "Haircut", Price = 15.00m, Description = "requried",Duration = "01:00" },
                new Service { Name = "Shave", Price = 10.00m , Description = "requried",Duration = "01:00"},
                new Service { Name = "Facial", Price = 25.00m , Description = "requried",Duration = "01:00"}
            };

            context.Services.AddRange(servicesToSeed);
            await context.SaveChangesAsync();
        }
    }
}

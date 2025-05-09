using BusinessLogicLayer.Services;
using CAMS.Application.Helpers;
using CAMS.Application.Interfaces;
using CAMS.Application.Services;
using CAMS.Data;
using CAMS.Data.Models;
using CAMS.Data.SeedingData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.Mapper;

namespace CAMS.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Configure the main DB context
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
            // Configure Identity with your custom User and Role entities
            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            // Register custom services
            builder.Services.AddScoped<RoleManager<Role>>();
            builder.Services.AddScoped<IManageUsersService, ManageUsersService>();
            builder.Services.AddScoped<IManageServicesService, ManageServicesService>();
            builder.Services.AddScoped<IManageAppointmentsService, ManageAppointmentsService>();
            builder.Services.AddScoped<INotificationManagerService, NotificationManagerService>();
            builder.Services.AddScoped<ISignalRNotifierService, SignalRNotifierService>();
            builder.Services.AddScoped(typeof(Lazy<>), typeof(LazyResolver<>));

            // AutoMapper profiles
            builder.Services.AddAutoMapper(typeof(BLLAutoMapperProfile));
            builder.Services.AddAutoMapper(typeof(PLAutoMapperProfile));

            // SignalR
            builder.Services.AddSignalR();

            var app = builder.Build();
            await ServiceSeeding.SeedAsync(app.Services);
            await DbInitializer.SeedAsync(app.Services);
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using DataAccessLayer.Data;
using DataAccessLayer.Models;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Helpers;
//using DataAccessLayer.Repositories;
//using DataAccessLayer.Repositories.Contracts;
//using BusinessLogicLayer.Services;
//using BusinessLogicLayer.Services.Contracts;
//using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString),
    ServiceLifetime.Transient); // added to avoid the error: A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext.

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<User, Role>() // was Services.AddDefaultIdentity<IdentityUser> before i created the ApplicatonUser
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI() //added this with the AddIdentity update
    .AddDefaultTokenProviders();//added this with the AddIdentity update

builder.Services.AddScoped<RoleManager<Role>>(); // Ensure RoleManager is properly registered

builder.Services.AddScoped<IManageUsers,ManageUsers>();
builder.Services.AddScoped<IManageServices, ManageServices>();
builder.Services.AddScoped<IManageAppointments, ManageAppointments>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));



builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();

//builder.Services.AddDbContext<CustomerAppointmentManagementSystemContext>(options =>
//    options.UseSqlServer(connectionString));
//builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//builder.Services.AddScoped<IEmployeeService, EmployeeService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute( // to open the home page on startup
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

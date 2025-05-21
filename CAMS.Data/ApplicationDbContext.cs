using System.Reflection;
using CAMS.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int,
    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    // i added this extend so that i overried User/Role/UserRole and convert the default id from string to int
    {

        public ApplicationDbContext() { } // added this because in HomeController.cs "new ApplicationDbContext()" caused the error "There is no argument given that corresponds to the required parameter 'options' of 'ApplicationDbContext.ApplicationDbContext(DbContextOptions<ApplicationDbContext>)'"
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ServiceDate> ServiceDates { get; set; }
        public DbSet<ServiceTimeSlot> ServiceTimeSlots { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.HasDefaultSchema("UserSchema"); // change the name of the schema, i wont use this so that i dont change the schema to the whole tables later... currently i just want to change the name of the schema of the identity tables

            //builder.Entity<IdentityUser>().ToTable("Users", "security"); // was this before i created the User... when i wanted to rename the columns names         

            //.Ignore(e => e.PhoneNumberConfirmed); //removes the PhoneNumberConfirmed column from the Users table
            builder.Entity<Role>().ToTable("Roles", "security");
            builder.Entity<UserRole>().ToTable("UserRoles", "security");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims", "security");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens", "security");

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

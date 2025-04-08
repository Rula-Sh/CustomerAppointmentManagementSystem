using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Emit;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int,
    IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    // i added this extend so that i overried User/Role/UserRole and convert the default id from string to int
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentService> AppointmentServices { get; set; }

        //public DbSet<Notification> Notifications { get; set; }
        //public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<IdentityUser>().ToTable("Users", "security"); // was this before i created the User... when i wanted to rename the columns names
            builder.Entity<User>().ToTable("Users", "security"); // change the name of the table and the schema

            //.Ignore(e => e.PhoneNumberConfirmed); //removes the PhoneNumberConfirmed column from the Users table
            builder.Entity<Role>().ToTable("Roles", "security");
            builder.Entity<UserRole>().ToTable("UserRoles", "security");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims", "security");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens", "security");

            // Many-to-Many relationship (User and Role)
            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId }); // ur => UserRole

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles) // r => Role
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)// u => User
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // One-to-Many relationship (User[Employee/Customer] and Appointment)
            builder.Entity<Appointment>()
                .HasOne(a => a.Customer) // a => Appointment
                .WithMany(c => c.CustomerAppointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Appointment>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.EmployeeAppointments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many relationship (Appointment and Service)
            builder.Entity<AppointmentService>(entity =>
            {
                entity.HasKey(apse => new { apse.AppointmentId, apse.ServiceId }); // apse => AppointmentService

                entity.HasOne(apse => apse.Service)
                    .WithMany(s => s.AppointmentServices)
                    .HasForeignKey(apse => apse.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(apse => apse.Appointment)
                    .WithMany(a => a.AppointmentServices)
                    .HasForeignKey(apse => apse.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

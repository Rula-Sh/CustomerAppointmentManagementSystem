
using CAMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Data.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "security"); // change the name of the table and the schema
        
        //builder.Entity<UserRole>(entity =>
            //{
            //    entity.HasKey(ur => new { ur.UserId, ur.RoleId }); // ur => UserRole

            //    entity.HasOne(ur => ur.Role)
            //        .WithMany(r => r.UserRoles) // r => Role
            //        .HasForeignKey(ur => ur.RoleId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(ur => ur.User)
            //        .WithMany(u => u.UserRoles)// u => User
            //        .HasForeignKey(ur => ur.UserId)
            //        .OnDelete(DeleteBehavior.Cascade);
            //});
    }
}

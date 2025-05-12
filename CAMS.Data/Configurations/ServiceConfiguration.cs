using CAMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Data.Configurations;

internal class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasMany(s => s.Appointments)
                .WithOne(a => a.Service)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.Price)
                .HasColumnType("decimal(6,2)");
    }
}

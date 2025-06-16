﻿using CAMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CAMS.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder
             .HasOne(a => a.Customer) // a => Appointment
             .WithMany(c => c.CustomerAppointments)
             .HasForeignKey(a => a.CustomerId)
             .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(a => a.Provider)
            .WithMany(e => e.ProviderAppointments)
            .HasForeignKey(a => a.ProviderId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

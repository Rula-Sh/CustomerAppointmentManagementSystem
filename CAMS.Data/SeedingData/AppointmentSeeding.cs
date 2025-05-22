using CAMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CAMS.Data.SeedingData
{
    public class AppointmentSeeding
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var services = await context.Services.ToDictionaryAsync(s => s.Name);


            var appointmentsToSeed = new List<Appointment>
            {
                new Appointment { CustomerId = 4, ProviderId= 1, ServiceId= services["Haircut"].Id, Name="Haircut", Date= $"{DateOnly.FromDateTime(DateTime.Today)} - 8:00 - 9:00", Status="Completed", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today)} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 4, ProviderId= 1, ServiceId= services["Hair coloring / dyeing"].Id, Name="Hair coloring / dyeing", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(5))} - 8:30 - 9:00", Status="Approved", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-1))} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 4, ProviderId= 1, ServiceId = services["Shave"].Id, Name="Shave", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(4))} - 11:00 - 12:00", Status="Pending", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today)} 8:28:44 PM") , Notes= ""},

                new Appointment { CustomerId = 5, ProviderId= 2, ServiceId= services["Electrical repairs"].Id, Name= "Electrical repairs", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(-1))} - 10:00 - 11:30", Status="Completed", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-1))} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 5, ProviderId= 2, ServiceId= services["Furniture assembly"].Id, Name= "Furniture assembly", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(-2))} - 09:00 - 10:00", Status="Approved", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-2))} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 5, ProviderId= 2, ServiceId= services["Appliance repair"].Id, Name= "Appliance repair", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(-4))} - 11:00 - 12:00", Status="Completed", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-4))} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 5, ProviderId= 2, ServiceId= services["Electrical repairs"].Id, Name= "Electrical repairs", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(-3))} - 13:00 - 14:30", Status="Approved", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-3))} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 5, ProviderId= 2, ServiceId= services["Furniture assembly"].Id, Name= "Furniture assembly", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(-2))} - 10:00 - 11:00", Status="Completed", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-2))} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 5, ProviderId= 2, ServiceId= services["Appliance repair"].Id, Name= "Appliance repair", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(-4))} - 12:30 - 13:30", Status="Approved", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-4))} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 5, ProviderId= 2, ServiceId= services["Electrical repairs"].Id, Name= "Electrical repairs", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(-3))} - 15:00 - 16:30", Status="Pending", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-3))} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 5, ProviderId= 2, ServiceId= services["Furniture assembly"].Id, Name= "Furniture assembly", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(-5))} - 14:00 - 15:00", Status="Completed", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-12))} 8:28:44 PM"), Notes= "" },
                new Appointment { CustomerId = 5, ProviderId= 2, ServiceId= services["Appliance repair"].Id, Name= "Appliance repair", Date= $"{DateOnly.FromDateTime(DateTime.Today.AddDays(-6))} - 09:00 - 10:00", Status="Approved", CreatedAt= Convert.ToDateTime($"{DateOnly.FromDateTime(DateTime.Today.AddDays(-7))} 8:28:44 PM"), Notes= "" },
            };

            context.Appointments.AddRange(appointmentsToSeed);
            await context.SaveChangesAsync();
        }
    }
}

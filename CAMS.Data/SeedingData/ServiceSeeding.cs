using CAMS.Data;
using CAMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

public static class ServiceSeeding
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.Services.Any())
        {
            // reset ID's
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Services', RESEED, 0)");
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('ServiceDates', RESEED, 0)");
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('ServiceTimeSlots', RESEED, 0)");

            var servicesToSeed = new List<Service>
            {
                new Service { Name = "Haircut", ProviderId= 1, Price = 15.00m, Description = "requried",Duration = "01:00",
                    ServiceDates = new List<ServiceDate> {
                        new ServiceDate { ServiceId = 1, Date = DateOnly.FromDateTime(DateTime.Today),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { ServiceDateId= 1, Time= "8:00 - 9:00"},
                                new ServiceTimeSlot { ServiceDateId= 1, Time= "9:00 - 10:00"},
                                },
                        },
                        new ServiceDate { ServiceId = 1, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { ServiceDateId=2, Time= "9:00 - 10:00"},
                            },
                        },
                    }
                },
                new Service { Name = "Shave", ProviderId= 1, Price = 10.00m , Description = "requried",Duration = "00:30",
                    ServiceDates = new List<ServiceDate> {
                        new ServiceDate { ServiceId = 2, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { ServiceDateId= 2, Time= "8:00 - 8:30"},
                                new ServiceTimeSlot { ServiceDateId= 2, Time= "8:30 - 9:00"},
                                new ServiceTimeSlot { ServiceDateId= 2, Time= "9:00 - 9:30"},
                                new ServiceTimeSlot { ServiceDateId= 2, Time= "9:00 - 9:30"},
                            },
                        },
                        new ServiceDate { ServiceId = 2, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { ServiceDateId= 2, Time= "10:00 - 10:30"},
                                new ServiceTimeSlot { ServiceDateId= 2, Time= "10:30 - 11:00"},
                            },
                        },
                    }
                },
                new Service { Name = "Facial", ProviderId= 1, Price = 25.00m , Description = "requried",Duration = "01:00",
                    ServiceDates = new List<ServiceDate> {
                        new ServiceDate { ServiceId = 3, Date = DateOnly.FromDateTime(DateTime.Today),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { ServiceDateId= 3, Time= "8:00 - 9:00"},
                            },
                        },
                        new ServiceDate { ServiceId = 3, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { ServiceDateId= 3, Time= "10:00 - 11:00"},
                                new ServiceTimeSlot { ServiceDateId= 3, Time= "11:00 - 12:00"},
                            },
                        },
                        new ServiceDate {ServiceId = 3, Date = DateOnly.FromDateTime(DateTime.Today.AddDays(6)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { ServiceDateId= 3, Time= "9:00 - 10:00"},
                            },
                        },
                    },
                }
            };

            context.Services.AddRange(servicesToSeed);
            await context.SaveChangesAsync();
        }
    }
}

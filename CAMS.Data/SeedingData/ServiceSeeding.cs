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
                new Service { Name = "Haircut", ProviderId= 1, Price = 15.00m, Description = "Cutting and shaping hair to a desired length or style.",Duration = "01:00",
                    ServiceDates = new List<ServiceDate> {
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time= "8:00 - 9:00" },
                                new ServiceTimeSlot { Time= "9:00 - 10:00" },
                                },
                        },
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time= "9:00 - 10:00" },
                            },
                        },
                    }
                },
                new Service { Name = "Hair coloring / dyeing", ProviderId= 1, Price = 10.00m , Description = "Changing the color of hair using dyes (full color, highlights, etc.).",Duration = "00:30",
                    ServiceDates = new List<ServiceDate> {
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time= "8:00 - 8:30" },
                                new ServiceTimeSlot { Time= "8:30 - 9:00" },
                                new ServiceTimeSlot { Time= "9:00 - 9:30" },
                                new ServiceTimeSlot { Time= "9:00 - 9:30" },
                            },
                        },
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time= "10:00 - 10:30" },
                                new ServiceTimeSlot { Time= "10:30 - 11:00" },
                            },
                        },
                    }
                },
                new Service { Name = "Shave", ProviderId= 1, Price = 25.00m , Description = "Hair removal using a razor, typically for the face or body.",Duration = "01:00",
                    ServiceDates = new List<ServiceDate> {
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot {Time= "8:00 - 9:00"},
                            },
                        },
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time= "10:00 - 11:00" },
                                new ServiceTimeSlot { Time= "11:00 - 12:00" },
                            },
                        },
                        new ServiceDate {Date = DateOnly.FromDateTime(DateTime.Today.AddDays(6)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time= "9:00 - 10:00" },
                            },
                        },
                    },
                },
                new Service {
                    Name = "Electrical repairs", ProviderId = 2, Price = 45.00m, Description = "Fixing electrical systems, outlets, lighting, and wiring.", Duration = "01:30",
                    ServiceDates = new List<ServiceDate> {
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time = "10:00 - 11:30" },
                            },
                        },
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time = "13:00 - 14:30" },
                                new ServiceTimeSlot { Time = "15:00 - 16:30" },
                            },
                        },
                    }
                },

                new Service {
                    Name = "Furniture assembly", ProviderId = 2, Price = 20.00m, Description = "Putting together flat-pack or modular furniture.", Duration = "1:00",
                    ServiceDates = new List<ServiceDate> {
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time = "09:00 - 10:00" },
                                new ServiceTimeSlot { Time = "10:00 - 11:00" },
                            },
                        },
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time = "14:00 - 15:00" },
                            },
                        },
                    }
                },

                new Service {
                    Name = "Appliance repair", ProviderId = 2, Price = 30.00m, Description = "Diagnosing and fixing household appliances (fridge, oven, washer).", Duration = "01:00",
                    ServiceDates = new List<ServiceDate> {
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-4)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time = "11:00 - 12:00" },
                                new ServiceTimeSlot { Time = "12:30 - 13:30" },
                            },
                        },
                        new ServiceDate { Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-6)),
                            ServiceTimeSlots = new List<ServiceTimeSlot> {
                                new ServiceTimeSlot { Time = "09:00 - 10:00" },
                            },
                        },
                    }
                },

            };

            context.Services.AddRange(servicesToSeed);
            await context.SaveChangesAsync();
        }
    }
}

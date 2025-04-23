using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ManageServices : IManageServices
    {
        private readonly ApplicationDbContext _context;

        public ManageServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceViewModel>> getServices()
        {
            var services = await _context.Services.Select(s => new ServiceViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Duration = s.Duration,
                Price = s.Price,
                Description = s.Description,
                DateTimeSlotGroups = s.ServiceDates.Select(d => new DateTimeSlotGroupViewModel
                {
                    Date = d.Date.ToString(),
                    TimeSlots = d.ServiceTimeSlots.Select(t => t.Time).ToList(),
                }).ToList()
            }).ToListAsync();

            return services;
        }

        public async Task addService(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task<Service> getService(int? id)
        {
            var service = await _context.Services
                .Include(p => p.ServiceDates)
                .ThenInclude(d => d.ServiceTimeSlots)
                .SingleOrDefaultAsync(m => m.Id == id);

            return service;
        }
        public ServiceViewModel getSelectedServiceDetails(Service service)
        {
            var serviceViewModel = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Price = service.Price,
                Description = service.Description,
                DateTimeSlotGroups = service.ServiceDates.Select(d => new DateTimeSlotGroupViewModel
                {
                    Date = d.Date.ToString(),
                    TimeSlots = d.ServiceTimeSlots.Select(t => t.Time).ToList(),
                }).ToList()
            };

            return serviceViewModel;
        }

        public async Task<Service> getServiceById(int? id)
        {
            return await _context.Services.FirstOrDefaultAsync(a => a.Id == id);
            // in other codes it was SingleOrDefaultAsync
        }

        public async Task DeleteService(Service service)
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }









        public string GetMostBookedServiceName()
        {
            var mostBookedService = _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved")
                                                    .GroupBy(a => a.ServiceId)
                                                    .Select(group => new
                                                    {
                                                        ServiceId = group.Key,
                                                        AppointmentCount = group.Count()
                                                    })
                                                    .OrderByDescending(g => g.AppointmentCount)
                                                    .FirstOrDefault();

            return _context.Services.Where(u => u.Id == mostBookedService.ServiceId).Select(e => e.Name).SingleOrDefault();

        }


        public async Task<List<string>> GetServicesNames()
        {
            return await _context.Services.Select(a => a.Name).ToListAsync();
        }

        public int GetTotalServices()
        {
            return _context.Services.Count();
        }
    }
}

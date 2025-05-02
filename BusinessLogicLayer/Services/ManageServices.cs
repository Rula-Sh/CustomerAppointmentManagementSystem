using AutoMapper;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
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
        private readonly INotificationManager _notificationsManager;
        private readonly IMapper _mapper;    

        public ManageServices(ApplicationDbContext context, INotificationManager notificationsManager, IMapper mapper)
        {
            _context = context;
            _notificationsManager = notificationsManager;
            _mapper = mapper;
        }

        public async Task<List<ServiceDTO>> GetServices()
        {
            /*var services = await _context.Services.Select(s => new ServiceViewModel
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
            }).ToListAsync();*/
            // using AutoMapper
            var servcies = await _context.Services.Include(s => s.ServiceDates).ThenInclude(d => d.ServiceTimeSlots).ToListAsync();

            var servicesViewModel = _mapper.Map<List<ServiceDTO>>(servcies);


            return servicesViewModel;
        }

        public async Task addService(ServiceDTO serviceDTO)
        {
/*
            // create Service
            *//*var service = new Service
            {
                Name = model.Name,
                Description = model.Description,
                Duration = model.Duration,
                Price = model.Price,
                ServiceDates = new List<ServiceDate>()
            };*//*
            // using AutoMapper
            var service = _mapper.Map<Service>(serviceDTO);

            // go through each DateTimeSlotGroup to get the all the dates and time-slots for each date
            foreach (var group in serviceDTO.DateTimeSlotGroups)
            {
                // parse the date string into DateOnly for Date prop in ServiceDate
                if (!DateOnly.TryParseExact(group.Date, "dd-MM-yyyy", out var date))
                {
                    // out var date is compiled if the parse was successful, create the var date and set the parsed group.Date (Parsed to DateOnly) to date
                    //ModelState.AddModelError("", $"Invalid date format: {group.Date}");
                    return;
                }

                // create ServiceDate
                *//*var serviceDate = new ServiceDate
                {
                    ServiceId = service.Id, // to link it to Services Table
                    Date = date,
                    ServiceTimeSlots = new List<ServiceTimeSlot>()
                };*//*
                // using AutoMapper
                var serviceDate = _mapper.Map<ServiceDate>(group);
                serviceDate.Date = date;

                foreach (var time in group.TimeSlots)
                {
                    // create ServiceTimeSlot
                    *//*serviceDate.ServiceTimeSlots.Add(new ServiceTimeSlot
                    {
                        ServiceDateId = serviceDate.Id, // to link it to ServiceDate Table
                        Time = time
                    });*//*
                    // using AutoMapper
                    var serviceTimeSlot = new ServiceTimeSlot
                    {
                        ServiceDateId = serviceDate.Id, // to link it to ServiceDate Table
                        Time = time
                    };

                    serviceDate.ServiceTimeSlots.Add(serviceTimeSlot);  // add it to ServiceDate Table

                }

                service.ServiceDates.Add(serviceDate); // add ServiceDate list to Services Table
            }*/
            var service = _mapper.Map<Service>(serviceDTO);

            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceDTO> getService(int? id)
        {
            var service = await _context.Services
                .Include(p => p.ServiceDates)
                .ThenInclude(d => d.ServiceTimeSlots)
                .SingleOrDefaultAsync(m => m.Id == id);

            return _mapper.Map<ServiceDTO>(service);
        }
/*        public ServiceDTO getSelectedServiceDetails(Service service)
        {
            *//*var serviceViewModel = new ServiceViewModel
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
            };*//*
            // using AutoMapper
            var serviceViewModel = _mapper.Map<ServiceDTO>(service);

            return serviceViewModel;
        }*/

        public async Task<ServiceDTO> getServiceById(int? id)
        {
            var service = await _context.Services.FirstOrDefaultAsync(a => a.Id == id);
            // in other codes it was SingleOrDefaultAsync
            return _mapper.Map<ServiceDTO>(service);
        }

        public async Task DeleteService(ServiceDTO serviceDTO)
        {
            await _notificationsManager.CreateNotificationOnServiceDelete(serviceDTO.Id);

            var existingService = await _context.Services.FindAsync(serviceDTO.Id);
            if (existingService != null)
            {
                _context.Services.Remove(existingService);
                await _context.SaveChangesAsync();
            }

            //var service = _mapper.Map<Service>(serviceDTO);
            //_context.Services.Remove(service);
            //await _context.SaveChangesAsync();
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
            // no auto mapper is used here becaause: 
            // 1- i am not mapping entities to DTOs or view models.
            // 2- i am only retrieving a string (the service name) here after some LINQ-based aggregation.

            //AutoMapper is typically used to convert between objects, like:
            // Entity → ViewModel //ViewModel → DTO //DTO → Entity
            //But in this case, i am using pure LINQ projection and ending up with a string result — no complex object mapping involved.
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

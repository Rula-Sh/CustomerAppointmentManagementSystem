using AutoMapper;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ManageServicesService : IManageServicesService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationManagerService _notificationsManager;
        private readonly Lazy<IManageAppointmentsService> _manageAppointmentsService;
        private readonly IMapper _mapper;

        public ManageServicesService(ApplicationDbContext context, INotificationManagerService notificationsManager, IMapper mapper, Lazy<IManageAppointmentsService> manageAppointmentsService)
        {
            _context = context;
            _notificationsManager = notificationsManager;
            _mapper = mapper;
            _manageAppointmentsService = manageAppointmentsService;
        }

        public async Task<List<ServiceDTO>> GetAllServices()
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
        public async Task<List<ServiceDTO>> GetAvailableServicesInAddAppointment(ClaimsPrincipal user)
        {
            var servicesIds = _manageAppointmentsService.Value.getServicesIdsFromAppointments(user);


            var servcies = await _context.Services.Where(s => !servicesIds.Contains(s.Id)).Include(s => s.ServiceDates).ThenInclude(d => d.ServiceTimeSlots).ToListAsync();

            var servicesViewModel = _mapper.Map<List<ServiceDTO>>(servcies);

            return servicesViewModel;
        }

        public async Task addService(ServiceDTO serviceDTO, ClaimsPrincipal user)
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

            await _notificationsManager.CreateNotificationOnServiceActionForAdmin(serviceDTO.Id, serviceDTO.Name, user, "Created");

            var service = _mapper.Map<Service>(serviceDTO);

            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task updateService(ServiceDTO serviceDTO, ClaimsPrincipal user)
        {
            await _notificationsManager.CreateNotificationOnServiceActionForAdmin(serviceDTO.Id, serviceDTO.Name, user, "Updated");

            //    var service = _mapper.Map<Service>(serviceDTO);

            //    _context.Services.Update(service);
            //await _context.SaveChangesAsync();

            var existingService = await _context.Services
                .Include(s => s.ServiceDates)
                .ThenInclude(g => g.ServiceTimeSlots)
                .FirstOrDefaultAsync(s => s.Id == serviceDTO.Id);

            if (existingService == null) return;

            // remove ServiceDates and ServiceTimeSlots data since when i Update the db i get them back
            _context.ServiceTimeSlots.RemoveRange(
                existingService.ServiceDates.SelectMany(g => g.ServiceTimeSlots));
            _context.ServiceDates.RemoveRange(existingService.ServiceDates);

            // update service properties and set the new DateTimeSlotGroups
            _mapper.Map(serviceDTO, existingService);
            // i CANT use: var service = _mapper.Map<Service>(serviceDTO); because it creates a new service, and i need to update it

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
            var service = await _context.Services.Include(s => s.ServiceDates).ThenInclude(sd => sd.ServiceTimeSlots).FirstOrDefaultAsync(a => a.Id == id);
            // in other codes it was SingleOrDefaultAsync
            return _mapper.Map<ServiceDTO>(service);
        }

        public async Task<bool> DoesTheServiceHaveAppointments(int? serviceId)
        {
            var appointments = await _manageAppointmentsService.Value.getActiveAppointmentsFromServiceId(serviceId);
            if (appointments.Count() == 0)
            {
                return false;
            }
            return true;
        }


        public async Task DeleteService(ServiceDTO serviceDTO, ClaimsPrincipal user)
        {
            //await _notificationsManager.CreateNotificationOnServiceDeleteForCustomer(serviceDTO.Id);
            await _notificationsManager.CreateNotificationOnServiceActionForAdmin(serviceDTO.Id, serviceDTO.Name, user, "Deleted");

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

            if (mostBookedService == null)
            {
                return "None";
            }
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


        public async Task<ServiceWithActiveAppointmentsDTO> getServiceWithActiveAppointments(int? id)
        {
            var service = await _context.Services
                .Include(p => p.ServiceDates)
                .ThenInclude(d => d.ServiceTimeSlots)
                .SingleOrDefaultAsync(m => m.Id == id);

            var activeAppointments = await _context.Appointments.Where(a => a.ServiceId == service.Id && a.Status == "Approved").Include(a => a.Customer).Include(a => a.Employee).ToListAsync();

            var serviceWithActiveAppointmentsDTO = _mapper.Map<ServiceWithActiveAppointmentsDTO>(service);
            serviceWithActiveAppointmentsDTO.ActiveAppointments = _mapper.Map<List<AppointmentDTO>>(activeAppointments);

            return serviceWithActiveAppointmentsDTO;
        }
    }
}
using System.Security.Claims;
using AutoMapper;
using CAMS.Application.DTOs;
using CAMS.Application.Interfaces;
using CAMS.Data;
using CAMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Application.Services
{
    public class ManageServicesService : IManageServicesService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationManagerService _notificationsManager;
        private readonly Lazy<IManageAppointmentsService> _manageAppointmentsService;
        private readonly IManageUsersService _manageUsersService;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public ManageServicesService(ApplicationDbContext context, INotificationManagerService notificationsManager, IMapper mapper, Lazy<IManageAppointmentsService> manageAppointmentsService, IManageUsersService manageUsersService, IAuditLogService auditLogService)
        {
            _context = context;
            _notificationsManager = notificationsManager;
            _mapper = mapper;
            _manageAppointmentsService = manageAppointmentsService;
            _manageUsersService = manageUsersService;
            _auditLogService = auditLogService;
        }

        public async Task<List<ServiceDTO>> GetAllServices()
        {
            // without using the AutoMapper
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
            var servcies = await _context.Services.Include(s => s.Employee).Include(s => s.ServiceDates).ThenInclude(d => d.ServiceTimeSlots).ToListAsync();

            var servicesDTO = _mapper.Map<List<ServiceDTO>>(servcies);


            return servicesDTO;
        }

        public async Task<List<ServiceDTO>> GetEmployeeServices(ClaimsPrincipal user)
        {
            var employeeId = int.Parse(_manageUsersService.GetUserId(user));

            var servcies = await _context.Services.Where(s => s.EmployeeId == employeeId).Include(s => s.ServiceDates).ThenInclude(d => d.ServiceTimeSlots).ToListAsync();

            var servicesViewModel = _mapper.Map<List<ServiceDTO>>(servcies);

            return servicesViewModel;
        }

        public async Task<List<ServiceDTO>> GetAvailableServicesInAddAppointment(ClaimsPrincipal user)
        {
            var servicesIds = _manageAppointmentsService.Value.getServicesIdsFromActiveAndPendingAppointments(user);


            var servcies = await _context.Services.Where(s => !servicesIds.Contains(s.Id)).Include(s => s.ServiceDates).ThenInclude(d => d.ServiceTimeSlots).ToListAsync();

            var servicesViewModel = _mapper.Map<List<ServiceDTO>>(servcies);

            return servicesViewModel;
        }

        public async Task addService(ServiceDTO serviceDTO, ClaimsPrincipal user)
        {
            /*// without using the AutoMapper
            //var service = new Service
            //{
            //    Name = model.Name,
            //    Description = model.Description,
            //    Duration = model.Duration,
            //    Price = model.Price,
            //    ServiceDates = new List<ServiceDate>()
            //};

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
                //var serviceDate = new ServiceDate
                //{
                //    ServiceId = service.Id, // to link it to Services Table
                //    Date = date,
                //    ServiceTimeSlots = new List<ServiceTimeSlot>()
                //};

                // using AutoMapper
                var serviceDate = _mapper.Map<ServiceDate>(group);
                serviceDate.Date = date;

                foreach (var time in group.TimeSlots)
                {
                    // create ServiceTimeSlot
                    serviceDate.ServiceTimeSlots.Add(new ServiceTimeSlot
                    {
                        ServiceDateId = serviceDate.Id, // to link it to ServiceDate Table
                        Time = time
                    });
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

            // using AutoMapper
            await _notificationsManager.CreateNotificationForAdminOnServiceAction(serviceDTO.Id, serviceDTO.Name, user, "Created");

            var service = _mapper.Map<Service>(serviceDTO);
            service.EmployeeId = int.Parse(_manageUsersService.GetUserId(user));

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            await _auditLogService.AddAuditLog(service.EmployeeId, "Employee", $"have created {service.Name} Service", "Create Service");
        }

        public async Task updateService(ServiceDTO serviceDTO, ClaimsPrincipal user)
        {
            await _notificationsManager.CreateNotificationForAdminOnServiceAction(serviceDTO.Id, serviceDTO.Name, user, "Updated");

            var existingService = await _context.Services
                .Include(s => s.ServiceDates)
                .ThenInclude(g => g.ServiceTimeSlots)
                .FirstOrDefaultAsync(s => s.Id == serviceDTO.Id);

            if (existingService == null) return;

            // remove ServiceDates and ServiceTimeSlots data, because when i Update, the db i get them back
            _context.ServiceTimeSlots.RemoveRange(
                existingService.ServiceDates.SelectMany(g => g.ServiceTimeSlots));
            _context.ServiceDates.RemoveRange(existingService.ServiceDates);

            // update service properties and set the new DateTimeSlotGroups
            _mapper.Map(serviceDTO, existingService);
            // i CANT use: var service = _mapper.Map<Service>(serviceDTO); because it creates a new service, and i need to update it

            await _context.SaveChangesAsync();

            await _auditLogService.AddAuditLog(serviceDTO.EmployeeId, "Employee", $"have updated {serviceDTO.Name} Service details", "Update Service");
        }

        public async Task<ServiceDTO> getServiceById(int? id)
        {
            var service = await _context.Services.Include(s => s.Employee).Include(s => s.ServiceDates).ThenInclude(sd => sd.ServiceTimeSlots).FirstOrDefaultAsync(a => a.Id == id);
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
            await _notificationsManager.CreateNotificationForAdminOnServiceAction(serviceDTO.Id, serviceDTO.Name, user, "Deleted");

            //var service = _mapper.Map<Service>(serviceDTO);
            //_context.Services.Remove(service);

            var existingService = await _context.Services.FindAsync(serviceDTO.Id);
            if (existingService != null)
            {
                _context.Services.Remove(existingService);
                await _context.SaveChangesAsync();

                await _auditLogService.AddAuditLog(existingService.EmployeeId, "Employee", $"have deleted {existingService.Name} Service", "Delete Service");
            }
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

        public async Task<bool> doesTheUserHaveActiveAppointments(int userId)
        {
            bool hasActiveAppointments;
            var userRole = await _context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name).ToListAsync();
            if (userRole.Contains("Employee"))
            {
                var employeeServiceIds = await _context.Services.Where(s => s.EmployeeId == userId).Select(s => s.Id).ToListAsync();

                hasActiveAppointments = await _context.Appointments
                    .AnyAsync(a => a.Status == "Approved" && employeeServiceIds.Contains(a.ServiceId));
                //AnyAsync is an asynchronous LINQ method provided by Entity Framework Core that checks whether any elements in a sequence satisfy a given condition — without loading all the data into memory.... It returns true if at least one element in the query matches the condition... otherwise, it returns false.
            }
            else
            {
                hasActiveAppointments = await _context.Appointments
                    .AnyAsync(a => a.Status == "Approved" && a.CustomerId == userId);
            }

            return hasActiveAppointments;
        }
    }
}
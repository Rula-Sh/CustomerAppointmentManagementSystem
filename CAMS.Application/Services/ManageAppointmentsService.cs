using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CAMS.Application.DTOs;
using CAMS.Application.Interfaces;
using CAMS.Data;
using CAMS.Data.Models;
using Microsoft.EntityFrameworkCore;
namespace CAMS.Application.Services
{
    public class ManageAppointmentsService : IManageAppointmentsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IManageUsersService _manageUsers;
        private readonly IMapper _mapper;
        private readonly INotificationManagerService _notificationsManager;
        private readonly IAuditLogService _auditLogService;

        public ManageAppointmentsService(ApplicationDbContext context, IManageUsersService manageUsers, IMapper mapper, INotificationManagerService notificationsManager, IAuditLogService auditLogService)
        {
            _context = context;
            _manageUsers = manageUsers;
            _mapper = mapper;
            _notificationsManager = notificationsManager;
            _auditLogService = auditLogService;
        }

        public async Task<List<AppointmentDTO>> getAppointmentsBasedOnRole(ClaimsPrincipal user)
        {
            var userId = _manageUsers.GetUserId(user);

            //var userId = Int32.Parse(User.Identity.GetUserId());
            var appointmentsQuery = _context.Appointments.AsQueryable();
            if (user.IsInRole("Provider"))
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.ProviderId == Int32.Parse(userId) && a.Status == "Approved");
            }
            else if (user.IsInRole("Customer"))
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.CustomerId == Int32.Parse(userId));
            }

            var appointmentList = await appointmentsQuery.Include(a => a.Customer).Include(a => a.Provider).ToListAsync();
            var appointments = _mapper.Map<List<AppointmentDTO>>(appointmentList);

            return appointments;
        }

        public List<int> getServicesIdsFromActiveAndPendingAppointments(ClaimsPrincipal user)
        {
            var userId = int.Parse(_manageUsers.GetUserId(user));
            return _context.Appointments.Include(a => a.Provider).Where(a => a.CustomerId == userId && (a.Status == "Approved" || a.Status == "Pending")).Select(a => a.ServiceId).ToList();
        }

        public async Task addAppointment(BookAppointmentDTO bookAppointmentDTO, ClaimsPrincipal user)
        {
            // using AutoMapper
            var appointment = _mapper.Map<Appointment>(bookAppointmentDTO);
            appointment.CustomerId = Int32.Parse(_manageUsers.GetUserId(user));
            appointment.ProviderId = bookAppointmentDTO.ProviderId; // again hardcod because of FK issues

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();


            await _notificationsManager.CreateNotificationForProviderOnAppointmentCreateOrDelete(appointment.Id, "Create");
            await _auditLogService.AddAuditLog(appointment.CustomerId, "Customer", "have booked an appointment", "Book Appointment");
        }

        public async Task<AppointmentDTO> appointmentDetails(int? id)
        {
            // using AutoMapper
            var appointment = await _context.Appointments
                .Where(a => a.Id == id)
                .ProjectTo<AppointmentDTO>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
            // ProjectTo<T>() is an AutoMapper method that allows you to map entities directly to DTOs or view models in the database query (e.g., LINQ to Entities), rather than loading the full entity into memory and then mapping it.
            // ProjectTo() builds the SQL query that fetches only the fields needed for the view model — it’s efficient and runs completely on the database side.

            return appointment;
        }

        public async Task<Appointment> getAppointmentById(int? id)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            return _mapper.Map<Appointment>(appointment);
            // in other codes it was SingleOrDefaultAsync
        }

        public async Task deleteAppointment(Appointment appointment)
        {
            if (appointment.Status == "Approved")
            {
                await _notificationsManager.CreateNotificationForProviderOnAppointmentCreateOrDelete(appointment.Id, "Delete");
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            await _auditLogService.AddAuditLog(appointment.CustomerId, "Customer", "have canceled their appointment", "Cancel Appointment");
        }

        public async Task<List<AppointmentDTO>> getPendingAppointments(ClaimsPrincipal user)
        {
            // using AutoMapper
            var ProviderId = int.Parse(_manageUsers.GetUserId(user));
            var appointments = await _context.Appointments.Include(a => a.Customer).Include(a => a.Provider).Where(a => a.Status == "Pending" && a.ProviderId == ProviderId).ToListAsync();
            var appointmentsViewModel = _mapper.Map<List<AppointmentDTO>>(appointments);

            return appointmentsViewModel;
        }

        public async Task updateAppointment(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public string getProviderNameWithMostCompleteAndApprovedAppointments()
        {
            //best Provider
            var bestProvider = _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved")
                                                    .GroupBy(a => a.ProviderId)
                                                    .Select(group => new
                                                    {
                                                        ProviderId = group.Key,
                                                        AppointmentCount = group.Count()
                                                    })
                                                    .OrderByDescending(g => g.AppointmentCount)
                                                    .FirstOrDefault();

            if (bestProvider == null)
            {
                return _context.Users.Where(u => u.Id == 1).Select(e => e.FullName).SingleOrDefault();
            }
            return _context.Users.Where(u => u.Id == bestProvider.ProviderId).Select(e => e.FullName).SingleOrDefault();
        }

        public double GetAverageAppointmentsPerProvider()
        {
            //Average Appointments per Provider
            var TotalAppointments = _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved").Count();
            var TotalProviders = _context.UserRoles.Include(u => u.User).Where(u => u.Role.Name == "Provider").Count();
            return (double)TotalAppointments / (TotalProviders - 1);
        }

        public List<DateTime> getLast7DaysDates()
        {
            // get the last 7 days from now
            return Enumerable.Range(0, 7)
                             .Select(i => DateTime.Now.Date.AddDays(-i))
                             .Reverse()
                             .ToList();
        }

        public List<string> getLast7Days()
        {
            // get the last 7 days from now
            var last7DaysDates = getLast7DaysDates();

            // set the last 7 days format to the days of the week
            return last7DaysDates.Select(d => d.ToString("ddd")).ToList();
        }

        public async Task<List<int>> getTotalAppointmentsFromPast7Days()
        {

            var last7DaysDates = getLast7DaysDates();

            // count approved appointments per day
            return last7DaysDates.Select(d => _context.Appointments.Where(a => a.Status == "Approved" || a.Status == "Completed").Count(a => a.CreatedAt.Date == d.Date)).ToList();
        }

        public List<DateTime> getLast4WeeksDates()
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            return Enumerable.Range(0, 4)
                .Select(i => startOfWeek.AddDays(-7 * i))
                .OrderBy(d => d)
                .ToList();

        }

        public List<int> getTotalApprovedAppointemntPerWeek()
        {
            // count approved appointments per week
            return getLast4WeeksDates().Select(weekStart =>
            {
                var weekEnd = weekStart.AddDays(7);
                return _context.Appointments.Where(a => a.Status == "Approved" || a.Status == "Completed").Count(a => a.CreatedAt >= weekStart && a.CreatedAt < weekEnd);
            }).ToList();
        }

        public List<int> GetAppointmentsStatusCount()
        {
            string[] status = { "Pending", "Approved", "Rejected", "Completed" };
            return status.Select(s => _context.Appointments.Count(a => a.Status == s)).ToList();
        }


        public async Task<List<int>> getTotalAppointmentsPerService()
        {
            var TotalAppointmentsPerService = await _context.Services.Select(service => new
                                                                    {
                                                                        ServiceId = service.Id,
                                                                        Count = service.Appointments
                                                                            .Count(a => a.Status == "Completed" || a.Status == "Approved")
                                                                    })
                                                                    .OrderBy(x => x.ServiceId)
                                                                    .ToListAsync();
            return TotalAppointmentsPerService.Select(x => x.Count).ToList();
        }

        public async Task<List<ActiveAppointmentDTO>> getTodaysAppointments()
        {
            var today = DateTime.Today.ToString("dd/MM/yyyy");

            return await _context.Appointments
                .Where(a =>
                    a.Date.StartsWith(today) && a.Status == "Approved")
                .ProjectTo<ActiveAppointmentDTO>(_mapper.ConfigurationProvider)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
            // ProjectTo<T>() is an AutoMapper method that allows you to map entities directly to DTOs or view models in the database query (e.g., LINQ to Entities), rather than loading the full entity into memory and then mapping it.
            // ProjectTo() builds the SQL query that fetches only the fields needed for the view model — it’s efficient and runs completely on the database side.
            // or
            //var appointments = await _context.Appointments
            //    .Where(a => a.Date.StartsWith(today) && a.Status == "Approved")
            //    .Include(a => a.Customer)
            //    .Include(a => a.Service)
            //    .OrderBy(a => a.Date)
            //    .ToListAsync();

            //return _mapper.Map<List<ActiveAppointmentDTO>>(appointments);
        }
        public int GetTotalAppointments()
        {
            return _context.Appointments.Count();
        }

        public async Task<List<AppointmentDTO>> getActiveAppointmentsFromServiceId(int? id)
        {
            var appointments = await _context.Appointments.Where(a => a.ServiceId == id).Where(a => a.Status == "Approved").ToListAsync();
            return _mapper.Map<List<AppointmentDTO>>(appointments);
        }

    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace BusinessLogicLayer.Services
{
    public class ManageAppointmentsService : IManageAppointmentsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IManageUsersService _manageUsers;
        private readonly IMapper _mapper;
        private readonly INotificationManagerService _notificationsManager;

        public ManageAppointmentsService(ApplicationDbContext context, IManageUsersService manageUsers, IMapper mapper, INotificationManagerService notificationsManager)
        {
            _context = context;
            _manageUsers = manageUsers;
            _mapper = mapper;
            _notificationsManager = notificationsManager;
        }

        public async Task<List<AppointmentDTO>> getAppointmentsBasedOnRole(ClaimsPrincipal user)
        {
            var userId = _manageUsers.GetUserId(user);

            //var userId = Int32.Parse(User.Identity.GetUserId());
            var appointmentsQuery = _context.Appointments.AsQueryable();
            if (user.IsInRole("Employee"))
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.EmployeeId == Int32.Parse(userId));
            }
            else if (user.IsInRole("Customer"))
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.CustomerId == Int32.Parse(userId));
            }

            /*var appointments = await appointmentsQuery.Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                CustomerName = a.Customer.FullName,
                EmployeeId = a.EmployeeId,
                Name = a.Name,
                Date = a.Date,
                Notes = a.Notes,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
            }).ToListAsync();*/
            var appointmentList = await appointmentsQuery.Include(a => a.Customer).Include(a => a.Employee).ToListAsync();
            var appointments = _mapper.Map<List<AppointmentDTO>>(appointmentList);

            return appointments;
        }

        public List<int> getServicesIdsFromAppointments(ClaimsPrincipal user)
        {
            var userId = int.Parse(_manageUsers.GetUserId(user));
            return _context.Appointments.Where(a => a.CustomerId == userId).Select(a => a.ServiceId).ToList();
        }

        /*public async Task<BookAppointmentDTO> ViewAddAppointment()
        {
            var services = await _manageServices.GetServices();

            var viewModel = new BookAppointmentDTO
            {
                Services = services,
            };

            return viewModel;
            // i  don’t need AutoMapper here because I'm not really mapping anything — just assigning a list to a property. 
        }*/

        public async Task addAppointment(BookAppointmentDTO bookAppointmentDTO, ClaimsPrincipal user)
        {
            /*var appointment = new Appointment
            {

                CustomerId = Int32.Parse(_manageUsers.GetUserId(user)),
                EmployeeId = 1, // had to set the employee Id to 1, so that i dont get a FOREIGN KEY constraint error.. couldnt either set it nullable
                ServiceId = model.ServiceId,
                Name = model.ServiceName,
                Date = model.Date,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                //AppointmentServices = new List<AppointmentService>()
                Notes = "",
            };*/
            // using AutoMapper
            var appointment = _mapper.Map<Appointment>(bookAppointmentDTO);
            appointment.CustomerId = Int32.Parse(_manageUsers.GetUserId(user));
            appointment.EmployeeId = 1; // again hardcod because of FK issues

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<AppointmentDTO> appointmentDetails(int? id)
        {

            /*var appointment = await _context.Appointments.Where(a => a.Id == id).Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                Customer = a.Customer,
                EmployeeId = a.EmployeeId,
                Employee = a.Employee,
                Name = a.Name,
                Date = a.Date,
                Notes = a.Notes,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
            }).SingleOrDefaultAsync();
            return appointment;*/
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
                await _notificationsManager.CreateNotificationOnAppointmentDelete(appointment.Id);
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AppointmentDTO>> getPendingAppointments(ClaimsPrincipal user)
        {
            /*var appointments = await _context.Appointments.Where(a => a.Status == "Pending").Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                CustomerName = a.Customer.FullName,
                EmployeeId = a.EmployeeId,
                Name = a.Name,
                Date = a.Date,
                Notes = a.Notes,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
            }).ToListAsync();
            return appointments;*/
            // using AutoMapper
            var appointments = await _context.Appointments.Include(a => a.Customer).Include(a => a.Employee).Where(a => a.Status == "Pending").ToListAsync();
            var appointmentsViewModel = _mapper.Map<List<AppointmentDTO>>(appointments);

            return appointmentsViewModel;
        }

        public async Task updateAppointment(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public string getEmployeeNameWithMostCompleteAndApprovedAppointments()
        {
            //best employee
            var bestEmployee = _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved")
                                                    .GroupBy(a => a.EmployeeId)
                                                    .Select(group => new
                                                    {
                                                        EmployeeId = group.Key,
                                                        AppointmentCount = group.Count()
                                                    })
                                                    .OrderByDescending(g => g.AppointmentCount)
                                                    .FirstOrDefault();

            if(bestEmployee == null)
            {
                return _context.Users.Where(u => u.Id == 1).Select(e => e.FullName).SingleOrDefault();
            }
            return _context.Users.Where(u => u.Id == bestEmployee.EmployeeId).Select(e => e.FullName).SingleOrDefault();
        }

        public double GetAverageAppointmentsPerEmployee()
        {
            //Average Appointments per Employee
            var TotalAppointments = _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved").Count();
            //var TotalEmployees = _context.Users.Include(u => u.UserRoles).Where(u=> u)
            var TotalEmployees = _context.UserRoles.Include(u => u.User).Where(u => u.Role.Name == "Employee").Count();
            return (double)TotalAppointments / (TotalEmployees - 1);
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
            return last7DaysDates.Select(d => d.ToString("dddd")).ToList();
        }

        public async Task<List<int>> getTotalAppointmentsFromPast7Days()
        {

            var last7DaysDates = getLast7DaysDates();

            // count approved appointments per day
            return last7DaysDates.Select(d => _context.Appointments.Where(a => a.Status == "Approved").Count(a => a.CreatedAt.Date == d.Date)).ToList();

            /*
             // old code
                // get Approved Appointments Dates (old old)
                var appointments = _context.Appointments.ToList();
                var parsedAppointments = appointments.Where(a => a.Status == "Approved").Select(a =>
                {
                    var datePart = a.CreatedAt.Date.ToString("M/d/yyyy"); // "4/14/2025"
                    return DateTime.ParseExact(datePart, "M/d/yyyy", CultureInfo.InvariantCulture);
                }).ToList();

                // get the last 7 days from now
                var last7DaysDates = Enumerable.Range(0, 7)
                                               .Select(i => DateTime.Now.Date.AddDays(-i))
                                               .Reverse()
                                               .ToList();

                // set the last 7 days format to the days of the week
                var last7DaysLabels = last7DaysDates.Select(d => d.ToString("dddd")).ToList();
                // count approved appointments per day
                var dailyCounts = last7DaysDates.Select(d => _context.Appointments.Where(a => a.Status == "Approved").Count(a => a.CreatedAt.Date == d.Date)).ToList();
             */
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
                return _context.Appointments.Where(a => a.Status == "Approved").Count(a => a.CreatedAt >= weekStart && a.CreatedAt < weekEnd);
            }).ToList();

            //code in controller
            /*
            // get the start of the weeks date
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var past4WeeksStart = Enumerable.Range(0, 4)
                .Select(i => startOfWeek.AddDays(-7 * i))
                .OrderBy(d => d)
                .ToList();

            // set the start of the week date format
            var weeklyLabels = past4WeeksStart.Select(d => d.ToString("MMM dd")).ToList();
            // count approved appointments per week
            var weeklyCounts = past4WeeksStart.Select(weekStart =>
            {
                var weekEnd = weekStart.AddDays(7);
                return _context.Appointments.Where(a => a.Status == "Approved").Count(a => a.CreatedAt >= weekStart && a.CreatedAt < weekEnd);
            }).ToList();
            */
        }

        public List<int> GetAppointmentsStatusCount()
        {
            string[] status = { "Pending", "Approved", "Rejected", "Completed" };
            return status.Select(s => _context.Appointments.Count(a => a.Status == s)).ToList();
        }


        public async Task<List<int>> getTotalAppointmentsPerService()
        {
            var TotalAppointmentsPerService = await _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved")
                                                             .Include(a => a.Service)
                                                             .GroupBy(a => a.Service.Name)
                                                             .Select(g => new TotalAppointmentsPerServiceDTO
                                                             {
                                                                 ServiceName = g.Key,
                                                                 Count = g.Count()
                                                             })
                                                             .ToListAsync();
            return TotalAppointmentsPerService.Select(x => x.Count).ToList();
        }

        public async Task<List<ActiveAppointmentDTO>> getTodaysAppointments()
        {
            var today = DateTime.Today.ToString("M/d/yyyy");

            /*return await _context.Appointments
                .Where(a =>
                    a.Date.StartsWith(today) && a.Status == "Approved")
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Select(a => new ActiveAppointmentViewModel
                {
                    CustomerName = a.Customer.FullName, // or FirstName + LastName
                    AppointmentDate = a.Date,
                    ServiceName = a.Service.Name
                })
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();*/
            //using AutoMapper
            return await _context.Appointments
                .Where(a =>
                    a.Date.StartsWith(today) && a.Status == "Approved")
                .ProjectTo<ActiveAppointmentDTO>(_mapper.ConfigurationProvider)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
            // ProjectTo<T>() is an AutoMapper method that allows you to map entities directly to DTOs or view models in the database query (e.g., LINQ to Entities), rather than loading the full entity into memory and then mapping it.
            // ProjectTo() builds the SQL query that fetches only the fields needed for the view model — it’s efficient and runs completely on the database side.
            // or
            var appointments = await _context.Appointments
                .Where(a => a.Date.StartsWith(today) && a.Status == "Approved")
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return _mapper.Map<List<ActiveAppointmentDTO>>(appointments);

        }
        public int GetTotalAppointments()
        {
            return _context.Appointments.Count();
        }

        public async Task<List<AppointmentDTO>> getActiveAppointmentsFromServiceId(int? id)
        {
            var appointments = await _context.Appointments.Where(a => a.ServiceId == id).Where(a => a.Status == "Pending" || a.Status == "Approved").ToListAsync();
            return _mapper.Map<List<AppointmentDTO>>(appointments);
        }

    }
}

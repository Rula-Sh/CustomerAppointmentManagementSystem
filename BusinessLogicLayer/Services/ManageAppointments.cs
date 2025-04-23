using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace BusinessLogicLayer.Services
{
    public class ManageAppointments : IManageAppointments
    {
        private readonly ApplicationDbContext _context;
        private readonly IManageUsers _manageUsers;
        private readonly IManageServices _manageServices;

        public ManageAppointments(ApplicationDbContext context, IManageUsers manageUsers, IManageServices manageServices)
        {
            _context = context;
            _manageUsers = manageUsers;
            _manageServices = manageServices;
        }

        public async Task<List<AppointmentViewModel>> getAppointmentsBasedOnRole(ClaimsPrincipal user)
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

            var appointments = await appointmentsQuery.Select(a => new AppointmentViewModel
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

            return appointments;
        }

        public async Task<BookAppointmentViewModel> ViewAddAppointment()
        {
            var services = await _manageServices.getServices();

            var viewModel = new BookAppointmentViewModel
            {
                Services = services,
            };
            return viewModel;
        }

        public async Task addAppointment(BookAppointmentViewModel model, ClaimsPrincipal user)
        {
            var appointment = new Appointment
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
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<AppointmentViewModel> appointmentDetails(int? id)
        {
            var appointment = await _context.Appointments.Where(a => a.Id == id).Select(a => new AppointmentViewModel
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

            return appointment;
        }

        public async Task<Appointment> getAppointmentById(int? id)
        {
            return await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            // in other codes it was SingleOrDefaultAsync
        }

        public async Task deleteAppointment(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AppointmentViewModel>> getPendingAppointments(ClaimsPrincipal user)
        {
            var appointments = await _context.Appointments.Select(a => new AppointmentViewModel
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

            return appointments;
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
                                                             .Select(g => new TotalAppointmentsPerServiceViewModel
                                                             {
                                                                 ServiceName = g.Key,
                                                                 Count = g.Count()
                                                             })
                                                             .ToListAsync();
            return TotalAppointmentsPerService.Select(x => x.Count).ToList();
        }

        public async Task<List<ActiveAppointmentViewModel>> getTodaysAppointments()
        {
            var today = DateTime.Today.ToString("M/d/yyyy");

            return await _context.Appointments
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
                .ToListAsync();
        }
        public int GetTotalAppointments()
        {
            return _context.Appointments.Count();
        }

    }
}

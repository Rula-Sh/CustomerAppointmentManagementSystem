using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        const string usersPath = "~/Views/Admin/Dashboard/Index.cshtml";

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
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

            var bestEmployeeName = _context.Users.Where(u => u.Id == bestEmployee.EmployeeId).Select(e => e.FullName).SingleOrDefault();


            //Average Appointments per Employee
            var TotalAppointments = _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved").Count();
            //var TotalEmployees = _context.Users.Include(u => u.UserRoles).Where(u=> u)
            var TotalEmployees = _context.UserRoles.Include(u => u.User).Where(u => u.Role.Name == "Employee").Count();
            double avgAppointmentsPerEmployee = (double)TotalAppointments / (TotalEmployees-1);

            var appointmentsPerEmployee = _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved").Count();

            //Most Booked Service
            var mostBookedService = _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved")
                                                    .GroupBy(a => a.ServiceId)
                                                    .Select(group => new
                                                    {
                                                        ServiceId = group.Key,
                                                        AppointmentCount = group.Count()
                                                    })
                                                    .OrderByDescending(g => g.AppointmentCount)
                                                    .FirstOrDefault();

            var mostBookedServiceName = _context.Services.Where(u => u.Id == mostBookedService.ServiceId).Select(e => e.Name).SingleOrDefault();


            // --------------------- Daily Booking ---------------------
            // get Approved Appointments Dates
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
            var dailyCounts = last7DaysDates.Select(d => _context.Appointments.Count(a => a.CreatedAt.Date == d.Date)).ToList();


            // --------------------- Weekly Booking (4 weeks) ---------------------
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


            // --------------------- Appointments Status ---------------------
            string[] status = { "Pending", "Approved", "Rejected", "Completed"};
            var appointmentsStatus = status.Select(s => _context.Appointments.Count(a => a.Status == s))
                                            .ToList();

            // --------------------- Appointments Per Service ---------------------
            var servicesLabel = _context.Services.Select(a => a.Name).ToList();
            var serviceAppointmentsCount = _context.Appointments.Where(a => a.Status == "Completed" || a.Status == "Approved")
                                                              .Include(a => a.Service)
                                                              .GroupBy(a => a.Service.Name)
                                                              .Select(g => new { ServiceName = g.Key, Count = g.Count() })
                                                              .ToList();

            // --------------------- Active Customers ---------------------
            var today = DateTime.Today.ToString("M/d/yyyy");

            var activeAppointments = _context.Appointments
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
                .ToList();



            var model = new DashboardViewModel
            {
                TotalUsers = _context.Users.Count(),
                TotalServices = _context.Services.Count(),
                TotalAppointments = _context.Appointments.Count(),

                BestEmployee = bestEmployeeName,
                AvgAppointmentsPerEmployee = avgAppointmentsPerEmployee,
                MostBookedService = mostBookedServiceName,
                
                last7Days = last7DaysLabels,
                dailyBookingCounts = dailyCounts,

                lastWeeksinMonth = weeklyLabels,
                weeklyBookingCounts = weeklyCounts,

                appointmentsStatusCount = appointmentsStatus,

                servicesLabel = servicesLabel,
                serviceAppointmentsCount = serviceAppointmentsCount.Select(x => x.Count).ToList(),

                ActiveAppointments= activeAppointments,
            };

            return View(usersPath, model);
        }
    }
}

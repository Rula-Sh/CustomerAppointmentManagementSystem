using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task addAppointment(BookAppointmentViewModel model,ClaimsPrincipal user)
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

    }
}

using System.Security.Claims;
using AutoMapper;
using CAMS.Application.DTOs;
using CAMS.Application.Interfaces;
using CAMS.Data;
using CAMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CAMS.Application.Services
{
    public class NotificationManagerService : INotificationManagerService
    {

        private readonly ApplicationDbContext _context;
        private readonly IManageUsersService _manageUsers;
        private readonly Lazy<IManageAppointmentsService> _manageAppointments; //Lazy<> preserves the architecture and breaks the cycle without redesigning everything (i had a circular dependency ManageServices → NotificationManager → ManageAppointmentsService → NotificationManager)
        //Using Lazy<>: 1- Breaks the circular dependency at runtime(by deferring resolution)
        //              2- Keeps your services' structure and separation of concerns intact
        //              3- Doesn’t require architectural overhaul or complex event systems
        // Lazy<T> is a generic class in the .NET Framework that enables lazy initialization — meaning the object of type T is not created until it's actually needed.
        private readonly IMapper _mapper;
        private readonly ISignalRNotifierService _signalRNotifier;
        public NotificationManagerService(ApplicationDbContext context, IManageUsersService manageUsers, Lazy<IManageAppointmentsService> manageAppointments, IMapper mapper, ISignalRNotifierService signalRNotifier)
        {
            _context = context;
            _manageUsers = manageUsers;
            _manageAppointments = manageAppointments;
            _mapper = mapper;
            _signalRNotifier = signalRNotifier;
        }

        public async Task<List<Notification>> GetUserNotifications(ClaimsPrincipal user)
        {
            var userId = Int32.Parse(_manageUsers.GetUserId(user));
            return await _context.Notifications.Where(n => n.UserId == userId && n.IsRead == false).ToListAsync();
        }
        public async Task CreateNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        /*public async Task CreateNotificationOnServiceDeleteForCustomer(int serviceId)
        {
            // get appointments related to the service before deleting it
            var appointmentsDTOs = await _manageAppointments.Value.getAppointmentsFromServiceId(serviceId);


            //create notifications for users with the deleted service appointment
            foreach (var appointment in appointmentsDTOs)
            {
                var notificationDTO = new NotificationDTO
                {
                    UserId = appointment.CustomerId,
                    Message = $"{appointment.Name} appointment on: {appointment.Date} has been canceled",
                };
                var notification = _mapper.Map<Notification>(notificationDTO);
                await CreateNotification(notification);
            }
            await _signalRNotifier.SendNotificationAsync();
        }*/

        public async Task CreateNotificationOnServiceActionForAdmin(int serviceId, string serviceName, ClaimsPrincipal user, string status)
        {
            var employeeId = int.Parse(_manageUsers.GetUserId(user));
            var employeeName = _context.Users.Where(u => u.Id == employeeId).Select(u => u.FullName).FirstOrDefault();
            if (employeeId != 1) {
                var notificationDTO = new NotificationDTO
                {
                    UserId = 1,
                    Message = $"Employee {employeeName} (ID: {employeeId}), Have {status} {(status == "Created" ? $"{serviceName} Service." : $"the Service {serviceName} (ID: {serviceId}).")}",
                };
                var notification = _mapper.Map<Notification>(notificationDTO);
                await CreateNotification(notification);

                await _signalRNotifier.SendNotificationAsync();
            }
        }

        public async Task CreateNotificationOnAppointmentDelete(int appointmentId)
        {
            // get appointment before deleting it
            var appointment = await _manageAppointments.Value.getAppointmentById(appointmentId);

            //create a notification for the     employee with the assigned deleted appointment
            var notificationDTO = new NotificationDTO
            {
                UserId = appointment.EmployeeId,
                Message = $"a customer has canceled their appointment on: {appointment.Date}",
            };
            var notification = _mapper.Map<Notification>(notificationDTO);
            await CreateNotification(notification);
            await _signalRNotifier.SendNotificationAsync();
        }

        public async Task ReadNotification(int notificationId)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.Id == notificationId);
            notification.IsRead = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }
}

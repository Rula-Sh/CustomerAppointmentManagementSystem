using AutoMapper;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace BusinessLogicLayer.Services
{
    public class NotificationManager : INotificationManager
    {

        private readonly ApplicationDbContext _context;
        private readonly IManageUsers _manageUsers;
        private readonly Lazy<IManageAppointments> _manageAppointments; //Lazy<> preserves the architecture and breaks the cycle without redesigning everything (i had a circular dependency ManageServices → NotificationManager → ManageAppointments → NotificationManager)
        //Using Lazy<>: 1-Breaks the circular dependency at runtime(by deferring resolution)
        //              2- Keeps your services' structure and separation of concerns intact
        //              3- Doesn’t require architectural overhaul or complex event systems
        private readonly IMapper _mapper;
        private readonly ISignalRNotifier _signalRNotifier;

        public NotificationManager(ApplicationDbContext context, IManageUsers manageUsers, Lazy<IManageAppointments> manageAppointments, IMapper mapper, ISignalRNotifier signalRNotifier)
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

        public async Task CreateNotificationOnServiceDelete(int serviceId)
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
        }

        public async Task CreateNotificationOnAppointmentDelete(int appointmentId)
        {
            // get appointment before deleting it
            var appointment = await _manageAppointments.Value.getAppointmentById(appointmentId);

            //create a notification for the employee with the assigned deleted appointment
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

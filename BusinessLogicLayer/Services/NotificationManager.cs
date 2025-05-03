using AutoMapper;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessLogicLayer.Services
{
    public class NotificationManager : INotificationManager
    {

        private readonly ApplicationDbContext _context;
        private readonly IManageUsers _manageUsers;
        private readonly IManageAppointments _manageAppointments;
        private readonly IMapper _mapper;

        public NotificationManager(ApplicationDbContext context, IManageUsers manageUsers, IManageAppointments manageAppointments, IMapper mapper)
        {
            _context = context;
            _manageUsers = manageUsers;
            _manageAppointments = manageAppointments;
            _mapper = mapper;
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
            var appointmentsDTOs = await _manageAppointments.getAppointmentsFromServiceId(serviceId);


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

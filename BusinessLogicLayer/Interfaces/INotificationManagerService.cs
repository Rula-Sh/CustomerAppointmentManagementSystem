using DataAccessLayer.Models;
using System.Security.Claims;

namespace BusinessLogicLayer.Interfaces
{
    public interface INotificationManagerService
    {
        Task<List<Notification>> GetUserNotifications(ClaimsPrincipal user);
        Task CreateNotification(Notification notification);
        Task CreateNotificationOnServiceDelete(int serviceId);
        Task CreateNotificationOnAppointmentDelete(int appointmentId);
        Task ReadNotification(int id);
    }
}

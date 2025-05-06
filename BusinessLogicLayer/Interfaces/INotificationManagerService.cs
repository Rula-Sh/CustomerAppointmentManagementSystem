using DataAccessLayer.Models;
using System.Security.Claims;

namespace BusinessLogicLayer.Interfaces
{
    public interface INotificationManagerService
    {
        Task<List<Notification>> GetUserNotifications(ClaimsPrincipal user);
        Task CreateNotification(Notification notification);
        //Task CreateNotificationOnServiceDeleteForCustomer(int serviceId);
        Task CreateNotificationOnServiceActionForAdmin(int serviceId, string serviceName, ClaimsPrincipal user, string action);
        Task CreateNotificationOnAppointmentDelete(int appointmentId);
        Task ReadNotification(int id);
    }
}

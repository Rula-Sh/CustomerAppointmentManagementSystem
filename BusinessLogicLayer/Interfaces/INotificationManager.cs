using DataAccessLayer.Models;
using System.Security.Claims;

namespace BusinessLogicLayer.Interfaces
{
    public interface INotificationManager
    {
        Task<List<Notification>> GetUserNotifications(ClaimsPrincipal user);
        Task CreateNotification(Notification notification);
        Task CreateNotificationOnServiceDelete(int serviceId);
        Task ReadNotification(int id);
    }
}

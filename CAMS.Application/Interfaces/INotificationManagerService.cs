using CAMS.Data.Models;
using System.Security.Claims;

namespace CAMS.Application.Interfaces
{
    public interface INotificationManagerService
    {
        Task<List<Notification>> GetUserNotifications(ClaimsPrincipal user);
        Task CreateNotification(Notification notification);
        //Task CreateNotificationOnServiceDeleteForCustomer(int serviceId);
        Task CreateNotificationForAdminOnServiceAction(int serviceId, string serviceName, ClaimsPrincipal user, string action);
        Task CreateNotificationForEmployeeOnAppointmentCreateOrDelete(int appointmentId, string action);
        Task CreateNotificationForCustomerOnAppointmentStatusChange(int appointmentId);
        Task ReadNotification(int id);
    }
}

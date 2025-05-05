using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationManagerService _notificationManager;

        public NotificationController(INotificationManagerService notificationManager)
        {
            _notificationManager = notificationManager;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationManager.GetUserNotifications(User);
            //return Ok(notifications);
            return Ok(new { notifs = notifications, notifscount = notifications.Count });
            //return View();
        }

        public IActionResult ReadNotification(int notificationId)
        {
            _notificationManager.ReadNotification(notificationId);
            return Ok();
        }
    }
}

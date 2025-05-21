using CAMS.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using CAMS.Web.SignalR;

namespace CAMS.Application.Services
{
    public class SignalRNotifierService : ISignalRNotifierService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotifierService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync()
        {
            await _hubContext.Clients.All.SendAsync("displayNotification", "");
        }
    }
}

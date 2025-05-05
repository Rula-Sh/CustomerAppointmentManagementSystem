using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.SignalR;
using PresentationLayer.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
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
            await _hubContext.Clients.All.SendAsync("displayNotification","");
        }
    }
}

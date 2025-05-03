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
    public class SignalRNotifier : ISignalRNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync()
        {
            await _hubContext.Clients.All.SendAsync("displayNotification","");
        }
    }
}

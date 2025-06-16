using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Application.Interfaces
{
    public interface ISignalRNotifierService
    {
        Task SendNotificationAsync();
    }
}

using DataAccessLayer.Models;
using DataAccessLayer.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IManageServices
    {
        Task<List<ServiceViewModel>> GetServices();
        Task addService(Service service);
        Task<Service> getService(int? id);
        ServiceViewModel getSelectedServiceDetails(Service service);
        Task DeleteService(Service service);
        Task<Service> getServiceById(int? id);

        string GetMostBookedServiceName();
        Task<List<string>> GetServicesNames();
        int GetTotalServices();
    }
}

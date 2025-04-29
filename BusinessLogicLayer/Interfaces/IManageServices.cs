using BusinessLogicLayer.DTOs;
using DataAccessLayer.Models;

namespace BusinessLogicLayer.Interfaces
{
    public interface IManageServices
    {
        Task<List<ServiceDTO>> GetServices();
        Task addService(Service service);
        Task<Service> getService(int? id);
        ServiceDTO getSelectedServiceDetails(Service service);
        Task DeleteService(Service service);
        Task<Service> getServiceById(int? id);

        string GetMostBookedServiceName();
        Task<List<string>> GetServicesNames();
        int GetTotalServices();
    }
}

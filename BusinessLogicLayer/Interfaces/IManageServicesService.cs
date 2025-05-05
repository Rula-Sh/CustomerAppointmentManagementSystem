using BusinessLogicLayer.DTOs;
using DataAccessLayer.Models;

namespace BusinessLogicLayer.Interfaces
{
    public interface IManageServicesService
    {
        Task<List<ServiceDTO>> GetServices();
        Task addService(ServiceDTO serviceDTO);
        Task<ServiceDTO> getService(int? id);
        //ServiceDTO getSelectedServiceDetails(ServiceDTO service);
        Task DeleteService(ServiceDTO serviceDTO);
        Task<ServiceDTO> getServiceById(int? id);

        string GetMostBookedServiceName();
        Task<List<string>> GetServicesNames();
        int GetTotalServices();
    }
}

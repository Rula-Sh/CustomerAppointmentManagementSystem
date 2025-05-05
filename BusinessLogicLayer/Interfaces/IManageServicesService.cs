using BusinessLogicLayer.DTOs;
using DataAccessLayer.Models;
using System.Security.Claims;

namespace BusinessLogicLayer.Interfaces
{
    public interface IManageServicesService
    {
        Task<List<ServiceDTO>> GetAllServices();
        Task<List<ServiceDTO>> GetAvailableServicesInAddAppointment(ClaimsPrincipal user);
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

using CAMS.Application.DTOs;
using CAMS.Data.Models;
using System.Security.Claims;

namespace CAMS.Application.Interfaces
{
    public interface IManageAppointmentsService
    {
        Task<List<AppointmentDTO>> getAppointmentsBasedOnRole(ClaimsPrincipal user);
        List<int> getServicesIdsFromActiveAndPendingAppointments(ClaimsPrincipal user);
        Task addAppointment(BookAppointmentDTO bookAppointmentDTO, ClaimsPrincipal user);
        Task<AppointmentDTO> appointmentDetails(int? id);
        Task<Appointment> getAppointmentById(int? id);
        Task deleteAppointment(Appointment appointment);
        Task<List<AppointmentDTO>> getPendingAppointments(ClaimsPrincipal user);
        Task updateAppointment(Appointment appointment);

        string getProviderNameWithMostCompletedAppointments();
        double GetAverageAppointmentsPerProvider();
        List<DateTime> getLast7DaysDates();
        List<string> getLast7Days();
        Task<List<int>> getTotalAppointmentsFromPast7Days();
        List<DateTime> getLast4WeeksDates();
        List<int> getTotalApprovedAppointemntPerWeek();
        List<int> GetAppointmentsStatusCount();
        Task<List<int>> getTotalAppointmentsPerService();
        Task<List<ActiveAppointmentDTO>> getTodaysAppointments();
        int GetTotalAppointments();
        Task<List<AppointmentDTO>> getActiveAppointmentsFromServiceId(int? id);
    }
}

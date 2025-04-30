using BusinessLogicLayer.DTOs;
using DataAccessLayer.Models;
using System.Security.Claims;

namespace BusinessLogicLayer.Interfaces
{
    public interface IManageAppointments
    {
        Task<List<AppointmentDTO>> getAppointmentsBasedOnRole(ClaimsPrincipal user);
        Task<BookAppointmentDTO> ViewAddAppointment();
        Task addAppointment(BookAppointmentDTO bookAppointmentDTO, ClaimsPrincipal user);
        Task<AppointmentDTO> appointmentDetails(int? id);
        Task<Appointment> getAppointmentById(int? id);
        Task deleteAppointment(Appointment appointment);
        Task<List<AppointmentDTO>> getPendingAppointments(ClaimsPrincipal user);
        Task updateAppointment(Appointment appointment);

        string getEmployeeNameWithMostCompleteAndApprovedAppointments();
        double GetAverageAppointmentsPerEmployee();
        List<DateTime> getLast7DaysDates();
        List<string> getLast7Days();
        Task<List<int>> getTotalAppointmentsFromPast7Days();
        List<DateTime> getLast4WeeksDates();
        List<int> getTotalApprovedAppointemntPerWeek();
        List<int> GetAppointmentsStatusCount();
        Task<List<int>> getTotalAppointmentsPerService();
        Task<List<ActiveAppointmentDTO>> getTodaysAppointments();
        int GetTotalAppointments();
    }
}

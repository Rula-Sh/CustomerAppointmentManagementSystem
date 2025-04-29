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
    public interface IManageAppointments
    {
        Task<List<AppointmentDTO>> getAppointmentsBasedOnRole(ClaimsPrincipal user);
        Task<BookAppointmentDTO> ViewAddAppointment();
        Task addAppointment(BookAppointmentDTO model, ClaimsPrincipal user);
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

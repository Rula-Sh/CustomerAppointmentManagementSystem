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
        Task<List<AppointmentViewModel>> getAppointmentsBasedOnRole(ClaimsPrincipal user);
        Task<BookAppointmentViewModel> ViewAddAppointment();
        Task addAppointment(BookAppointmentViewModel model, ClaimsPrincipal user);
        Task<AppointmentViewModel> appointmentDetails(int? id);
        Task<Appointment> getAppointmentById(int? id);
        Task deleteAppointment(Appointment appointment);
        Task<List<AppointmentViewModel>> getPendingAppointments(ClaimsPrincipal user);
        Task updateAppointment(Appointment appointment);
    }
}

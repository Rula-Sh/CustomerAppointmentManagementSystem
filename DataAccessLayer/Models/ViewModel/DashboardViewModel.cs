using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ViewModel
{
    public class DashboardViewModel
    {
        //public List<User> Users  { get; set; }
        //public List<Service> Services { get; set; }
        //public List<Appointment> Appointments { get; set; }

        public int TotalUsers { get; set; }
        public int TotalServices { get; set; }
        public int TotalAppointments { get; set; }

        public string BestEmployee { get; set; }
        public double AvgAppointmentsPerEmployee { get; set; }
        public string MostBookedService { get; set; }

        public List<string> last7Days { get; set; }
        public List<int> dailyBookingCounts { get; set; }

        public List<string> lastWeeksinMonth { get; set; }
        public List<int> weeklyBookingCounts { get; set; }

        public List<int> appointmentsStatusCount { get; set; }

        public List<string> servicesLabel { get; set; }
        public List<int> serviceAppointmentsCount { get; set; }

        public List<ActiveAppointmentViewModel> ActiveAppointments { get; set; }

    }
}

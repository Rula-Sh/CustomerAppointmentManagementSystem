﻿namespace CAMS.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalServices { get; set; }
        public int TotalAppointments { get; set; }

        public string BestProvider { get; set; }
        public double AvgAppointmentsPerProvider { get; set; }
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

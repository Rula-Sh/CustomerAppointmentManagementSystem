namespace BusinessLogicLayer.DTOs
{
    public class DashboardDTO
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

        public List<ActiveAppointmentDTO> ActiveAppointments { get; set; }

    }
}

namespace CAMS.Data.Models
{
    public class AppointmentService
    {
        // Many-to-Many relationship table (Appointment and Service)
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}

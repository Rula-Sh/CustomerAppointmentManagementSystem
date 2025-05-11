using System.ComponentModel.DataAnnotations.Schema;

namespace CAMS.Data.Models
{
    public class AppointmentService
    {
        // Many-to-Many relationship table (Appointment and Service)
        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}

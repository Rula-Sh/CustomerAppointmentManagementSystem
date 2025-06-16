using CAMS.Data.Models;

namespace CAMS.Web.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        // Foreign keys
        public int CustomerId { get; set; }
        public int ProviderId { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        // One-to-Many relationship (User[Provider/Customer] and Appointment) *** Foreign keys included on top
        public virtual User Customer { get; set; }
        public virtual User Provider { get; set; }

        // Many-to-Many relationship (Appointment and Service)
        public virtual ICollection<AppointmentService> AppointmentServices { get; set; }
    }
}

using DataAccessLayer.Models;

namespace BusinessLogicLayer.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }

        // Foreign keys
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        //public string CustomerName { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        // One-to-Many relationship (User[Employee/Customer] and Appointment) *** Foreign keys included on top
        public virtual User Customer { get; set; }
        public virtual User Employee { get; set; }

        // Many-to-Many relationship (Appointment and Service)
        public virtual ICollection<AppointmentService> AppointmentServices { get; set; }
    }
}

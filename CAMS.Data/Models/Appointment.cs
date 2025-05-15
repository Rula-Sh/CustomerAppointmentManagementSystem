using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAMS.Data.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        // Foreign keys
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        [MaxLength(200)]
        public string Notes { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        // One-to-Many relationship (User[Employee/Customer] and Appointment) *** Foreign keys included on top
        [ForeignKey("CustomerId")] 
        public virtual User Customer { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual User Employee { get; set; }

        // Many-to-Many relationship (Appointment and Service)
        //public virtual ICollection<AppointmentService> AppointmentServices { get; set; }
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
    }
}
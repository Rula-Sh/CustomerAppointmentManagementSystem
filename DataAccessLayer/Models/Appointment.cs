using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        // Foreign keys
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [ForeignKey("Employee")]
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
        public virtual User Customer { get; set; }
        public virtual User Employee { get; set; }

        // Many-to-Many relationship (Appointment and Service)
        //public virtual ICollection<AppointmentService> AppointmentServices { get; set; }
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
    }
}

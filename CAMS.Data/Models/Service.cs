using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(150)]
        public string Description { get; set; }
        [Required]
        public string Duration { get; set; }
        [Required]
        public decimal Price { get; set; }
        //public DateTime[] AvailableTimeSlots { get; set; }

        [Required]
        public virtual ICollection<ServiceDate> ServiceDates { get; set; }
        //public virtual ICollection<DateTimeSlotGroup> DateTimeSlotGroup { get; set; }

        // Many-to-Many relationship (Appointment and Service)
        //public virtual ICollection<AppointmentService> AppointmentServices { get; set; }
        [Required]
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [Required]
        [MaxLength(150)]
        public string Description { get; set; }
        [Required]
        public string Duration { get; set; }
        [Required]
        [Range(0, 9999.99, ErrorMessage = "Price must be between 0 and 9999.99")]
        public decimal Price { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual User Employee { get; set; }

        public virtual ICollection<ServiceDate> ServiceDates { get; set; } = new List<ServiceDate>();

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}

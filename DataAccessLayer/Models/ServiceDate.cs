using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class ServiceDate
    {
        public int Id { get; set; }
        [Required]
        [ForeignKey("ServiceId")]
        public int ServiceId { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public Service Service { get; set; }
        [Required]
        public virtual ICollection<ServiceTimeSlot> ServiceTimeSlots { get; set; }
    }
}

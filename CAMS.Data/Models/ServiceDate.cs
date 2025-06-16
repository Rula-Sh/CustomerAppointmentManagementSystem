using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class ServiceDate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ServiceId { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }
        public virtual ICollection<ServiceTimeSlot> ServiceTimeSlots { get; set; } = new List<ServiceTimeSlot>();
    }
}

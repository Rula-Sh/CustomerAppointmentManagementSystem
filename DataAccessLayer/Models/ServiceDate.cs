using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class ServiceDate
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public DateOnly Date { get; set; }
        public Service Service { get; set; }
        public virtual ICollection<ServiceTimeSlot> ServiceTimeSlots { get; set; }

    }
}

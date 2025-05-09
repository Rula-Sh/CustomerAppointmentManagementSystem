using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class ServiceTimeSlot
    {
        public int Id { get; set; }
        public int ServiceDateId { get; set; }
        public string Time { get; set; }
        public ServiceDate ServiceDate { get; set; }
    }
}

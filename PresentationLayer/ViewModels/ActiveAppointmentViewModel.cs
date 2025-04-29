using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ViewModel
{
    public class ActiveAppointmentViewModel
    {
        public string CustomerName { get; set; }
        public string AppointmentDate { get; set; }
        public string ServiceName { get; set; }
    }
}

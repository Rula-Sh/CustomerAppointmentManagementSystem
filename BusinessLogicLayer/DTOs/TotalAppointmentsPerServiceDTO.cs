using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ViewModel
{
    public class TotalAppointmentsPerServiceDTO
    {
        public string ServiceName { get; set; }
        public int Count { get; set; }
    }
}

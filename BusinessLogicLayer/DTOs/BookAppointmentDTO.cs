using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ViewModel
{
    public class BookAppointmentDTO
    {
        public int Id { get; set; }
        public string Date { get; set; }

        public List<ServiceDTO> Services { get; set; }
        public int ServiceId { get; set; }
        public decimal ServicePrice { get; set; }
        public string ServiceName { get; set; }
    }
}

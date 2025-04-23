using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ViewModel
{
    public class DashboardViewModel
    {
        public List<User> Users  { get; set; }
        public List<Service> Services { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}

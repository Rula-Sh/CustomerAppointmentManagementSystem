using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ViewModel
{
    public class DateTimeSlotGroupViewModel
    {
        public string Date { get; set; } // "dd-MM-yyyy" format from frontend
        public List<string> TimeSlots { get; set; }

        //when it was a model
        /*public int Id { get; set; }
        public int ServiceId { get; set; }
        public DateOnly Date { get; set; }
        public List<string> TimeSlots { get; set; }

        public Service Service { get; set; }*/
    }
}

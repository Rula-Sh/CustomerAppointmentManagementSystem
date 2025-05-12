using CAMS.Data.Models;

namespace CAMS.Web.ViewModels
{
    public class ServiceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmployeeId { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public decimal Price { get; set; }
        //public List<DateTime> AvailableTimeSlots { get; set; }

        //public List<string> SelectedDates { get; set; }
        //public Dictionary<string, List<string>> TimeSlots { get; set; }// Dictionary where key is the date string and value is a list of time strings
        public User Employee { get; set; }
        public List<DateTimeSlotGroupViewModel> DateTimeSlotGroups { get; set; }
    }
}

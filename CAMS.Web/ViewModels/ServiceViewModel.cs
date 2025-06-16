using CAMS.Data.Models;

namespace CAMS.Web.ViewModels
{
    public class ServiceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProviderId { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public decimal Price { get; set; }
        public User Provider { get; set; }
        public List<DateTimeSlotGroupViewModel> DateTimeSlotGroups { get; set; }
    }
}

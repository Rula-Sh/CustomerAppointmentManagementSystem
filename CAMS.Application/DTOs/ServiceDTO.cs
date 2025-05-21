using CAMS.Data.Models;

namespace CAMS.Application.DTOs
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmployeeId { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public decimal Price { get; set; }

        public User Employee { get; set; }
        public List<DateTimeSlotGroupDTO> DateTimeSlotGroups { get; set; }
    }
}

namespace CAMS.Application.DTOs
{
    public class DateTimeSlotGroupDTO
    {
        public string Date { get; set; } // saved as "dd/MM/yyyy" format and viewed as "dd-MM-yyyy" format
        public List<string> TimeSlots { get; set; }
    }
}

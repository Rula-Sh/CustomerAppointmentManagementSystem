namespace BusinessLogicLayer.DTOs
{
    public class DateTimeSlotGroupDTO
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

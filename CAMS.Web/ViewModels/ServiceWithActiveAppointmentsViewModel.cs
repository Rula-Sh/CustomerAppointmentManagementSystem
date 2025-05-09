namespace PresentationLayer.ViewModels
{
    public class ServiceWithActiveAppointmentsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public decimal Price { get; set; }
        public List<DateTimeSlotGroupViewModel> DateTimeSlotGroups { get; set; }

        public int AppointmentId { get; set; }
        public List<AppointmentViewModel> ActiveAppointments { get; set; }
    }
}

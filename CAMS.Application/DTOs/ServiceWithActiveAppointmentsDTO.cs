namespace CAMS.Application.DTOs
{
    public class ServiceWithActiveAppointmentsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public decimal Price { get; set; }

        public int ProviderId { get; set; }
        public UserDTO Provider { get; set; }

        public List<DateTimeSlotGroupDTO> DateTimeSlotGroups { get; set; }

        public int AppointmentId { get; set; }
        public List<AppointmentDTO> ActiveAppointments { get; set; }
    }
}
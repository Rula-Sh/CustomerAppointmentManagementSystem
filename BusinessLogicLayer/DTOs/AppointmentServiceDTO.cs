namespace BusinessLogicLayer.DTOs
{
    public class AppointmentServiceDTO
    {
        // Many-to-Many relationship table (Appointment and Service)
        public int AppointmentId { get; set; }
        public AppointmentDTO Appointment { get; set; }
        public int ServiceId { get; set; }
        public ServiceDTO Service { get; set; }
    }
}

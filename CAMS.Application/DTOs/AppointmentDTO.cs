using CAMS.Data.Models;

namespace CAMS.Application.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }

        // Foreign keys
        public int CustomerId { get; set; }
        public int ProviderId { get; set; }

        public string Name { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User Customer { get; set; }
        public virtual User Provider { get; set; }

        public virtual ICollection<AppointmentService> AppointmentServices { get; set; }
    }
}

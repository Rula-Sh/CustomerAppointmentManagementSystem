using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class ServiceTimeSlot
    {
        public int Id { get; set; }
        [Required]
        [ForeignKey("ServiceDate")]
        public int ServiceDateId { get; set; }
        [Required]
        public string Time { get; set; }
        public ServiceDate ServiceDate { get; set; }
    }
}

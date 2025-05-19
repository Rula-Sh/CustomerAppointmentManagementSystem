using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PerformerId { get; set; }
        [Required]
        public string actionDescription { get; set; }
        [Required]
        public string ActionType { get; set; }
        [Required]
        public DateTime Time { get; set; }

        [ForeignKey("PerformerId")]
        public User User { get; set; }
    }
}

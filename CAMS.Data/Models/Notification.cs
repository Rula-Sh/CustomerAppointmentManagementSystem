using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Message { get; set; }
        [Required]
        public bool IsRead { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}

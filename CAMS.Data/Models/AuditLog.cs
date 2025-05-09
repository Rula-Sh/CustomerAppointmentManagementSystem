using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int PerformerId { get; set; }
        public string Performer { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
    }
}

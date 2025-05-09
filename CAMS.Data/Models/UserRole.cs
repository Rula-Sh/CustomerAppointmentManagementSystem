using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        // Many-to-Many relationship table (User and Role)
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual Role Role { get; set; }
        public int RoleId { get; set; }
    }
}

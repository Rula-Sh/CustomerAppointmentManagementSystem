using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        // Many-to-Many relationship table (User and Role)
        public User User { get; set; }
        //public int UserId { get; set; }
        public Role Role { get; set; }
        //public int RoleId { get; set; }
    }
}

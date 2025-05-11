using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        // Many-to-Many relationship table (User and Role)
        //public int UserId { get; set; }
        public virtual User User { get; set; }
        //public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        //commented UserId RoleId because of the error: "System.InvalidOperationException: 'The value of 'UserRole.UserId' is unknown when attempting to save changes. This is because the property is also part of a foreign key for which the principal entity in the relationship is not known.'"
    }
}

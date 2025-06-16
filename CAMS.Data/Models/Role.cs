using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class Role : IdentityRole<int>
    {
        // The properties of IdentityRole are already included in this class

        // Many-to-Many relationship (User and Role)
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

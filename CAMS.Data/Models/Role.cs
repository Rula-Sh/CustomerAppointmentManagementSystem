using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMS.Data.Models
{
    public class Role : IdentityRole<int>
    {
        //public int Id { get; set; }
        //public string Name { get; set; }

        // Many-to-Many relationship (User and Role)
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}

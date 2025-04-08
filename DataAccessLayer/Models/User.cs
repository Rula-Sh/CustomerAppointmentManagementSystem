using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class User : IdentityUser<int> // make the Id to be of type int instead of the default string in IdentityUser... doing so, i must convert the type to string to all Identity tables
    {
        //public int Id { get; set; }
        public string FullName { get; set; }
        //public string Email { get; set; }
        //public string PasswordHash { get; set; }
        //public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }

        // Many-to-Many relationship (User and Role)
        public virtual ICollection<UserRole> UserRoles { get; set; }

        // One-to-Many relationship (User[Employee/Customer] and Appointment)
        public virtual ICollection<Appointment> CustomerAppointments { get; set; }
        public virtual ICollection<Appointment> EmployeeAppointments { get; set; }
    }
}

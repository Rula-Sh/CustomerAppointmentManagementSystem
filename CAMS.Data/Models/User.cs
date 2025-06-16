using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CAMS.Data.Models
{
    public class User : IdentityUser<int> // make the Id to be of type int instead of the default string in IdentityUser... doing so, i must convert the type to string to all Identity tables
                                          // i made the class extends IdentityUser<int> so that I can update on it in the database and not replace it
    {
        // The properties of IdentityUser are already included in this class


        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(60)]
        public string FullName { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public byte[]? ProfilePicture { get; set; }

        [RegularExpression(@"^07[789]\d{7}$", ErrorMessage = "Phone number must start with 077, 078, or 079 and contain 10 digits in total.")]
        public string? PhoneNumber { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime LastActivityDate { get; set; }

        // Many-to-Many relationship (User and Role)
        public virtual ICollection<UserRole> UserRoles { get; set; }

        // One-to-Many relationship (User[Provider/Customer] and Appointment)
        public virtual ICollection<Appointment> CustomerAppointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Appointment> ProviderAppointments { get; set; } = new List<Appointment>();

        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();



        // after creating the properties run add-migration AddNewColumnsToUsersTable on the Package Manager Console ->
        // this will result to an empty migration because in the ApplicationDbContext inherits IdentityDbContext.. and IdentityDbContext uses the IdentityUser Which i changed it to be used in the ApplicationUser... thats why i need to: 
        // 1- Change the line: builder.Entity<IdentityUser>().ToTable("Users", "security"); to => builder.Entity<ApplicationUser>().ToTable("Users", "security");...
        // 2- Change the inheritance to : IdentityDbContext<ApplicationUser> so that the  IdentityDbContext uses the ApplicationUser rather than the default IdentityUser....
        // 3- Change the program.cs file on the line builder.Services.AddDefaultIdentity<IdentityUser> to => builder.Services.AddIdentity<ApplicationUser, IdentityRole>
        // 4- Remove the old migration AddNewColumnsToUsersTable so that i dont get an error that it already exist.. buy running Remove-Migration on the Package Manager Console
        // 5- Add the same imgration again by running add-migration AddNewColumnsToUsersTable on the Package Manager Console
        // 6- Finally Update the database by runnning update-database on the Package Manager Console
        // even though i updated the database with the required columns but still i will get an error when i run te application in IdentityUser... that is because _Layout.cshtml calls _LoginPartial... and that file still uses the old IdentityUser... so i need to update it to ApplicationUser
        // but still if i wanted for example to register a user i will also get the same error... that is because i also need to update all the pages from using the IdentityUser to using the ApplicationUser
        // there is another _Layout file that is inside the Manage file... it has the _ManageNav that we also need to replace the IdentityUser with ApplicationUser so that i dont get an error when i want to update my profile

    }
}

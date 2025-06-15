namespace CAMS.Web.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string LastActivity { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }

}

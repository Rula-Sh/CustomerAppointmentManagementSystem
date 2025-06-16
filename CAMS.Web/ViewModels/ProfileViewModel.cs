using System.ComponentModel.DataAnnotations;

namespace CAMS.Web.ViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public IFormFile? ProfilePictureUpload { get; set; }
        public byte[]? ProfilePicture { get; set; }
        [RegularExpression(@"^07[789]\d{7}$", ErrorMessage = "Phone number must start with 077, 078, or 079 and contain 10 digits in total.")]
        public string? PhoneNumber { get; set; }
    }
}

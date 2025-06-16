using System.ComponentModel.DataAnnotations;

namespace CAMS.Web.ViewModels
{
    public class AddProviderViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^07[789]\d{7}$", ErrorMessage = "Phone number must start with 077, 078, or 079 and contain 10 digits in total.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Profile Picture Upload is required.")]
        public IFormFile ProfilePictureUpload { get; set; }
        [Required(ErrorMessage = "Profile Picture is required.")]
        public byte[] ProfilePicture { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }
    }
}

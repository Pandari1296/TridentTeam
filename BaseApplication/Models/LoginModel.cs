using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Models
{
    public class LoginModel
    {
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]
        public string? UserEmail { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

    }


    public class ForgotPasswordModel
    {
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]
        public string? UserEmail { get; set; }
    }

    public class ChangePasswordModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{6,}$", ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Number and 1 Special Character")]
        public string? Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}

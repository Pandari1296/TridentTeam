using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Models
{
    public class EmailInviteModel
    {
        [Display(Name = "Email 1:")]
        [Required(ErrorMessage = "The email address is required")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters only.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]
        public string Email1 { get; set; } = "";

        [Display(Name = "Email 2:")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters only.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]
        public string? Email2 { get; set; }

        [Display(Name = "Email 3:")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters only.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]
        public string? Email3 { get; set; }

        [Display(Name = "Email4 :")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters only.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]
        public string? Email4 { get; set; }

        [Display(Name = "Email 5 :")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters only.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]
        public string? Email5 { get; set; }

        [Display(Name = "Email 6 :")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters only.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]
        public string? Email6 { get; set; }
    }
}

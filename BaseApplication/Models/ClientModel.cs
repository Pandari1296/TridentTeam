using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Models
{
    public class ClientModel
    {
        public int Id { get; set; }
        [Display(Name = "Name:")]
        [Required(ErrorMessage = "The Name is required")]
        public string? Name { get; set; }
        [Display(Name = "Email:")]
        [Required(ErrorMessage = "The email address is required")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters only.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]

        public string? Email { get; set; }
        [Display(Name = "Phone:")]
        [Required(ErrorMessage = "The Phone number is required")]
        public string? Phone { get; set; }
        [Display(Name = "Point Of Contacts:")]
        [Required(ErrorMessage = "The Point Of Contact is required")]
        public string? PointOfContacts { get; set; }
        [Display(Name = "Status:")]
        [Required(ErrorMessage = "The Status is required")]
        public bool? Status { get; set; }
    }
}

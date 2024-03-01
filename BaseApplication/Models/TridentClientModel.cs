using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Models
{
    //public class TridentClientModel
    //{
    //    public int Id { get; set; }
    //    [Display(Name = "Name:")]
    //    [Required(ErrorMessage = "The Name is required")]
    //    public string? Name { get; set; }
    //    [Display(Name = "Email:")]
    //    [Required(ErrorMessage = "The email address is required")]
    //    [MaxLength(50, ErrorMessage = "Max length is 50 characters only.")]
    //    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com,@uitts.com and @tapinnov.com domains are allowed.")]
    //    public string? Email { get; set; }
    //    [Display(Name = "Phone:")]
    //    [Required(ErrorMessage = "The Phone number is required")]
    //    public string? Phone { get; set; }
    //    [Display(Name = "Point Of Contacts:")]
    //    [Required(ErrorMessage = "The Point Of Contact is required")]
    //    public string? CoordinatorId { get; set; }
    //    [Display(Name = "Status:")]
    //    [Required(ErrorMessage = "The Status is required")]
    //    public bool? Status { get; set; }
    //}
    public class TridentClientModel
    {
        public int Id { get; set; }

        [Display(Name = "Name:")]
        [Required(ErrorMessage = "The Name is required")]
        public string? Name { get; set; }

        [Display(Name = "Email:")]
        [Required(ErrorMessage = "The email address is required")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters only.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(TeamTrident\.com|uitts\.com|tapinnov\.com)$", ErrorMessage = "Invalid email format. Only @TeamTrident.com, @uitts.com and @tapinnov.com domains are allowed.")]
        public string? Email { get; set; }

        [Display(Name = "Phone:")]
        //[MaxLength(10, ErrorMessage = "Max length is 10 characters only.")]
        [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Invalid phone number format. Please enter a valid phone number.")]
        [Required(ErrorMessage = "The Phone number is required")]
        public string? Phone { get; set; }
        [Display(Name = "AlternatePhone:")]
        //[MaxLength(10, ErrorMessage = "Max length is 10 characters only.")]
        [RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Invalid phone number format. Please enter a valid phone number.")]
        [Required(ErrorMessage = "The AlternatePhone number is required")]
        public string? AlternatePhone { get; set; }

        [Display(Name = "Point Of Contacts:")]
        [Required(ErrorMessage = "The Point Of Contact is required")]
        public int CoordinatorId { get; set; } // Change the type to int
        [Display(Name = "Status:")]
        [Required(ErrorMessage = "The Status is required")]
        public bool? Status { get; set; }
        [Display(Name = "ZipCode:")]
        [Required(ErrorMessage = "The ZipCode is required")]
        public string? ZipCode { get; set; }
        [Display(Name = "State:")]
        [Required(ErrorMessage = "The State is required")]
        public string? State { get; set; }
        [Display(Name = "City:")]
        [Required(ErrorMessage = "The City is required")]
        public string? City { get; set; }
        [Display(Name = "Address1:")]
        [Required(ErrorMessage = "The Address1 is required")]
        public string? Address1 { get; set; }
        [Display(Name = "Notes:")]
        [Required(ErrorMessage = "The Notes is required")]
        public string? Notes { get; set; }
    }
}

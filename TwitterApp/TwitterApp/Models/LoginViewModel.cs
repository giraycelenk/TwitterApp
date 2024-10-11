using System.ComponentModel.DataAnnotations;

namespace TwitterApp.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Adress")]
        public string? Email { get; set; }
        [Required]
        [StringLength(10,ErrorMessage = "{0} field must minimum {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }
    }
}
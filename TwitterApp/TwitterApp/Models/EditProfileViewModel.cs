using System.ComponentModel.DataAnnotations;

namespace TwitterApp.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Location { get; set; }
        [Required(ErrorMessage = "Birth Date is required.")]
        public DateTime BirthDate { get; set; }
    }
}
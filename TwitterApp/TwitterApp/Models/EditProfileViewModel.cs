namespace TwitterApp.Models
{
    public class EditProfileViewModel
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
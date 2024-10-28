using TwitterApp.Entity;

public class ProfileViewModel
{
    public User? User { get; set; }
    public User? CurrentUser { get; set; }
    public bool IsFollowing { get; set; } 
    public bool IsFollower{ get; set; } 
    public string? Tab { get; set; }
}
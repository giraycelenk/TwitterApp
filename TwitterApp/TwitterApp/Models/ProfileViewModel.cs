using TwitterApp.Entity;

public class ProfileViewModel
{
    public User? User { get; set; }
    public List<Tweet> Tweets { get; set; } = new();
}
using TwitterApp.Entity;

public class ProfileViewModel
{
    public User? User { get; set; }
    public List<Tweet> Tweets { get; set; } = new();
    public bool IsFollowing { get; set; } 
    public Dictionary<int, bool> IsLikedByCurrentUser { get; set; }
    public Dictionary<int, bool> IsRetweetedByCurrentUser { get; set; }
    
}
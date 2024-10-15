using TwitterApp.Data.Concrete.EfCore;
using TwitterApp.Entity;
using Microsoft.EntityFrameworkCore;
public class ProfileViewModel
{
    public User? User { get; set; }
    public List<Tweet> Tweets { get; set; } = new();
    public bool IsFollowing { get; set; } 
    
}
using TwitterApp.Entity;

namespace TwitterApp.Models
{
    public class TweetDetailsViewModel
    {
        public Tweet Tweet { get; set; } = new();
        // public Dictionary<int,bool> IsLikedByCurrentUser = new();
        // public Dictionary<int, bool> IsRetweetedByCurrentUser { get; set; } = new();
    }
}
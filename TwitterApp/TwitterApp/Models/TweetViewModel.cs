using TwitterApp.Entity;

namespace TwitterApp.Models
{
    public class TweetViewModel
    {
        public List<Tweet> Tweets { get; set; } = new();
        public bool IsProfilePage { get; set; } 
        public User User { get; set; } = null!;
        public User PageUser { get; set; } = null!;
        public Dictionary<int, bool> IsLikedByCurrentUser { get; set; }
        public Dictionary<int, bool> IsRetweetedByCurrentUser { get; set; }
        public Dictionary<int, User> FollowedRetweetsUsers { get; set; }

    }
}
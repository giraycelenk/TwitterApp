using TwitterApp.Entity;

namespace TwitterApp.Models
{
    public class TweetViewModel
    {
        public List<Tweet> Tweets { get; set; } = new();
        public Dictionary<int, bool> IsLikedByCurrentUser { get; set; }
        public Dictionary<int, bool> IsRetweetedByCurrentUser { get; set; }

    }
}
using TwitterApp.Entity;

namespace TwitterApp.Models
{
    public class TweetViewModel
    {
        public List<Tweet> Tweets {get;set;} = new();
    }
}
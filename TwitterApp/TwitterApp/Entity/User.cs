namespace TwitterApp.Entity
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public List<Tweet> Tweets { get; set; } = new List<Tweet>();
        public List<Like> Likes { get; set; } = new List<Like>();
        public List<Retweet> Retweets { get; set; } = new List<Retweet>();
        public List<Mention> Mentions { get; set; } = new List<Mention>();
    }
}
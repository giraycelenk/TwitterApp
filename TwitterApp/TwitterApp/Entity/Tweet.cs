namespace TwitterApp.Entity
{
    public class Tweet
    {
        public int TweetId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime TweetDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsMentionTweet { get; set; }
        public Tweet? MentionedTweet { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public List<Like> Likes { get; set; } = new List<Like>();
        public List<Retweet> Retweets { get; set; } = new List<Retweet>();
        public List<Mention> Mentions { get; set; } = new List<Mention>();
    }
}
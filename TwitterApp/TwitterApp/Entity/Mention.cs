namespace TwitterApp.Entity
{
    public class Mention
    {
        public int MentionId { get; set; }
        public int TweetId { get; set; }
        public Tweet Tweet { get; set; } = new(); 
        public int MentionUserId { get; set; } 
        public DateTime MentionDate { get; set; }
        public int MentionTweetId { get; set; }
        public Tweet MentionTweet { get; set; } = null!; 
        public User MentionUser { get; set; } = null!; 
        
    }
}
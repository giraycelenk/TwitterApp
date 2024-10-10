namespace TwitterApp.Entity
{
    public class Mention
    {
        public int MentionId { get; set; }
        public int TweetId { get; set; }
        public int MentionedUserId { get; set; }
        public DateTime MentionDate { get; set; }
        public Tweet Tweet { get; set; } = null!;
        public User MentionedUser { get; set; } = null!;
        
    }
}
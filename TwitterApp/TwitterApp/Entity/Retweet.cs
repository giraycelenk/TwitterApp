namespace TwitterApp.Entity
{
    public class Retweet
    {
        public int RetweetId { get; set; }
        public int UserId { get; set; }
        public int TweetId { get; set; }
        public DateTime RetweetDate { get; set; }

        public User User { get; set; } = null!;
        public Tweet Tweet { get; set; } = null!;
    }
}
namespace TwitterApp.Entity
{
    public class Like
    {
        public int LikeId { get; set; }
        public int UserId { get; set; }
        public int TweetId { get; set; }

        
        public User User { get; set; } = null!;
        public Tweet Tweet { get; set; } = null!;
    }
}
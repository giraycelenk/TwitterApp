namespace TwitterApp.Entity
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public string? HeaderImageUrl { get; set; } = string.Empty;
        public string? Bio { get; set; } 
        public string? Location { get; set; } 
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; } 
        public List<Tweet> Tweets { get; set; } = new List<Tweet>();
        public List<Like> Likes { get; set; } = new List<Like>();
        public List<Retweet> Retweets { get; set; } = new List<Retweet>();
        public List<Mention> Mentions { get; set; } = new List<Mention>();
        public List<UserFollow> Followers { get; set; } = new List<UserFollow>(); 
        public List<UserFollow> Following { get; set; } = new List<UserFollow>();
    }
}
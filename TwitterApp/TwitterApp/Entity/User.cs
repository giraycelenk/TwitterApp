namespace TwitterApp.Entity
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Image { get; set; }
        public string? Bio { get; set; } 
        public DateTime RegistrationDate { get; set; } 
        public DateTime BirthDate { get; set; } 
        public List<Tweet> Tweets { get; set; } = new List<Tweet>();
        public List<Like> Likes { get; set; } = new List<Like>();
        public List<Retweet> Retweets { get; set; } = new List<Retweet>();
        public List<Mention> Mentions { get; set; } = new List<Mention>();
        public List<User> Followers { get; set; } = new List<User>(); 
        public List<User> Following { get; set; } = new List<User>();
    }
}
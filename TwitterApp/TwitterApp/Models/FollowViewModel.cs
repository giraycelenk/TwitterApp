using TwitterApp.Entity;

namespace TwitterApp.Models
{
    public class FollowViewModel
    {
        public User? User { get; set; }
        public User? CurrentUser { get; set; }
        public List<User>? Followers { get; set; }
        public List<User>? Following { get; set; }
        public Dictionary<int,bool> IsFollowing { get; set; } 
        public Dictionary<int,bool> IsFollower{ get; set; } 
    }
}
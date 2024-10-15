namespace TwitterApp.Entity
{
    public class UserFollow
    {
        public int FollowerUserId { get; set; } 
        public User FollowerUser { get; set; } 

        public int FollowingUserId { get; set; } 
        public User FollowingUser { get; set; } 
    }
}
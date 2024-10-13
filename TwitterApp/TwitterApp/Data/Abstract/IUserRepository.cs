using TwitterApp.Entity;

namespace TwitterApp.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users {get;}
        void CreateUser(User User);
        User GetUserById(int UserId);
        List<Tweet> GetTweetsByUserId(int UserId);
        List<User> GetFollowers(int UserId);
        List<User> GetFollowings(int UserId);
        
    }
}
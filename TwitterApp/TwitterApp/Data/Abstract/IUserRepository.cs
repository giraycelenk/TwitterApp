using TwitterApp.Entity;
using TwitterApp.Models;

namespace TwitterApp.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users {get;}
        void CreateUser(User User);
        User GetUserById(int UserId);
        List<Tweet> GetTweetsByUserId(int UserId);
        List<User> GetFollowers(int UserId);
        Task<FollowViewModel> GetFollowersForProfileAsync(string username,int userId);
        Task<FollowViewModel> GetFollowingForProfileAsync(string username,int userId);
        List<User> GetFollowings(int UserId);
        public int GetIdByUsername(string userName);
        Task<bool> FollowUserAsync(int followerUserId, int followingUserId);
        Task<bool> UnfollowUserAsync(int currentUserId, int userIdToUnfollow);
        Task<UserFollow> GetFollowAsync(int followerUserId, int followingUserId);
        Task<ProfileViewModel> GetProfileByUserNameAsync(int userId,string username,string tab);
        

    }
}
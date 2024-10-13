using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Entity;

namespace TwitterApp.Data.Concrete.EfCore
{
    public class EfUserRepository : IUserRepository
    {
        private TwitterContext _context;
        public EfUserRepository(TwitterContext context)
        {
            _context = context;
        }

        public IQueryable<User> Users => _context.Users;

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUserById(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            return user ?? new();
        }
        public List<Tweet> GetTweetsByUserId(int userId)
        {
            var user = GetUserById(userId);
            if (user == null)
            {
                return new List<Tweet>();
            }
            return user.Tweets;
        }

        public List<User> GetFollowers(int userId)
        {
            var user = GetUserById(userId);
            if (user == null)
            {
                return new List<User>();
            }
            return user.Followers;
        }
        public List<User> GetFollowings(int userId)
        {
            var user = GetUserById(userId);
            if (user == null)
            {
                return new List<User>();
            }
            return user.Following;
        }
    }
}
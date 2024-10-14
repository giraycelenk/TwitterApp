using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Entity;
using TwitterApp.ViewComponents;

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
        public int GetIdByUsername(string userName)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userName);
            if(user != null)
            {
                return user.UserId;
            }
            return 0;
        }
        public List<Tweet> GetTweetsByUserId(int userId)
        {
            var tweets = _context.Tweets 
                    .Where(t => t.UserId == userId) 
                    .OrderByDescending(t => t.TweetDate) 
                    .ToList();
            
            return tweets ?? new List<Tweet>(); 
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
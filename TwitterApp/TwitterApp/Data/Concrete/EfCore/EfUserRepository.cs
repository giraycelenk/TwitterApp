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
                    .Include(t => t.Likes)
                    .Include(t => t.Retweets)
                    .Where(t => t.UserId == userId) 
                    .OrderByDescending(t => t.TweetDate) 
                    .ToList();
            
            return tweets ?? new List<Tweet>(); 
        }

        public List<User> GetFollowers(int userId)
        {
            var userFollows = _context
                        .UserFollows
                        .Where(uf => uf.FollowingUserId == userId)
                        .ToList();

            return userFollows.Select(uf => uf.FollowerUser).ToList();
        }
        public List<User> GetFollowings(int userId)
        {
            var userFollows = _context
                                .UserFollows
                                .Where(uf => uf.FollowerUserId == userId)
                                .ToList();

            return userFollows.Select(uf => uf.FollowingUser).ToList();
        }
        public async Task<bool> IsFollowing(int followerUserId, int followingUserId)
        {
            return await _context
                        .UserFollows
                        .AnyAsync(uf => uf.FollowerUserId == followerUserId && uf.FollowingUserId == followingUserId);
        }

        public async Task<bool> FollowUserAsync(int currentUserId, int userIdToFollow)
        {
            
            var existingFollow = await _context
                                        .UserFollows
                                        .FirstOrDefaultAsync(f => f.FollowerUserId == currentUserId && f.FollowingUserId == userIdToFollow);
            
            if (existingFollow != null)
            {
                return false; 
            }

            var newFollow = new UserFollow
            {
                FollowerUserId = currentUserId,
                FollowingUserId = userIdToFollow
            };

            await _context.UserFollows.AddAsync(newFollow); 
            await _context.SaveChangesAsync();

            return true; 
        }

        public async Task<bool> UnfollowUserAsync(int currentUserId, int userIdToUnfollow)
        {
            var followRelation = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerUserId == currentUserId && f.FollowingUserId == userIdToUnfollow);
            
            if (followRelation != null)
            {
                _context.UserFollows.Remove(followRelation);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<UserFollow> GetFollowAsync(int followerUserId, int followingUserId)
        {
            return await _context.UserFollows
                                .FirstOrDefaultAsync(f => f.FollowerUserId == followerUserId && f.FollowingUserId == followingUserId);
        }
    }
}
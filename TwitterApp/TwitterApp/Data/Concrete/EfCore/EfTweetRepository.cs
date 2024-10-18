using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Entity;

namespace TwitterApp.Data.Concrete.EfCore
{
    public class EfTweetRepository : ITweetRepository
    {
        private TwitterContext _context;

        public EfTweetRepository(TwitterContext context)
        {
            _context = context;
        }

        public IQueryable<Tweet> Tweets => _context.Tweets;
        public IQueryable<Like> Likes => _context.Likes;

        public void CreateTweet(Tweet tweet)
        {
            _context.Tweets.Add(tweet);
            _context.SaveChanges();
        }
        public void AddLike(int tweetId, int userId)
        {
            var tweet = _context
                        .Tweets
                        .Include(t => t.Likes) 
                        .FirstOrDefault(t => t.TweetId == tweetId);
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (tweet != null && user != null)
            {
                var like = new Like { TweetId = tweetId, UserId = userId };
                tweet.Likes.Add(like); 
                _context.SaveChanges(); 
            }
        }

        public void RemoveLike(int tweetId, int userId)
        {
            var like = _context
                        .Likes
                        .FirstOrDefault(l => l.TweetId == tweetId && l.UserId == userId);
            if (like != null)
            {
                _context.Likes.Remove(like); 
                _context.SaveChanges(); 
            }
        }
        public void AddRetweet(int tweetId, int userId)
        {
            var tweet = _context
                        .Tweets
                        .Include(t => t.Retweets) 
                        .FirstOrDefault(t => t.TweetId == tweetId);
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (tweet != null && user != null)
            {
                var retweet = new Retweet { TweetId = tweetId, UserId = userId , RetweetDate = DateTime.Now};
                tweet.Retweets.Add(retweet); 
                _context.SaveChanges(); 
            }
        }

        public void RemoveRetweet(int tweetId, int userId)
        {
            var retweet = _context
                        .Retweets
                        .FirstOrDefault(l => l.TweetId == tweetId && l.UserId == userId);
            if (retweet != null)
            {
                _context.Retweets.Remove(retweet); 
                _context.SaveChanges(); 
            }
        }
        public void UpdateTweet(Tweet tweet)
        {
            _context.Tweets.Update(tweet);
            _context.Entry(tweet).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public Task<List<Tweet>> GetAllTweetsAsync()
        {
            return _context.Tweets
                .Include(t => t.User)
                .Include(t => t.Likes) 
                .Include(t => t.Retweets) 
                .OrderByDescending(t => t.TweetDate) 
                .ToListAsync();
        }

        
    }
}
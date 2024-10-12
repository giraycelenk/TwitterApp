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

        public void CreateTweet(Tweet tweet)
        {
            _context.Tweets.Add(tweet);
            _context.SaveChanges();
        }
    }
}
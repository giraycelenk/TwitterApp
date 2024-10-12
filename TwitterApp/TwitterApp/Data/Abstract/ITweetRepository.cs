using TwitterApp.Entity;

namespace TwitterApp.Data.Abstract
{
    public interface ITweetRepository
    {
        IQueryable<Tweet> Tweets {get;}
        void CreateTweet(Tweet Tweet);
    }
}
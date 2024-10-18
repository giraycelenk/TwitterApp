using TwitterApp.Entity;

namespace TwitterApp.Data.Abstract
{
    public interface ITweetRepository
    {
        IQueryable<Tweet> Tweets {get;}
        IQueryable<Like> Likes {get;}
        void CreateTweet(Tweet Tweet);
        void AddLike(int tweetId, int userId);
        void RemoveLike(int tweetId, int userId);
        void AddRetweet(int tweetId, int userId);
        void RemoveRetweet(int tweetId, int userId);
        void UpdateTweet(Tweet tweet);
        Task<List<Tweet>> GetAllTweetsAsync();
    }
}
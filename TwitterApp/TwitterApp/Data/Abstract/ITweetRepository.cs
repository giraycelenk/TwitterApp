using TwitterApp.Entity;
using TwitterApp.Models;

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
        void DeleteTweet(int tweetId);
        Task AddMention(int tweetId, int userId,TweetCreateModel tweet);
        Task<List<Tweet>> GetAllTweetsAsync();
        Task<List<Tweet>> GetAllTweetsAndRetweetsByUserIdAsync(int userId,bool isProfilePage);
        Task<List<Tweet>> GetUserTweetsByUserIdAsync(int userId);
        Task<List<Tweet>> GetUserRetweetsByUserIdAsync(int userId);
        Task<List<Tweet>> GetFollowedTweetsByUserIdAsync(int userId);
        Task<List<Tweet>> GetFollowedRetweetsByUserIdAsync(int userId);
        Task<List<Retweet>> GetUserAndFollowedRetweetsForDictionaryAsync(int userId);
        Task<Dictionary<int,User>> GetFollowedRetweetsDictionaryAsync(int userId);
        Task<TweetViewModel> GetTweetsAndActivitiesByUserIdAsync(int currentUserId,int userId,User currentUser,User pageUser,bool isProfilePage);
        Task<TweetViewModel> GetUserLikesByUsernameAsync(int currentUserId,int userId,User currentUser,User pageUser);
        Task<TweetDetailsViewModel> GetTweetDetailsAsync(int tweetId,int currentUserId);
        Task<List<Tweet>> GetTweetMentionsAsync(int tweetId);
        int ProfilePageAndIdControl(int currentUserId,int userId,bool isProfilePage);
    }
}
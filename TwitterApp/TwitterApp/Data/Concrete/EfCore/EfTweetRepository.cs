using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Entity;
using TwitterApp.Models;

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
        public async Task<List<Tweet>> GetAllTweetsAndRetweetsByUserIdAsync(int userId,bool isProfilePage)
        {
            List<Tweet> allTweetsAndRetweets;
            var userTweets = GetUserTweetsByUserIdAsync(userId);
            var userRetweets = GetUserRetweetsByUserIdAsync(userId);
  
            if(!isProfilePage)
            {
                var followedUserTweets = GetFollowedTweetsByUserIdAsync(userId);
                var followedUserRetweets = GetFollowedRetweetsByUserIdAsync(userId);                           
                allTweetsAndRetweets = (await userTweets)
                                            .Union(await userRetweets)
                                            .Union(await followedUserTweets)
                                            .Union(await followedUserRetweets)
                                            .GroupBy(t => t.TweetId) 
                                            .Select(g => g.First())
                                            .OrderByDescending(t => t.TweetDate)
                                            .ToList();
            }
            else
            {
                allTweetsAndRetweets = (await userTweets)
                                .Union(await userRetweets)
                                .GroupBy(t => t.TweetId)
                                .Select(g => g.First())
                                .OrderByDescending(t => t.TweetDate)
                                .ToList();
            }
            return allTweetsAndRetweets ?? new List<Tweet>();
        }
        
        public async Task<TweetViewModel> GetTweetsAndActivitiesByUserIdAsync(int currentUserId,int userId,User currentUser,User pageUser,bool isProfilePage)
        {
            var viewModel = new TweetViewModel
            {
                Tweets = new List<Tweet>(),
                IsProfilePage = isProfilePage,
                User = currentUser,
                PageUser = pageUser,
                IsLikedByCurrentUser = new Dictionary<int, bool>(),
                IsRetweetedByCurrentUser = new Dictionary<int, bool>()
            };
            
            var tweets = await GetAllTweetsAndRetweetsByUserIdAsync(ProfilePageAndIdControl(currentUserId,userId,isProfilePage),isProfilePage);

            viewModel.Tweets.AddRange(tweets);
            viewModel.IsLikedByCurrentUser = tweets.ToDictionary(t => t.TweetId, t => t.Likes.Any(l => l.UserId == currentUserId));
            viewModel.IsRetweetedByCurrentUser = tweets.ToDictionary(t => t.TweetId, t => t.Retweets.Any(r => r.UserId == currentUserId));
            viewModel.FollowedRetweetsUsers = await GetFollowedRetweetsDictionaryAsync(ProfilePageAndIdControl(currentUserId,userId,isProfilePage));

            return viewModel;
        }

        public async Task<List<Tweet>> GetUserTweetsByUserIdAsync(int userId)
        {
            List<Tweet> userTweets =await _context.Tweets
                                        .Include(t => t.User)
                                        .Include(t => t.Likes)
                                        .Include(t => t.Retweets)
                                        .Where(t => t.UserId == userId && !t.IsDeleted)
                                        .Select(t => new Tweet
                                        {
                                            TweetId = t.TweetId,
                                            Content = t.Content,
                                            TweetDate = t.TweetDate,
                                            Retweets = t.Retweets,
                                            Likes = t.Likes,
                                            Mentions = t.Mentions,
                                            User = t.User
                                        }).ToListAsync();  
            return userTweets ?? new List<Tweet>();
        }

        public async Task<List<Tweet>> GetUserRetweetsByUserIdAsync(int userId)
        {
            List<Tweet> userRetweets =await _context.Retweets
                                        .Include(t => t.User)
                                        .Include(r => r.Tweet)
                                        .ThenInclude(t => t.Likes)
                                        .Include(r => r.Tweet)
                                        .ThenInclude(t => t.Retweets)
                                        .Include(r => r.Tweet)
                                        .ThenInclude(t => t.Mentions)
                                        .Where(r => r.UserId == userId && !r.Tweet.IsDeleted) 
                                        .Select(r => new Tweet
                                        {
                                            TweetId = r.Tweet.TweetId,
                                            Content = r.Tweet.Content,
                                            TweetDate = r.RetweetDate,
                                            Retweets = r.Tweet.Retweets,
                                            Likes = r.Tweet.Likes,
                                            Mentions = r.Tweet.Mentions,
                                            User = r.Tweet.User
                                        }).ToListAsync();
            return userRetweets ?? new List<Tweet>();
        }
        

        public async Task<List<Tweet>> GetFollowedTweetsByUserIdAsync(int userId)
        {
            List<Tweet> followedUserTweets = await _context.Tweets
                                        .Include(t => t.User)
                                        .Include(t => t.Likes)
                                        .Include(t => t.Retweets)
                                        .Include(t => t.Mentions)
                                        .Where(t => _context.UserFollows
                                            .Where(f => f.FollowerUserId == userId)
                                            .Select(f => f.FollowingUserId)
                                            .Contains(t.UserId) && !t.IsDeleted)
                                        .Select(t => new Tweet
                                        {
                                            TweetId = t.TweetId,
                                            Content = t.Content,
                                            TweetDate = t.TweetDate,
                                            Retweets = t.Retweets,
                                            Likes = t.Likes,
                                            Mentions = t.Mentions,
                                            User = t.User
                                        }).ToListAsync();
            return followedUserTweets ?? new List<Tweet>();
        }

        public async Task<List<Tweet>> GetFollowedRetweetsByUserIdAsync(int userId)
        {
            List<Tweet> followedUserRetweets =await _context.Retweets
                                            .Include(t => t.User)
                                            .Include(r => r.Tweet)
                                            .ThenInclude(t => t.Likes)
                                            .Include(r => r.Tweet)
                                            .ThenInclude(t => t.Retweets)
                                            .Include(r => r.Tweet)
                                            .ThenInclude(t => t.Mentions)
                                            .Where(r => _context.UserFollows
                                                .Where(f => f.FollowerUserId == userId)
                                                .Select(f => f.FollowingUserId)
                                                .Contains(r.UserId) && !r.Tweet.IsDeleted)
                                            .Select(r => new Tweet
                                            {
                                                TweetId = r.Tweet.TweetId,
                                                Content = r.Tweet.Content,
                                                TweetDate = r.RetweetDate,
                                                Retweets = r.Tweet.Retweets,
                                                Likes = r.Tweet.Likes,
                                                Mentions = r.Tweet.Mentions,
                                                User = r.Tweet.User
                                            }).ToListAsync();
            return followedUserRetweets ?? new List<Tweet>();
        }
        public async Task<List<Retweet>> GetUserAndFollowedRetweetsForDictionaryAsync(int userId)
        {
            List<Retweet> userRetweets =await _context.Retweets
                                        .Include(t => t.User)
                                        .Include(r => r.Tweet)
                                        .ThenInclude(t => t.Likes)
                                        .Include(r => r.Tweet)
                                        .ThenInclude(t => t.Retweets)
                                        .Include(r => r.Tweet)
                                        .ThenInclude(t => t.Mentions)
                                        .Where(r => r.UserId == userId && !r.Tweet.IsDeleted) 
                                        .Select(r => new Retweet
                                        {
                                            TweetId = r.TweetId,
                                            UserId = r.UserId
                                        }).ToListAsync();
            
            List<Retweet> followedUserRetweets =await _context.Retweets
                                            .Include(t => t.User)
                                            .Include(r => r.Tweet)
                                            .ThenInclude(t => t.Likes)
                                            .Include(r => r.Tweet)
                                            .ThenInclude(t => t.Retweets)
                                            .Include(r => r.Tweet)
                                            .ThenInclude(t => t.Mentions)
                                            .Where(r => _context.UserFollows
                                                .Where(f => f.FollowerUserId == userId)
                                                .Select(f => f.FollowingUserId)
                                                .Contains(r.UserId) && !r.Tweet.IsDeleted)
                                            .Select(r => new Retweet
                                            {
                                                TweetId = r.TweetId,
                                                UserId = r.UserId
                                            }).ToListAsync();
                                            
            var allRetweetsForDictionary = userRetweets
            .Union(followedUserRetweets)
            .GroupBy(t => t.TweetId) 
            .Select(g => g.First())
            .OrderByDescending(t => t.RetweetDate)
            .ToList();
            return allRetweetsForDictionary ?? new List<Retweet>();
        }
        public async Task<Dictionary<int,User>> GetFollowedRetweetsDictionaryAsync(int userId)
        {
            Dictionary<int,User> followedUserRetweetsDictionary = new Dictionary<int, User>();
            foreach(var retweet in await GetUserAndFollowedRetweetsForDictionaryAsync(userId))
            {
                User user = _context.Users.FirstOrDefault(u => u.UserId == retweet.UserId);
                followedUserRetweetsDictionary[retweet.TweetId] = user ?? new User(); 
            }
             
            return followedUserRetweetsDictionary ?? new Dictionary<int,User>();
        }
        public int ProfilePageAndIdControl(int currentUserId, int userId, bool isProfilePage)
        {
            int tweetsId = isProfilePage && currentUserId != userId ? userId : currentUserId;
            return tweetsId;
        }
    }
}
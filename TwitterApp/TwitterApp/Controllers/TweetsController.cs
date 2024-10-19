using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterApp.Data.Abstract;
using TwitterApp.Entity;

namespace TwitterApp.Controllers
{
    public class TweetsController:Controller
    {
        private ITweetRepository _tweetRepository;
        public TweetsController(ITweetRepository tweetRepository)
        {
            _tweetRepository = tweetRepository;
        }
        [HttpPost]
        public IActionResult LikeTweet(int tweetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tweet = _tweetRepository
                        .Tweets
                        .Include(t => t.Likes)
                        .FirstOrDefault(t => t.TweetId == tweetId);
            
            if (tweet != null)
            {
                var likeExists = tweet.Likes.Any(l => l.UserId == userId);
                if(!likeExists)
                {
                    _tweetRepository.AddLike(tweetId,userId);
                    var updatedLikeCount = tweet.Likes.Count();
                    return Json(new { success = true,likeCount = updatedLikeCount});
                }
            }

            return Json(new { success = false});
        }
        [HttpPost]
        public IActionResult UnlikeTweet(int tweetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tweet = _tweetRepository
                        .Tweets
                        .Include(t => t.Likes)
                        .FirstOrDefault(t => t.TweetId == tweetId);

            if(tweet != null && tweet.Likes.Any(l => l.UserId == userId))
            {
                _tweetRepository.RemoveLike(tweetId,userId);
                var updatedLikeCount = tweet.Likes.Count();
                return Json(new { success = true , likeCount = updatedLikeCount});
            }
            
            return Json(new { success = false}); 
        }
        [HttpPost]
        public IActionResult Retweet(int tweetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tweet = _tweetRepository
                        .Tweets
                        .Include(t => t.Retweets)
                        .FirstOrDefault(t => t.TweetId == tweetId);
            
            if (tweet != null && !tweet.Retweets.Any(r => r.UserId == userId))
            {
                _tweetRepository.AddRetweet(tweetId, userId);
                var updatedRetweetCount = tweet.Retweets.Count();
                return Json(new { success = true, retweetCount = updatedRetweetCount });
            }

            return Json(new { success = false});
        }
        [HttpPost]
        public IActionResult UnRetweet(int tweetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tweet = _tweetRepository
                        .Tweets
                        .Include(t => t.Retweets)
                        .FirstOrDefault(t => t.TweetId == tweetId);
            
            if (tweet != null && tweet.Retweets.Any(r => r.UserId == userId))
            {
                _tweetRepository.RemoveRetweet(tweetId, userId); 
                var updatedRetweetCount = tweet.Retweets.Count();
                return Json(new { success = true, retweetCount = updatedRetweetCount });
            }

            return Json(new { success = false });
        }
    }
}
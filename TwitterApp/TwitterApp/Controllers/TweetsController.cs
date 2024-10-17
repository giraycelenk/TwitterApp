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
                    var newLike = new Like { UserId = userId, TweetId = tweetId };
                    tweet.Likes.Add(newLike);
                    _tweetRepository.UpdateTweet(tweet);
                    var updatedLikeCount = tweet.Likes.Count();
                    return Json(new { success = true,likeCount = updatedLikeCount});
                }
                else
                {
                    return Json(new { success = false});
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
            if (tweet != null)
            {
                var like = tweet.Likes.FirstOrDefault(l => l.UserId == userId);
                if(like != null)
                {
                    tweet.Likes.Remove(like);
                    _tweetRepository.UpdateTweet(tweet);
                    var updatedLikeCount = tweet.Likes.Count();
                    return Json(new { success = true , likeCount = updatedLikeCount});
                }
                else
                {
                    return Json(new { success = false}); 
                }
            }
            return Json(new { success = false}); 
        }
    }
}